using System;
using System.Collections.Generic;

namespace DnSrtChecker.Models
{
    public partial class TransactionMismatch
    {
        public TransactionMismatch()
        {
            TransactionAffiliation = new HashSet<TransactionAffiliation>();
        }

        public int LTransactionMismatchId { get; set; }
        public string SzDescription { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public virtual ICollection<TransactionAffiliation> TransactionAffiliation { get; set; }
    }
}
