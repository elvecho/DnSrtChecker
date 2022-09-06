using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Serilog;
using Microsoft.Extensions.Logging;
using DnSrtChecker.Models;
using DnSrtChecker.Persistence;
using DnSrtChecker.Models.ViewModels;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Http;
using DnSrtChecker.Models.ViewModels.ModelsFunctions;
using DnSrtChecker.FiltersmodelBindRequest;

namespace DnSrtChecker
{
    [Authorize(Roles ="OperatorIT")]
    public class RtServersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoreRepository _storeRepository;
        private readonly IRtServerRepository _rtServerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RtServersController> _logger;
        public static List<RtServer> listRtServer { get; set; }

        public RtServersController(IUnitOfWork unitOfWork,
                                    IStoreRepository storeRepository,
                                    IRtServerRepository rtServerRepository,
                                    IMapper mapper,
                                    ILogger<RtServersController> logger
                                    )
        {
            _unitOfWork = unitOfWork;
            _storeRepository = storeRepository;
            _rtServerRepository = rtServerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: RtServers
        public async Task<IActionResult> Index()
        {
            _logger.LogDebug("Start: Index RtServerController");
            //call new method 
            var listRtServer = await _rtServerRepository.ListRtServerByUser();
            //var listRtServer = await _rtServerRepository.ListRtServerStatusNew(new FiltersmodelBindRequest.FiltersmodelBindingRequest());
            //var listServerRT = await _rtServerRepository.ListRtServer();
            _logger.LogDebug($"END: Return list of all RT Servers length:{listRtServer.Count}");
            //_logger.LogDebug($"END: Return list of all RT Servers length:{listServerRT.Count}");

            //var v = _mapper.Map<List<RtServer>, List<RtServerViewModel>>(listServerRT);
            return //View(_mapper.Map<List<RtServer>, List<RtServerViewModel>>(listServerRT));
                  View(_mapper.Map<List<RtServerByUser>,List<RtServerViewModel>>( listRtServer));
        }

        // GET: RtServers/Details/5
        public async Task<IActionResult> Details(string id,int retailStoreId,int storeGroupId)
        {
            _logger.LogDebug($"Start: Search and get the Server RT with ID if exist : {id}");

            if (id == null)
            {
                return NotFound();
            }

            var rtServer = await _rtServerRepository.GetRtServerStatus(id,retailStoreId,storeGroupId);
            if (rtServer == null)
            {
                return NotFound();
            }
            _logger.LogDebug($"END: return View Rt Server Detail has an Id : {id}");

            return View(_mapper.Map<RtServer,RtServerViewModel>(rtServer));
        }


        // GET: RtServers/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogDebug("Start: Get 'Create View' to create a new Rt Server");

            ViewData["BOnDutyFlag"] = "true";

            ViewBag.LRetailStoreId = new SelectList(await _storeRepository.ListStores(), "LRetailStoreId", "SzDescription");
            if (TempData["fromCreateStore"]!=null && TempData["fromCreateStore"].ToString() != "")
            {
                _logger.LogDebug("Enter in this condition => turn back from a create new Store or Store group");
                var rtServer = JsonConvert.DeserializeObject<RtServerViewModel>(TempData["rtServer"].ToString());
                TempData["fromCreateStore"] = null;
                _logger.LogDebug($"END-1: Turn Back to the Rt Server Create keep in input's values inserted by the user");

                return View(rtServer);
            }
            _logger.LogDebug($"END: Get the ' Create Form'.");

            return View();
        }

