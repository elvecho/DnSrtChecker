using DnSrtChecker.Models;
using DnSrtChecker.Models.ViewModels;
using DnSrtChecker.Models.ViewModels.ModelsFunctions;
using DnSrtChecker.ModelsHelper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DnSrtChecker.Persistence
{
    public interface ITransmissionRepository
    {
        Task<RtServerTransmissionDetail> GetTransmission(int RtServerOperationId, string rtServerId);
        Task<RtServerTransmissionDetail> GetTransmission(string rtServerId, string deviceId, DateTime dPosDateTime);
        Task<List<RtServerTransmissionDetail>> GetTransmission(string rtServerId, DateTime dPosDateTime);
        Task<List<RtServerTransmissionDetail>> ListTransmission();
        Task<List<RtServerTransmissionDetail>> ListTransmissionByServerId(string rtServer);
        Task<List<TransmissionsGroupedByDay>> ListTransmissionByServerIdByDay(string rtServer,string date,int nmbrDays);
        List<RtServerTransmissionDetail> GetTrasmissionRecapLastClosure();
        Task<List<UserActivityLog>> GetUserActivityByTransaction(string rtserverId);
        Boolean AddTransmission(RtServerTransmission transmission);
        Boolean RemoveTransmission(RtServerTransmission transmission);
        Task<List<TransmissionsGroupedByDay>> ListTransmissionByServerIdDay(string rtServer,string date);
        Task<List<TransmissionsGroupedByDay>> ListTransmissionByServerIdOfDay(string rtServer, string date);
        Task<IEnumerable<ListServersStatusHomeBYDay>> ListTransmissionAllServersNonCompliantByDate(IEnumerable<string> list, DateTime trxDateFrom, DateTime trxDateTo);
        Task<List<TransmissionsGroupedByDay>> ListTransmissionByServerIdByDay22(string rtServer, string date, int nmbrDays);
        Task<List<TransmissionsGroupedByDay>> ListTransmissionByServerIdOfDay22(string rtServer, string date);
        Task<List<TransmissionsList>> GetTransmissionsByDay(string userName, string id, int storeId, int storeGroupId, string date);
    }
}
