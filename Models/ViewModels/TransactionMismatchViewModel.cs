using System;
using System.Collections.Generic;
using System.Text;

namespace DnSrtChecker.Models.ViewModels
{
    public class TransactionMismatchViewModel
    {
        public TransactionMismatchViewModel()
        {
            TransactionAffiliation = new HashSet<TransactionAffiliationViewModel>();
        }

        public int LTransactionMismatchId { get; set; }
        public string SzDescription { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public  ICollection<TransactionAffiliationViewModel> TransactionAffiliation { get; set; }
    }
}
