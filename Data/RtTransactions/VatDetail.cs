using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace DnSrtChecker.Data.RtTransactions
{
    public class VatDetail
    {
        [XmlIgnore]
        public CultureInfo cultureInfo { get; set; } = CultureInfo.CreateSpecificCulture("it-IT");

        [XmlIgnore]
        public decimal Rate { get; set; }

        [XmlElement("Aliquota")]
        public string RateFormatted
        {

            get {
                return  Rate.ToString("00.00",cultureInfo);
                //return Rate.ToString().Replace('.', ',');
            }
            set {
                decimal rate;
                decimal.TryParse(value, NumberStyles.Any, cultureInfo, out rate);
                Rate = rate;
                //Rate = decimal.Parse(value.Replace('.',',')); 
            }
        }

        [XmlElement("CodiceEsenzioneIVA")]
        public string VatExemption { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
