using DnSrtChecker.Services;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DnSrtChecker.Models.ViewModels
{
    public class TransactionAffiliationViewModel
    {
        public TransactionAffiliationViewModel()
        {
            TransactionDocument = new HashSet<TransactionDocumentViewModel>();
            TransactionVat = new HashSet<TransactionVatViewModel>();
        }


        public string SzRtDocumentId { get; set; }
        public string SzRtServerId { get; set; }
        public int LRetailStoreId { get; set; }
        public int LStoreGroupId { get; set; }
        public int LRtServerOperationId { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? DBusinessDate { get; set; }
        public DateTime? DPosDateTime { get; set; }
        public int? LPosWorkstationNmbr { get; set; }
        public int? LPosTaNmbr { get; set; }
        public decimal? _dPosTransactionTurnover { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DPosTransactionTurnover {
            get {
                return (_dPosTransactionTurnover * (LPosReceivedTransactionCounter ?? 1));
            }
            set {

                this._dPosTransactionTurnover = Convert.ToDecimal(value);
            }
        }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? DRtDateTime { get; set; }
        public string SzRtDeviceId { get; set; }
        [DisplayFormat(DataFormatString = "{0:0000}")]
        public int? LRtClosureNmbr { get; set; }
        [DisplayFormat(DataFormatString = "{0:0000}")]
        public int? LRtDocumentNmbr { get; set; }
        public decimal? _dRtTransactionTurnover { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DRtTransactionTurnover {
            get { return (_dRtTransactionTurnover * (LRtReceivedTransactionCounter ?? 1)); }
            set {
                decimal dRtTransactionTurnover;
                decimal.TryParse(value.ToString(), out dRtTransactionTurnover);
                this._dRtTransactionTurnover = dRtTransactionTurnover;
            }
        }
        public bool? BRtNonCompliantFlag { get; set; }
        public bool? NonCompliantFlag {
            get
            {
               
                if ((BRtNonCompliantFlag == true && !LTransactionMismatchId.HasValue)
                       || (TransactionHasMismatch)) {
                    return true;
                }
                else { return false; }
            }
        }
        public bool errorNonCompliant { get; set; } 
        public bool HasError
        {
            get
            {
                return RtServer.TransactionRtError.Where(x => x.SzRtDeviceId == SzRtDeviceId && x.LRtDocumentNmbr == LRtDocumentNmbr).Count() > 0;
            }
        }
        public int? LTransactionMismatchId { get; set; }
        public bool TransactionHasMismatch
        {
            get
            {

                if (LTransactionMismatchId.HasValue
                    && (
                    (LTransactionMismatchId == 1 || LTransactionMismatchId == 2
                    || LTransactionMismatchId == 5 || LTransactionMismatchId == 6)
                    || (LTransactionMismatchId == 3 && DRtDateTime.Value.Date < DateTime.Today.Date)
                    || (LTransactionMismatchId == 4 && DPosDateTime.Value.Date < DateTime.Today.Date)
                    || (
                    (LTransactionMismatchId!=3 && LTransactionMismatchId!=4)&&
                    (DPosTransactionTurnover != DRtTransactionTurnover))
                        && ((!DPosDateTime.HasValue || DPosDateTime.Value.Date < DateTime.Today.Date)
                        && (!DRtDateTime.HasValue || DRtDateTime.Value.Date < DateTime.Today.Date))

                    )
                    )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool TransactionIsChecked
        {
            get
            {

                if ((TransactionHasMismatch == true
                    || NonCompliantFlag == true
                    || errorNonCompliant == true) && BTransactionCheckedFlag != true)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public bool TransactionNonConformOrHasMismatch { 
            get {
                if ((TransactionHasMismatch == true
                       || NonCompliantFlag == true
                       || errorNonCompliant == true) )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool TransactionIsArchived
        {
            get
            {
                // if (BTransactionCheckedFlag==true && ( TransactionHasMismatch==false && BRtNonCompliantFlag!=true))
                //return TransactionHasMismatch == false
                //        && BRtNonCompliantFlag != true
                //        && errorNonCompliant!=true
                //        && BTransactionCheckedFlag != false;
                if ((TransactionHasMismatch == true
                    || NonCompliantFlag == true
                    || errorNonCompliant == true) && BTransactionArchivedFlag != true)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public bool? BTransactionCheckedFlag { get; set; }
        public string SzTranscationCheckNote { get; set; }
        public bool? BTransactionArchivedFlag { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }
        public string DocumentRT {
            get {
                return string.Format("{0:0000} {1:0000}", LRtClosureNmbr, LRtDocumentNmbr);
            }
        }
        public byte? LPosReceivedTransactionCounter { get; set; }
        public byte? LRtReceivedTransactionCounter { get; set; }
        public string SzUserName { get; set; }
        public string LogDescription { get; set; }
        public List<UserActivityLogViewModel>  UserActivityChecked{get;set; }
        public List<UserActivityLogViewModel> UserActivityArchived { get; set; }
        public List<UserActivityLogViewModel> UserActivityNote { get; set; }
        public List<UserActivityLogViewModel> UserActivityLogViewModel { get; set; }
        public TransactionMismatchViewModel LTransactionMismatch { get; set; }
        public  RtServerViewModel RtServer { get; set; }
        public  ICollection<TransactionDocumentViewModel> TransactionDocument { get; set; }

        public ICollection<TransactionVatViewModel> TransactionVat { get;set;}

       public Store store { get; set; }
       public StoreGroup storeGroup { get;set;}
    }
}
