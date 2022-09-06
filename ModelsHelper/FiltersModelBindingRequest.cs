using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.FiltersmodelBindRequest
{
    public class FiltersmodelBindingRequest
    {
        public string StoreGroup { get; set; }
        public string Store { get; set; }
        public int StoreGroupID { get; set; }
        public int StoreID { get; set; }
        public string ServerRt { get; set; }
        public string Status { get; set; }
        public string NonCompliant { get; set; }
        public string Error { get; set; }
        public string TransactionDateFrom { get; set; }
        public string TransactionDateTo { get; set; }
        public string TransmissionDateFrom { get; set; }
        public string TransmissionDateTo { get; set; }
        public string PosWorkstationNmbr { get; set; }        
        public string  PosTaNmbr { get; set; } //numero transazione di cassa
        //public string IsChecked { get; set; }
        public string NonCompliantOrHasMismatch { get; set; }
        public string RtClosureNmbr { get; set; }
        public string  RtDocumentNmbr { get; set; }
        public string _isChecked { get; set; }
        public string _isArchived { get; set; }
        public string IsCheckedOrArchived { get; set; }
        public string IsArchived { get { return _isArchived; }
            set { if (IsCheckedOrArchived == "isArchived") 
                { value = "true"; this._isArchived = value; } 
                }
        }
        public string IsChecked { get; set; }
        public string Conformity { get; set; }
        public string DayFilter { get; set; }

        public static string UserName { get; set; }
    }
}
