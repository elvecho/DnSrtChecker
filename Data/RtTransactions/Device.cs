using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace DnSrtChecker.Data.RtTransactions
{
    public class Device
    {
        [XmlElement("Marchio")]
        public string Brand { get; set; }

        [XmlElement("Modello")]
        public string Model { get; set; }

        [XmlElement("Matricola")]
        public string SerialNumber { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
