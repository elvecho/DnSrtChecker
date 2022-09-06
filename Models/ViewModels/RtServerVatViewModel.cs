using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Models.ViewModels
{
    public class RtServerVatViewModel
    {
        public RtServerVatViewModel()
        {
            TransactionVat = new HashSet<TransactionVatViewModel>();
        }

        public string SzVatCodeId { get; set; }
        public string SzVatNature { get; set; }
        public string SzDescription { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public  ICollection<TransactionVatViewModel> TransactionVat { get; set; }
    }
}
