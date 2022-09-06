using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DnSrtChecker.Models.ViewModels
{
    public class RtServersHomeViewModel
    {
        public string SzRtServerId { get; set; }
        public int LRetailStoreId { get; set; }
        public int LStoreGroupId { get; set; }
        public bool? BOnDutyFlag { get; set; }
        public string IsChecked { get; set; }
        public string NonCompliantOrHasMismatch { get; set; }
        public RtServerStatusViewModel RtServerStatus { get; set; }
        public bool? BOnError { get; set; }
        public bool? NonCompliant { get; set; }
        public bool TrasnmissionError { get; set; }
        public bool TrasnmissionChecked { get; set; }
        public StoreViewModel L { get; set; }
        //Edit Home recap Transactions and Trasmissions of last closure
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? DateLastClosure { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? TotalRT { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? TotalTP { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? TotalADE { get; set; }
        public bool IsExcluded { get; set; }
    }
}
