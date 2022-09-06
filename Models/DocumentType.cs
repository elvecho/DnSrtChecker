using System;
using System.Collections.Generic;

namespace DnSrtChecker.Models
{
    public partial class DocumentType
    {
        public DocumentType()
        {
            TransactionDocument = new HashSet<TransactionDocument>();
        }

        public int LDocumentTypeId { get; set; }
        public string SzDescription { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public virtual ICollection<TransactionDocument> TransactionDocument { get; set; }
    }
}
