using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Models.ViewModels
{
    public class StoreViewModel
    {
        public StoreViewModel()
        {
            RtServer = new HashSet<RtServerViewModel>();
        }

        public int LRetailStoreId { get; set; }
        public int LStoreGroupId { get; set; }
        public string SzDescription { get; set; }
        public string StoreNameComplet
        {
            get
            {
                return string.Format("({0}) {1}", LRetailStoreId, SzDescription);
            }
        }

        public DateTime? DLastUpdateLocal { get; set; }
        public  StoreGroupViewModel LStoreGroup { get; set; }
        [JsonIgnore]
        public  ICollection<RtServerViewModel> RtServer { get; set; }

        
    }
}
