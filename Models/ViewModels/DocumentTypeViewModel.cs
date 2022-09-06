using System;
using System.Collections.Generic;
using System.Text;

namespace DnSrtChecker.Models.ViewModels
{
    public class DocumentTypeViewModel
    {
        public DocumentTypeViewModel()
        {
            TransactionDocument = new HashSet<TransactionDocumentViewModel>();
        }

        public int LDocumentTypeId { get; set; }
        public string SzDescription { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }
        public ICollection<TransactionDocumentViewModel> TransactionDocument { get; set; }
    }
}
