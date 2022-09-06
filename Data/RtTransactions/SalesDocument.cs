using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace DnSrtChecker.Data.RtTransactions
{
    public class SalesDocument
    { 
        [XmlElement("CCDCPrecedente")]
        public string PreviousCCDC { get; set; }

        [XmlElement("PuntoCassa")]
        public string RegisterId { get; set; }

        [XmlElement("Scontrino")]
        public Receipt Receipt { get; set; }
        [XmlElement("RettificaScontrino")]
        public CanceledReceipt CanceledReceipt { get; set; }

        [XmlElement("CCDC")]
        public string CCDC { get; set; }
        public string CCDC1 { get { return CCDC.Substring(0, CCDC.Length / 2); } }
        public string CCDC2 { get { return CCDC.Substring(CCDC.Length / 2, CCDC.Length - (CCDC.Length / 2)); } }
        [XmlElement("NonConforme")]
        public int NonCompliant { get; set; }
        /*Editor:SoukainaInformation 
         *Edit: added fields for publisher
          */
        [XmlElement("IdentificativoDGFE")]
        public string DGFEId { get; set; }
        [XmlElement("Cassa")]

        public string DeviceId { get; set; }
        [XmlElement("Marchio")]

        public string Mark { get; set; }
        [XmlElement("Modello")]

        public string Model { get; set; }
        [XmlElement("Matricola")]

        public string ServerRT { get; set; }
        /*End changes*/
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
