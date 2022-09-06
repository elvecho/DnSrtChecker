using System;
using System.Collections.Generic;
using System.Text;

namespace DnSrtChecker.Models.ViewModels
{
    public class RtServersTransactionsGroupedByStoreAndStoreGrpViewModel
    {
        public int LRetailStoreId { get; set; }
        public int LStoreGroupId { get; set; }
        public List<RtServerViewModel> RtServers { get; set; }
    }
}
