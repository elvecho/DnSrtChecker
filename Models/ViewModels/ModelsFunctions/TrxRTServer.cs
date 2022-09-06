using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Models.ViewModels.ModelsFunctions
{
    public class TrxRTServer
    {
        public DateTime? DRtDeviceClosureDateTime { get; set; } //OK
        public string SzRtServerID { get; set; } //OK
        public string SzGroupDescription { get; set; }//OK

        private decimal? _total { get; set; }
        [NotMapped]

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]        
        public decimal? Total
        {
            get { return _total; }
            set
            {
                decimal total;
                decimal.TryParse(value.ToString(), out total);
                this._total = total;
            }
        }
        public int LRetailStoreId { get; set; }
        public int LStoreGroupId { get; set; }
        public int TrasnmissionError { get; set; }
        public string szRetailStoreDescription { get; set; }
        //TrasnmissionError
        /*
         szRtServerID                        seriale del server rt//ok
dRtDeviceClosureDateTime            business date                 //ok
lRetailstoreID                        codice negozio                //ok
szGroupDescription                    insegna                        //OK
bWarningFlag                        flag per differenze ?            //OK       
bOnDutyFlag                            se 1 il server è attivo       //OK   
bOnErrorFlag                        stato di errore del server rt    //OK
TotalADE                            totale giorno negozio Agenzia delle entrate
TotalTP                                totale giorno negozio delle casse TP.Net
TotalRT                                totale giorno negozio letto dal server RT
         */
        public int BOnDutyFlag { get; set; }
        public int BWarningFlag { get; set; }
        public int BOnErrorFlag { get; set; }
        //string per poi dividere su 100

        public string TotalADE { get; set; }
        public string TotalTP { get; set; }
        public string TotalRT { get; set; }
        //[NotMapped]
        //public bool IsExcluded { get; internal set; }
        //public Store L { get; set; }
        //public RtServerStatus RtServerStatus { get; set; }
    }
}
