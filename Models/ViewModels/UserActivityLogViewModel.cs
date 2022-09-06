using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Models.ViewModels
{
    public class UserActivityLogViewModel
    {
        public long LUserActivityLogId { get; set; }
        public DateTime DUserActivityDateTime { get; set; }
        public string SzUserName { get; set; }
        public byte LUserActivityId { get; set; }
        public string SzRtDocumentId { get; set; }
        public string SzRtDocumentColumn { get; set; }
        public string SzOldValue { get; set; }
        public string SzNewValue { get; set; }
        public string SzRtServerId { get; set; }
        public int? LRtServerOperationId { get; set; }
        public int? LRtDeviceTransmissionId { get; set; }
        public string SzRtDeviceId { get; set; }
        public string SzTablename { get; set; }

        public  UserActivity LUserActivity { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public string ToStringChecked()
        {
            var flagValue ="Verificata";
            var res="";
            var oldValue = SzOldValue == "1" ? "Verificata" :SzOldValue=="0"? "Non Verificata":"";
            var newValue = SzNewValue == "1" ? "Verificata" : SzNewValue == "0" ? "Non Verificata" : "";
            if(oldValue=="" && string.IsNullOrEmpty(oldValue))
            {
              res= $"{SzUserName} ha modificato il valore di {flagValue}  a {newValue}";
            }
            else
            {
                res = $"{SzUserName} ha modificato il valore di {flagValue}  da  {oldValue} a {newValue}";

            }
            return res;// $"{SzUserName} ha modificato il valore di {flagValue}  da  {oldValue} a {newValue}";
        }
        public string ToStringArchived()
        {
            var flagValue =  "Archiviata";
            var res = "";
            var oldValue = SzOldValue == "1" ? "Archiviata" : SzOldValue=="0"?"Non Archiviata":"";
            var newValue = SzNewValue == "1" ? "Archiviata" :SzNewValue=="0"? "Non Archiviata":"";
            if(oldValue=="" && string.IsNullOrEmpty(oldValue))
            {
                res = $"{SzUserName} ha modificato il valore di {flagValue} a {newValue}";
            }
            else
            {
                res = $"{SzUserName} ha modificato il valore di {flagValue}  da  {oldValue} a {newValue}";
            }
            return res;//$"{SzUserName} ha modificato il valore di {flagValue}  da  {oldValue} a {newValue}";
        }

        public string ToStringNote()
        {
            var flagValue = "Note";
            return $"{SzUserName} ha modificato il valore di {flagValue}  da  {SzOldValue} a {SzNewValue}";
        }

        public string ToStringGlobal()
        {
            var res = "";
            if(SzRtDocumentColumn== "bTransactionCheckedFlag") { res = ToStringChecked(); }
            if (SzRtDocumentColumn == "bTransactionArchivedFlag") { res = ToStringArchived(); }
            if (SzRtDocumentColumn == "szTranscationCheckNote") { res = ToStringNote(); }
            return res;

        }

        
      

    }
}
