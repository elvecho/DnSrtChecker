using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Models.ViewModels
{
    public class RtServerTransmissionDetailRtDataMapped
    {
        public long LRtDataId { get; set; }
        public string SzRtServerId { get; set; }
        public int LRtServerOperationId { get; set; }
        public int LRtDeviceTransmissionId { get; set; }
        public string SzRtDeviceId { get; set; }
        public decimal DVatRate { get; set; }
        public decimal? DVatAmount { get; set; }
        public string SzVatNature { get; set; }
        public string SzVatLegalReference { get; set; }
        public bool? BVatVentilation { get; set; }
        public decimal? DSaleAmount { get; set; }
        public decimal? DReturnAmount { get; set; }
        public decimal? DVoidAmount { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }
        public  RtServerTransmissionDetail RtServerTransmissionDetail { get; set; }
        public ICollection<int> ListLRtDeviceTransmissionId { get; set; }
        public ICollection<int> ListLRtServerOperationId { get; set; }
        
    }
}
