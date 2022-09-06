using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace DnSrtChecker.Data.RtTransactions
{
    public class ReceiptDetail
    {
        [XmlElement("Vendita")]
        public ItemSale ItemSale { get; set; }

        [XmlElement("ModificatoreSuArticolo")]
        public ItemModifier ItemModifier { get; set; }

        [XmlElement("Pagamento")]
        public Tender Tender { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
