using System;
using System.Collections.Generic;

namespace DnSrtChecker.Models
{
    public partial class TransactionAffiliation
    {
        public TransactionAffiliation()
        {
            TransactionDocument = new HashSet<TransactionDocument>();
            TransactionVat = new HashSet<TransactionVat>();
        }

        public string SzRtDocumentId { get; set; }
        public string SzRtServerId { get; set; }
        public int LRetailStoreId { get; set; }
        public int LStoreGroupId { get; set; }
        public DateTime? DBusinessDate { get; set; }
        public DateTime? DPosDateTime { get; set; }
        public int? LPosWorkstationNmbr { get; set; }
        public int? LPosTaNmbr { get; set; }
        public decimal? DPosTransactionTurnover { get; set; }
        public byte? LPosReceivedTransactionCounter { get; set; }
        public DateTime? DRtDateTime { get; set; }
        public string SzRtDeviceId { get; set; }
        public int? LRtClosureNmbr { get; set; }
        public int? LRtDocumentNmbr { get; set; }
        public decimal? DRtTransactionTurnover { get; set; }
        public byte? LRtReceivedTransactionCounter { get; set; }
        public bool? BRtNonCompliantFlag { get; set; }
        public int? LTransactionMismatchId { get; set; }
        public bool? BTransactionCheckedFlag { get; set; }
        public string SzTranscationCheckNote { get; set; }
        public bool? BTransactionArchivedFlag { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }
        public string SzUserName { get; set; }

        public virtual TransactionMismatch LTransactionMismatch { get; set; }
        public virtual RtServer RtServer { get; set; }
        public virtual ICollection<TransactionDocument> TransactionDocument { get; set; }
        public virtual ICollection<TransactionVat> TransactionVat { get; set; }
    }
}
