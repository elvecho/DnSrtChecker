using DnSrtChecker.ModelsHelper;
using DnSrtChecker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DnSrtChecker.Models.ViewModels
{
    public class TransactionByServerRtForTransmissions
    {
        public string RtServerId { get; set; }
        public string RtDeviceId { get; set; }
        public string LPosWorkstationNmbr { get; set; }
        public int? ClosureNumber { get; set; }
        public string ClosureNumberS { get; set; }
        public DateTime? DRtDateTime { get; set; }
        public DateTime? DPosDateTime { get; set; }
        public List<TransactionAffiliation> TransactionAffiliations { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? TotalServerRT { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? TotalTP { get; set; }
        public IEnumerable<TransactionVatForTrasmissionsViewModel> TransactionVats { get; set; }
        public IEnumerable<TransactionVatForTrasmissionsViewModel> TransactionVatsEsen { get; set; }

        public IEnumerable<TransactionAffiliationViewModel> TransactionAffiliationsChecked { get; set; }

    }
}
