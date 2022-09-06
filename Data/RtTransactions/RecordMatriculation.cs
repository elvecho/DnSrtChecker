using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DnSrtChecker.Data.RtTransactions
{
    [XmlRoot("RecordImmatricolazione")]

    public class RecordMatriculation
    {
        [XmlElement("DataOra")]
        public DateTime DateAndTime { get; set; }

        [XmlElement("Misuratore")]
        public Measurer Measurer { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
