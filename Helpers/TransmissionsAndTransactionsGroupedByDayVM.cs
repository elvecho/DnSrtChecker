using DnSrtChecker.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Helpers
{
    public class TransmissionsAndTransactionsGroupedByDayVM
    {
        public string SzRtServerId { get; set; }
        public DateTime? OperationClosureDatetime { get; set; }
        public int? LRtDeviceTransmissionId { get; set; }
        public string LRtDeviceTransmissionIdS { get; set; }
        public int LRtServerOperationId { get; set; }
        public string LRtServerOperationIdS { get; set; }
        public string SzRtDeviceId { get; set; }
        public string LPosWorkstation { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? TotalAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? TotalRtServer { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? TotalTP { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? TotalSale { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? TotalVoid { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? TotalReturn { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? TotalVat { get; set; }
        [DisplayFormat(DataFormatString = " ({0:C2})", ApplyFormatInEditMode = true)]
        public decimal? MismatchTPAdE { get { return TotalAmount-TotalTP; } }
        [DisplayFormat(DataFormatString = " ({0:C2})", ApplyFormatInEditMode = true)]
        public decimal? MismatchRtAdE { get { return TotalAmount - TotalRtServer; } set { } }
        public bool HasVentilation { get; set; }
        public List<bool?> BTransactionCheckedFlag { get; set; }
        public List<string> SzTranscationCheckNote { get; set; }
        public List<bool?> BTransactionArchivedFlag { get; set; }
        public List<string> SzUserName { get; set; }

        public List<RtServerTransmissionDetailViewModel> RtServerTransmissionsDetail { get; set; }
        public List<TransactionVatForTrasmissionsViewModel> TransactionVats { get; set; }
        public List<RtServerTransmissionDetailRtDataViewModel> RtServerTransmissionsDetailRtData { get; set; }
        public List<RtServerTransmissionDetailRtReportViewModel> RtServerTransmissionsDetailRtReport { get; set; }

    }
}
