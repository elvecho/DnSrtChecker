using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DnSrtChecker.Models.ViewModels
{
    public class RtServerTransmissionDetailViewModel
    {
        public RtServerTransmissionDetailViewModel()
        {
            RtServerTransmissionDetailRtData = new HashSet<RtServerTransmissionDetailRtDataViewModel>();
            RtServerTransmissionDetailRtReport = new HashSet<RtServerTransmissionDetailRtReportViewModel>();
        }

        public string SzRtServerId { get; set; }
        public int LRtServerOperationId { get; set; }
        public int LRtDeviceTransmissionId { get; set; }
        public string SzRtDeviceId { get; set; }
       
        public string SzRtDeviceType { get; set; }
        public string SzRtTransmissionFormat { get; set; }
        public DateTime? DRtInactivityDateTimeFrom { get; set; }
        public DateTime? DRtInactivityDateTimeTo { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? DRtDeviceClosureDateTime { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }
        public bool? BTransactionCheckedFlag { get; set; }
        public string SzTranscationCheckNote { get; set; }
        public bool? BTransactionArchivedFlag { get; set; }
        public string SzUserName { get; set; }
        public string LogDescription { get; set; }
        public List<UserActivityLogViewModel> UserActivityChecked { get; set; }
        public List<UserActivityLogViewModel> UserActivityArchived { get; set; }
        public List<UserActivityLogViewModel> UserActivityNote { get; set; }
        public List<UserActivityLogViewModel> UserActivityLogViewModel { get; set; }
        public TransactionMismatchViewModel LTransactionMismatch { get; set; }

        public  RtServerTransmissionViewModel RtServerTransmission { get; set; }
        public  ICollection<RtServerTransmissionDetailRtDataViewModel> RtServerTransmissionDetailRtData { get; set; }
        public  ICollection<RtServerTransmissionDetailRtReportViewModel> RtServerTransmissionDetailRtReport { get; set; }

    }
}

