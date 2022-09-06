using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace DnSrtChecker.Data.RtTransactions
{
    [XmlRoot("RestituzioneMemDettaglio")]
    public class MemoryDetail
    { 
        [XmlElement("ProgressivoPacchetto")]
        public int PackageSequenceNumber { get; set; }

        [XmlElement("DataRichiesta")]
        public string RequestDate { get; set; }

        [XmlElement("IdentificativoDGFE")]
        public string DGFEId { get; set; }

        [XmlElement("PuntoCassa")]
        public string DeviceId { get; set; }

        [XmlElement("Sessione")]
        public string Session { get; set; }

        [XmlElement("DocumentoCommerciale")]
        public List<SalesDocument> SalesDocuments { get; set; }

        [XmlElement("RichiestaMemDettaglio")]
        public MemoryDetailRequest MemoryDetailRequest { get; set; }
        public MemoryDetail()
        {
            SalesDocuments = new List<SalesDocument>();
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
