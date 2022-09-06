using System;
using System.Collections.Generic;
using System.Text;

namespace DnSrtChecker.Models.ViewModels
{
    public class RtServerTransmissionViewModel
    {
        public RtServerTransmissionViewModel()
        {
            RtServerTransmissionDetail = new HashSet<RtServerTransmissionDetailViewModel>();

        }
        public string SzRtServerId { get; set; }
        public int LRtServerOperationId { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public virtual ICollection<RtServerTransmissionDetailViewModel> RtServerTransmissionDetail { get; set; }
    }
}
