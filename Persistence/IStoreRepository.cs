using DnSrtChecker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnSrtChecker.Persistence
{
    public interface IStoreRepository
    {
        Task<List<Store>> ListStores();
        Task<Store> GetStore(int id);
        Task<Store> GetStoreByStoreGroup(int id, int storeGroupId);
        Boolean AddStore(Store store);
        Boolean RemoveStore(Store store);
    }
}
