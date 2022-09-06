using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DnSrtChecker.Data.RtTransactions
{
    [XmlRoot("Misuratore")]
    public class Measurer
    {

        [XmlElement("Marchio")]
        public string  Mark { get; set; }

        [XmlElement("Modello")]
        public string Model { get; set; }
        [XmlElement("Matricola")]
        public string ServerRT { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}