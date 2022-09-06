using DnSrtChecker.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Models.ViewModels.ModelsFunctions
{
    public class TransmissionsList 
    {

        public int lRetailstoreID { get; set; }
        public int lPosWorkstationNmbr { get; set; }
        public DateTime dRtDeviceClosureDateTime { get; set; }
        public string szRtServerID { get; set; }
        public int lRtDeviceTransmissionID { get; set; }
        public string szRtDeviceID { get; set; }
        public int LRtServerOperationId { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal dvatrate { get; set; }
        public string SzVatNature { get ; set; }

        //public decimal DTotalRT { get { return string.IsNullOrEmpty(TotalRT) ?
        //0: Convert.ToDecimal(TotalRT) / 100; } }


        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]

        public string DSaleAmount { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal SaleAmountD
        {
            get{
                return Convert.ToDecimal(DSaleAmount)/100;
            }
        }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]

        public string DReturnAmount { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal ReturnAmountD
        {
            get
            {
                return Convert.ToDecimal(DReturnAmount) / 100;
            }
        }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]

        public string DvoidAmount { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal voidAmountD
        {
            get
            {
                return Convert.ToDecimal(DvoidAmount) / 100;
            }
        }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]

        public string DVatAmount { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]

        public decimal VatAmountD
        {
            get
            {
                return Convert.ToDecimal(DVatAmount) / 100;
            }
        }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]

        public string TotalADE { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]

        public decimal TotalADED
        {
            get
            {
                return Convert.ToDecimal(TotalADE) / 100;
            }
        }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]

        public string TotalTP { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]

        public decimal TotalTPD
        {
            get
            {
                return Convert.ToDecimal(TotalTP) / 100;
            }
        }

        [NotMapped]
        [DisplayFormat(DataFormatString = "({0:C2})", ApplyFormatInEditMode = true)]
        public decimal GTotalMismatchRT { get { return Convert.ToDecimal(DSumAmount) -
                    Convert.ToDecimal(TotalADE); } }

        [NotMapped]
        [DisplayFormat(DataFormatString = "({0:C2})", ApplyFormatInEditMode = true)]
        public decimal GTotalMismatchTP { get { return Convert.ToDecimal(DSumAmount)
                    - Convert.ToDecimal(TotalTP); } }
        [NotMapped]
        public bool BTransactionCheckedFlag { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = " ({0:C2})", ApplyFormatInEditMode = true)]
        public decimal MismatchTPAdE { get { return DTotalADESum
                    - TotalTPSum; } }

        [NotMapped]
        [DisplayFormat(DataFormatString = " ({0:C2})", ApplyFormatInEditMode = true)]
        public decimal MismatchRtAdE { get { return DSumAmount -
                    Convert.ToDecimal(TotalADE); } set { } }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal DSumAmount
        {
            get { return (Convert.ToDecimal(DSaleAmount) + Convert.ToDecimal(DVatAmount)
                    - Convert.ToDecimal(DReturnAmount) - Convert.ToDecimal(DvoidAmount)) / 100; }

        }
        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal DReturnAmountSum { get; set; }
        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal DSaleAmountSum { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]

        public decimal DvoidAmountSum { get; set; }
        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]


        public decimal DVatAmountSum { get; set; }
        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]


        public decimal DTotalADESum { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]

        public decimal TotalTPSum { get; set; }
        /*
         <td>@Html.DisplayFor(x => c.DSaleAmount)</td>
                                <td>@Html.DisplayFor(x => c.DReturnAmount)</td>
                                <td>@Html.DisplayFor(x => c.DvoidAmount)</td>
                                <td>@Html.DisplayFor(x => c.DVatAmount)</td>
                                <td>@Html.DisplayFor(x => c.TotalADE)</td>
                                <td>@Html.DisplayFor(x => c.TotalTP)</td>
         */
        //public List<string> SzTranscationCheckNote { get; set; }
        //public bool? BTransactionArchivedFlag { get; set; }


        //public string SzRtServerId { get; set; }
        //public string LRetailStoreId { get; set; } //ok
        ////public StoreViewModel Store { get; set; }

        ////public string StoreGroupId { get; set; }

        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        //public DateTime? dRtDeviceClosureDateTime { get; set; }
        //[DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        //public decimal? DSaleAmount { get; set; }
        //[DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        //public decimal? TotalRtServer { get; set; }
        //[DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        //public decimal? TotalTP { get; set; }
        //[DisplayFormat(DataFormatString = "({0:C2})", ApplyFormatInEditMode = true)]
        //public decimal? TotalMismatchRT { get { return DSaleAmount - TotalRtServer; } }
        //[DisplayFormat(DataFormatString = "({0:C2})", ApplyFormatInEditMode = true)]
        //public decimal? TotalMismatchTP { get { return DSaleAmount - TotalTP; } }
        ////public bool? BTransactionCheckedFlag { get; set; }
        ////public List<string> SzTranscationCheckNote { get; set; }
        ////public bool? BTransactionArchivedFlag { get; set; }
        ////public List<string> SzUserName { get; set; }
        ////public List<TransmissionsAndTransactionsGroupedByDayVM> TransmissionsGroupedByDay { get; set; }
        ////public List<UserActivityLogViewModel> UserActivityLogViewModel { get; set; }
    }
}
