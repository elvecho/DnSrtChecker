using System;
using System.Collections.Generic;

namespace DnSrtChecker.Models
{
    public partial class StoreGroup
    {
        public StoreGroup()
        {
            Store = new HashSet<Store>();
        }

        public int LStoreGroupId { get; set; }
        public string SzDescription { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public virtual ICollection<Store> Store { get; set; }
    }
}
