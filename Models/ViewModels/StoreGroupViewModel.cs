using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Models.ViewModels
{
    public class StoreGroupViewModel
    {
        public StoreGroupViewModel()
        {
            Store = new HashSet<StoreViewModel>();
        }

        public int LStoreGroupId { get; set; }
        public string SzDescription { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }
        [JsonIgnore]
        public  ICollection<StoreViewModel> Store { get; set; }
       
    }
}
