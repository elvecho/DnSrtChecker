using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace DnSrtChecker.Data.RtTransactions
{
    public class ItemSale
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
                //return Amount.ToString().Replace('.', ',');
                return Amount.ToString(cultureInfo);
            }
            set {
                //Amount = decimal.Parse(value.Replace('.', ',')); 
                decimal amount;
                decimal.TryParse(value, NumberStyles.Any, cultureInfo, out amount);
                Amount = amount; 
            }
        }

        [XmlIgnore]
        public decimal Quantity { get; set; }

        [XmlElement("Quantita")]
        public string QuantityFormatted
        {
            get {
                //return Quantity.ToString().Replace('.', ','); 
                return Quantity.ToString(cultureInfo);
            }
            set {
                decimal quantity;
                decimal.TryParse(value, NumberStyles.Any, cultureInfo, out quantity);
                Quantity = quantity;
                //Quantity = decimal.Parse(value.Replace('.', ','));
            }
        }

        [XmlIgnore]
        public decimal Price { get; set; }

        [XmlElement("PrezzoUnitario")]
        public string PriceFormatted
        {
            get {
              //  return Price.ToString().Replace('.', ','); 
                return Price.ToString(cultureInfo); 
            }
            set {
                decimal price;
                decimal.TryParse(value, NumberStyles.Any, cultureInfo, out price);
                Price = price;
                //Price = decimal.Parse(value.Replace('.', ','));
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
