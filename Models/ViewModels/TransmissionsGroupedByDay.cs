using DnSrtChecker.Models;
using DnSrtChecker.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Models.ViewModels
{
    public class TransmissionsGroupedByDay
    {
        public string SzRtServerId { get; set; }
        public DateTime? OperationClosureDatetime { get; set; }
        public DateTime? LastClosureDateTime { get; set; }
        public int? LRtDeviceTransmissionId { get; set; }
        public string LRtDeviceTransmissionIdS { get; set; }
        public int LRtServerOperationId { get; set; }
        public string LRtServerOperationIdS { get; set; }
        public string SzRtDeviceId { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TotalRtServer { get; set; }
        public decimal? TotalTP { get; set; }
        public List<bool?> BTransactionCheckedFlag { get; set; }
        public List<string> SzTranscationCheckNote { get; set; }
        public List<bool?> BTransactionArchivedFlag { get; set; }
        public List<string> SzUserName { get; set; }
        public List<RtServerTransmissionDetail> RtServerTransmissionsDetail { get; set; }
        public List<RtServerTransmissionDetailRtData> RtServerTransmissionsDetailRtData { get; set; }
        public List<RtServerTransmissionDetailRtReport> RtServerTransmissionsDetailRtReport { get; set; }

    }
}
