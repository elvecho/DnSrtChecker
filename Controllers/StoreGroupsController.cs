using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DnSrtChecker.Models;
using DnSrtChecker.Persistence;
using AutoMapper;
using DnSrtChecker.Models.ViewModels;
using Serilog;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace DnSrtChecker.Controllers
{
    [Authorize(Roles = "OperatorIT")]

    public class StoreGroupsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStoreRepository _storeRepository;
        //private readonly IRtServerRepository _rtServerRepository;
        private readonly IStoreGroupRepository _storeGroupRepository;
        private readonly ILogger<StoreGroupsController> _logger;

        public StoreGroupsController(IUnitOfWork unitOfWork,
                                IMapper mapper,
                                //IRtServerRepository rtServerRepository,
                                IStoreRepository storeRepository,
                                IStoreGroupRepository storeGroupRepository, ILogger<StoreGroupsController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _storeGroupRepository = storeGroupRepository;
            _storeRepository = storeRepository;
            //_rtServerRepository = rtServerRepository;
            _logger = logger;
        }

        // GET: StoreGroups
        public async Task<IActionResult> Index()
        {
            _logger.LogDebug("Start: Index StoreGroup");
            //var listRtServer = await _rtServerRepository.ListRtServerStatusNew(new FiltersmodelBindRequest.FiltersmodelBindingRequest());

            var listStoreGroups = await _storeGroupRepository.ListStoreGroups();
            _logger.LogDebug($"END: Return list of all Store groups length:{listStoreGroups.Count}");

            return View(_mapper.Map<List<StoreGroup>, List<StoreGroupViewModel>>(listStoreGroups));
        }

        // GET: StoreGroups/Details/5
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogDebug($"Start: Search and get the Stor Group with ID if exist : {id}");

            if (id == null)
            {
                return NotFound();
            }

            var storeGroup = _storeGroupRepository.GetStoreGroup(id);
            if (storeGroup == null)
            {
                return NotFound();
            }
            _logger.LogDebug($"END: return 'View Detail StoreGroup'  which has an Id : {id}");

            return View(_mapper.Map<StoreGroup, StoreGroupViewModel>(await storeGroup));
        }

        // GET: StoreGroups/Create
        public IActionResult Create([FromQuery] string fromCreateStorePlus)
        {
            _logger.LogDebug($"Start: Get 'Create Form' to create a new StoreGroup | Input:{fromCreateStorePlus}");

            if (fromCreateStorePlus!=null && fromCreateStorePlus=="true")
            {
                TempData["fromCreateStorePlus"] = "true";
            }

            _logger.LogDebug($"END: Get the' Create Form Store Group'");

            return View();
        }

        // POST: StoreGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( StoreGroupViewModel storeGroupVM)
        {
            _logger.LogDebug($"Start: Get the object to create | Input:{storeGroupVM}");

            var previousPage = TempData["fromCreateStorePlus"]?.ToString();// != null ? TempData["fromCreateStorePlus"].ToString() : null;
           
            var storeGroup = _mapper.Map<StoreGroupViewModel, StoreGroup>(storeGroupVM);
           
            if (ModelState.IsValid)
            {
               try
               {
                    var result = _storeGroupRepository.AddStoreGroup(storeGroup);
                    await _unitOfWork.CompleteAsync();

                    if (previousPage != null)
                        {
                            TempData["fromCreateStorePlus"] = null;
                            return RedirectToAction("Create", "Stores");
                        }

                    _logger.LogDebug($"END: Store group successfully created.");
                    return RedirectToAction(nameof(Index));
               }
               catch(DbUpdateException e)
               {
                    _logger.LogCritical($"END: Item add failed");

                    if (StoreGroupExists(storeGroup.LStoreGroupId))
                    {
                        ModelState.AddModelError("storeGroup", "Esiste gia un'insegna con il codice inserito.");
                        return View(_mapper.Map<StoreGroup, StoreGroupViewModel>(storeGroup));
                    }
               }
            }
                
            return View(_mapper.Map<StoreGroup,StoreGroupViewModel>(storeGroup));

        }

        // GET: StoreGroups/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogDebug($"Start: Get 'Edit View' to Edit an existing StoreGroup | Input:{id} ");

            if (id == null)
            {
                return NotFound();
            }

            var storeGroup = await _storeGroupRepository.GetStoreGroup(id);

            if (storeGroup == null)
            {
                return NotFound();
            }

            _logger.LogDebug($"END: Redirect to 'Edit View StoreGroup'");

            return View(_mapper.Map<StoreGroup,StoreGroupViewModel>(storeGroup));
        }

        // POST: StoreGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StoreGroupViewModel storeGroupVM)
        {
            _logger.LogDebug($"Start:PostMethod to edit existing object | Input:{id} , {storeGroupVM}");

            var storeGroup = _mapper.Map<StoreGroupViewModel, StoreGroup>(storeGroupVM);
            if (id != storeGroup.LStoreGroupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.UpdateAsync(storeGroup);
                    await _unitOfWork.CompleteAsync();

                    _logger.LogDebug($"END: Item updated successfully ");
                }
                catch (DbUpdateConcurrencyException e)
                {
                    _logger.LogCritical($"Fatal:Cannot update the StoreGroup | Error:{e.Message}");

                    if (!StoreGroupExists(storeGroup.LStoreGroupId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                _logger.LogDebug($"END: Redirect to Index");

                return RedirectToAction(nameof(Index));
            }
            return View(_mapper.Map<StoreGroup,StoreGroupViewModel>(storeGroup));
        }

        // GET: StoreGroups/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogDebug($"Start: Get 'Delete Form' to Delete  an existing StoreGroup | Input:{ id}");

            if (id == null)
            {
                return NotFound();
            }

            var storeGroup = await _storeGroupRepository.GetStoreGroup(id);
            if(storeGroup == null)
            {
                return NotFound();
            }

            _logger.LogDebug($"END: Get 'Delete Form'");

            return View(_mapper.Map<StoreGroup, StoreGroupViewModel>(storeGroup));
        }

        // POST: StoreGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogDebug($"Start: Confirm deleting StoreGroup | Input: {id}");

            var storeGroup = await _storeGroupRepository.GetStoreGroup(id);
            var result=_storeGroupRepository.RemoveStoreGroup(storeGroup);
            await _unitOfWork.CompleteAsync();

            _logger.LogDebug($"END: StoreGroup Deleted and redirect to Index | Input :{result}");

            return RedirectToAction(nameof(Index));
        }

        private bool StoreGroupExists(int id)
        {
            _logger.LogDebug($"Start: Check if the Item exists | Input:{id}");

            var founded = (_storeGroupRepository.GetStoreGroup(id)) != null ? true : false;

            _logger.LogDebug($"END: Output:{founded}");

            return founded;
        }
    }
}
