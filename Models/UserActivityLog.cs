using System;
using System.Collections.Generic;

namespace DnSrtChecker.Models
{
    public partial class UserActivityLog
    {
        public long LUserActivityLogId { get; set; }
        public DateTime DUserActivityDateTime { get; set; }
        public string SzUserName { get; set; }
        public byte LUserActivityId { get; set; }
        public string SzRtDocumentId { get; set; }
        public string SzRtDocumentColumn { get; set; }
        public string SzOldValue { get; set; }
        public string SzNewValue { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }
        public string SzRtServerId { get; set; }
        public int? LRtServerOperationId { get; set; }
        public int? LRtDeviceTransmissionId { get; set; }
        public string SzRtDeviceId { get; set; }
        public string SzTablename { get; set; }

        public virtual UserActivity LUserActivity { get; set; }


    }
}
