using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DnSrtChecker.Models.ViewModels
{
    public class RtServersTransactionsListViewModel
    {
        public RtServersTransactionsListViewModel()
        {
            TransactionAffiliation = new HashSet<TransactionAffiliationViewModel>();
            TransactionRtError = new HashSet<TransactionRtErrorViewModel>();
        }

        public string SzRtServerId { get; set; }
        public int LRetailStoreId { get; set; }
        public int LStoreGroupId { get; set; }
        public string SzLocation { get; set; }
        public string SzIpAddress { get; set; }
        public string SzUsername { get; set; }
        public string SzPassword { get; set; }
        public bool BOnDutyFlag { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public virtual StoreViewModel L { get; set; }
        public virtual RtServerStatusViewModel RtServerStatus { get; set; }
        [JsonIgnore]
        public virtual ICollection<TransactionAffiliationViewModel> TransactionAffiliation { get; set; }
        [JsonIgnore]
        public virtual ICollection<TransactionRtErrorViewModel> TransactionRtError { get; set; }
    }
}
