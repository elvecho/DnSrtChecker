using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DnSrtChecker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DnSrtChecker.Persistence
{
    public class StoreRepository : IStoreRepository
    {
        public readonly RT_ChecksContext _dbContext;
        private readonly IRtServerRepository _rtServerRepository;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public StoreRepository(RT_ChecksContext dbContext, ILogger<StoreRepository> logger,
            IRtServerRepository rtServerRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _rtServerRepository = rtServerRepository;
        }

        public bool AddStore(Store store)
        {
            try
            {
                _dbContext.Store.Add(store);
                _logger.LogDebug($"Store {store.SzDescription}  added successufully");
                return true;
            }catch(Exception e)
            {
                _logger.LogError($"Error Adding Store {store.SzDescription} : {e.Message}");
                return false;
            }
        }
        public bool RemoveStore(Store store)
        {
            try
            {
                _dbContext.Store.Remove(store);
                _logger.LogDebug($"Store {store.SzDescription}  removed successufully");

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error deleting Store {store.SzDescription} : {e.Message}");
                return false;
            }
        }
        public async Task<Store> GetStore(int id)
        {

            return (await ListStores()).FirstOrDefault(x=>x.LRetailStoreId==id) ;
        }
        public async Task<Store> GetStoreByStoreGroup(int id,int storeGroupId)
        {
            return (await ListStores()).FirstOrDefault(x => x.LRetailStoreId == id && x.LStoreGroupId==storeGroupId);
        }
        public async Task<List<Store>> ListStores()
        {
            //limitare i store visibili a quelli per l'user
            //var rtServerList = _dbContext.RtServer.ToList();
            var listRtServer = RtServerRepository.ListRtServers.Count != 0
                ? RtServerRepository.ListRtServers
                : await _rtServerRepository.ListRtServerStatusNew(new FiltersmodelBindRequest.FiltersmodelBindingRequest());
            // Mi serve una lista di LRetailStoreId corrispondente all'user
            List<int> storeIDList = listRtServer.Select(x => x.LRetailStoreId).ToList();
            return await _dbContext.Store.
            Include(x => x.LStoreGroup)
            .Where(x => storeIDList.Contains(x.LRetailStoreId))
            .OrderBy(x => x.LStoreGroupId).ThenBy(x => x.LRetailStoreId)
            .ToListAsync();
        }

    }
}