using DnSrtChecker.FiltersmodelBindRequest;
using DnSrtChecker.Models;
using DnSrtChecker.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Services
{
    public class FilterList<T>
    {
        public FilterList()
        {
        }
        public static List<T> ListFiltered(List<T> listToFilter,Func<T,bool> filters)
        {
            var temp = listToFilter.AsQueryable().Where(filters).Select(x => x).ToList();
            return temp;
        }
       
        
    }
}
