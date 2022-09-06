using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DnSrtChecker.Models
{
    public partial class Store
    {
        public Store()
        {
            RtServer = new HashSet<RtServer>();
        }

        public int LRetailStoreId { get; set; }
        public int LStoreGroupId { get; set; }
        public string SzDescription { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public virtual StoreGroup LStoreGroup { get; set; }
        public virtual ICollection<RtServer> RtServer { get; set; }
        [NotMapped]
        public string CompleteDescription 
        { get { return $"({LStoreGroup.LStoreGroupId})  {LStoreGroup.SzDescription} ({LRetailStoreId}) {SzDescription}"; }
          private set { } }
    }
}
