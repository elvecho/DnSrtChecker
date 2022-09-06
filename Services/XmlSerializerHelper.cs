using DnSrtChecker.Data.RtTransactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DnSrtChecker.Services
{
    public class XmlSerializerHelper
    {
		public XmlSerializerHelper()
		{
		}
		public static SalesDocument GetMemoryDetailFromXml(string content)
		{
			
			XmlSerializer serializer = new XmlSerializer(typeof(SalesDocument));

			using (TextReader reader = new StringReader(content))
			{
				SalesDocument result = (SalesDocument)serializer.Deserialize(reader);
				return result;
			}

		
		}
	}
}
