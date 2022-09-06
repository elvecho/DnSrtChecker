using System.Globalization;
using System.Text.Json;
using System.Xml.Serialization;

namespace DnSrtChecker.Data.RtTransactions
{
    public class VatInfo
    {

        [XmlIgnore]
        public CultureInfo cultureInfo { get; set; } = CultureInfo.CreateSpecificCulture("it-IT");

        [XmlIgnore]
        public decimal GrossAmount { get; set; }

        [XmlElement("Aliquota")]
        public string RateFormatted
        {
            get {
                return Rate.ToString(cultureInfo);
                //return Rate.ToString().Replace('.', ','); 
            }
            set {
                decimal rate;
                decimal.TryParse(value, NumberStyles.Any, cultureInfo, out rate);
                Rate = rate;
                //Rate = decimal.Parse(value.Replace('.', ',')); 
            }
        }

        [XmlIgnore]
        public decimal Rate { get; set; }

        [XmlElement("CodiceIva")]
        public int VatCode { get; set; }

        [XmlElement("Natura")]
        public string Nature { get; set; }

        [XmlElement("Attiva")]
        public int Active { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
