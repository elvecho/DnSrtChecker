using DnSrtChecker.Helpers;
using DnSrtChecker.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.ModelsHelper
{
    public class TransmissionsByDayToIndexView
    {
        public string SzRtServerId { get; set; }
        public string LRetailStoreId { get; set; }
        public StoreViewModel Store { get; set; }
       
        public string StoreGroupId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OperationClosureDatetime { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? GTotalAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? GTotalRtServer { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? GTotalTP { get; set; }
        [DisplayFormat(DataFormatString = "({0:C2})", ApplyFormatInEditMode = true)]
        public decimal? GTotalMismatchRT { get { return GTotalAmount - GTotalRtServer; } }
        [DisplayFormat(DataFormatString = "({0:C2})", ApplyFormatInEditMode = true)]
        public decimal? GTotalMismatchTP { get { return GTotalAmount - GTotalTP; } }
        public bool? BTransactionCheckedFlag { get; set; }
        public List<string> SzTranscationCheckNote { get; set; }
        public bool? BTransactionArchivedFlag { get; set; }
        public List<string> SzUserName { get; set; }
        public List<TransmissionsAndTransactionsGroupedByDayVM> TransmissionsGroupedByDay { get; set; }
        public List<UserActivityLogViewModel> UserActivityLogViewModel { get; set; }


    }
}
