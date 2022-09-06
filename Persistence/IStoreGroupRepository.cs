using DnSrtChecker.Models;
using DnSrtChecker.Models.ViewModels.ModelsFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Persistence
{
   public interface IStoreGroupRepository
    {
        Task<List<StoreGroup>> ListStoreGroups();
        Task<StoreGroup> GetStoreGroup(int id);
        Boolean AddStoreGroup(StoreGroup storeGroup);
        Boolean RemoveStoreGroup(StoreGroup storeGroup);
    }
}
