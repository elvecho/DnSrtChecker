using DnSrtChecker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnSrtChecker.Persistence
{
    public class TransactionRtErrorRepository : ITransactionRtErrorRepository
    {
        public readonly RT_ChecksContext _dbContext;

        public TransactionRtErrorRepository(RT_ChecksContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<TransactionRtError>> GetTransactionRtError(int IdDocument, string rtServerId, int retailStoreId, int storeGroupId,string deviceId)
        {
            var x=await  _dbContext.TransactionRtError.Include(x => x.RtServer)
                                                    .ThenInclude(x => x.TransactionRtError)
                                                     .Where(x => x.LRtDocumentNmbr == IdDocument
                                                     && x.SzRtServerId == rtServerId
                                                     //  && x.LRetailStoreId == retailStoreId && x.LStoreGroupId == storeGroupId
                                                       && x.SzRtDeviceId==deviceId)
                                                         .ToListAsync();
            return x;
                                
        }
        public async Task<List<TransactionRtError>> GetTransactionRtError(string rtServerId, string deviceId, int lRtClosureNmbr,int lRtDocumentNmbr)
        {
            var x = await _dbContext.TransactionRtError.Include(x => x.RtServer)
                                                       .ThenInclude(x => x.TransactionRtError)
                                                       .Where(x => x.SzRtServerId == rtServerId
                                                       && x.SzRtDeviceId == deviceId 
                                                       && x.LRtDocumentNmbr == lRtDocumentNmbr 
                                                       && x.LRtClosureNmbr==lRtClosureNmbr
                                                        )
                                                       .ToListAsync();
            return x;

        }
        public async Task<List<TransactionRtError>> GetTransactionsRtErrorByServer(string rtServerId)
        {
            var listTransactionError = await _dbContext.TransactionRtError
                                                        //.Include(x => x.RtServer)
                                                      // .ThenInclude(x => x.TransactionRtError)
                                                       .Where(x => x.SzRtServerId == rtServerId)
                                                       .ToListAsync();
            return listTransactionError;

        }
        public  Task<IEnumerable<TransactionRtError>> GetTransactionsRtErrorByServer2(string rtServerId)
        {
            //var lastUpdate = Convert.ToDateTime(dateFrom);
            var listTransactionError =  _dbContext.TransactionRtError
                                                       //.Include(x => x.RtServer)
                                                       // .ThenInclude(x => x.TransactionRtError)
                                                       .Where(x => x.SzRtServerId == rtServerId)
                                                       .Select(x=>new TransactionRtError
                                                       {
                                                            DRtDateTime=x.DRtDateTime,
                                                             LRtDocumentNmbr=x.LRtDocumentNmbr,
                                                              LRetailStoreId=x.LRetailStoreId,
                                                               LRtClosureNmbr=x.LRtClosureNmbr,
                                                                LStoreGroupId=x.LStoreGroupId,
                                                                  SzDescription=x.SzDescription,
                                                                   SzRtDeviceId=x.SzRtDeviceId,
                                                                    SzRtServerId=x.SzRtServerId
                                                       })
                                        .AsEnumerable();
            return Task.FromResult(listTransactionError);

        }
        public  Task<IEnumerable<TransactionRtError>> GetTransactionsRtErrorByServer(string rtServerId,IEnumerable<string> errorTable)
        {
            var listTransactionError =  _dbContext.TransactionRtError
                                                       .Where(x => x.SzRtServerId == rtServerId
                                                       //&& errorTable.Any(t=>x.SzDescription.StartsWith(t))
                                                       )
                                                       .AsEnumerable();
            return Task.FromResult(listTransactionError);

        }
    }
}
