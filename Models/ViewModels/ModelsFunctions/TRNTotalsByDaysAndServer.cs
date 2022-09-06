using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Models.ViewModels.ModelsFunctions
{
    public class TRNTotalsByDaysAndServer:TRNTotalsByServer
    {
       

        public string SzVatCodeId { get; set; }
        public decimal? DPosVatRate { get; set; }
        public decimal? DPosGrossAmountTotale { get; set; }
        public decimal? DRtGrossAmountTotale { get; set; }
        public string SzVatNature { get; set; }

       

    }
}
