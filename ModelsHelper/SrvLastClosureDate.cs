using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.ModelsHelper
{
    public class SrvLastClosureDate
    {
        public string Srv { get; set; }
        public DateTime LastDate { get; set; }
        public ICollection<bool?> TRXCheckedFlag { get; set; }
        public SrvLastClosureDate()
        {
            TRXCheckedFlag = new List<bool?>();
        }
    }
}
