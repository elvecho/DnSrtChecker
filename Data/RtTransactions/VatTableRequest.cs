using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace DnSrtChecker.Data.RtTransactions
{
    [XmlRoot("RichiestaTabellaIva")]
    public class VatTableRequest
    {
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
