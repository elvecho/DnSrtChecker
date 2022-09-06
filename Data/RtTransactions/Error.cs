using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace DnSrtChecker.Data.RtTransactions
{
    public class Error
    {
        [XmlElement("PuntoCassa")]
        public string DeviceId { get; set; }

        [XmlElement("Descrizione")]
        public string Description { get; set; }

        [XmlElement("Documento")]
        public Document Document { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
