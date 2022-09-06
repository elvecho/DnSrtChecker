using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DnSrtChecker.Data.RtTransactions
{
    public class ReceiptReference
    {
        [XmlElement("DataRegistrazione")]
        public string Date { get; set; }

        [XmlElement("OrarioRegistrazione")]
        public string Time { get; set; }

        [XmlElement("NumeroProgressivo")]
        public string Sequence { get; set; }

        public DateTime DateTime {
            get {
                var date = new DateTime(Int32.Parse(Date.Substring(4, 4))
                                      , Int32.Parse(Date.Substring(2, 2))
                                      , Int32.Parse(Date.Substring(0, 2))
                                      , Int32.Parse(Time.Substring(0, 2))
                                      , Int32.Parse(Time.Substring(3, 2))
                                      ,0);
                return date;
                    } 
        }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
