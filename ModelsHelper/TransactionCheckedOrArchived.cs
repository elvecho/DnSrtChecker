using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.ModelsHelper
{
    public class TransactionCheckedOrArchived
    {
        
        public string IdTransaction { get; set; }
        public string RtServerId { get; set; }
        public int RetailStoreId { get; set; }
        public int StoreGroupId { get; set; }
        public bool IsChecked { get; set; }
        public bool IsArchived { get; set; }
        public string CheckNote { get; set; }
    }
}
