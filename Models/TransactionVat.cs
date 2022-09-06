using System;
using System.Collections.Generic;

namespace DnSrtChecker.Models
{
    public partial class TransactionVat
    {
        public string SzRtDocumentId { get; set; }
        public string SzVatCodeId { get; set; }
        public decimal? DPosVatRate { get; set; }
        public decimal? DPosGrossAmount { get; set; }
        public decimal? DPosNetAmount { get; set; }
        public decimal? DPosVatAmount { get; set; }
        public decimal? DRtVatRate { get; set; }
        public decimal? DRtGrossAmount { get; set; }
        public decimal? DRtNetAmount { get; set; }
        public decimal? DRtVatAmount { get; set; }
        public bool? BVatMismatchFlag { get; set; }
        public bool? BVatCheckedFlag { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public virtual TransactionAffiliation SzRtDocument { get; set; }
        public virtual RtServerVat SzVatCode { get; set; }
    }
}