        // POST: RtServers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RtServerViewModel rtServerVM)
        {
            _logger.LogDebug($"Start: Get the object to create | Input:{rtServerVM}");

            var rtServer = _mapper.Map<RtServerViewModel, RtServer>(rtServerVM);
            rtServer.L = await _storeRepository.GetStore(rtServer.LRetailStoreId);
            if (ModelState.IsValid)
            {
                try
                {
                    CreateNewStatusRtServer(rtServer);

                    var result = _rtServerRepository.AddRtServer(rtServer);
                    await _unitOfWork.CompleteAsync();
                    _logger.LogDebug($"END-1: Output: {result}");
                    return RedirectToAction(nameof(Index));
                }
                catch(DbUpdateException e)
                {
                    _logger.LogCritical($"END: Item add failed");
                    if (await RtServerExists(rtServer.SzRtServerId, rtServer.LRetailStoreId, rtServer.LStoreGroupId))
                    {
                        ModelState.AddModelError("RtServer", "Errore:Esiste gia un Server RT con il codice ,lo store e l'insegna inseriti.");
                    }
                }
            }
            ViewBag.LRetailStoreId = new SelectList(await _storeRepository.ListStores(), "LRetailStoreId", "SzDescription");
            _logger.LogDebug("END: Server RT created successfully.");
            return View(_mapper.Map<RtServer,RtServerViewModel>(rtServer));
        }


        //Function onclick on + to create new store from RtServer Form
        public  IActionResult RedirectToStore([FromQuery] string rtServer)
        {
            _logger.LogDebug($"Start: redirect to 'Create View Store' to add New store during create new Rt Server | Input:{rtServer}");

            TempData["rtServer"] = rtServer;
            TempData["fromCreateRT"] = true;

            _logger.LogDebug("END: Redirect to 'Create Form Store'");

            return RedirectToAction("Create", "Stores");
          
        }

        // GET: RtServers/Edit/5
        public async Task<IActionResult> Edit(string id,int retailStoreId,int storeGroupId)
        {
            _logger.LogDebug($"Start: Get 'Edit Form' to Edit an existing Rt Server | Input:{id} ");

            if (id == null)
            {
                return NotFound();
            }

            var rtServer = await _rtServerRepository.GetRtServerStatus(id,retailStoreId, storeGroupId);
            //var serverRT = await _rtServerRepository.GetRtServer(id, retailStoreId, storeGroupId);
            if (rtServer == null)
            {
                return NotFound();
            }
            ViewData["LRetailStoreId"] = new SelectList(await _storeRepository.ListStores(), "LRetailStoreId", "CompleteDescription", rtServer.LRetailStoreId );
            var rtServerVM = _mapper.Map<RtServer, RtServerViewModel>(rtServer);

            _logger.LogDebug("END: Redirect to 'Edit Form RtServer'");

            return View(rtServerVM);
        }

