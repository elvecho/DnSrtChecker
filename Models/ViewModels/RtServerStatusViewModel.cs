using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DnSrtChecker.Models.ViewModels
{
    public class RtServerStatusViewModel
    {
        public string SzRtServerId { get; set; }
        public int LRetailStoreId { get; set; }
        public int LStoreGroupId { get; set; }
        public bool? BOnErrorFlag { get; set; }
        public string SzErrorDescription { get; set; }
        public bool? BVatVentilationFlag { get; set; }
        public short? LLastClosureNmbr { get; set; }
        public string SzLastCloseResult { get; set; }
        public short? LMemoryAvailable { get; set; }
        public decimal? _dGrandTotalAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DGrandTotalAmount { 
            get { return _dGrandTotalAmount/10; }
            set {
                 decimal tmp;
                 decimal.TryParse(value.ToString(), out tmp);
                _dGrandTotalAmount = tmp;
            }
        }
        public short? LPendingTransmissionNmbr { get; set; }
        public short? LPendingTransmissionDays { get; set; }
        public bool? BRunningTransmissionFlag { get; set; }
        public short? LTransmissionScheduleMinutesLeft { get; set; }
        public short? LTransmissionScheduleHoursRepeat { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? DLastDateTimeRead { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? DLastDateTimeCollected { get; set; }
        public DateTime? DLastDateTimeTransactionsCollected { get; set; }
        public DateTime? DLastDateTimeTransmissionsCollected { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? DLastUpdateLocal { get; set; }
        public bool? BWarningFlag { get; set; }
        public RtServerViewModel RtServer { get; set; }
    }
}
