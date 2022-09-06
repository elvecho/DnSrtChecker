using System;
using System.Collections.Generic;

namespace DnSrtChecker.Models
{
    public partial class MailRecipient
    {
        public int LStoreGroupId { get; set; }
        public string SzRecipients { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }
    }
}
