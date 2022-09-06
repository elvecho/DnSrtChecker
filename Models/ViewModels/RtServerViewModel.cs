using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DnSrtChecker.Models.ViewModels
{
    public class RtServerViewModel
    {
        public RtServerViewModel()
        {
            TransactionAffiliation = new HashSet<TransactionAffiliationViewModel>();
            TransactionRtError = new HashSet<TransactionRtErrorViewModel>();
        }

        public string SzRtServerId { get; set; }
        public int LRetailStoreId { get; set; }
        public int LStoreGroupId { get; set; }
        public string szRetailStoreDescription { get; set; }
        public string SzGroupDescription { get; set; }
        public string SzLocation { get; set; }
        public string SzIpAddress { get; set; }
        public string SzUsername { get; set; }
        public string SzPassword { get; set; }
        public bool? BOnDutyFlag { get; set; }
        public bool? BWarningFlag { get; set; }
        public bool? BOnErrorFlag { get; set; }
        
        public int TrasnmissionError { get; set; }
        //ADE,TP, RT in string per poi dividere su 100
        public string TotalADE { get; set; }
        public string TotalTP { get; set; }
        public string TotalRT { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal DTotalRT { get { return string.IsNullOrEmpty(TotalRT) ? 0: Convert.ToDecimal(TotalRT) / 100; } }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal DTotalADE { get { return string.IsNullOrEmpty(TotalADE) ? 0 : Convert.ToDecimal(TotalADE) / 100; } }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal DTotalTP { get { return string.IsNullOrEmpty(TotalTP) ? 0 : Convert.ToDecimal(TotalTP) / 100; } }
       
        public DateTime? DRtDeviceClosureDateTime { get; set; }
        //public bool IsExcluded { get; internal set; }
        public StoreViewModel L { get; set; }
        public  RtServerStatusViewModel RtServerStatus { get; set; }
        [JsonIgnore]
        public ICollection<TransactionAffiliationViewModel> TransactionAffiliation { get; set; }
        [JsonIgnore]
        public  ICollection<TransactionRtErrorViewModel> TransactionRtError { get; set; }
        
    }
}