using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DnSrtChecker.Data.RtTransactions
{
    public class CanceledReceipt:Receipt
    {
        [XmlElement("RiferimentoDocumento")]
        public ReceiptReference ReceiptReference { get; set; }

        public CanceledReceipt()
        {
            ReceiptDetails = new List<ReceiptDetail>();
            VatTotals = new List<VatTotal>();
            CultureInfo = CultureInfo.CreateSpecificCulture("it-IT");
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
