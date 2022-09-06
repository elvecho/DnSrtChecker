using DnSrtChecker.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Models.ViewModels
{
    public class TransamissionsViewModel
    {
        public string SzRtServerId { get; set; }
        public int StoreId { get; set; }
        public int StoreGroupId { get; set; }
        public decimal TotalADE { get; set; }
        public decimal TotalRTServer { get; set; }
        public decimal TotalTP { get; set; }
       
    }
}
