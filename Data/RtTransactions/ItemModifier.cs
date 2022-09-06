using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace DnSrtChecker.Data.RtTransactions
{
    public class ItemModifier
    {
        [XmlIgnore]
        public CultureInfo cultureInfo { get; set; } = CultureInfo.CreateSpecificCulture("it-IT");

        [XmlElement("Descrizione")]
        public string Description { get; set; }

        [XmlIgnore]
        public decimal Amount { get; set; }

        [XmlElement("Importo")]
        public string AmountFormatted
        {
            get {
                return Amount.ToString(cultureInfo);
                //return Amount.ToString().Replace('.', ','); 
            }
            set {
                //Amount = decimal.Parse(value.Replace('.', ',')); 
                decimal amount;
                decimal.TryParse(value, NumberStyles.Any, cultureInfo, out amount);
                Amount = amount;
            }
        }

        [XmlElement("Segno")]
        public string Sign { get; set; }

        [XmlElement("CodiceIVA")]
        public VatDetail VatCode { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
