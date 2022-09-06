using DnSrtChecker.Models;
using DnSrtChecker.ModelsHelper;
using DnSrtChecker.Helpers;
using DnSrtChecker.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnSrtChecker.FiltersmodelBindRequest;
using DnSrtChecker.Models.ViewModels.ModelsFunctions;

namespace DnSrtChecker.Persistence
{
    public interface IRtServerRepository
    {
        Task<RtServer> GetRtServer(string id,int retailStoreId,int storeGroupId);
        Task<RtServer> GetRtServerStatus(string id, int retailStoreId, int storeGroupId);
        Task<RtServer> GetRtServer(string id);
        Task<List<RtServer>> ListRtServer();
        Task<List<RtServer>> ListRtServerGroupedByServerId(string serverId);
        Task<List<ListServersGrouped>> ListRtServerByStoreAndStoreGroup();       
        Boolean AddRtServer(RtServer rtServer);
        Boolean RemoveProject(RtServer rtServer);
        Task<List<RtServer>> ListRtServerStatus();//
        Task<List<RtServerByUser>> ListRtServerByUser();
        Task<List<TrxRTServer>> ListRtServerStatusNew(FiltersmodelBindingRequest filters);
        Task<IEnumerable<RtServer>> ListRtServerStatusHome();
        Task<List<ListServersGrouped>> FilteredList(FiltersmodelBindingRequest filters);
        //Task<List<ListServersGrouped>> FilteredList2(FiltersmodelBindingRequest filters);

        RtServerStatus GetRtServerStatus(string id);
        Task<List<ListServersStatusHomeBYDay>> ListRtServerStatusHomeFiltered(FiltersmodelBindingRequest filters);
        List<ListServersStatusHomeBYDay> ListTransactionsByDayForHome(List<string> list, DateTime trxDateFrom, DateTime trxDateTo);
        List<ListServersStatusHomeBYDay> ListServersStatusHomeOfDay(FiltersmodelBindingRequest filters);
        List<ListServersStatusHomeBYDay> ListTransactionsOfDayForHome(List<string> listSrv, DateTime dayFilter1, DateTime dayFilter2);
        List<ListServersStatusHomeBYDay> ListServersHomeFilteredByDay(FiltersmodelBindingRequest filters);
        List<ListServersStatusHomeBYDay>  ListTrxFilteredByDays(List<string> listSrv, DateTime dayFrom, DateTime dayTo);
        List<ListServersStatusHomeBYDay> ListTrnFilteredByDay(List<string> listSrv, DateTime dayFrom, DateTime dayTo);
        Task<List<ListServersStatusHomeBYDay>> ListRtServerStatusHomeFiltered2(FiltersmodelBindingRequest filters);





        List<RtServersHome> ListRTServersFiltered(FiltersmodelBindingRequest filters);
        Task<IEnumerable<ListServersStatusHomeBYDay>> ListTransactionsByDayForHome2(IEnumerable<string> list, DateTime trxDateFrom, DateTime trxDateTo, IEnumerable<string> errTable);
        Task<IEnumerable<ListServersStatusHomeBYDay>> ListTransactionsByDayForHome3(IEnumerable<string> list, DateTime trxDateFrom, DateTime trxDateTo, IEnumerable<string> errTable);
        Task<IEnumerable<RtServersHome>> ListTransactionsByDayForHome22(IEnumerable<string> list, DateTime trxDateFrom, DateTime trxDateTo, IEnumerable<string> errTable);
    }
}
