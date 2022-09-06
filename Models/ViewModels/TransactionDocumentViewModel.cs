using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace DnSrtChecker.Models.ViewModels
{
    public class TransactionDocumentViewModel
    {
        public long LTransactionDocumentId { get; set; }
        public string SzRtDocumentId { get; set; }
        public int? LDocumentTypeId { get; set; }
        public string SzDocumentNote { get; set; }
        public string SzDocumentName { get; set; }
        public string SzDocumentAttachment { get; set; }
        public string SzDocumentAttachmentTxt { get; set;}
        public DateTime? DLastUpdateLocal { get; set; }
        //Display Xml Inlined
        public string XmlFormat {
            get {
                XDocument doc = XDocument.Parse(SzDocumentAttachment);
                return doc.ToString();
            }
        }
        public string StringFromXmlDocument { get; set; }
        public string DocumentAttachmentByteToString
        {
            get
            {
                if (!string.IsNullOrEmpty(SzDocumentAttachmentTxt))
                {
                    byte[] tmp = Convert.FromBase64String(SzDocumentAttachmentTxt);
                    var x = Encoding.Default.GetString(tmp);
                    //sprint 3 punto d questo il punto per eventuali modifiche
                    return Encoding.Default.GetString(tmp).ToString();
                }
                return "";
                   
            }
           
        }
        public  DocumentTypeViewModel LDocumentType { get; set; }
        public  TransactionAffiliationViewModel LTransactionAffiliation { get; set; }

       
    }
}
