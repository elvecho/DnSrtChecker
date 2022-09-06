using DnSrtChecker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.ModelsHelper
{
    public class RtServersHome
    {
        public string SzRtServerId { get; set; }
        public int LRetailStoreId { get; set; }
        public int LStoreGroupId { get; set; }
        public bool? BOnDutyFlag { get; set; }
        public string IsChecked { get; set; }
        public bool TransmissionChecked { get; set; }
        public string NonCompliantOrHasMismatch { get; set; }
        public RtServerStatus RtServerStatus { get; set; }
        public bool? BOnError { get; set; }
        public bool? NonCompliant { get; set; }
        public bool TrasnmissionError { get; set; }
        public string SzRtDeviceId { get; set; }
        public Store L { get; set; }
        //Edit Home recap Transactions and Trasmissions of last closure
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? DateLastClosure { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? TotalRT { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? TotalTP { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? TotalADE { get; set; }
        public int? LRtDocumentNmbr { get; set; }
        public bool? TransactionChecked { get; set; }
    }
}
