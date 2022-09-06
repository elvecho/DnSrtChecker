using System;
using System.Collections.Generic;
using System.Text;

namespace DnSrtChecker.Helpers
{
    public class EmailModel
    {
       
        public string To { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool IsBodyHtml { get; set; }
    }
}
