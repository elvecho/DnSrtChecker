using DnSrtChecker.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DnSrtChecker.Persistence
{
    public interface ITransactionRtErrorRepository
    {
        Task<List<TransactionRtError>> GetTransactionRtError(int idTransaction,string rtServerId, int retailStoreId,int storeGroupId, string deviceId);
        Task<List<TransactionRtError>> GetTransactionRtError(string rtServerId, string deviceId, int lRtClosureNmbr, int lRtDocumentNmbr);
        Task<List<TransactionRtError>> GetTransactionsRtErrorByServer(string rtServerId);
        Task<IEnumerable<TransactionRtError>> GetTransactionsRtErrorByServer(string rtServerId,IEnumerable<string> errorTable);
         Task<IEnumerable<TransactionRtError>> GetTransactionsRtErrorByServer2(string rtServerId);
    }
}
