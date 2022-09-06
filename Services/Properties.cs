using DnSrtChecker.ModelsHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Services
{
    public class Properties
    {
        public int HomeNmbrDays { get; set; }
        public int TrxNmbrDays { get; set; }
        public int MaxTransactions { get; set; }
        public string FlagViewTransactions { get; set; }
        public List<Error> TransactionErrorTable { get; set; }
        public Properties()
        {}
    }
}
