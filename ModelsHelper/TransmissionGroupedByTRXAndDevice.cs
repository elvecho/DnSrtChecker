using DnSrtChecker.Models;
using DnSrtChecker.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.ModelsHelper
{
     public class TransmissionGroupedByTRXAndDevice
    {
        public int TransmissionId { get; set; }
        public string DeviceId { get; set; }
        public string LPosWorkstationNmbr { get; set; }
        public List<RtServerTransmissionDetailViewModel> RtServerTransmissionsDetail { get; set; }

        public List<RtServerTransmissionDetailRtData> RtServerTransmissionsDetailRtData { get; set; }
    }
}
