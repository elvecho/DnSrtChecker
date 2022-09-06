using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace DnSrtChecker.Data.RtTransactions
{
    [XmlRoot("RichiestaMemDettaglio")]
    public class MemoryDetailRequest
    {
        [XmlElement("PuntoCassa")]
        public string DeviceId { get; set; }

        [XmlElement("DataDa")]
        public string DateFrom { get; set; }

        [XmlElement("DataA")]
        public string DateTo { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
