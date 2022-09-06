using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnSrtChecker.Models;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DnSrtChecker.Persistence
{
    public class StoreGroupRepository : IStoreGroupRepository
    {
        private readonly RT_ChecksContext _dbContext;
        private readonly IRtServerRepository _rtServerRepository;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public StoreGroupRepository(RT_ChecksContext dbContext, ILogger<StoreGroupRepository> logger
                                    ,IRtServerRepository rtServerRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _rtServerRepository = rtServerRepository;
        }

        public bool AddStoreGroup(StoreGroup storeGroup)
        {
            try
            {
                _logger.LogDebug($"StoreGroup {storeGroup.SzDescription}  added successufully");

                _dbContext.Add(storeGroup);
                return true;
            } catch (Exception e){
                _logger.LogError($"Error adding StoreGroup {storeGroup.SzDescription} : {e.Message}");

                return false;
            }
        }

        public async Task<StoreGroup> GetStoreGroup(int id)
        {
           return (await ListStoreGroups()).FirstOrDefault(x => x.LStoreGroupId == id);
        }

        public async Task<List<StoreGroup>> ListStoreGroups()
        {
            //var storeGroupList = _dbContext.StoreGroup.ToList();
            //List<StoreGroup> stGroupList = new List<StoreGroup>();
            //per eliminare i storeGroup non autorizzati ci appoggiamo sulla 
            //lista dei RTServer
            //var rtServerList = _dbContext.RtServer.ToList();
            var listRtServer = RtServerRepository.ListRtServers.Count != 0 
                ? RtServerRepository.ListRtServers 
                : await _rtServerRepository.ListRtServerStatusNew(new FiltersmodelBindRequest.FiltersmodelBindingRequest());

            List<int> storeGroupIDList = listRtServer.Select(x => x.LStoreGroupId).ToList();
            var storeGroupList = _dbContext.StoreGroup.ToList()
                .Where(x => storeGroupIDList.Contains(x.LStoreGroupId));
            //.Where(x=> storeIDList.Contains(x.LRetailStoreId))
            //foreach (var storeGroup  in storeGroupList)
            //{
            //    if (_dbContext.RtServer.ToList().Select(x =>
            //    x.LStoreGroupId).Contains(storeGroup.LStoreGroupId))
            //    {
            //        stGroupList.Add(storeGroup);
            //    }
            //}

           return storeGroupList.ToList();
        }

        public bool RemoveStoreGroup(StoreGroup storeGroup)
        {
            try
            {
                _dbContext.Remove(storeGroup);
                _logger.LogDebug($"StoreGroup {storeGroup.SzDescription}  Removed successufully");

                return true;
            }
            catch(Exception e)
            {
                _logger.LogError($"Error Removing StoreGroup {storeGroup.SzDescription} : {e.Message}");
                return false;
            }
        }
    }
}
