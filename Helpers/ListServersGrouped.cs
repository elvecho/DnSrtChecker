using DnSrtChecker.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DnSrtChecker.Helpers
{
    public class ListServersGrouped
    {
        public int LRetailStoreId { get; set; }
        public int LStoreGroupId { get; set; }
        public List<RtServer> RtServers { get; set; }
    }
}
