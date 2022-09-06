using DnSrtChecker.Models.ViewModels;
using DnSrtChecker.Models;
using DnSrtChecker.ModelsHelper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DnSrtChecker.FiltersmodelBindRequest;
using DnSrtChecker.Models.ViewModels.ModelsFunctions;

namespace DnSrtChecker.Persistence
{
    public interface ITransactionRepository
    {
        Task<TransactionAffiliation> GetTransaction(string SzRtDocumentId, string SzRtServerId,
            int LRetailStoreId, int LStoreGroupId);
        Task<List<TransactionAffiliation>> ListTransaction();
        Task<List<TransactionAffiliation>> ListTransactionByServerRt(RtServer rtServer);
        Task<List<TransactionAffiliation>> ListTransactionNonCompliant();
        Task<List<TransactionByServerRtForTransmissions>> ListTransactionServerRt(string rtServerId,int storeId,int storeGroupId);
        Boolean AddTransaction(TransactionAffiliation transaction);
        Boolean RemoveTransaction(TransactionAffiliation transaction);
        List<RtServer> ListRtServerNonCompliant();
        List<TransactionByServerRtForTransmissions> ListTransactionServerRtAll(IEnumerable<SrvLastClosureDate> listdate);
        Task<List<TransmissionsGroupedByDay>> ListServerWithTotalsTrnAndTrm();
        Task<List<UserActivityLog>> GetUserActivityByTransaction(string idTransaction);

        Task<List<TransactionByServerRtForTransmissions>> ListTransactionServerRtOfDay(string rtServerId, int storeId, int storeGroupId, DateTime? date);
        Task<List<TransactionByServerRtForTransmissions>> ListTransactionServerRtDay(string rtServerId, int storeId, int storeGroupId, DateTime? date);
        Task<List<TransactionAffiliation>> ListTransactionByServerRtTrn(RtServer rtServer);
        Task<List<TransactionByServerRtForTransmissions>> ListTransactionServerRtMonth(string rtServerId, int storeId, int storeGroupId, DateTime? date,int nmbrDays);

        Task<List<TransactionAffiliation>> ListTransactionsByServerRt(RtServer rtServer);
        Task<IEnumerable<TransactionAffiliation>> GetTransactionsMismtach(RtServer rtserver);
        Task<IEnumerable<TransactionAffiliation>> GetTransactionsMismtach(string rtserver);
        Task<IEnumerable<TransactionAffiliation>> GetTransactionsError(RtServer rtserver);
        Task<IEnumerable<TransactionAffiliation>> GetTransactionsError(string rtserver);
        Task<IEnumerable<TransactionAffiliation>> GetTransactionsCompliant(RtServer rtserver, IEnumerable<string> noncompliant,int trnNmbr);
        Task<IEnumerable<TransactionAffiliation>> GetTransactionsCompliant(string rtserver, IEnumerable<string> noncompliant,int trnNmbr,FiltersmodelBindingRequest filters);
        List<string> ListSrvHasOneMsmatch();
        Task<IEnumerable<TransactionAffiliation>> GetTransactionsCompliantOthers(string rtserver, IEnumerable<string> noncompliant, int trnNmbr, FiltersmodelBindingRequest filters);
        List<TransactionAffiliation> ListSrvHasMsmatchTPAndRT(DateTime dFrom, DateTime dTo);
        Task<List<TransactionByServerRtForTransmissions>> ListTransactionServerRtMonth22(string rtServerId, int storeId, int storeGroupId, DateTime? date, int nmbrDays);
        Task<List<TransactionList>> GetTransactions(string UserName, string RtDeviceClosureDateTime,
            string ServerRt, string RtDeviceID, string Store, string PosWorkstationNmbr,
            string TransactionCheckedFlag, string TransactionArchivedFlag,
            string HasMismatch, string RtNonCompliantFlag, string PosTaNmbr,
            string RtClosureNmbr, string RtDocumentNmbr);
    }
}
