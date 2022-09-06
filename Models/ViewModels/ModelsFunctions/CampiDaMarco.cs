using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Models.ViewModels.ModelsFunctions
{
    public class CampiDaMarco
    {
        public int lRetailstoreID { get; set; }
        public int lPosWorkstationNmbr { get; set; }
        public DateTime dRtDeviceClosureDateTime { get; set; }
        public string szRtServerID { get; set; }
        public int lRtDeviceTransmissionID { get; set; }
        public string szRtDeviceID { get; set; }
        public int LRtServerOperationId { get; set; }
        public decimal dvatrate { get; set; }
        public string SzVatNature { get; set; }
        public string DSaleAmount { get; set; }
        public string DReturnAmount { get; set; }
        public string DvoidAmount { get; set; }
        public string DVatAmount { get; set; }
        public string TotalADE { get; set; }
        public string TotalTP { get; set; }
    }
}
