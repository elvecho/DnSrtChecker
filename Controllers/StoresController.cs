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
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Serilog;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace DnSrtChecker.Controllers
{
    [Authorize(Roles = "OperatorIT")]

    public class StoresController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStoreRepository _storeRepository;
        private readonly IStoreGroupRepository _storeGroupRepository;
        private readonly ILogger<StoresController> _logger;

        public StoresController(IUnitOfWork unitOfWork,
                                IMapper mapper,
                                IStoreRepository storeRepository,
                                IStoreGroupRepository storeGroupRepository,
                                ILogger<StoresController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _storeGroupRepository = storeGroupRepository;
            _storeRepository = storeRepository;
            _logger = logger;
                }

        // GET: Stores
        public async Task<IActionResult> Index()
        {
            _logger.LogDebug("Start: Index Store");

            var listStores =await  _storeRepository.ListStores();
            
            _logger.LogDebug($"END: Return list of all Store length:{listStores.Count}");

            return View(_mapper.Map<List<Store>,List<StoreViewModel>>( listStores));
        }

        // GET: Stores/Details/5
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogDebug($"Start: Search and get the Stor with ID if exist : {id}");

            if (id == null)
            {
                return NotFound();
            }

            var store = _storeRepository.GetStore(id);
            if (store == null)
            {
                return NotFound();
            }

            _logger.LogDebug($"END: Store Detail  which has an Id : {id}");

            return View(_mapper.Map<Store,StoreViewModel>(await store));
        }

        // GET: Stores/Create
          public async  Task<IActionResult> Create()
          {
            _logger.LogDebug($"Start: Get 'Create Form' to create a new Store");

            ViewData["LStoreGroupId"] = new SelectList(await _storeGroupRepository.ListStoreGroups(), "LStoreGroupId", "SzDescription");
            _logger.LogDebug($"END: Get the' Create Form Store'");

            return View();
          }
      
        // POST: Stores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( StoreViewModel storeVM)
        {
            _logger.LogDebug($"Start: Get the object to create | Input:{storeVM}");

            // var rtserver = JsonConvert.DeserializeObject<RtServer>(ViewData["rtServer"].ToString());
            var store = _mapper.Map<StoreViewModel, Store>(storeVM);
            var previousPage= TempData["fromCreateRT"]!=null? TempData["fromCreateRT"].ToString():null;
            if (ModelState.IsValid)
            {
                try
                {
                    var result = _storeRepository.AddStore(store);
                    await _unitOfWork.CompleteAsync();

                    if (previousPage != null)
                    {
                        TempData["fromCreateRT"] = null;
                        TempData["fromCreateStore"] = true;
                        return RedirectToAction("Create", "RtServers");
                    }
                    return RedirectToAction(nameof(Index));

                }
                catch (DbUpdateException e) {
                    if (StoreExists(store.LRetailStoreId,store.LStoreGroupId))
                    {
                        ModelState.AddModelError("store", "Esiste gia un punto di vendita con il codice e l'insegna inseriti.");
                    }
                }
            }
            
            _logger.LogCritical($"END: Item add failed");
            ViewData["LStoreGroupId"] = new SelectList(await _storeGroupRepository.ListStoreGroups(), "LStoreGroupId", "SzDescription", store.LStoreGroupId);

            return View(_mapper.Map<Store,StoreViewModel>(store));
        }

        // GET: Stores/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogDebug($"Start: Get 'Edit Form' to Edit an existing Store | Input:{id} ");

            if (id == null)
            {
                return NotFound();
            }
            
            var store =await _storeRepository.GetStore(id);
            if (store == null)
            {
                return NotFound();
            }
            ViewData["LStoreGroupId"] = new SelectList(await _storeGroupRepository.ListStoreGroups(), "LStoreGroupId", "SzDescription", store.LStoreGroupId);
           
            _logger.LogDebug($"END: Redirect to 'Edit Form Store'");

            return View(_mapper.Map<Store,StoreViewModel>(store));
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StoreViewModel storeVM)
        {
            _logger.LogDebug($"Start:PostMethod to edit existing object with new data | Input:{id} , {storeVM}");

            if (id != storeVM.LRetailStoreId)
            {
                return NotFound();
            }
            var store = _mapper.Map<StoreViewModel, Store>(storeVM);
            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.UpdateAsync(store);
                    await _unitOfWork.CompleteAsync();
                    _logger.LogDebug($"END: Item updated successfully ");

                }
                catch (DbUpdateConcurrencyException e)
                {
                    Log.Fatal($"Fatal:Cannot update the Store | Error:{e.Message}");

                    if (!StoreExists(store.LRetailStoreId,store.LStoreGroupId))
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
            ViewData["LStoreGroupId"] = new SelectList(await  _storeGroupRepository.ListStoreGroups(), "LStoreGroupId", "SzDescription", store.LStoreGroupId);
            return View(_mapper.Map<Store,StoreViewModel>(store));
        }

        // GET: Stores/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogDebug($"Start: Get 'Delete Form' to Delete  an existing Store | Input:{ id}");

            if (id == null)
            {
                return NotFound();
            }

            var store =await  _storeRepository.GetStore(id);
            if (store == null)
            {
                return NotFound();
            }
            _logger.LogDebug($"END: Get 'Delete Form' ");

            return View(_mapper.Map<Store,StoreViewModel>(store));
        }

        // POST: Stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogDebug($"Start: Confirm deleting Store| Input: {id}");

            var store = await _storeRepository.GetStore(id);
            var result=_storeRepository.RemoveStore(store);
            await _unitOfWork.CompleteAsync();
            _logger.LogDebug($"END: Store Deleted and redirect to Index | Input :{result}");

            return RedirectToAction(nameof(Index));
        }

        private bool StoreExists(int id,int storeGroupId)
        {
            _logger.LogDebug($"Start: Check if the Store exists | Input:{id}");
            var founded= (_storeRepository.GetStoreByStoreGroup(id,storeGroupId)) != null ? true : false;
            _logger.LogDebug($"END: Output:{founded}");

            return founded;
            
        }
    }
}
