using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Models.ViewModels.ModelsFunctions
{
    public class TransactionList
    {
        
        public DateTime? dPosDateTime { get; set; }
        
        
        public DateTime? DRtDateTime { get; set; }
        public int? LPosWorkstationNmbr { get; set; } //OK
        public int? LPosTaNmbr { get; set; }//OK
        //=============================================================
        [DisplayFormat(DataFormatString = "{0:0000}")]
        public int? LRtClosureNmbr { get; set; }

        [DisplayFormat(DataFormatString = "{0:0000}")]
        public int? LRtDocumentNmbr { get; set; }

        public string SzRtDocumentId { get; set; }

        //=============================================================

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public string DPosTransactionTurnover { get; set; }        
        public string SzRtDeviceId { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public string DRtTransactionTurnover { get; set; }
        public int? BRtNonCompliantFlag { get; set; }
        public int? BTransactionCheckedFlag { get; set; }
        public int? BTransactionArchivedFlag { get; set; }
        public int? LTransactionMismatchID { get; set; }
    }
}
