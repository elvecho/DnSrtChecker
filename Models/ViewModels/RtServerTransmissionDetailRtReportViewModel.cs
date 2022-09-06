using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DnSrtChecker.Models.ViewModels
{
    public class RtServerTransmissionDetailRtReportViewModel
    {
        public long LRtReportId { get; set; }
        public string SzRtServerId { get; set; }
        public int LRtServerOperationId { get; set; }
        public int LRtDeviceTransmissionId { get; set; }
        public string SzRtDeviceId { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? DEventDateTime { get; set; }
        public string SzEventType { get; set; }
        public string SzEventNote { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public virtual RtServerTransmissionDetail RtServerTransmissionDetail { get; set; }

    }
}
