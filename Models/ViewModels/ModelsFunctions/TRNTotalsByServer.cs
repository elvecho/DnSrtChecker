using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Models.ViewModels.ModelsFunctions
{
    public class TRNTotalsByServer
    {
        public DateTime? DPosDateTime { get; set; }
        public string SzRtDeviceId { get; set; }
        public int? LRtClosureNmbr { get; set; }
        public decimal? TotalTP { get; set; }
        public decimal? TotalRT { get; set; }
    }
}
