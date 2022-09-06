using System;
using System.Collections.Generic;

namespace DnSrtChecker.Models
{
    public partial class IndexMaintenance
    {
        public DateTime DDate { get; set; }
        public string SzSchemaName { get; set; }
        public string SzTableName { get; set; }
        public string SzIndexName { get; set; }
        public double? LPrePercFragmentation { get; set; }
        public string SzActionToDo { get; set; }
        public double? LPostPercFragmentation { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }
    }
}
