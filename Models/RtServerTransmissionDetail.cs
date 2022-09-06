using System;
using System.Collections.Generic;

namespace DnSrtChecker.Models
{
    public partial class RtServerTransmissionDetail
    {
        public RtServerTransmissionDetail()
        {
            RtServerTransmissionDetailRtData = new HashSet<RtServerTransmissionDetailRtData>();
            RtServerTransmissionDetailRtReport = new HashSet<RtServerTransmissionDetailRtReport>();
        }

        public string SzRtServerId { get; set; }
        public int LRtServerOperationId { get; set; }
        public int LRtDeviceTransmissionId { get; set; }
        public string SzRtDeviceId { get; set; }
        public string SzRtDeviceType { get; set; }
        public string SzRtTransmissionFormat { get; set; }
        public DateTime? DRtInactivityDateTimeFrom { get; set; }
        public DateTime? DRtInactivityDateTimeTo { get; set; }
        public DateTime? DRtDeviceClosureDateTime { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }
        public bool? BTransactionCheckedFlag { get; set; }
        public string SzTranscationCheckNote { get; set; }
        public bool? BTransactionArchivedFlag { get; set; }
        public string SzUserName { get; set; }

        public virtual RtServerTransmission RtServerTransmission { get; set; }
        public virtual ICollection<RtServerTransmissionDetailRtData> RtServerTransmissionDetailRtData { get; set; }
        public virtual ICollection<RtServerTransmissionDetailRtReport> RtServerTransmissionDetailRtReport { get; set; }

    }
}
