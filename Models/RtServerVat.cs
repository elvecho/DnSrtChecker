using System;
using System.Collections.Generic;

namespace DnSrtChecker.Models
{
    public partial class RtServerVat
    {
        public RtServerVat()
        {
            TransactionVat = new HashSet<TransactionVat>();
        }

        public string SzVatCodeId { get; set; }
        public string SzVatNature { get; set; }
        public string SzDescription { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public virtual ICollection<TransactionVat> TransactionVat { get; set; }
    }
}
