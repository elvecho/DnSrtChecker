using System.Globalization;
using System.Text.Json;
using System.Xml.Serialization;

namespace DnSrtChecker.Data.RtTransactions
{
    public class VatTotal
    {
        [XmlIgnore]
        public CultureInfo cultureInfo { get; set; } = CultureInfo.CreateSpecificCulture("it-IT");

        [XmlIgnore]
        public decimal GrossAmount { get; set; }

        [XmlElement("Importo")]
        public string GrossAmountFormatted
        {
            get {
                return GrossAmount.ToString(cultureInfo);
                //return GrossAmount.ToString().Replace('.', ',');
            }
            set
            {
                decimal grossAmount;
                decimal.TryParse(value, NumberStyles.Any,cultureInfo, out grossAmount);
                GrossAmount = grossAmount;
                //GrossAmount = decimal.Parse(value.Replace('.', ',')); 
            }
        }

        [XmlIgnore]
        public decimal NetAmount { get; set; }

        [XmlElement("BaseImponibile")]
        public string NetAmountFormatted
        {
            get {
                return NetAmount.ToString(cultureInfo);

                //return NetAmount.ToString().Replace('.', ','); 
            }
            set {
                decimal netAmount;
                decimal.TryParse(value, NumberStyles.Any, cultureInfo, out netAmount);
                NetAmount = netAmount;
               //NetAmount = decimal.Parse(value.Replace('.', ',')); 
            }
        }

        [XmlIgnore]
        public decimal TaxAmount { get; set; }

        [XmlElement("Imposta")]
        public string TaxAmountFormatted
        {
            get {
                //return TaxAmount.ToString().Replace('.', ','); 
                return TaxAmount.ToString(cultureInfo);

            }
            set {
                decimal taxAmount;
                decimal.TryParse(value, NumberStyles.Any, cultureInfo, out taxAmount);
                TaxAmount = taxAmount;
                //TaxAmount = decimal.Parse(value.Replace('.', ','));

            }
        }

        [XmlElement("CodiceIVA")]
        public VatDetail VatCode { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