        // POST: RtServers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, RtServerViewModel  rtServerVM)
        {
            _logger.LogDebug($"Start: Get the object to edit | Input:{id} , {rtServerVM}");

            var rtServer = _mapper.Map<RtServerViewModel, RtServer>(rtServerVM);
            rtServer.SzRtServerId = rtServer.RtServerStatus.SzRtServerId;
            rtServer.LRetailStoreId = rtServer.L.LRetailStoreId;
            rtServer.LStoreGroupId = rtServer.L.LStoreGroupId;
            if (!rtServer.SzRtServerId.Trim().Contains(id))
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var exist = await RtServerExists(rtServer.SzRtServerId.Trim(), rtServer.LRetailStoreId, rtServer.LStoreGroupId);
                try
                {
                  
                    if (!exist)
                    {

                        //CreateNewStatusRtServer(rtServer);
                        rtServer.L = await _storeRepository.GetStore(rtServer.LRetailStoreId);
                        var newRtServer = _rtServerRepository.AddRtServer(rtServer);
                       
                    }
                    else
                    {
                        var rtServerToActivate = await _rtServerRepository.GetRtServerStatus(rtServer.SzRtServerId, rtServer.LRetailStoreId, rtServer.LStoreGroupId);
                        rtServerToActivate.BOnDutyFlag = rtServer.BOnDutyFlag;
                        if (rtServerToActivate.L.LStoreGroup.SzDescription.ToLower().Contains("ekom"))
                        {
                            rtServerToActivate.RtServerStatus.BVatVentilationFlag = true;
                        }
                        _unitOfWork.UpdateAsync(rtServerToActivate);

                    }
                    
                    await _unitOfWork.CompleteAsync();

                }
                catch (DbUpdateConcurrencyException e)
                {
                    _logger.LogCritical($"Fatal:Cannot update the object | Error:{e.Message}");
                    if (!await RtServerExists(rtServer.SzRtServerId,rtServer.LRetailStoreId,rtServer.LStoreGroupId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                _logger.LogDebug("END: Server RT edited successfully.");

                return RedirectToAction(nameof(Index));
            }
            ViewData["LRetailStoreId"] = new SelectList(await _storeRepository.ListStores(), "LRetailStoreId", "SzDescription", rtServer.LRetailStoreId);
            return View(rtServer);
        }

        // GET: RtServers/Delete/5
        public async Task<IActionResult> Delete(string id, int retailStoreId,int storeGroupId)
        {
            _logger.LogDebug($"Start: Get 'Delete Form' to Delete logically(change status to off) an existing Rt Server | Input:{ id}");

            if (id == null)
            {
                return NotFound();
            }
            
            var rtServer =await _rtServerRepository.GetRtServerStatus(id,retailStoreId,storeGroupId);
            if (rtServer == null)
            {
                return NotFound();
            }
            _logger.LogDebug($"END: Get 'Delete Form'");
            return View(_mapper.Map<RtServer,RtServerViewModel>(rtServer));
        }

        // POST: RtServers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id, int LRetailStoreId, int LStoreGroupId)
        {
            _logger.LogDebug($"Start: Confirm template to delete Rt Server | Input: {id}");

            var rtServer = await _rtServerRepository.GetRtServerStatus(id, LRetailStoreId, LStoreGroupId);
            rtServer.BOnDutyFlag = false;
            _unitOfWork.UpdateAsync(rtServer);
            await _unitOfWork.CompleteAsync();

            _logger.LogDebug("END: Disable Rt Server and redirect to List Server RT.");

            return RedirectToAction(nameof(Index));
        }
        
        private async  Task<bool> RtServerExists(string id,int retailStoreId, int storeGroupId)
        {
            _logger.LogDebug($"Start: Check if the Item already exists | Input:{id}");
            
            var founded =(await   _rtServerRepository.GetRtServerStatus(id, retailStoreId, storeGroupId)) != null ? true : false;

            _logger.LogDebug($"END: Output:{founded}");

            return founded;
        }

        public async Task<JsonResult> StatusServerChange(string param)
        {
            _logger.LogDebug($"Start: Method to change status Rt Server | Input: {param}");
            
            var parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(param);
            var id = parameters["id"].ToString();
            var retailStoreId = Int32.Parse(parameters["retailStoreId"].ToString());
            var storeGroupId= Int32.Parse(parameters["storeGroupId"].ToString());
            var status = bool.Parse( parameters["statusSRV"].ToString());
            var changed = false;
           
                var server = await _rtServerRepository.GetRtServerStatus(id, retailStoreId, storeGroupId);

                if (server != null && !server.BOnDutyFlag.Equals(status))
                {
                    server.BOnDutyFlag = status;
                    _unitOfWork.UpdateAsync(server);
                    await _unitOfWork.CompleteAsync();
                    changed = true;
                }
            
          
            _logger.LogDebug($"END: Update item with id= {id} | Output:{changed}");

            return Json(new { statusServer = changed }); 
        }
       
        public RtServer CreateNewStatusRtServer(RtServer rtServer)
        {
            //da controllare
            _logger.LogDebug($"Start: Create server rt status Object");

            rtServer.RtServerStatus = new RtServerStatus()
            {
                SzRtServerId = rtServer.SzRtServerId,
                LRetailStoreId = rtServer.LRetailStoreId,
                LStoreGroupId = rtServer.LStoreGroupId,
                BOnErrorFlag = false,
                //BVatVentilationFlag non va forzata si va solo in lettura ,detto da N.G
                //BVatVentilationFlag = rtServer.L.LStoreGroup.SzDescription.ToLower().Contains("ekom") ? true : false
            };
            _logger.LogDebug("END: object status Created");
            return rtServer;
        }
        
    }
}
