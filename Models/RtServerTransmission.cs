using System;
using System.Collections.Generic;

namespace DnSrtChecker.Models
{
    public partial class RtServerTransmission
    {
        public RtServerTransmission()
        {
            RtServerTransmissionDetail = new HashSet<RtServerTransmissionDetail>();
        }

        public string SzRtServerId { get; set; }
        public int LRtServerOperationId { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public virtual ICollection<RtServerTransmissionDetail> RtServerTransmissionDetail { get; set; }
    }
}
