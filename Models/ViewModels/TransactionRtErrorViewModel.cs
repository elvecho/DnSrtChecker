using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DnSrtChecker.Models.ViewModels
{
    public class TransactionRtErrorViewModel
    {
        public long LRtErrorId { get; set; }
        public string SzRtServerId { get; set; }
        public int LRetailStoreId { get; set; }
        public int LStoreGroupId { get; set; }
        public string SzRtDeviceId { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? DRtDateTime { get; set; }
        public int? LRtClosureNmbr { get; set; }
        public int? LRtDocumentNmbr { get; set; }
        public string SzDescription { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]

        public DateTime? DLastUpdateLocal { get; set; }

        public  RtServerViewModel RtServer { get; set; }
    }
}
