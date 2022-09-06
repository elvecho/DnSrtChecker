using System;
using System.Collections.Generic;

namespace DnSrtChecker.Models
{
    public partial class UserActivity
    {
        public UserActivity()
        {
            UserActivityLog = new HashSet<UserActivityLog>();
        }

        public byte LUserActivityId { get; set; }
        public string SzDescription { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public virtual ICollection<UserActivityLog> UserActivityLog { get; set; }
    }
}
