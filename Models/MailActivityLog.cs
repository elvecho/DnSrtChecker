using System;
using System.Collections.Generic;

namespace DnSrtChecker.Models
{
    public partial class MailActivityLog
    {
        public long LMailActivityLogId { get; set; }
        public string SzRecipients { get; set; }
        public string SzSubject { get; set; }
        public string SzBody { get; set; }
        public DateTime? DMailSentDateTime { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }
    }
}
