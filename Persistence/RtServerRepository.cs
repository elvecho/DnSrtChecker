using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using DnSrtChecker.FiltersmodelBindRequest;
using DnSrtChecker.Helpers;
using DnSrtChecker.Models;
using DnSrtChecker.Models.ViewModels;
using DnSrtChecker.Models.ViewModels.ModelsFunctions;
using DnSrtChecker.ModelsHelper;
using DnSrtChecker.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RazorEngineCore;

namespace DnSrtChecker.Persistence
{
    public class RtServerRepository : IRtServerRepository
    {

        public readonly RT_ChecksContext _dbContext;
        private readonly ILogger _logger;
        public readonly ITransactionRepository _transactionRepository;
        //public readonly IStoreRepository _storeRepository;
        private readonly IMapper _mapper;
        private readonly IOptions<Properties> _properties;
        public static List<TrxRTServer> ListRtServers;


        public RtServerRepository(RT_ChecksContext dbContext,
                                ILogger<RtServerRepository> logger,
                                ITransactionRepository transactionRepository,
                                //IStoreRepository storeRepository,
                                IOptions<Properties> properties,
                                IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _transactionRepository = transactionRepository;
            //_storeRepository = storeRepository;
            _properties = properties;
            _mapper = mapper;
        }

        public bool AddRtServer(RtServer rtServer)
        {
            try
            {
                _dbContext.RtServer.Add(rtServer);
                _logger.LogDebug($"Server RT Matricule: {rtServer.SzRtServerId} added successfully");
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error adding Server RT Matricule {rtServer.SzRtServerId}  :{e.Message}");

                return false;
            }
        }
        public async Task<RtServer> GetRtServerStatus(string id, int retailStoreId, int storeGroupId)
        {
            return await _dbContext.RtServer
                .Include(st => st.RtServerStatus)
                .Include(s => s.L)
                .ThenInclude(sg => sg.LStoreGroup)
                .FirstOrDefaultAsync(x => x.SzRtServerId.Contains(id)
                                && x.LRetailStoreId == retailStoreId
                                && x.LStoreGroupId == storeGroupId);
        }

        public async Task<RtServer> GetRtServer(string id, int retailStoreId, int storeGroupId)
        {
            var baseQ = _dbContext.RtServer.Where(x => x.SzRtServerId.Contains(id)
                                && x.LRetailStoreId == retailStoreId
                                && x.LStoreGroupId == storeGroupId);
            var res = baseQ.FirstOrDefault();

            baseQ.Include(x => x.L).ThenInclude(x => x.LStoreGroup).Load();
            baseQ.Include(x => x.RtServerStatus).Load();
            //baseQ.Include(x => x.TransactionRtError).Load();
            //baseQ.Include(x => x.TransactionAffiliation).Load();

            return res;


        }

        public async Task<RtServer> GetRtServer(string id)
        {
            return await _dbContext.RtServer
                .Include(st => st.RtServerStatus)
                .Include(s => s.L)
                .ThenInclude(sg => sg.LStoreGroup)
                .Include(ta => ta.TransactionAffiliation)
                .AsNoTracking()
                .Include(t => t.TransactionRtError)
                .FirstOrDefaultAsync(x => x.SzRtServerId == id);
        }

        public RtServerStatus GetRtServerStatus(string id)
        {
            var srv = _dbContext.RtServerStatus
                .FirstOrDefault(x => x.SzRtServerId == id);
            return srv;
        }
        public async Task<List<RtServer>> ListRtServer()
        {
            return await _dbContext.RtServer
                .Include(st => st.RtServerStatus)
                .Include(s => s.L)
                .ThenInclude(sg => sg.LStoreGroup)
                .Include(t => t.TransactionRtError)
                .OrderBy(x => x.L.LStoreGroupId)
                .ThenBy(x => x.L.LRetailStoreId)
                .ThenBy(x => x.SzRtServerId)
                //.Include(ta => ta.TransactionAffiliation)
                .ToListAsync();

        }
 
        public async Task<List<TrxRTServer>> ListRtServerStatusNew(FiltersmodelBindingRequest filters)
        {
            //aggiugere tutti i filtri  passarli)
            DateTime? dateInit;
            DateTime? dateTo;
            string format = "dd-MM-yyyy";
            bool firstAccess = false;
            string trmDateFrom = filters.TransmissionDateFrom;
            string trmdateTo = filters.TransmissionDateTo;
            
            if (filters.TransmissionDateFrom == null)
            {
                //GET DAY OF WEEK NAME , IF IS "MONDAY OR SUNDAY OR SATURDAY"
                //DISPLAY FRIDAY'S TRANSACTIONS
                string dayOfWeek = DateTime.Now.DayOfWeek.ToString().ToLower();
                int dayToDisplay = dayOfWeek == "Monday" ? -3 : (dayOfWeek == "Sunday" ? -2 : 
                    (dayOfWeek == "Saturday" ? -1 : -Math.Abs(_properties.Value.HomeNmbrDays)));
                firstAccess = true;
                filters.TransmissionDateFrom =
                    DateTime.Now.AddDays(dayToDisplay).ToString("yyyy-MM-dd");
                format = "yyyy-MM-dd";
            }

            if (filters.TransmissionDateTo == null)
                filters.TransmissionDateTo = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            dateInit = DateTime.ParseExact(
                      filters.TransmissionDateFrom,
                      format,
                      System.Globalization.CultureInfo.InvariantCulture);
            dateTo = DateTime.ParseExact(
                       filters.TransmissionDateTo,
                       format,
                       System.Globalization.CultureInfo.InvariantCulture);
            var TrxRTServerList = new List<TrxRTServer>();
            //string.IsNullOrEmpty(filters.Store)
                //&& filters.ServerRt == null && filters.Status && filters.Error == null && filters.Conformity == null
                //&& filters.NonCompliant == null &&
            if (trmDateFrom == null && trmdateTo == null && filters.StoreGroup == null 
                && filters.ServerRt == null && filters.Status == null && filters.Error == null && filters.Conformity == null 
                && ListRtServers !=null && filters.NonCompliant == null && ListRtServers.Count != 0)
            {
                firstAccess = true;
                return ListRtServers;
            }
            try
            {
                
                TrxRTServerList = await _dbContext.trxTotalsResult(FiltersmodelBindingRequest.UserName, dateInit.Value.Date.ToString("yyyy-MM-dd"),
                dateTo.Value.Date.ToString("yyyy-MM-dd"), filters.ServerRt, filters.Store,
                filters.StoreGroup, filters.Status, filters.Error, filters.NonCompliant)
                .ToListAsync();
            }
            catch (Exception Ex)
            {

                
            }
            //var prova = TrxRTServerList.Where(x => x.LRetailStoreId == 187);
            //tolgo i transazioni con totalADE == 0 and x.TotalRT != "0" and x.TotalTP !=0
            TrxRTServerList = TrxRTServerList.Count() > 0 ?
                TrxRTServerList.Where(x => !(x.TotalADE == "0" && x.TotalRT == "0" && x.TotalTP =="0")).ToList() :
                TrxRTServerList ;
            ListRtServers = firstAccess ? TrxRTServerList : ListRtServers;
            //prova = TrxRTServerList.Where(x => x.LRetailStoreId == 187);
            //filtrare in base al valore di filters.NonCompliant (null, true = non conformi, false = conformi)
            //TrxRTServerList = filters.NonCompliant != null ? (filters.NonCompliant == "true" ?
            //                  TrxRTServerList.Where(x => x.BWarningFlag == 1).ToList() : TrxRTServerList)
            //                  : TrxRTServerList;
            //TrxRTServerList.Where(x => x.BWarningFlag == 0 && x.IsExcluded).ToList()) : TrxRTServerList;
            return TrxRTServerList;
        }
        public async Task<List<RtServerByUser>> ListRtServerByUser()
        {
            var listRtServerByUser = new List<RtServerByUser>();
            try
            {
                listRtServerByUser = await _dbContext.rtServerByUser(FiltersmodelBindingRequest.UserName)
                .OrderBy(x => x.LStoreGroupId).ThenBy(x => x.LRetailStoreId)
                .ToListAsync();
            }
            catch (Exception Ex)
            {


            }
            return  listRtServerByUser;

        }
        public async Task<List<RtServer>> ListRtServerStatus()
        {
            return await _dbContext.RtServer
                .Include(st => st.RtServerStatus)
                .Include(s => s.L)
                .ThenInclude(sg => sg.LStoreGroup)
                .ToListAsync();

        }
        public Task<IEnumerable<RtServer>> ListRtServerStatusHome()
        {

            var res = _dbContext.RtServer
                .Include(st => st.RtServerStatus)
                .Include(s => s.L)
                .ThenInclude(sg => sg.LStoreGroup)
                .Where(x => x.RtServerStatus != null)
                .AsEnumerable()
                ;
            return Task.FromResult(res);
        }


        //new Filter List Home 04/06/2020
        public async Task<List<ListServersStatusHomeBYDay>> ListRtServerStatusHomeFiltered(FiltersmodelBindingRequest filters)
        {

            DateTime trxDateFrom = DateTime.ParseExact(
                      filters.TransmissionDateFrom,
                      "dd-MM-yyyy",
                      System.Globalization.CultureInfo.InvariantCulture);

            DateTime trxDateTo = DateTime.ParseExact(
                       filters.TransmissionDateTo,
                       "dd-MM-yyyy",
                       System.Globalization.CultureInfo.InvariantCulture);

            var baseQ = _dbContext.RtServer
                .Include(st => st.RtServerStatus)
                .Include(s => s.L)
                .ThenInclude(sg => sg.LStoreGroup)
                .Where(x => x.RtServerStatus != null);

            var lists = baseQ;

            var list = lists;
            if (!string.IsNullOrEmpty(filters.ServerRt))
            {
                list = list.Where(s => s.SzRtServerId.ToLower().Contains(filters.ServerRt.ToLower()));//.ToList();
            }

            if (!string.IsNullOrEmpty(filters.StoreGroup))
            {
                var storeGroup = 0;
                if (int.TryParse(filters.StoreGroup, out storeGroup))
                {
                    list = list.Where(x => x.LStoreGroupId == storeGroup);//.ToList();
                }
                else
                {
                    list = list.Where(x => x.L.LStoreGroup.SzDescription.ToLower()
                                                                            .Contains(filters.StoreGroup.ToLower()));//.ToList();
                }
            }

            if (!string.IsNullOrEmpty(filters.Store))
            {
                var store = 0;
                if (int.TryParse(filters.Store, out store))
                {
                    list = list.Where(x => x.L.LRetailStoreId == store);
                }
                else
                {
                    list = list.Where(x => string.Format("({0}) {1}", x.L.LRetailStoreId, x.L.SzDescription).ToLower()
                                                                         .Contains(filters.Store.ToLower()));//.ToList();
                }
            }

            if (!string.IsNullOrEmpty(filters.Status))
            {
                Boolean.TryParse(filters.Status, out var status);
                list = list.Where(s => s.BOnDutyFlag == status);//.ToList();
            }

            if (!string.IsNullOrEmpty(filters.Error))
            {
                Boolean.TryParse(filters.Error, out var error);
                list = list.Where(s => s.RtServerStatus.BOnErrorFlag == error);//.ToList();
            }

            var listSrv = list.Select(x => x.SzRtServerId).ToList();
            //modifica11022021
            var listTrx = ListTransmissionsByDayForHome(listSrv, trxDateFrom, trxDateFrom);
            var listTrn = ListTransactionsByDayForHome(listSrv, trxDateFrom, trxDateFrom);

            listTrx.ForEach(x =>
            {
                var listserverHome = new List<RtServersHome>();

                x.ListRtServersHome.ToList().ForEach(srv =>
                {
                    var totalADE = srv.TotalADE ?? 0.00m;
                    var foundDate = listTrn.Where(trn => trn.OperationClosureDatetime.Value.Date == x.OperationClosureDatetime.Value.Date).FirstOrDefault();

                    if (foundDate != null && foundDate.ListRtServersHome.Count() > 0 && foundDate.ListRtServersHome != null)
                    {
                        var found = foundDate.ListRtServersHome;
                        var foundsrv = found.Where(tmp => srv.SzRtServerId == tmp.SzRtServerId).ToList();

                        if (foundsrv != null && foundsrv.Count() > 0)
                        {
                            var totalTP = foundsrv.Sum(x => x.TotalTP ?? 0.00m);
                            foreach (var s in foundsrv.ToList())
                            {
                                RtServersHome rtServer = new RtServersHome();
                                //rtServer = srv;
                                rtServer.SzRtServerId = srv.SzRtServerId;
                                rtServer.TotalADE = totalADE;
                                rtServer.LRetailStoreId = s.LRetailStoreId;
                                rtServer.LStoreGroupId = s.LStoreGroupId;
                                rtServer.L = s.L;
                                rtServer.BOnDutyFlag = s.BOnDutyFlag;
                                rtServer.BOnError = s.BOnError;
                                rtServer.IsChecked = s.IsChecked;
                                rtServer.NonCompliant = s.NonCompliant;
                                rtServer.NonCompliantOrHasMismatch = s.NonCompliant.ToString();
                                rtServer.RtServerStatus = s.RtServerStatus;
                                rtServer.TotalTP = totalTP;
                                rtServer.TrasnmissionError = totalADE.Equals(totalTP);
                                rtServer.DateLastClosure = x.OperationClosureDatetime;
                                rtServer.TransmissionChecked = srv.TransmissionChecked;
                                listserverHome.Add(rtServer);
                            }

                        }
                    }
                    //else
                    //{
                    //    var tmpsrv = list.Where(x => x.SzRtServerId == srv.SzRtServerId);

                    //    foreach (var s in tmpsrv)
                    //    {
                    //        srv.BOnDutyFlag = s.BOnDutyFlag;
                    //        srv.BOnError = s.BOnDutyFlag;
                    //        srv.IsChecked = "false";
                    //        srv.L = s.L;
                    //        srv.LRetailStoreId = s.LRetailStoreId;
                    //        srv.LStoreGroupId = s.LStoreGroupId;
                    //        srv.NonCompliant = true;
                    //        srv.NonCompliantOrHasMismatch = "true";
                    //        srv.RtServerStatus = s.RtServerStatus;
                    //        srv.TotalTP = 0.00m;
                    //        srv.TrasnmissionError = srv.TotalADE.Equals(0.00m);
                    //        srv.TransmissionChecked = srv.TransmissionChecked;
                    //        srv.DateLastClosure = x.OperationClosureDatetime;
                    //        listserverHome.Add(srv);

                    //    }
                    //}
                });
                x.ListRtServersHome = listserverHome.ToList()
                                          .GroupBy(x => new { x.SzRtServerId, x.LRetailStoreId, x.LStoreGroupId }, (key, group) => new RtServersHome
                                          {
                                              SzRtServerId = key.SzRtServerId,
                                              L = group.FirstOrDefault().L,
                                              LRetailStoreId = key.LRetailStoreId,
                                              LStoreGroupId = key.LStoreGroupId,
                                              BOnDutyFlag = group.FirstOrDefault().BOnDutyFlag,
                                              BOnError = group.FirstOrDefault().BOnError,
                                              NonCompliant = !group.All(x => x.NonCompliant != true),
                                              NonCompliantOrHasMismatch = (!group.All(x => x.NonCompliant != true)).ToString(),
                                              RtServerStatus = group.FirstOrDefault().RtServerStatus,
                                              TotalTP = group.Sum(x => x.TotalTP ?? 0.00m),
                                              TotalADE = group.Sum(x => x.TotalADE ?? 0.00m),
                                              TransmissionChecked = !group.Any(x => x.TransmissionChecked != true),
                                              TrasnmissionError = group.Sum(x => x.TotalADE).Equals(group.Sum(x => x.TotalTP)),
                                              DateLastClosure = group.FirstOrDefault().DateLastClosure

                                          }).Where(x => !x.TrasnmissionError && !x.TransmissionChecked)
                                          .OrderByDescending(x => x.NonCompliant).ToList();

                //x.ListRtServersHome = x.ListRtServersHome.Where(x => !x.TrasnmissionError && !x.TransmissionChecked).ToList();

            });
            if (!string.IsNullOrEmpty(filters.NonCompliant))
            {
                var Noncompliant = bool.Parse(filters.NonCompliant);
                listTrx.ForEach(x =>
                {
                    x.ListRtServersHome = x.ListRtServersHome.Where(x => x.NonCompliant == Noncompliant).ToList();
                });
            }
            var listRes = listTrx.Select(x => new ListServersStatusHomeBYDay
            {
                OperationClosureDatetime = x.OperationClosureDatetime,
                ListRtServersHome = new List<RtServersHome>()
            }).ToList();
            listRes.ForEach(x =>
            {
                if (x.OperationClosureDatetime.Value.Date == trxDateFrom.Date)
                {
                    x.ListRtServersHome = listTrx.Where(x => x.OperationClosureDatetime.Value.Date == trxDateFrom.Date).SelectMany(x => x.ListRtServersHome).ToList();
                }
            });
            //  .OrderByDescending(x => x.OperationClosureDatetime.Value.Date).ToList();
            //var lastClosureDate = listRes.First().OperationClosureDatetime;
            //var listSrvLastDay = listTrx.Where(x => x.OperationClosureDatetime.Value.Date == lastClosureDate.Value.Date).SelectMany(x => x.ListRtServersHome);
            //listRes.ForEach(x =>
            //{
            //    if (x.OperationClosureDatetime.Value.Date == lastClosureDate.Value.Date)
            //    {
            //        x.ListRtServersHome = listSrvLastDay.ToList();
            //    }
            //});

            //return listRes;

            return listTrx;

        }
        public List<RtServersHome> ListRTServersFiltered(FiltersmodelBindingRequest filters)
        {
            DateTime trxDateFrom = DateTime.ParseExact(
                    filters.TransmissionDateFrom,
                    "dd-MM-yyyy",
                    System.Globalization.CultureInfo.InvariantCulture);

            DateTime trxDateTo = DateTime.ParseExact(
                       filters.TransmissionDateTo,
                       "dd-MM-yyyy",
                       System.Globalization.CultureInfo.InvariantCulture);
            var baseQ = _dbContext.RtServer
                .Include(st => st.RtServerStatus)
                .Include(s => s.L)
                .ThenInclude(sg => sg.LStoreGroup)
                .Where(x => x.RtServerStatus != null);

            var lists = baseQ;

            var list = lists;
            if (!string.IsNullOrEmpty(filters.ServerRt))
            {
                list = list.Where(s => s.SzRtServerId.ToLower().Contains(filters.ServerRt.ToLower()));//.ToList();
            }

            if (!string.IsNullOrEmpty(filters.StoreGroup))
            {
                var storeGroup = 0;
                if (int.TryParse(filters.StoreGroup, out storeGroup))
                {
                    list = list.Where(x => x.LStoreGroupId == storeGroup);//.ToList();
                }
                else
                {
                    list = list.Where(x => x.L.LStoreGroup.SzDescription.ToLower()
                                                                            .Contains(filters.StoreGroup.ToLower()));//.ToList();
                }
            }

            if (!string.IsNullOrEmpty(filters.Store))
            {
                var store = 0;
                if (int.TryParse(filters.Store, out store))
                {
                    list = list.Where(x => x.L.LRetailStoreId == store);
                }
                else
                {
                    list = list.Where(x => string.Format("({0}) {1}", x.L.LRetailStoreId, x.L.SzDescription).ToLower()
                                                                         .Contains(filters.Store.ToLower()));//.ToList();
                }
            }

            if (!string.IsNullOrEmpty(filters.Status))
            {
                Boolean.TryParse(filters.Status, out var status);
                list = list.Where(s => s.BOnDutyFlag == status);//.ToList();
            }

            if (!string.IsNullOrEmpty(filters.Error))
            {
                Boolean.TryParse(filters.Error, out var error);
                list = list.Where(s => s.RtServerStatus.BOnErrorFlag == error);//.ToList();
            }
            var l = list.Select(x => x.SzRtServerId).AsEnumerable();
            var listTotalADE = new List<TrxRTServer>();
            try
            {
                listTotalADE = _dbContext.trxTotalsResult(FiltersmodelBindingRequest.UserName, trxDateFrom.ToString("yyyy-MM-dd"), trxDateTo.Date.AddDays(1).ToString("yyyy-MM-dd"))
                                        .Where(x => l.Any(t => t == x.SzRtServerID.ToString())).ToList();
            }
            catch (Exception)
            {

            }
            
            
            var listTotaleTP = _dbContext.TransactionAffiliation
                               .Where(x => (x.DPosDateTime >= trxDateFrom.Date
                                                                && x.DPosDateTime < trxDateTo.Date.AddDays(1)
                                                             //  || (x.DPosDateTime == null && x.DRtDateTime >= trxDateFrom.Date && x.DRtDateTime < trxDateTo.Date.AddDays(1))
                                                               )
                                                               && l.Any(s => s == x.SzRtServerId)
                                                                )
                            .Select(x => new TransactionAffiliation
                            {
                                SzRtDeviceId=x.SzRtDeviceId,
                                DPosDateTime = x.DPosDateTime,
                                DPosTransactionTurnover = ((x.DPosTransactionTurnover ??0)* (x.LPosReceivedTransactionCounter ?? 1)),
                                DRtDateTime = x.DRtDateTime,
                                DRtTransactionTurnover = ((x.DRtTransactionTurnover ??0)* (x.LRtReceivedTransactionCounter ?? 1)),
                                SzRtServerId = x.SzRtServerId,

                            })
                            .AsEnumerable()
                           .GroupBy(x => new { SzRtServerId=x.SzRtServerId, Date = x.DPosDateTime != null ? x.DPosDateTime.Value.Date : x.DRtDateTime.Value.Date },
                           (key, group) => new
                           {
                               SzRtServerId = key.SzRtServerId,
                               Date=key.Date,
                               TotaleTP=group.Sum(x=>x.DPosTransactionTurnover),
                               TotalRT=group.Sum(x=>x.DRtTransactionTurnover)
                           }).ToList();
            var res =new List<RtServersHome>();
            foreach(var t in listTotalADE)
            {
                var found = listTotaleTP.Where(x => x.Date.Date == t.DRtDeviceClosureDateTime.Value.Date && t.SzRtServerID.ToString() == x.SzRtServerId);
               
                if (found != null)
                {
                     var tp = found.Sum(x => x.TotaleTP);
                    var rts = found.Sum(x => x.TotalRT);
                    if (!(tp.Equals(t.Total) && rts.Equals(t.Total)))
                    {
                        var r = new RtServersHome()
                        {
                            SzRtServerId = t.SzRtServerID.ToString(),
                            DateLastClosure = t.DRtDeviceClosureDateTime,
                            TotalADE = t.Total
                        };
                        res.Add(r);
                    }
                }
            }
            
            
                return res;

        }

        public async Task<List<ListServersStatusHomeBYDay>> ListRtServerStatusHomeFiltered2(FiltersmodelBindingRequest filters)
        {

            DateTime trxDateFrom = DateTime.ParseExact(
                      filters.TransmissionDateFrom,
                      "dd-MM-yyyy",
                      System.Globalization.CultureInfo.InvariantCulture);

            DateTime trxDateTo = DateTime.ParseExact(
                       filters.TransmissionDateTo,
                       "dd-MM-yyyy",
                       System.Globalization.CultureInfo.InvariantCulture);

            var baseQ = _dbContext.RtServer
                .Include(st => st.RtServerStatus)
                .Include(s => s.L)
                .ThenInclude(sg => sg.LStoreGroup)
                .Where(x => x.RtServerStatus != null);

            var lists = baseQ;

            var list = lists;
            if (!string.IsNullOrEmpty(filters.ServerRt))
            {
                list = list.Where(s => s.SzRtServerId.ToLower().Contains(filters.ServerRt.ToLower()));//.ToList();
            }

            if (!string.IsNullOrEmpty(filters.StoreGroup))
            {
                var storeGroup = 0;
                if (int.TryParse(filters.StoreGroup, out storeGroup))
                {
                    list = list.Where(x => x.LStoreGroupId == storeGroup);//.ToList();
                }
                else
                {
                    list = list.Where(x => x.L.LStoreGroup.SzDescription.ToLower()
                                                                            .Contains(filters.StoreGroup.ToLower()));//.ToList();
                }
            }

            if (!string.IsNullOrEmpty(filters.Store))
            {
                var store = 0;
                if (int.TryParse(filters.Store, out store))
                {
                    list = list.Where(x => x.L.LRetailStoreId == store);
                }
                else
                {
                    list = list.Where(x => string.Format("({0}) {1}", x.L.LRetailStoreId, x.L.SzDescription).ToLower()
                                                                         .Contains(filters.Store.ToLower()));//.ToList();
                }
            }

            if (!string.IsNullOrEmpty(filters.Status))
            {
                Boolean.TryParse(filters.Status, out var status);
                list = list.Where(s => s.BOnDutyFlag == status);//.ToList();
            }

            if (!string.IsNullOrEmpty(filters.Error))
            {
                Boolean.TryParse(filters.Error, out var error);
                list = list.Where(s => s.RtServerStatus.BOnErrorFlag == error);//.ToList();
            }

            var listSrv = list.Select(x => x.SzRtServerId).ToList();

            var listTrx = ListTransmissionsByDayForHome2(listSrv, trxDateFrom, trxDateTo);
            var listTrn = await ListTransactionsByDayForHome2(listSrv, trxDateFrom, trxDateFrom,_properties.Value.TransactionErrorTable.Select(x=>x.Value));

            listTrx.ForEach(x =>
            {
                var listserverHome = new List<RtServersHome>();

                x.ListRtServersHome.ToList().ForEach(srv =>
                {
                    var totalADE = srv.TotalADE ?? 0.00m;
                    var foundDate = listTrn.Where(trn => trn.OperationClosureDatetime.Value.Date == x.OperationClosureDatetime.Value.Date).FirstOrDefault();

                    if (foundDate != null && foundDate.ListRtServersHome.Count() > 0 && foundDate.ListRtServersHome != null)
                    {
                        var found = foundDate.ListRtServersHome;
                        var foundsrv = found.Where(tmp => srv.SzRtServerId == tmp.SzRtServerId).ToList();

                        if (foundsrv != null && foundsrv.Count() > 0)
                        {
                            var totalTP = foundsrv.Sum(x => x.TotalTP ?? 0.00m);
                            var totalRT = foundsrv.Sum(x => x.TotalRT ?? 0.00m);
                            foreach (var s in foundsrv.ToList())
                            {
                                RtServersHome rtServer = new RtServersHome();
                                //rtServer = srv;
                                rtServer.SzRtServerId = srv.SzRtServerId;
                                rtServer.TotalADE = totalADE;
                                rtServer.LRetailStoreId = s.LRetailStoreId;
                                rtServer.LStoreGroupId = s.LStoreGroupId;
                                rtServer.L = s.L;
                                rtServer.BOnDutyFlag = s.BOnDutyFlag;
                                rtServer.BOnError = s.BOnError;
                                rtServer.IsChecked = s.IsChecked;
                                rtServer.NonCompliant = s.NonCompliant;
                                rtServer.NonCompliantOrHasMismatch = s.NonCompliant.ToString();
                                rtServer.RtServerStatus = s.RtServerStatus;
                                rtServer.TotalTP = totalTP;
                                rtServer.TotalRT = totalRT;
                                rtServer.TrasnmissionError = totalADE.Equals(totalTP) || totalADE.Equals(totalRT);
                                rtServer.DateLastClosure = x.OperationClosureDatetime;
                                rtServer.TransmissionChecked = srv.TransmissionChecked;
                                listserverHome.Add(rtServer);
                            }

                        }
                    }
                    //else
                    //{
                    //    var tmpsrv = list.Where(x => x.SzRtServerId == srv.SzRtServerId);

                    //    foreach (var s in tmpsrv)
                    //    {
                    //        srv.BOnDutyFlag = s.BOnDutyFlag;
                    //        srv.BOnError = s.BOnDutyFlag;
                    //        srv.IsChecked = "false";
                    //        srv.L = s.L;
                    //        srv.LRetailStoreId = s.LRetailStoreId;
                    //        srv.LStoreGroupId = s.LStoreGroupId;
                    //        srv.NonCompliant = true;
                    //        srv.NonCompliantOrHasMismatch = "true";
                    //        srv.RtServerStatus = s.RtServerStatus;
                    //        srv.TotalTP = 0.00m;
                    //        srv.TrasnmissionError = srv.TotalADE.Equals(0.00m);
                    //        srv.TransmissionChecked = srv.TransmissionChecked;
                    //        srv.DateLastClosure = x.OperationClosureDatetime;
                    //        listserverHome.Add(srv);

                    //    }
                    //}
                });
                x.ListRtServersHome = listserverHome.ToList()
                                          .GroupBy(x => new { x.SzRtServerId, x.LRetailStoreId, x.LStoreGroupId }, (key, group) => new RtServersHome
                                          {
                                              SzRtServerId = key.SzRtServerId,
                                              L = group.FirstOrDefault().L,
                                              LRetailStoreId = key.LRetailStoreId,
                                              LStoreGroupId = key.LStoreGroupId,
                                              BOnDutyFlag = group.FirstOrDefault().BOnDutyFlag,
                                              BOnError = group.FirstOrDefault().BOnError,
                                              NonCompliant = !group.All(x => x.NonCompliant != true),
                                              NonCompliantOrHasMismatch = (!group.All(x => x.NonCompliant != true)).ToString(),
                                              RtServerStatus = group.FirstOrDefault().RtServerStatus,
                                              TotalTP = group.Sum(x => x.TotalTP ?? 0.00m),
                                              TotalADE = group.Sum(x => x.TotalADE ?? 0.00m),
                                              TransmissionChecked = !group.Any(x => x.TransmissionChecked != true),
                                              TrasnmissionError = group.Sum(x => x.TotalADE).Equals(group.Sum(x => x.TotalTP)),
                                              DateLastClosure = group.FirstOrDefault().DateLastClosure

                                          }).Where(x => !x.TrasnmissionError && !x.TransmissionChecked)
                                          .OrderByDescending(x => x.NonCompliant).ToList();

                //x.ListRtServersHome = x.ListRtServersHome.Where(x => !x.TrasnmissionError && !x.TransmissionChecked).ToList();

            });
            if (!string.IsNullOrEmpty(filters.NonCompliant))
            {
                var Noncompliant = bool.Parse(filters.NonCompliant);
                listTrx.ForEach(x =>
                {
                    x.ListRtServersHome = x.ListRtServersHome.Where(x => x.NonCompliant == Noncompliant).ToList();
                });
            }
            var listRes = listTrx.Select(x => new ListServersStatusHomeBYDay
            {
                OperationClosureDatetime = x.OperationClosureDatetime,
                ListRtServersHome = new List<RtServersHome>()
            }).ToList();
            listRes.ForEach(x =>
            {
                if (x.OperationClosureDatetime.Value.Date == trxDateFrom.Date)
                {
                    x.ListRtServersHome = listTrx.Where(x => x.OperationClosureDatetime.Value.Date == trxDateFrom.Date).SelectMany(x => x.ListRtServersHome).ToList();
                }
            });


            return listTrx;

        }

        //public List<ListServersStatusHomeBYDay> ListTransactionsByDayForHome(List<string> list, DateTime trxDateFrom, DateTime trxDateTo)
        //{

        //    var errTable = _properties.Value.TransactionErrorTable.Select(x => x.Value).ToList();

        //    //var trr = _dbContext.TransactionRtError.Include(x => x.RtServer)
        //    //                                        .Where(x => x.DRtDateTime.Value.Date >= trxDateFrom 
        //    //                                                    && x.DRtDateTime.Value.Date <= trxDateTo
        //    //                                                    && list.Any(s=>s==x.SzRtServerId))
        //    //                .ToList();
        //    //var tr=trr.Where(x => errTable.Any(er => x.SzDescription.Contains(er))).ToList();




        //    var baseQ = _dbContext.TransactionAffiliation.Where(x => x.DPosDateTime.Value.Date >= trxDateFrom
        //                                                         && x.DPosDateTime.Value.Date <= trxDateTo
        //                                                         && x.DPosDateTime != null
        //                                                         && list.Any(s => s == x.SzRtServerId)
        //                                                        );

        //   var listTrn = baseQ;

        //    baseQ.Include(x => x.RtServer).ThenInclude(x => x.TransactionRtError).Load();


        //    //problema è anche qui
        //    var listg = listTrn.ToList()
        //                                .GroupBy(x => x.DPosDateTime.Value.Date, (key, group)
        //                                     => new ListServersStatusHomeBYDay
        //                                     {
        //                                         OperationClosureDatetime = key,
        //                                         ListRtServersHome = group.AsQueryable()
        //                                         .GroupBy(x => new { x.SzRtServerId,x.LRetailStoreId,x.LStoreGroupId}, (key, group) =>
        //                                         new RtServersHome
        //                                         {
        //                                             LRetailStoreId = key.LRetailStoreId,
        //                                             LStoreGroupId = key.LStoreGroupId,
        //                                             SzRtServerId = key.SzRtServerId,
        //                                             NonCompliant = group.AsQueryable().Any(x => (
        //                                                                        x.LTransactionMismatchId == 1
        //                                                                        ||
        //                                                                        x.LTransactionMismatchId == 2
        //                                                                        ||
        //                                                                        (x.LTransactionMismatchId == 3 && x.DRtDateTime.Value.Date < DateTime.Today)
        //                                                                        ||
        //                                                                        (x.LTransactionMismatchId == 4 && x.DPosDateTime.Value.Date < DateTime.Today)
        //                                                                        ||
        //                                                                        ((x.DPosTransactionTurnover.HasValue && x.DRtTransactionTurnover.HasValue)
        //                                                                        && x.DRtTransactionTurnover * (x.LRtReceivedTransactionCounter ?? 1) != x.DPosTransactionTurnover * (x.LPosReceivedTransactionCounter ?? 1))
        //                                                                        ||
        //                                                                        (errTable.Any(er => x.RtServer.TransactionRtError.Where(e => e.SzRtDeviceId == x.SzRtDeviceId && x.LRtDocumentNmbr == x.LRtDocumentNmbr).Where(x => x.SzDescription.Contains(er)).Count() > 0))
        //                                                                        )
        //                                                                       && (x.BTransactionCheckedFlag != true)
        //                                                ),
        //                                             L = group.FirstOrDefault().RtServer.L,
        //                                             RtServerStatus = group.FirstOrDefault().RtServer.RtServerStatus,
        //                                             BOnError = group.FirstOrDefault().RtServer.RtServerStatus.BOnErrorFlag,
        //                                             BOnDutyFlag = group.FirstOrDefault().RtServer.BOnDutyFlag,
        //                                             TotalTP = group.Where(x => x.SzRtDeviceId != null).Sum(x => x.DPosTransactionTurnover * x.LPosReceivedTransactionCounter ?? 1)

        //                                         }).ToList()
        //                         }).ToList();
        //    var listStatus = _dbContext.RtServerStatus;//.ToList();
        //    var listStore = _dbContext.Store;//.ToList();
        //    var listStoreGroup = _dbContext.StoreGroup;//.ToList();
        //    listg.ForEach(x =>
        //    {
        //        x.ListRtServersHome.ForEach(srv =>
        //        {
        //            srv.RtServerStatus = listStatus.FirstOrDefault(x => x.SzRtServerId == srv.SzRtServerId && x.LRetailStoreId == srv.LRetailStoreId && x.LStoreGroupId == srv.LStoreGroupId);
        //            srv.L = listStore.FirstOrDefault(x => x.LRetailStoreId == srv.LRetailStoreId);
        //            srv.L.LStoreGroup = listStoreGroup.FirstOrDefault(x => x.LStoreGroupId == srv.LStoreGroupId);
        //            srv.BOnError = listStatus.FirstOrDefault(x => x.SzRtServerId == srv.SzRtServerId 
        //                                                       && x.LRetailStoreId == srv.LRetailStoreId 
        //                                                       && x.LStoreGroupId == srv.LStoreGroupId)
        //                                                           .BOnErrorFlag;
        //        });
        //    });
        //    return listg;

        //}
        public List<ListServersStatusHomeBYDay> ListTransactionsByDayForHome(List<string> list, DateTime trxDateFrom, DateTime trxDateTo)
        {

            var errTable = _properties.Value.TransactionErrorTable.Select(x => x.Value).ToList();

            var trr = _dbContext.TransactionRtError
                                                     //.Where(x => x.DRtDateTime.Value.Date >= trxDateFrom
                                                     //            && x.DRtDateTime.Value.Date <= trxDateTo
                                                     //            && list.Any(s => s == x.SzRtServerId))
                                                     .Where(x => x.DRtDateTime >= trxDateFrom.Date
                                                                && x.DRtDateTime <= trxDateTo.Date.AddDays(1)
                                                                && list.Any(s => s == x.SzRtServerId))
                                                    .Select(x => new TransactionRtError
                                                    {
                                                        LRtClosureNmbr = x.LRtClosureNmbr,
                                                        LRetailStoreId = x.LRetailStoreId,
                                                        LRtDocumentNmbr = x.LRtDocumentNmbr,
                                                        LStoreGroupId = x.LStoreGroupId,
                                                        SzDescription = x.SzDescription,
                                                        SzRtDeviceId = x.SzRtDeviceId,
                                                        SzRtServerId = x.SzRtDeviceId,
                                                        DRtDateTime = x.DRtDateTime
                                                    }).ToList();

            var tr = trr.Where(x => errTable.Any(er => x.SzDescription.Contains(er)))
                                    .Select(x => new { x.SzRtServerId, x.LRetailStoreId, x.LStoreGroupId, x.LRtDocumentNmbr, x.SzRtDeviceId });

            var errorSrv = tr.Select(x => x.SzRtServerId).ToList();
            //var listStatus = _dbContext.RtServerStatus;//.ToList();
            var listStore = _dbContext.Store.Include(x => x.LStoreGroup);
            //var listSrv = _dbContext.RtServer.Select(x => new { x.BOnDutyFlag, x.SzRtServerId, x.LRetailStoreId, x.LStoreGroupId }).ToList();


            var baseQ = _dbContext.TransactionAffiliation
                                                                //.Where(x =>( x.DPosDateTime.Value.Date >= trxDateFrom.Date
                                                                // && x.DPosDateTime.Value.Date < trxDateTo.Date.AddDays(1)
                                                                //|| (x.DPosDateTime == null && x.DRtDateTime.Value.Date >= trxDateFrom.Date && x.DRtDateTime.Value.Date < trxDateTo.Date.AddDays(1))
                                                                //) 
                                                                //&& list.Any(s => s == x.SzRtServerId)
                                                                //)
                                                                .Where(x => (x.DPosDateTime >= trxDateFrom.Date
                                                                 && x.DPosDateTime < trxDateTo.Date.AddDays(1)
                                                                || (x.DPosDateTime == null && x.DRtDateTime >= trxDateFrom.Date && x.DRtDateTime < trxDateTo.Date.AddDays(1))
                                                                )
                                                                && list.Any(s => s == x.SzRtServerId)
                                                                )
                .Select(x => new RtServersHome
                {
                    DateLastClosure = x.DPosDateTime != null ? x.DPosDateTime.Value.Date : x.DRtDateTime.Value.Date,//x.DPosDateTime.Value.Date,
                    LRetailStoreId = x.LRetailStoreId,
                    LStoreGroupId = x.LStoreGroupId,
                    SzRtServerId = x.SzRtServerId,
                    L = listStore.Where(e => e.LRetailStoreId == x.LRetailStoreId).FirstOrDefault(),
                    RtServerStatus = x.RtServer.RtServerStatus,
                    BOnDutyFlag = x.RtServer.BOnDutyFlag,
                    SzRtDeviceId = x.SzRtDeviceId,
                    BOnError = x.RtServer.RtServerStatus.BOnErrorFlag,
                    TransactionChecked = x.BTransactionCheckedFlag,
                    LRtDocumentNmbr = x.LRtDocumentNmbr,
                    //NonCompliant = x.RtServer.RtServerStatus.BWarningFlag,
                    NonCompliant = ((x.LTransactionMismatchId != null) && (x.LTransactionMismatchId == 1
                                 ||
                                    x.LTransactionMismatchId == 2
                                 ||
                                   (x.LTransactionMismatchId == 3 && x.DRtDateTime < DateTime.Today.Date)
                                 ||
                                   (x.LTransactionMismatchId == 4 && x.DPosDateTime < DateTime.Today.Date))
                                 ||
                                  (
                                  (x.DPosTransactionTurnover.HasValue && x.DRtTransactionTurnover.HasValue)
                                   && x.DRtTransactionTurnover * (x.LRtReceivedTransactionCounter ?? 1)
                                   != x.DPosTransactionTurnover * (x.LPosReceivedTransactionCounter ?? 1)
                                  )
                                 ||
                                  (errorSrv.Any(er => er == x.SzRtServerId)))
                                  && x.BTransactionCheckedFlag != true,
                    TotalTP = (x.DPosTransactionTurnover ?? 0) * (x.LPosReceivedTransactionCounter ?? 1)

                }).AsNoTracking().ToList();


            var listRes = baseQ
                .GroupBy(x => x.DateLastClosure.Value.Date, (key, group) =>
            new ListServersStatusHomeBYDay
            {
                OperationClosureDatetime = key,
                ListRtServersHome = group.GroupBy(t => new { t.SzRtServerId, t.LRetailStoreId, t.LStoreGroupId }, (key, group)
                      => new RtServersHome
                      {
                          BOnDutyFlag = group.FirstOrDefault().BOnDutyFlag,
                          BOnError = group.FirstOrDefault().BOnError,
                          L = group.FirstOrDefault().L,
                          LRetailStoreId = key.LRetailStoreId,
                          LStoreGroupId = key.LStoreGroupId,
                          RtServerStatus = group.FirstOrDefault().RtServerStatus,
                          SzRtDeviceId = group.FirstOrDefault().SzRtDeviceId,
                          SzRtServerId = key.SzRtServerId,
                          TotalTP = group.Where(x => x.SzRtDeviceId != null).Sum(x => x.TotalTP),
                          NonCompliant = group.Any(x => (x.NonCompliant == true || tr.Any(er => er.LRtDocumentNmbr == x.LRtDocumentNmbr
                                                    && er.SzRtDeviceId == x.SzRtDeviceId
                                                    && er.SzRtServerId == x.SzRtServerId)) && x.TransactionChecked != true)

                      }).ToList()
            }).ToList();

            return listRes;

        }

        public Task<IEnumerable<ListServersStatusHomeBYDay>> ListTransactionsByDayForHome2(IEnumerable<string> list, DateTime trxDateFrom, DateTime trxDateTo,IEnumerable<string> errTable)
        {


            var trerr = _dbContext.TransactionRtError.Where(x => x.DRtDateTime >= trxDateFrom.Date
                                                                  && x.DRtDateTime < trxDateTo.AddDays(1).Date
                                                                  && list.Any(s => s == x.SzRtServerId))
                                                                    .AsEnumerable();
            var trnError = trerr.Where(x => errTable.Any(t => x.SzDescription.StartsWith(t)))
                                                    .Select(x => new { x.SzRtServerId, x.LRetailStoreId, x.LStoreGroupId, x.LRtDocumentNmbr, x.SzRtDeviceId,x.LRtClosureNmbr })
                                                    .ToList();

            var rtstatus = _dbContext.RtServerStatus.Select(x=>new RtServerStatus
                        {
                BOnErrorFlag=x.BOnErrorFlag, 
                BWarningFlag=x.BWarningFlag,
                 LRetailStoreId=x.LRetailStoreId,
                  LStoreGroupId=x.LStoreGroupId,
                   SzRtServerId=x.SzRtServerId,
                    SzLastCloseResult=x.SzLastCloseResult,
                     LLastClosureNmbr=x.LLastClosureNmbr
                   
            }).ToList();
            var rt = _dbContext.RtServer.Select(x =>new RtServer{ BOnDutyFlag= x.BOnDutyFlag
                ,LRetailStoreId=x.LRetailStoreId,
                  LStoreGroupId=x.LStoreGroupId,
                   SzRtServerId=x.SzRtServerId }).ToList();
            var storeGroup = _dbContext.StoreGroup.ToList();
            var store = _dbContext.Store.ToList();
            var baseQ2 = _dbContext.TransactionAffiliation
                            .Where(x => (x.DPosDateTime >= trxDateFrom.Date
                                                                && x.DPosDateTime < trxDateTo.Date.AddDays(1)
                                                               || (x.DPosDateTime == null && x.DRtDateTime >= trxDateFrom.Date && x.DRtDateTime < trxDateTo.Date.AddDays(1))
                                                               )
                                                               && list.Any(s => s == x.SzRtServerId)
                                                                )
                            .Select(x => new TransactionAffiliation
                            {
                                BRtNonCompliantFlag = x.BRtNonCompliantFlag,
                                BTransactionCheckedFlag = x.BTransactionCheckedFlag,
                                DPosDateTime = x.DPosDateTime,
                                DPosTransactionTurnover = (x.DPosTransactionTurnover??0) * (x.LPosReceivedTransactionCounter ?? 1),
                                DRtDateTime = x.DRtDateTime,
                                DRtTransactionTurnover = (x.DRtTransactionTurnover ??0)* (x.LRtReceivedTransactionCounter ?? 1),
                                LRetailStoreId = x.LRetailStoreId,
                                LRtClosureNmbr = x.LRtClosureNmbr,
                                LRtDocumentNmbr = x.LRtDocumentNmbr,
                                LStoreGroupId = x.LStoreGroupId,
                                LTransactionMismatch = x.LTransactionMismatch,
                                LTransactionMismatchId = x.LTransactionMismatchId,
                                SzRtDeviceId = x.SzRtDeviceId,
                                SzRtServerId = x.SzRtServerId
                            })
                            .AsEnumerable();
          
            var res =baseQ2
                            .GroupBy(x=>x.DPosDateTime != null ? x.DPosDateTime.Value.Date : x.DRtDateTime.Value.Date, (key, group) =>
            new ListServersStatusHomeBYDay
            {
                OperationClosureDatetime = key,
                ListRtServersHome = group.AsEnumerable().GroupBy(t => new { t.SzRtServerId, t.LRetailStoreId, t.LStoreGroupId }, (key, group)
                      => new RtServersHome
                      {
                          BOnDutyFlag =rt.Where(t=>t.SzRtServerId==key.SzRtServerId && t.LRetailStoreId==key.LRetailStoreId && t.LStoreGroupId==key.LStoreGroupId).FirstOrDefault().BOnDutyFlag,
                          BOnError = rtstatus.Any(t => t.SzRtServerId == key.SzRtServerId && t.LRetailStoreId == key.LRetailStoreId && t.LStoreGroupId == key.LStoreGroupId) ? rtstatus.Where(t => t.SzRtServerId == key.SzRtServerId && t.LRetailStoreId == key.LRetailStoreId && t.LStoreGroupId == key.LStoreGroupId).FirstOrDefault().BOnErrorFlag:null,
                          L = store.FirstOrDefault(x=>x.LRetailStoreId==key.LRetailStoreId),
                          LRetailStoreId = key.LRetailStoreId,
                          LStoreGroupId = key.LStoreGroupId,
                          RtServerStatus = rtstatus.Any(t => t.SzRtServerId == key.SzRtServerId && t.LRetailStoreId == key.LRetailStoreId && t.LStoreGroupId == key.LStoreGroupId) ? rtstatus.Where(t => t.SzRtServerId == key.SzRtServerId && t.LRetailStoreId == key.LRetailStoreId && t.LStoreGroupId == key.LStoreGroupId).FirstOrDefault() : null,
                          SzRtDeviceId = group.FirstOrDefault().SzRtDeviceId,
                          SzRtServerId = key.SzRtServerId,
                          TotalTP = group.Where(x => x.SzRtDeviceId != null).Sum(x => x.DPosTransactionTurnover),
                          TotalRT=group.Where(x => x.SzRtDeviceId != null).Sum(x => x.DRtTransactionTurnover ),

                          NonCompliant = group.Any(x => ((x.LTransactionMismatchId != null) && (x.LTransactionMismatchId == 1
                                 ||
                                    x.LTransactionMismatchId == 2
                                 //||
                                 //  (x.LTransactionMismatchId == 3 && x.DRtDateTime < DateTime.Today.Date)
                                 //||
                                 //  (x.LTransactionMismatchId == 4 && x.DPosDateTime < DateTime.Today.Date)
                                   )
                                 ||
                                  (
                                  (x.DPosTransactionTurnover.HasValue && x.DRtTransactionTurnover.HasValue)
                                   && (x.DRtTransactionTurnover * (x.LRtReceivedTransactionCounter ?? 1))
                                   != (x.DPosTransactionTurnover * (x.LPosReceivedTransactionCounter ?? 1))
                                  )
                                 ||
                                  (trnError.Any(er => er.SzRtServerId == x.SzRtServerId && er.SzRtDeviceId==x.SzRtDeviceId && er.LRtDocumentNmbr==x.LRtDocumentNmbr 
                                  && er.LRtClosureNmbr==x.LRtClosureNmbr && x.BRtNonCompliantFlag==true)))
                                  && x.BTransactionCheckedFlag != true)

                      }).AsEnumerable()
            })
                            ;
            //var t = res.SelectMany(x=>x.ListRtServersHome).ToList();
            return  Task.FromResult( res);      
        }
        public Task<IEnumerable<RtServersHome>> ListTransactionsByDayForHome22(IEnumerable<string> list, DateTime trxDateFrom, DateTime trxDateTo, IEnumerable<string> errTable)
        {


            var trerr = _dbContext.TransactionRtError.Where(x => x.DRtDateTime >= trxDateFrom.Date
                                                                  && x.DRtDateTime < trxDateTo.AddDays(1).Date
                                                                  && list.Any(s => s == x.SzRtServerId))
                                                                    .AsEnumerable();
            var trnError = trerr.Where(x => errTable.Any(t => x.SzDescription.StartsWith(t)))
                                                    .Select(x => new { x.SzRtServerId, x.LRetailStoreId, x.LStoreGroupId, x.LRtDocumentNmbr, x.SzRtDeviceId, x.LRtClosureNmbr })
                                                    .ToList();

            var rtstatus = _dbContext.RtServerStatus.Select(x => new RtServerStatus
            {
                BOnErrorFlag = x.BOnErrorFlag,
                BWarningFlag = x.BWarningFlag,
                LRetailStoreId = x.LRetailStoreId,
                LStoreGroupId = x.LStoreGroupId,
                SzRtServerId = x.SzRtServerId,
                SzLastCloseResult = x.SzLastCloseResult,
                LLastClosureNmbr = x.LLastClosureNmbr

            }).ToList();
            var rt = _dbContext.RtServer.Select(x => new RtServer
            {
                BOnDutyFlag = x.BOnDutyFlag
                ,
                LRetailStoreId = x.LRetailStoreId,
                LStoreGroupId = x.LStoreGroupId,
                SzRtServerId = x.SzRtServerId
            }).ToList();
            var storeGroup = _dbContext.StoreGroup.ToList();
            var store = _dbContext.Store.ToList();
            
            var baseQ1 = _dbContext.TransactionAffiliation
                            .Where(x => ((x.DPosDateTime >= trxDateFrom.Date
                                                                && x.DPosDateTime < trxDateTo.AddDays(1).Date)
                                                               )
                                                               && list.Any(s => s == x.SzRtServerId)
                                                                )
                            .Select(x => new TransactionAffiliation
                            {
                                BRtNonCompliantFlag = x.BRtNonCompliantFlag,
                                BTransactionCheckedFlag = x.BTransactionCheckedFlag,
                                DPosDateTime = x.DPosDateTime,
                                DPosTransactionTurnover = x.DPosTransactionTurnover??0,
                                LPosReceivedTransactionCounter=x.LPosReceivedTransactionCounter??1,
                                DRtDateTime = x.DRtDateTime,
                                DRtTransactionTurnover = x.DRtTransactionTurnover ?? 0,
                                LRtReceivedTransactionCounter=x.LRtReceivedTransactionCounter??1,
                                LRetailStoreId = x.LRetailStoreId,
                                LRtClosureNmbr = x.LRtClosureNmbr,
                                LRtDocumentNmbr = x.LRtDocumentNmbr,
                                LStoreGroupId = x.LStoreGroupId,
                                LTransactionMismatch = x.LTransactionMismatch,
                                LTransactionMismatchId = x.LTransactionMismatchId,
                                SzRtDeviceId = x.SzRtDeviceId,
                                SzRtServerId = x.SzRtServerId
                            })
                            .AsEnumerable()
                        .GroupBy(x => new { DPosDateTime = x.DPosDateTime != null ? x.DPosDateTime.Value.Date : x.DRtDateTime.Value.Date, x.SzRtServerId, x.LRetailStoreId, x.LStoreGroupId })
                        .Select( x => new RtServersHome
                        {
                            DateLastClosure=x.Key.DPosDateTime,
                            BOnDutyFlag = rt.Where(t => t.SzRtServerId ==x.Key.SzRtServerId && t.LRetailStoreId == x.Key.LRetailStoreId && t.LStoreGroupId == x.Key.LStoreGroupId)
                                            .FirstOrDefault().BOnDutyFlag,
                            BOnError = rtstatus.Any(t => t.SzRtServerId == x.Key.SzRtServerId && t.LRetailStoreId == x.Key.LRetailStoreId && t.LStoreGroupId == x.Key.LStoreGroupId) ? rtstatus.Where(t => t.SzRtServerId == x.Key.SzRtServerId && t.LRetailStoreId == x.Key.LRetailStoreId && t.LStoreGroupId == x.Key.LStoreGroupId).FirstOrDefault().BOnErrorFlag : null,
                            L = store.FirstOrDefault(r => r.LRetailStoreId == x.Key.LRetailStoreId),
                            LRetailStoreId = x.Key.LRetailStoreId,
                            LStoreGroupId = x.Key.LStoreGroupId,
                            RtServerStatus = rtstatus.Any(t => t.SzRtServerId == x.Key.SzRtServerId && t.LRetailStoreId == x.Key.LRetailStoreId && t.LStoreGroupId == x.Key.LStoreGroupId) ? rtstatus.Where(t => t.SzRtServerId == x.Key.SzRtServerId && t.LRetailStoreId == x.Key.LRetailStoreId && t.LStoreGroupId == x.Key.LStoreGroupId).FirstOrDefault() : null,
                            SzRtDeviceId = x.FirstOrDefault().SzRtDeviceId,
                            SzRtServerId = x.Key.SzRtServerId,
                            
                            TotalTP = x.Sum(x => x.DPosTransactionTurnover*x.LPosReceivedTransactionCounter),
                            TotalRT = x.Where(x => x.SzRtDeviceId != null).Sum(x => x.DRtTransactionTurnover*x.LRtReceivedTransactionCounter),

                            NonCompliant = x.Any(x => ((x.LTransactionMismatchId != null) && (x.LTransactionMismatchId == 1
                                   ||
                                      x.LTransactionMismatchId == 2
                                    
                                     )
                                   ||
                                    (
                                    (x.DPosTransactionTurnover.HasValue && x.DRtTransactionTurnover.HasValue)
                                     && (x.DRtTransactionTurnover * (x.LRtReceivedTransactionCounter ?? 1))
                                     != (x.DPosTransactionTurnover * (x.LPosReceivedTransactionCounter ?? 1))
                                    )
                                   ||
                                    (trnError.Any(er => er.SzRtServerId == x.SzRtServerId && er.SzRtDeviceId == x.SzRtDeviceId && er.LRtDocumentNmbr == x.LRtDocumentNmbr
                                    && er.LRtClosureNmbr == x.LRtClosureNmbr && x.BRtNonCompliantFlag == true)))
                                    && x.BTransactionCheckedFlag != true)

                        }).Distinct();

                            ;
            return Task.FromResult(baseQ1);
        }
        public Task<IEnumerable<ListServersStatusHomeBYDay>> ListTransactionsByDayForHome3(IEnumerable<string> list, DateTime trxDateFrom, DateTime trxDateTo,IEnumerable<string> errTable)
        {

            var trerr = _dbContext.TransactionRtError.Where(x => x.DRtDateTime >= trxDateFrom.Date
                                                                  && x.DRtDateTime <= trxDateTo.Date.AddDays(1)
                                                                  && list.Any(s => s == x.SzRtServerId))
                                                                    .AsEnumerable();
            var trnError = trerr.Where(x => errTable.Any(t => x.SzDescription.StartsWith(t)))
                                                    .Select(x => new { x.SzRtServerId, x.LRetailStoreId, x.LStoreGroupId, 
                                                                        x.LRtDocumentNmbr, x.SzRtDeviceId ,x.LRtClosureNmbr})
                                                    .ToList();

            var rtstatus = _dbContext.RtServerStatus.Select(x => new RtServerStatus
            {
                BOnErrorFlag = x.BOnErrorFlag,
                BWarningFlag = x.BWarningFlag,
                LRetailStoreId = x.LRetailStoreId,
                LStoreGroupId = x.LStoreGroupId,
                SzRtServerId = x.SzRtServerId,
                SzLastCloseResult = x.SzLastCloseResult,
                LLastClosureNmbr = x.LLastClosureNmbr

            }).ToList();
            var rt = _dbContext.RtServer.Select(x => new RtServer
            {
                BOnDutyFlag = x.BOnDutyFlag
                ,
                LRetailStoreId = x.LRetailStoreId,
                LStoreGroupId = x.LStoreGroupId,
                SzRtServerId = x.SzRtServerId
            }).ToList();
            var storeGroup = _dbContext.StoreGroup.ToList();
            var store = _dbContext.Store.ToList();
            var baseQ2 = _dbContext.TransactionAffiliation
                            .Where(x => (x.DPosDateTime >= trxDateFrom.Date
                                                                && x.DPosDateTime < trxDateTo.Date.AddDays(1)
                                                               || (x.DPosDateTime == null && x.DRtDateTime >= trxDateFrom.Date && x.DRtDateTime < trxDateTo.Date.AddDays(1))
                                                               )
                                                               && list.Any(s => s == x.SzRtServerId)
                                                                )
                            .Select(x => new TransactionAffiliation
                            {
                                BRtNonCompliantFlag = x.BRtNonCompliantFlag,
                                BTransactionCheckedFlag = x.BTransactionCheckedFlag,
                                DPosDateTime = x.DPosDateTime,
                                DPosTransactionTurnover = (x.DPosTransactionTurnover ?? 0) * (x.LPosReceivedTransactionCounter ?? 1),
                                DRtDateTime = x.DRtDateTime,
                                DRtTransactionTurnover = (x.DRtTransactionTurnover ?? 0) * (x.LRtReceivedTransactionCounter ?? 1),
                                LRetailStoreId = x.LRetailStoreId,
                                LRtClosureNmbr = x.LRtClosureNmbr,
                                LRtDocumentNmbr = x.LRtDocumentNmbr,
                                LStoreGroupId = x.LStoreGroupId,
                                LTransactionMismatch = x.LTransactionMismatch,
                                LTransactionMismatchId = x.LTransactionMismatchId,
                                SzRtDeviceId = x.SzRtDeviceId,
                                SzRtServerId = x.SzRtServerId
                            })
                            .AsEnumerable();

            var res = baseQ2
               .GroupBy(x => x.DPosDateTime != null ? x.DPosDateTime.Value.Date : x.DRtDateTime.Value.Date, (key2, group) =>
              new ListServersStatusHomeBYDay
              {
                  OperationClosureDatetime = key2,
                  ListRtServersHome = group.AsEnumerable().GroupBy(t => new { t.SzRtServerId, t.LRetailStoreId, t.LStoreGroupId }, (key, group)
                        => new RtServersHome
                        {
                             DateLastClosure=key2,
                            BOnDutyFlag = rt.Where(t => t.SzRtServerId == key.SzRtServerId && t.LRetailStoreId == key.LRetailStoreId && t.LStoreGroupId == key.LStoreGroupId).FirstOrDefault().BOnDutyFlag,
                            BOnError = rtstatus.Any(t => t.SzRtServerId == key.SzRtServerId && t.LRetailStoreId == key.LRetailStoreId && t.LStoreGroupId == key.LStoreGroupId) ? rtstatus.Where(t => t.SzRtServerId == key.SzRtServerId && t.LRetailStoreId == key.LRetailStoreId && t.LStoreGroupId == key.LStoreGroupId).FirstOrDefault().BOnErrorFlag : null,
                            L = store.FirstOrDefault(x => x.LRetailStoreId == key.LRetailStoreId),
                            LRetailStoreId = key.LRetailStoreId,
                            LStoreGroupId = key.LStoreGroupId,
                            RtServerStatus = rtstatus.Any(t => t.SzRtServerId == key.SzRtServerId && t.LRetailStoreId == key.LRetailStoreId && t.LStoreGroupId == key.LStoreGroupId) ? rtstatus.Where(t => t.SzRtServerId == key.SzRtServerId && t.LRetailStoreId == key.LRetailStoreId && t.LStoreGroupId == key.LStoreGroupId).FirstOrDefault() : null,
                            SzRtDeviceId = group.FirstOrDefault().SzRtDeviceId,
                            SzRtServerId = key.SzRtServerId,
                            TotalTP = group.Where(x => x.SzRtDeviceId != null).Sum(x => x.DPosTransactionTurnover),
                            TotalRT = group.Where(x => x.SzRtDeviceId != null).Sum(x => x.DRtTransactionTurnover),
                            NonCompliant = group.Any(x => ((x.LTransactionMismatchId != null) && (x.LTransactionMismatchId == 1
                                   ||
                                      x.LTransactionMismatchId == 2
                                   ||
                                     (x.LTransactionMismatchId == 3 && x.DRtDateTime < DateTime.Today.Date)
                                   ||
                                     (x.LTransactionMismatchId == 4 && x.DPosDateTime < DateTime.Today.Date))
                                   ||
                                    (
                                    (x.DPosTransactionTurnover.HasValue && x.DRtTransactionTurnover.HasValue)
                                     && (x.DRtTransactionTurnover * (x.LRtReceivedTransactionCounter ?? 1))
                                     != (x.DPosTransactionTurnover * (x.LPosReceivedTransactionCounter ?? 1))
                                    )
                                   ||
                                    (trnError.Any(er => er.SzRtServerId == x.SzRtServerId && er.SzRtDeviceId == x.SzRtDeviceId 
                                                        && er.LRtDocumentNmbr == x.LRtDocumentNmbr && er.LRtClosureNmbr==x.LRtClosureNmbr)&& x.BRtNonCompliantFlag==true))
                                                        && x.BTransactionCheckedFlag != true)

                        }).AsEnumerable()
              })
                            ;
            var t = res.SelectMany(x => x.ListRtServersHome).ToList();
            return Task.FromResult(res);
        }


            public List<ListServersStatusHomeBYDay> ListTransmissionsByDayForHome(List<string> list, DateTime trxDateFrom, DateTime trxDateTo)
        {

            var baseQ = _dbContext.RtServerTransmissionDetail.Where(x => x.DRtDeviceClosureDateTime.Value.Date >= trxDateFrom.Date
                                                                       && x.DRtDeviceClosureDateTime.Value.Date <= trxDateTo.Date
                                                                       && list.Any(srv => srv == x.SzRtServerId)
                                                                          )

                                                                        .Select(x => new
                                                                        {
                                                                            SzRtServerId = x.SzRtServerId,
                                                                            BTransmissionCheckedFlag = x.BTransactionCheckedFlag,
                                                                            DRtDeviceClosureDateTime = x.DRtDeviceClosureDateTime,
                                                                            SzRtDeviceId = x.SzRtDeviceId,
                                                                            totalADE = x.RtServerTransmissionDetailRtData.Sum(x => x.DSaleAmount + x.DVatAmount - x.DReturnAmount - x.DVoidAmount)
                                                                        })
                                                                        .AsEnumerable()
                                                                        .GroupBy(x => new { DRtDeviceClosureDateTime = x.DRtDeviceClosureDateTime.Value.Date }
                                                                         , (key, group) =>
                                                              new ListServersStatusHomeBYDay
                                                              {
                                                                  OperationClosureDatetime = group.Max(x => x.DRtDeviceClosureDateTime),
                                                                  ListRtServersHome = group.GroupBy(x => x.SzRtServerId, (key, group) =>
                                                                          new RtServersHome
                                                                          {
                                                                              SzRtServerId = key,
                                                                              TransmissionChecked = !group.Any(x => x.BTransmissionCheckedFlag != true),
                                                                              TotalADE = group.Sum(x => x.totalADE)
                                                                          }).ToList()
                                                              })
                                                                    ;
            return baseQ.ToList();

            
        }
        public List<ListServersStatusHomeBYDay> ListTransmissionsByDayForHome2(List<string> list, DateTime trxDateFrom, DateTime trxDateTo)
        {
            var baseQ = _dbContext.RtServerTransmissionDetail.Where(x => x.DRtDeviceClosureDateTime.Value.Date >= trxDateFrom.Date
                                                                       && x.DRtDeviceClosureDateTime.Value.Date <= trxDateTo.Date
                                                                       && list.Any(srv => srv == x.SzRtServerId)
                                                                     )
                                                                        .Select(x => new
                                                                        {
                                                                            SzRtServerId = x.SzRtServerId,
                                                                            BTransmissionCheckedFlag = x.BTransactionCheckedFlag,
                                                                            DRtDeviceClosureDateTime = x.DRtDeviceClosureDateTime,
                                                                            SzRtDeviceId = x.SzRtDeviceId,
                                                                            totalADE = x.RtServerTransmissionDetailRtData.Sum(x => x.DSaleAmount + x.DVatAmount - x.DReturnAmount - x.DVoidAmount)
                                                                        })
                                                                        .AsEnumerable()
                                                                        .GroupBy(x => new { DRtDeviceClosureDateTime = x.DRtDeviceClosureDateTime.Value.Date }
                                                                         , (key, group) =>
                                                              new ListServersStatusHomeBYDay
                                                              {
                                                                  OperationClosureDatetime = group.Max(x => x.DRtDeviceClosureDateTime),
                                                                  ListRtServersHome = group.GroupBy(x => x.SzRtServerId, (key, group) =>
                                                                          new RtServersHome
                                                                          {
                                                                              SzRtServerId = key,
                                                                              TransmissionChecked = !group.Any(x => x.BTransmissionCheckedFlag != true),
                                                                              TotalADE = group.Sum(x => x.totalADE)
                                                                          }).ToList()
                                                              })
                                                                    ;

            var listRes = baseQ.ToList();
            return listRes;
        }
        //End new Filter List Home

        public async Task<List<RtServer>> ListRtServerGroupedByServerId(string serverId)
        {
            return await _dbContext.RtServer
                .Include(st => st.RtServerStatus)
                .Include(s => s.L)
                .ThenInclude(sg => sg.LStoreGroup)
                .Include(t => t.TransactionRtError)
                .Include(ta => ta.TransactionAffiliation)
                .Where(x => x.SzRtServerId == serverId)
                .ToListAsync();

        }
        public async Task<List<ListServersGrouped>> ListRtServerByStoreAndStoreGroup()
        {
            var list1 = (await _dbContext.RtServer
                            .Include(st => st.RtServerStatus)
                            .Include(s => s.L)
                            .ThenInclude(sg => sg.LStoreGroup)
                            .Include(ta => ta.TransactionAffiliation)
                            .AsNoTracking()
                            .ToListAsync())
                           ;

            var list = list1
            .GroupBy(x => new { x.LStoreGroupId, x.LRetailStoreId }, (key, group)
                => new ListServersGrouped
                {
                    LRetailStoreId = key.LRetailStoreId,
                    LStoreGroupId = key.LStoreGroupId,
                    RtServers = group.ToList()
                })
                    .ToList();

            return list;
        }

        public bool RemoveProject(RtServer rtServer)
        {
            try
            {
                _logger.LogDebug($"Server RT Matricule: {rtServer.SzRtServerId} is disabled successfully");
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error adding Server RT Matricule {rtServer.SzRtServerId}  :{e.Message}");

                return false;
            }
        }



        public List<RtServersHomeViewModel> FilteredListHome(List<RtServersHomeViewModel> listFiltered, FiltersmodelBindingRequest filters)
        {

            if (!string.IsNullOrEmpty(filters.ServerRt))
            {
                listFiltered = listFiltered.Where(s => s.SzRtServerId.ToLower()
                                                            == filters.ServerRt.ToLower()).ToList();
            }
            if (!string.IsNullOrEmpty(filters.StoreGroup))
            {
                var storeGroup = 0;
                if (int.TryParse(filters.StoreGroup, out storeGroup))
                {
                    listFiltered = listFiltered.Where(x => x.LStoreGroupId == storeGroup).ToList();
                }
                else
                {
                    listFiltered = listFiltered.Where(x => x.L.LStoreGroup.SzDescription.ToLower()
                                                                                                      .Contains(filters.StoreGroup.ToLower())).ToList();
                }
            }

            if (!string.IsNullOrEmpty(filters.Store))
            {
                var store = 0;
                if (int.TryParse(filters.Store, out store))
                {
                    listFiltered = listFiltered.Where(x => x.L.LRetailStoreId == store).ToList();

                }
                else
                {
                    listFiltered = listFiltered.Where(x => x.L.StoreNameComplet.ToLower()
                                                                         .Contains(filters.Store.ToLower())).ToList();

                }

            }

            if (!string.IsNullOrEmpty(filters.Status))
            {
                listFiltered = listFiltered.Where(s => s.BOnDutyFlag.ToString().ToLower()
                                                        .Contains(filters.Status.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(filters.Error))
            {
                listFiltered = listFiltered.Where(s => (s.BOnError.ToString().ToLower()
                                                                                     .Contains(filters.Error.ToLower()))).ToList();
            }
            if (!string.IsNullOrEmpty(filters.NonCompliant))
            {
                var Noncompliant = bool.Parse(filters.NonCompliant);
                listFiltered = listFiltered.Where(x => x.NonCompliant.ToString().ToLower().Contains(filters.NonCompliant.ToLower())).ToList();
            }
            return listFiltered;
        }

        //to testing
        //public async Task<List<ListServersGrouped>> FilteredList2(FiltersmodelBindingRequest filters)
        //{

        //    var listServer = new List<ListServersGrouped>();
        //    Expression<Func<TransactionAffiliation, bool>> expressTrnFrom = null;
        //    if (!string.IsNullOrEmpty(filters.TransactionDateFrom))
        //    {
        //        DateTime transactionDate = DateTime.ParseExact(
        //            filters.TransactionDateFrom,
        //            "dd-MM-yyyy",
        //            System.Globalization.CultureInfo.InvariantCulture);
        //        //expressTrnFrom=x=>x.DRtDateTime.HasValue && x.DRtDateTime.Value.Date >= transactionDate;
        //        expressTrnFrom = x => (x.DRtDateTime.Value.Date != null ? x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date) >= transactionDate;
        //    }
        //    Expression<Func<TransactionAffiliation, bool>> expressTrnTo = null;
        //    if (!string.IsNullOrEmpty(filters.TransactionDateTo))
        //    {
        //        DateTime transactionDate = DateTime.ParseExact(
        //            filters.TransactionDateTo,
        //            "dd-MM-yyyy",
        //            System.Globalization.CultureInfo.InvariantCulture);


        //        expressTrnTo = x => (x.DRtDateTime.Value.Date != null ? x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date) <= transactionDate;

        //    }

        //    Expression<Func<TransactionAffiliation, bool>> expressTrnPos = null;
        //    if (!string.IsNullOrEmpty(filters.Pos))
        //    {
        //        expressTrnPos = x => x.LPosWorkstationNmbr.ToString() == filters.Pos;

        //    }

        //    Expression<Func<TransactionAffiliation, bool>> expressTrnClosureNmbr = null;

        //    if (!string.IsNullOrEmpty(filters.LRtClosureNmbr))
        //    {
        //        expressTrnClosureNmbr = x => x.LRtClosureNmbr.ToString() == filters.LRtClosureNmbr;

        //    }

        //    Expression<Func<TransactionAffiliation, bool>> expressTrnDocumentNmbr = null;

        //    if (!string.IsNullOrEmpty(filters.LRtDocumentNmbr))
        //    {

        //        expressTrnDocumentNmbr = x => x.LRtDocumentNmbr.ToString() == filters.LRtDocumentNmbr;
        //    }

        //    var baseQ = _dbContext.TransactionAffiliation.Where(x => x.SzRtServerId == filters.ServerRt);
        //    if (expressTrnFrom != null)
        //    {
        //        baseQ = baseQ.Where(expressTrnFrom);

        //    }
        //    if (expressTrnTo != null)
        //    {
        //        baseQ = baseQ.Where(expressTrnTo);

        //    }
        //    if (expressTrnPos != null)
        //    {
        //        baseQ = baseQ.Where(expressTrnPos);

        //    }
        //    if (expressTrnClosureNmbr != null)
        //    {
        //        baseQ = baseQ.Where(expressTrnClosureNmbr);

        //    }
        //    baseQ.Include(x => x.RtServer).ThenInclude(x=>x.RtServerStatus).Load();
        //    baseQ.Include(x => x.RtServer).ThenInclude(x => x.L).ThenInclude(x => x.LStoreGroup).Load();
        //    baseQ.Include(x => x.RtServer).ThenInclude(x => x.TransactionRtError).Load();
        //    var listSrvs = baseQ.ToList().GroupBy(x => new { x.SzRtServerId, x.LRetailStoreId, x.LStoreGroupId }, (key, group) =>
        //            new ListServersGrouped
        //            {
        //                LRetailStoreId=key.LRetailStoreId,
        //                 LStoreGroupId=key.LStoreGroupId,
        //                 RtServers=group.GroupBy(x=>x.RtServer).Select(x=>x.Key).ToList()
        //            });



        //    return listSrvs.ToList();



        //}
        public async Task<List<ListServersGrouped>> FilteredList(FiltersmodelBindingRequest filters)
        {
            //var test = await FilteredList2(filters);
            var listServers2 = new List<ListServersGrouped>();
            if (!string.IsNullOrEmpty(filters.ServerRt))
            {
                var baseQ = (_dbContext.RtServer.Where(x => x.SzRtServerId == filters.ServerRt));
                var listSrv = baseQ;
                baseQ.Include(x => x.L).ThenInclude(x => x.LStoreGroup).Load();
                baseQ.Include(x => x.RtServerStatus).Include(x => x.TransactionRtError).Load();


                listServers2 = listSrv.ToList()
                                         .GroupBy(x => new { x.LStoreGroupId, x.LRetailStoreId }, (key, group)
                                 => new ListServersGrouped
                                 {
                                     LRetailStoreId = key.LRetailStoreId,
                                     LStoreGroupId = key.LStoreGroupId,
                                     RtServers = group.ToList()
                                 }).ToList();
            }
            else
            {
                listServers2 = await ListRtServerByStoreAndStoreGroup();
            }
            listServers2.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(filters.ServerRt))
                {
                    x.RtServers = x.RtServers.Where(x => x.SzRtServerId.ToLower()
                                                         == filters.ServerRt.ToLower()).ToList();
                }
                if (!string.IsNullOrEmpty(filters.Error))
                {
                    x.RtServers = x.RtServers.Where(x => x.RtServerStatus.BOnErrorFlag.ToString().ToLower()
                                                                 .Contains(filters.Error.ToLower())).ToList();
                }
                //var trn = new List<TransactionAffiliation>();
                //Expression<Func<TransactionAffiliation, bool>> expressTrnFrom = null;
                //if (!string.IsNullOrEmpty(filters.TransactionDateFrom))
                //{
                //    DateTime transactionDate = DateTime.ParseExact(
                //        filters.TransactionDateFrom,
                //        "dd-MM-yyyy",
                //        System.Globalization.CultureInfo.InvariantCulture);
                //    //expressTrnFrom=x=>x.DRtDateTime.HasValue && x.DRtDateTime.Value.Date >= transactionDate;
                //    expressTrnFrom=x=>(x.DRtDateTime.Value.Date!=null?x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date) >= transactionDate;
                //}
                //Expression<Func<TransactionAffiliation, bool>> expressTrnTo = null;
                //if (!string.IsNullOrEmpty(filters.TransactionDateTo))
                //{
                //    DateTime transactionDate = DateTime.ParseExact(
                //        filters.TransactionDateTo,
                //        "dd-MM-yyyy",
                //        System.Globalization.CultureInfo.InvariantCulture);

                //    //x.RtServers.ForEach(x =>
                //    //{
                //    //    x.TransactionAffiliation = x.TransactionAffiliation
                //    //    .Where(x => x.DRtDateTime.HasValue
                //    //                    && x.DRtDateTime.Value.Date <= transactionDate).ToList();
                //    //});

                //   // expressTrnTo = x => x.DRtDateTime.HasValue && x.DRtDateTime.Value.Date <= transactionDate;
                //    expressTrnTo = x =>( x.DRtDateTime.Value.Date!=null ? x.DRtDateTime.Value.Date:x.DPosDateTime.Value.Date) <= transactionDate;

                //}

                //Expression<Func<TransactionAffiliation, bool>> expressTrnPos = null;
                //if (!string.IsNullOrEmpty(filters.Pos))
                //{
                //    //x.RtServers.ForEach(x =>
                //    //{
                //    //    x.TransactionAffiliation = x.TransactionAffiliation.Where(x => x.LPosWorkstationNmbr.ToString() == filters.Pos

                //    //                                                                    ).ToList();
                //    //});
                //    expressTrnPos = x => x.LPosWorkstationNmbr.ToString() == filters.Pos;

                //}

                //Expression<Func<TransactionAffiliation, bool>> expressTrnClosureNmbr = null;

                //if (!string.IsNullOrEmpty(filters.LRtClosureNmbr))
                //{
                //    //x.RtServers.ForEach(srv =>
                //    //{
                //    //    srv.TransactionAffiliation = srv.TransactionAffiliation.Where(x => x.LRtClosureNmbr.ToString() == filters.LRtClosureNmbr).ToList();
                //    //});
                //    expressTrnClosureNmbr=x => x.LRtClosureNmbr.ToString() == filters.LRtClosureNmbr;

                //}

                //Expression<Func<TransactionAffiliation, bool>> expressTrnDocumentNmbr = null;

                //if (!string.IsNullOrEmpty(filters.LRtDocumentNmbr))
                //{
                //    //x.RtServers.ForEach(srv =>
                //    //{
                //    //    srv.TransactionAffiliation = srv.TransactionAffiliation.Where(x => x.LRtDocumentNmbr.ToString() == filters.LRtDocumentNmbr).ToList();
                //    //});
                //    expressTrnDocumentNmbr=x => x.LRtDocumentNmbr.ToString() == filters.LRtDocumentNmbr;


                //}

                //x.RtServers.ForEach(x =>
                //{
                //    var trn= _dbContext.TransactionAffiliation
                //    .Where(x => x.SzRtServerId == filters.ServerRt);
                //    if (expressTrnFrom != null)
                //    {
                //        trn = trn.Where(expressTrnFrom);

                //    }
                //    if (expressTrnTo != null)
                //    {
                //        trn = trn.Where(expressTrnTo);

                //    }
                //    if (expressTrnPos != null)
                //    {
                //        trn = trn.Where(expressTrnPos);

                //    }
                //    if (expressTrnClosureNmbr != null)
                //    {
                //        trn = trn.Where(expressTrnClosureNmbr);

                //    }
                //    if (expressTrnDocumentNmbr != null)
                //    {
                //        trn = trn.Where(expressTrnDocumentNmbr);

                //    }
                //    x.TransactionAffiliation = trn.ToList();

                //});

            });

            return listServers2;

            //var listServers = new List<ListServersGrouped>();

            //if (!string.IsNullOrEmpty(filters.ServerRt))
            //{
            //    var baseQ = (_dbContext.RtServer.Where(x => x.SzRtServerId == filters.ServerRt));
            //    var listSrv = baseQ;
            //    baseQ.Include(x => x.TransactionAffiliation).Load();

            //    baseQ.Include(x => x.L).ThenInclude(x => x.LStoreGroup).Load();
            //    baseQ.Include(x => x.RtServerStatus).Include(x => x.TransactionRtError).Load();


            //    listServers = listSrv.ToList()
            //                             .GroupBy(x => new { x.LStoreGroupId, x.LRetailStoreId }, (key, group)
            //                     => new ListServersGrouped
            //                     {
            //                         LRetailStoreId = key.LRetailStoreId,
            //                         LStoreGroupId = key.LStoreGroupId,
            //                         RtServers = group.ToList()
            //                     }).ToList();
            //}
            //else
            //{
            //    listServers = await ListRtServerByStoreAndStoreGroup();
            //}

            //listServers.ForEach(x =>
            //{
            //    if (!string.IsNullOrEmpty(filters.ServerRt))
            //    {
            //        x.RtServers = x.RtServers.Where(x => x.SzRtServerId.ToLower()
            //                                             == filters.ServerRt.ToLower()).ToList();
            //    }

            //    if (!string.IsNullOrEmpty(filters.Pos))
            //    {
            //        x.RtServers.ForEach(x =>
            //        {
            //            x.TransactionAffiliation = x.TransactionAffiliation.Where(x => x.LPosWorkstationNmbr.ToString() == filters.Pos

            //                                                                            ).ToList();
            //        });
            //    }

            //    if (!string.IsNullOrEmpty(filters.Error))
            //    {
            //        x.RtServers = x.RtServers.Where(x => x.RtServerStatus.BOnErrorFlag.ToString().ToLower()
            //                                                     .Contains(filters.Error.ToLower())).ToList();
            //    }
            //    if (!string.IsNullOrEmpty(filters.TransactionDateFrom))
            //    {
            //        DateTime transactionDate = DateTime.ParseExact(
            //            filters.TransactionDateFrom,
            //            "dd-MM-yyyy",
            //            System.Globalization.CultureInfo.InvariantCulture);

            //        x.RtServers.ForEach(x =>
            //        {
            //            x.TransactionAffiliation = x.TransactionAffiliation
            //            .Where(x => x.DRtDateTime.HasValue
            //                            && x.DRtDateTime.Value.Date >= transactionDate).ToList();
            //        });
            //    }
            //    if (!string.IsNullOrEmpty(filters.TransactionDateTo))
            //    {
            //        DateTime transactionDate = DateTime.ParseExact(
            //            filters.TransactionDateTo,
            //            "dd-MM-yyyy",
            //            System.Globalization.CultureInfo.InvariantCulture);

            //        x.RtServers.ForEach(x =>
            //        {
            //            x.TransactionAffiliation = x.TransactionAffiliation
            //            .Where(x => x.DRtDateTime.HasValue
            //                            && x.DRtDateTime.Value.Date <= transactionDate).ToList();
            //        });
            //    }
            //    if (!string.IsNullOrEmpty(filters.LRtClosureNmbr))
            //    {
            //        x.RtServers.ForEach(srv =>
            //        {
            //            srv.TransactionAffiliation = srv.TransactionAffiliation.Where(x => x.LRtClosureNmbr.ToString() == filters.LRtClosureNmbr).ToList();
            //        });
            //    }
            //    if (!string.IsNullOrEmpty(filters.LRtDocumentNmbr))
            //    {
            //        x.RtServers.ForEach(srv =>
            //        {
            //            srv.TransactionAffiliation = srv.TransactionAffiliation.Where(x => x.LRtDocumentNmbr.ToString() == filters.LRtDocumentNmbr).ToList();
            //        });
            //    }
            //});

            //return listServers;

        }

        public List<ListServersStatusHomeBYDay> ListServersStatusHomeOfDay(FiltersmodelBindingRequest filters)
        {


            var baseQ = _dbContext.RtServer
                .Include(st => st.RtServerStatus)
                .Include(s => s.L)
                .ThenInclude(sg => sg.LStoreGroup)
                .Where(x => x.RtServerStatus != null);

            var lists = baseQ;

            var list = lists;//.ToList();
            if (!string.IsNullOrEmpty(filters.ServerRt))
            {
                list = list.Where(s => s.SzRtServerId.ToLower()
                                                            == filters.ServerRt.ToLower());//.ToList();
            }

            if (!string.IsNullOrEmpty(filters.StoreGroup))
            {
                var storeGroup = 0;
                if (int.TryParse(filters.StoreGroup, out storeGroup))
                {
                    list = list.Where(x => x.LStoreGroupId == storeGroup);//.ToList();
                }
                else
                {
                    list = list.Where(x => x.L.LStoreGroup.SzDescription.ToLower()
                                                                            .Contains(filters.StoreGroup.ToLower()));//.ToList();
                }
            }

            if (!string.IsNullOrEmpty(filters.Store))
            {
                var store = 0;
                if (int.TryParse(filters.Store, out store))
                {
                    list = list.Where(x => x.L.LRetailStoreId == store);//.ToList();
                }
                else
                {
                    list = list.Where(x => string.Format("({0}) {1}", x.L.LRetailStoreId, x.L.SzDescription).ToLower()
                                                                         .Contains(filters.Store.ToLower()));//.ToList();
                }
            }

            if (!string.IsNullOrEmpty(filters.Status))
            {
                list = list.Where(s => s.BOnDutyFlag.ToString().ToLower()
                                                        .Contains(filters.Status.ToLower()));//.ToList();
            }

            if (!string.IsNullOrEmpty(filters.Error))
            {
                list = list.Where(s => (s.RtServerStatus.BOnErrorFlag.ToString().ToLower()
                                                                                     .Contains(filters.Error.ToLower())));//.ToList();
            }
            DateTime day = DateTime.ParseExact(
                     filters.DayFilter,
                     "dd-MM-yyyy",
                     System.Globalization.CultureInfo.InvariantCulture);


            var listSrv = list.Select(x => x.SzRtServerId).ToList();

            var listTrx = ListTransmissionsByDayForHome(listSrv, day, day);

            var listTrn = ListTransactionsOfDayForHome(listSrv, day, day);


            listTrx.ForEach(x =>
            {
                var listserverHome = new List<RtServersHome>();

                x.ListRtServersHome.ToList().ForEach(srv =>
                {
                    var totalADE = srv.TotalADE ?? 0.00m;
                    var foundDate = listTrn.Where(trn => trn.OperationClosureDatetime.Value.Date == x.OperationClosureDatetime.Value.Date).FirstOrDefault();

                    if (foundDate != null && foundDate.ListRtServersHome.Count() > 0 && foundDate.ListRtServersHome != null)
                    {
                        var foundsrv = foundDate.ListRtServersHome.Where(tmp => srv.SzRtServerId == tmp.SzRtServerId).ToList();

                        if (foundsrv != null && foundsrv.Count() > 0)
                        {
                            var totalTP = foundsrv.Sum(x => x.TotalTP ?? 0.00m);
                            foreach (var s in foundsrv.ToList())
                            {
                                RtServersHome rtServer = new RtServersHome();
                                //rtServer = srv;
                                rtServer.SzRtServerId = srv.SzRtServerId;
                                rtServer.TotalADE = totalADE;
                                rtServer.LRetailStoreId = s.LRetailStoreId;
                                rtServer.LStoreGroupId = s.LStoreGroupId;
                                rtServer.L = s.L;
                                rtServer.BOnDutyFlag = s.BOnDutyFlag;
                                rtServer.BOnError = s.BOnError;
                                rtServer.IsChecked = s.IsChecked;
                                rtServer.NonCompliant = s.NonCompliant;
                                rtServer.NonCompliantOrHasMismatch = s.NonCompliant.ToString();
                                rtServer.RtServerStatus = s.RtServerStatus;
                                rtServer.TotalTP = totalTP;
                                rtServer.TrasnmissionError = totalADE.Equals(totalTP);
                                rtServer.DateLastClosure = x.OperationClosureDatetime;
                                rtServer.TransmissionChecked = srv.TransmissionChecked;
                                listserverHome.Add(rtServer);
                            }

                        }
                    }
                    else
                    {
                        var tmpsrv = list.Where(x => x.SzRtServerId == srv.SzRtServerId);

                        foreach (var s in tmpsrv)
                        {
                            srv.BOnDutyFlag = s.BOnDutyFlag;
                            srv.BOnError = s.RtServerStatus.BOnErrorFlag;
                            srv.IsChecked = "false";
                            srv.L = s.L;
                            srv.LRetailStoreId = s.LRetailStoreId;
                            srv.LStoreGroupId = s.LStoreGroupId;
                            srv.NonCompliant = s.RtServerStatus.BWarningFlag;
                            srv.NonCompliantOrHasMismatch = "true";
                            srv.RtServerStatus = s.RtServerStatus;
                            srv.TotalTP = 0.00m;
                            srv.TrasnmissionError = srv.TotalADE.Equals(0.00m);
                            srv.TransmissionChecked = srv.TransmissionChecked;
                            srv.DateLastClosure = x.OperationClosureDatetime;
                            listserverHome.Add(srv);

                        }
                    }
                });
                x.ListRtServersHome = listserverHome.ToList()
                                          .GroupBy(x => new { x.SzRtServerId, x.LRetailStoreId, x.LStoreGroupId }, (key, group) => new RtServersHome
                                          {
                                              SzRtServerId = key.SzRtServerId,
                                              L = group.FirstOrDefault().L,
                                              LRetailStoreId = key.LRetailStoreId,
                                              LStoreGroupId = key.LStoreGroupId,
                                              BOnDutyFlag = group.FirstOrDefault().BOnDutyFlag,
                                              BOnError = group.FirstOrDefault().BOnError,
                                              NonCompliant = !group.All(x => x.NonCompliant != true),
                                              NonCompliantOrHasMismatch = (!group.All(x => x.NonCompliant != true)).ToString(),
                                              RtServerStatus = group.FirstOrDefault().RtServerStatus,
                                              TotalTP = group.Sum(x => x.TotalTP ?? 0.00m),
                                              TotalADE = group.Sum(x => x.TotalADE ?? 0.00m),
                                              TransmissionChecked = !group.Any(x => x.TransmissionChecked != true),
                                              TrasnmissionError = group.Sum(x => x.TotalADE).Equals(group.Sum(x => x.TotalTP)),
                                              DateLastClosure = group.FirstOrDefault().DateLastClosure

                                          }).Where(x => !x.TrasnmissionError && !x.TransmissionChecked).OrderByDescending(x => x.NonCompliant).ToList();

                //  x.ListRtServersHome = x.ListRtServersHome.Where(x => !x.TrasnmissionError && !x.TransmissionChecked).ToList();

            });
            if (!string.IsNullOrEmpty(filters.NonCompliant))
            {
                var Noncompliant = bool.Parse(filters.NonCompliant);
                listTrx.ForEach(x =>
                {
                    x.ListRtServersHome = x.ListRtServersHome.Where(x => x.NonCompliant == Noncompliant).ToList();
                });
            }
            var listRes = listTrx.Select(x => new ListServersStatusHomeBYDay
            {
                OperationClosureDatetime = x.OperationClosureDatetime,
                ListRtServersHome = new List<RtServersHome>()
            })
                .OrderByDescending(x => x.OperationClosureDatetime.Value.Date).ToList();
            var lastClosureDate = listRes.First().OperationClosureDatetime;
            var listSrvLastDay = listTrx.Where(x => x.OperationClosureDatetime.Value.Date == lastClosureDate.Value.Date).SelectMany(x => x.ListRtServersHome);
            listRes.ForEach(x =>
            {
                if (x.OperationClosureDatetime.Value.Date == lastClosureDate.Value.Date)
                {
                    x.ListRtServersHome = listSrvLastDay.ToList();
                }
            });
            // var test = ListServersHomeFilteredByDay(filters);
            return listRes;

        }

        public List<ListServersStatusHomeBYDay> ListTransactionsOfDayForHome(List<string> listSrv, DateTime dayFilter1, DateTime dayFilter2)
        {
            var listStore = _dbContext.Store.Include(x => x.LStoreGroup).ToList();
            var dayFrom = dayFilter1.Date;
            var dayTo = dayFilter2.AddDays(1);
            var errTable = _properties.Value.TransactionErrorTable.Select(x => x.Value).ToList();

            var trr = _dbContext.TransactionRtError
                                                    .Where(x => x.DRtDateTime >= dayFilter1.Date
                                                                && x.DRtDateTime < dayFilter2.Date.AddDays(1)
                                                                && listSrv.Any(s => s == x.SzRtServerId))
                                                    .Select(x => new TransactionRtError
                                                    {
                                                        LRtClosureNmbr = x.LRtClosureNmbr,
                                                        LRetailStoreId = x.LRetailStoreId,
                                                        LRtDocumentNmbr = x.LRtDocumentNmbr,
                                                        LStoreGroupId = x.LStoreGroupId,
                                                        SzDescription = x.SzDescription,
                                                        SzRtDeviceId = x.SzRtDeviceId,
                                                        SzRtServerId = x.SzRtDeviceId
                                                    }).ToList();

            var tr = trr.Where(x => errTable.Any(er => x.SzDescription.Contains(er)))
                                    .Select(x => new { x.SzRtServerId, x.LRetailStoreId, x.LStoreGroupId, x.LRtDocumentNmbr, x.SzRtDeviceId });

            var errorSrv = tr.Select(x => x.SzRtServerId).ToList();
            var baseQ = _dbContext.TransactionAffiliation.Where(x => x.DPosDateTime.Value.Date >= dayFrom
                                                                 && x.DPosDateTime.Value.Date < dayTo
                                                                && listSrv.Any(s => s == x.SzRtServerId)
                                                                 )
                                                        .Select(x => new RtServersHome
                                                        {
                                                            DateLastClosure = x.DPosDateTime.Value.Date,
                                                            LRetailStoreId = x.LRetailStoreId,
                                                            LStoreGroupId = x.LStoreGroupId,
                                                            SzRtServerId = x.SzRtServerId,
                                                            L = x.RtServer.L,
                                                            // L = listStore.Where(e => e.LRetailStoreId == x.LRetailStoreId).FirstOrDefault(),
                                                            RtServerStatus = x.RtServer.RtServerStatus,
                                                            BOnDutyFlag = x.RtServer.BOnDutyFlag,
                                                            SzRtDeviceId = x.SzRtDeviceId,
                                                            BOnError = x.RtServer.RtServerStatus.BOnErrorFlag,
                                                            TransactionChecked = x.BTransactionCheckedFlag,
                                                            //NonCompliant = x.RtServer.RtServerStatus.BWarningFlag,
                                                            NonCompliant = (x.LTransactionMismatchId != null && (x.LTransactionMismatchId == 1
                                                                               ||
                                                                               x.LTransactionMismatchId == 2
                                                                               ||
                                                                               (x.LTransactionMismatchId == 3 && x.DRtDateTime < DateTime.Today)
                                                                               ||
                                                                                (x.LTransactionMismatchId == 4 && x.DPosDateTime < DateTime.Today)))
                                                                                ||
                                                                                ((x.DPosTransactionTurnover.HasValue && x.DRtTransactionTurnover.HasValue)
                                                                                   && x.DRtTransactionTurnover * (x.LRtReceivedTransactionCounter ?? 1) != x.DPosTransactionTurnover * (x.LPosReceivedTransactionCounter ?? 1))
                                                                                  ||
                                                                                (errorSrv.Any(er => er == x.SzRtServerId)),
                                                            TotalTP = (x.DPosTransactionTurnover ?? 0) * (x.LPosReceivedTransactionCounter ?? 1)

                                                        }).ToList();//.AsNoTracking().ToList();

            var listRes = baseQ.GroupBy(x => x.DateLastClosure.Value.Date)
                                                        .Select(x =>
                                                        new ListServersStatusHomeBYDay
                                                        {
                                                            OperationClosureDatetime = x.Key,
                                                            ListRtServersHome = x.GroupBy(t => new { t.SzRtServerId, t.LRetailStoreId, t.LStoreGroupId }, (key, group)
                                                               => new RtServersHome
                                                               {
                                                                   BOnDutyFlag = group.FirstOrDefault().BOnDutyFlag,
                                                                   BOnError = group.FirstOrDefault().BOnError,
                                                                   L = group.FirstOrDefault().L,
                                                                   LRetailStoreId = key.LRetailStoreId,
                                                                   LStoreGroupId = key.LStoreGroupId,
                                                                   RtServerStatus = group.FirstOrDefault().RtServerStatus,
                                                                   SzRtDeviceId = group.FirstOrDefault().SzRtDeviceId,
                                                                   SzRtServerId = key.SzRtServerId,
                                                                   //  NonCompliant = group.FirstOrDefault().NonCompliant,
                                                                   NonCompliant = group.Any(x => (x.NonCompliant == true || tr.Any(er => er.SzRtServerId == x.SzRtServerId
                                                                                                                               && er.LRetailStoreId == x.LRetailStoreId
                                                                                                                               && er.LStoreGroupId == x.LStoreGroupId
                                                                                                                               && er.SzRtDeviceId == x.SzRtDeviceId
                                                                                                                               && er.LRtDocumentNmbr == x.LRtDocumentNmbr)) && x.TransactionChecked != true),
                                                                   TotalTP = group.Where(x => x.SzRtDeviceId != null).Sum(x => x.TotalTP)
                                                               }).ToList()
                                                        })
                                                        ;






            return listRes.ToList();
        }
        public List<ListServersStatusHomeBYDay> ListServersHomeFilteredByDay(FiltersmodelBindingRequest filters)
        {
            DateTime trxDateFrom = DateTime.ParseExact(
                       filters.TransmissionDateFrom,
                       "dd-MM-yyyy",
                       System.Globalization.CultureInfo.InvariantCulture);

            DateTime trxDateTo = DateTime.ParseExact(
                       filters.TransmissionDateTo,
                       "dd-MM-yyyy",
                       System.Globalization.CultureInfo.InvariantCulture);

            var baseQ = _dbContext.RtServer
                .Include(st => st.RtServerStatus)
                .Include(s => s.L)
                .ThenInclude(sg => sg.LStoreGroup)
                .Where(x => x.RtServerStatus != null);

            var lists = baseQ;
            //await baseQ.Include(x => x.TransactionAffiliation)
            //    .Select(x=>x.TransactionAffiliation.Where(x=>x.DPosDateTime.Value.Date>=trxDateFrom && x.DPosDateTime.Value.Date<=trxDateTo))
            //    .LoadAsync();

            var list = lists;//.ToList();
            if (!string.IsNullOrEmpty(filters.ServerRt))
            {
                list = list.Where(s => s.SzRtServerId.ToLower()
                                                            == filters.ServerRt.ToLower());//.ToList();
            }

            if (!string.IsNullOrEmpty(filters.StoreGroup))
            {
                var storeGroup = 0;
                if (int.TryParse(filters.StoreGroup, out storeGroup))
                {
                    list = list.Where(x => x.LStoreGroupId == storeGroup);//.ToList();
                }
                else
                {
                    list = list.Where(x => x.L.LStoreGroup.SzDescription.ToLower()
                                                                            .Contains(filters.StoreGroup.ToLower()));//.ToList();
                }
            }

            if (!string.IsNullOrEmpty(filters.Store))
            {
                var store = 0;
                if (int.TryParse(filters.Store, out store))
                {
                    list = list.Where(x => x.L.LRetailStoreId == store);//.ToList();
                }
                else
                {
                    list = list.Where(x => string.Format("({0}) {1}", x.L.LRetailStoreId, x.L.SzDescription).ToLower()
                                                                         .Contains(filters.Store.ToLower()));//.ToList();
                }
            }

            if (!string.IsNullOrEmpty(filters.Status))
            {
                list = list.Where(s => s.BOnDutyFlag.ToString().ToLower()
                                                        .Contains(filters.Status.ToLower()));//.ToList();
            }

            if (!string.IsNullOrEmpty(filters.Error))
            {
                list = list.Where(s => (s.RtServerStatus.BOnErrorFlag.ToString().ToLower()
                                                                                     .Contains(filters.Error.ToLower())));//.ToList();
            }

            var listSrv = list.Select(x => x.SzRtServerId).ToList();

            var listTrx = ListTrxFilteredByDays(listSrv, trxDateFrom, trxDateTo);

            var listTrn = ListTrnFilteredByDay(listSrv, trxDateFrom, trxDateTo);

            listTrx.ForEach(x =>
            {
                var listserverHome = new List<RtServersHome>();

                x.ListRtServersHome.ToList().ForEach(srv =>
                {
                    var totalADE = srv.TotalADE ?? 0.00m;
                    var foundDate = listTrn.Where(trn => trn.OperationClosureDatetime.Value.Date == x.OperationClosureDatetime.Value.Date).FirstOrDefault();

                    if (foundDate != null && foundDate.ListRtServersHome.Count() > 0 && foundDate.ListRtServersHome != null)
                    {
                        var found = foundDate.ListRtServersHome;
                        var foundsrv = found.Where(tmp => srv.SzRtServerId == tmp.SzRtServerId).ToList();

                        if (foundsrv != null && foundsrv.Count() > 0)
                        {
                            var totalTP = foundsrv.Sum(x => x.TotalTP ?? 0.00m);
                            foreach (var s in foundsrv.ToList())
                            {
                                RtServersHome rtServer = new RtServersHome();
                                //rtServer = srv;
                                rtServer.SzRtServerId = srv.SzRtServerId;
                                rtServer.TotalADE = totalADE;
                                rtServer.LRetailStoreId = s.LRetailStoreId;
                                rtServer.LStoreGroupId = s.LStoreGroupId;
                                rtServer.L = s.L;
                                rtServer.BOnDutyFlag = s.BOnDutyFlag;
                                rtServer.BOnError = s.BOnError;
                                rtServer.IsChecked = s.IsChecked;
                                rtServer.NonCompliant = s.NonCompliant;
                                rtServer.NonCompliantOrHasMismatch = s.NonCompliant.ToString();
                                rtServer.RtServerStatus = s.RtServerStatus;
                                rtServer.TotalTP = totalTP;
                                rtServer.TrasnmissionError = totalADE.Equals(totalTP);
                                rtServer.DateLastClosure = x.OperationClosureDatetime;
                                rtServer.TransmissionChecked = srv.TransmissionChecked;
                                listserverHome.Add(rtServer);
                            }

                        }
                    }
                    else
                    {
                        var tmpsrv = list.Where(x => x.SzRtServerId == srv.SzRtServerId);

                        foreach (var s in tmpsrv)
                        {
                            srv.BOnDutyFlag = s.BOnDutyFlag;
                            srv.BOnError = s.BOnDutyFlag;
                            srv.IsChecked = "false";
                            srv.L = s.L;
                            srv.LRetailStoreId = s.LRetailStoreId;
                            srv.LStoreGroupId = s.LStoreGroupId;
                            srv.NonCompliant = true;
                            srv.NonCompliantOrHasMismatch = "true";
                            srv.RtServerStatus = s.RtServerStatus;
                            srv.TotalTP = 0.00m;
                            srv.TrasnmissionError = srv.TotalADE.Equals(0.00m);
                            srv.TransmissionChecked = srv.TransmissionChecked;
                            srv.DateLastClosure = x.OperationClosureDatetime;
                            listserverHome.Add(srv);

                        }
                    }
                });
                x.ListRtServersHome = listserverHome.ToList()
                                          .GroupBy(x => new { x.SzRtServerId, x.LRetailStoreId, x.LStoreGroupId }, (key, group) => new RtServersHome
                                          {
                                              SzRtServerId = key.SzRtServerId,
                                              L = group.FirstOrDefault().L,
                                              LRetailStoreId = key.LRetailStoreId,
                                              LStoreGroupId = key.LStoreGroupId,
                                              BOnDutyFlag = group.FirstOrDefault().BOnDutyFlag,
                                              BOnError = group.FirstOrDefault().BOnError,
                                              NonCompliant = !group.All(x => x.NonCompliant != true),
                                              NonCompliantOrHasMismatch = (!group.All(x => x.NonCompliant != true)).ToString(),
                                              RtServerStatus = group.FirstOrDefault().RtServerStatus,
                                              TotalTP = group.Sum(x => x.TotalTP ?? 0.00m),
                                              TotalADE = group.Sum(x => x.TotalADE ?? 0.00m),
                                              TransmissionChecked = !group.Any(x => x.TransmissionChecked != true),
                                              TrasnmissionError = group.Sum(x => x.TotalADE).Equals(group.Sum(x => x.TotalTP)),
                                              DateLastClosure = group.FirstOrDefault().DateLastClosure

                                          }).ToList();

                x.ListRtServersHome = x.ListRtServersHome.Where(x => !x.TrasnmissionError && !x.TransmissionChecked).ToList();

            });
            if (!string.IsNullOrEmpty(filters.NonCompliant))
            {
                var Noncompliant = bool.Parse(filters.NonCompliant);
                listTrx.ForEach(x =>
                {
                    x.ListRtServersHome = x.ListRtServersHome.Where(x => x.NonCompliant == Noncompliant).ToList();
                });
            }

            var listRes = listTrx.Select(x => new ListServersStatusHomeBYDay
            {
                OperationClosureDatetime = x.OperationClosureDatetime,
                ListRtServersHome = new List<RtServersHome>()
            })
            .ToList();
            var lastClosureDate = listRes.FirstOrDefault().OperationClosureDatetime;
            var lastListSrv = listTrx.Where(x => x.OperationClosureDatetime.Value.Date == lastClosureDate.Value.Date).FirstOrDefault().ListRtServersHome;
            listRes.ForEach(x =>
            {
                if (x.OperationClosureDatetime.Value.Date == lastClosureDate.Value.Date)
                {
                    x.ListRtServersHome = lastListSrv;
                }
            });
            return listRes;


        }

        public List<ListServersStatusHomeBYDay> ListTrnFilteredByDay(List<string> listSrv, DateTime dayFrom, DateTime dayTo)
        {
            var errTable = _properties.Value.TransactionErrorTable.Select(x => x.Value).ToList();

            var trr = _dbContext.TransactionRtError.Include(x => x.RtServer)
                                                   .Where(x => x.DRtDateTime.Value.Date >= dayFrom.Date && x.DRtDateTime.Value.Date < dayTo.Date.AddDays(1))
                                                   .ToList();
            var tr = trr.Where(x => errTable.Any(er => x.SzDescription.Contains(er))).ToList();

            var baseQ2 = _dbContext.TransactionAffiliation.Where(x =>
                                                                      x.DPosDateTime.Value.Date >= dayFrom.Date
                                                                   && x.DPosDateTime.Value.Date < dayTo.Date.AddDays(1)
                                                                   && x.DPosDateTime != null
                                                                   && listSrv.Any(s => s == x.SzRtServerId)
                                                              )
                                                            .Select(x => new TransactionAffiliation
                                                            {
                                                                BRtNonCompliantFlag = x.BRtNonCompliantFlag,
                                                                BTransactionCheckedFlag = x.BTransactionCheckedFlag,
                                                                DPosDateTime = x.DPosDateTime,
                                                                DRtDateTime = x.DRtDateTime,
                                                                DPosTransactionTurnover = x.DPosTransactionTurnover * x.LPosReceivedTransactionCounter,
                                                                DRtTransactionTurnover = x.DRtTransactionTurnover * x.LRtReceivedTransactionCounter,
                                                                LRetailStoreId = x.LRetailStoreId,
                                                                LRtClosureNmbr = x.LRtClosureNmbr,
                                                                LRtDocumentNmbr = x.LRtDocumentNmbr,
                                                                LStoreGroupId = x.LStoreGroupId,
                                                                LTransactionMismatchId = x.LTransactionMismatchId,
                                                                RtServer = x.RtServer,
                                                                SzRtDeviceId = x.SzRtDeviceId,
                                                                SzRtServerId = x.SzRtServerId
                                                            })
                                                           .ToList()
                .GroupBy(x => x.DPosDateTime.Value.Date)
                .Select(x => new ListServersStatusHomeBYDay
                {
                    OperationClosureDatetime = x.Key,
                    ListRtServersHome = x.GroupBy(x => new { x.SzRtServerId, x.LRetailStoreId, x.LStoreGroupId })
                                            .Select(x => new RtServersHome
                                            {
                                                LRetailStoreId = x.Key.LRetailStoreId,
                                                LStoreGroupId = x.Key.LStoreGroupId,
                                                SzRtServerId = x.Key.SzRtServerId,
                                                NonCompliant = x.Any(x =>
                                                   (
                                                                                  x.LTransactionMismatchId == 1
                                                                                  ||
                                                                                  x.LTransactionMismatchId == 2
                                                                                  ||
                                                                                  (x.LTransactionMismatchId == 3 && x.DRtDateTime.Value.Date < DateTime.Today)
                                                                                  ||
                                                                                   (x.LTransactionMismatchId == 4 && x.DPosDateTime.Value.Date < DateTime.Today)
                                                                                   ||
                                                                                   (x.DRtTransactionTurnover != x.DPosTransactionTurnover)
                                                                                     ||
                                                                                   (errTable.Any(er => tr.Where(s => s.SzRtDeviceId == x.SzRtDeviceId && s.LRtDocumentNmbr == x.LRtDocumentNmbr)
                                                                                                   .Any(err => err.SzDescription.StartsWith(er))))
                                                        )
                                                    && (x.BTransactionCheckedFlag != true)
                                             ),
                                                L = x.FirstOrDefault().RtServer.L,
                                                RtServerStatus = x.FirstOrDefault().RtServer.RtServerStatus,
                                                BOnError = x.FirstOrDefault().RtServer.RtServerStatus.BOnErrorFlag,
                                                BOnDutyFlag = x.FirstOrDefault().RtServer.BOnDutyFlag,
                                                TotalTP = x.Where(x => x.SzRtDeviceId != null).Sum(x => x.DPosTransactionTurnover * x.LPosReceivedTransactionCounter ?? 1)

                                            }).ToList()
                }).ToList();

            //var baseQ = _dbContext.TransactionAffiliation.Where(x =>
            //                                                        x.DPosDateTime.Value.Date >= dayFrom.Date
            //                                                     && x.DPosDateTime.Value.Date < dayTo.Date.AddDays(1)
            //                                                     && x.DPosDateTime != null
            //                                                     && listSrv.Any(s => s == x.SzRtServerId)
            //                                                  )
            //                                                   .Select(x => new RtServersHome
            //                                                   {
            //                                                       DateLastClosure = x.DPosDateTime.Value.Date,
            //                                                       LRetailStoreId = x.LRetailStoreId,
            //                                                       LStoreGroupId = x.LStoreGroupId,
            //                                                       SzRtServerId = x.SzRtServerId,
            //                                                       L=x.RtServer.L,
            //                                                       RtServerStatus = x.RtServer.RtServerStatus,
            //                                                       BOnDutyFlag = x.RtServer.BOnDutyFlag,
            //                                                       SzRtDeviceId = x.SzRtDeviceId,
            //                                                       BOnError = x.RtServer.RtServerStatus.BOnErrorFlag,
            //                                                       TransactionChecked = x.BTransactionCheckedFlag,
            //                                                       //NonCompliant = x.RtServer.RtServerStatus.BWarningFlag,
            //                                                       TotalTP = (x.DPosTransactionTurnover ?? 0) * (x.LPosReceivedTransactionCounter ?? 1)
            //                                                   }).ToList();

            //var listRes = baseQ.GroupBy(x => x.DateLastClosure.Value.Date)
            //                                            .Select(x =>
            //                                            new ListServersStatusHomeBYDay
            //                                            {
            //                                                OperationClosureDatetime = x.Key,
            //                                                ListRtServersHome = x.GroupBy(t => new { t.SzRtServerId, t.LRetailStoreId, t.LStoreGroupId }, (key, group)
            //                                                   => new RtServersHome
            //                                                   {
            //                                                       BOnDutyFlag = group.FirstOrDefault().BOnDutyFlag,
            //                                                       BOnError = group.FirstOrDefault().BOnError,
            //                                                       L = group.FirstOrDefault().L,
            //                                                       LRetailStoreId = key.LRetailStoreId,
            //                                                       LStoreGroupId = key.LStoreGroupId,
            //                                                       RtServerStatus = group.FirstOrDefault().RtServerStatus,
            //                                                       SzRtDeviceId = group.FirstOrDefault().SzRtDeviceId,
            //                                                       SzRtServerId = key.SzRtServerId,
            //                                                       //NonCompliant = group.FirstOrDefault().NonCompliant,
            //                                                       NonCompliant = group.Any(x => x.NonCompliant != true),
            //                                                       TotalTP = group.Where(x => x.SzRtDeviceId != null).Sum(x => x.TotalTP)
            //                                                   }).ToList()
            //                                            }).ToList()
            //                                            ;

            // return listRes;
            return baseQ2;
        }
        public List<ListServersStatusHomeBYDay> ListTrxFilteredByDays(List<string> listSrv, DateTime dayFrom, DateTime dayTo)
        {

            var baseQ = _dbContext.RtServerTransmissionDetail.Where(x => x.DRtDeviceClosureDateTime >= dayFrom.Date
                                                                    && x.DRtDeviceClosureDateTime < dayTo.Date.AddDays(1)
                                                                    && listSrv.Any(s => s == x.SzRtServerId)
                                                                   );

            var listTrx = baseQ;
            baseQ.Include(x => x.RtServerTransmissionDetailRtData).Load();


            var listRe = listTrx.ToList().GroupBy(x => x.DRtDeviceClosureDateTime.Value.Date)
                .Select(x => new ListServersStatusHomeBYDay
                {
                    OperationClosureDatetime = x.Max(x => x.DRtDeviceClosureDateTime),
                    ListRtServersHome = x.GroupBy(x => x.SzRtServerId, (key, group) =>
                         new RtServersHome
                         {
                             SzRtServerId = key,
                             TransmissionChecked = !group.Any(x => x.BTransactionCheckedFlag != true),
                             TotalADE = group.Sum(x => x.RtServerTransmissionDetailRtData.Sum(x => x.DSaleAmount + x.DVatAmount - x.DReturnAmount - x.DVoidAmount))
                         }).ToList()
                }).ToList();
            var listRes = listTrx.ToList().GroupBy(x => new { DRtDeviceClosureDateTime = x.DRtDeviceClosureDateTime.Value.Date }
                                                              , (key, group) =>
                                                              new ListServersStatusHomeBYDay
                                                              {
                                                                  OperationClosureDatetime = group.Max(x => x.DRtDeviceClosureDateTime),
                                                                  ListRtServersHome = group.GroupBy(x => x.SzRtServerId, (key, group) =>
                                                                          new RtServersHome
                                                                          {
                                                                              SzRtServerId = key,
                                                                              TransmissionChecked = !group.Any(x => x.BTransactionCheckedFlag != true),
                                                                              TotalADE = group.Sum(x => x.RtServerTransmissionDetailRtData.Sum(x => x.DVatAmount + x.DSaleAmount - x.DReturnAmount - x.DVoidAmount))
                                                                          }).ToList()
                                                              }).ToList();
            return listRes;
        }


        public async Task<List<ListServersGrouped>> FilteredListNonCompliant(FiltersmodelBindingRequest filters)
        {
            //var test = await FilteredList2(filters);
            var listServers2 = new List<ListServersGrouped>();
            if (!string.IsNullOrEmpty(filters.ServerRt))
            {
                var baseQ = (_dbContext.RtServer.Where(x => x.SzRtServerId == filters.ServerRt));
                var listSrv = baseQ;
                baseQ.Include(x => x.L).ThenInclude(x => x.LStoreGroup).Load();
                baseQ.Include(x => x.RtServerStatus).Include(x => x.TransactionRtError).Load();


                listServers2 = listSrv.ToList()
                                         .GroupBy(x => new { x.LStoreGroupId, x.LRetailStoreId }, (key, group)
                                 => new ListServersGrouped
                                 {
                                     LRetailStoreId = key.LRetailStoreId,
                                     LStoreGroupId = key.LStoreGroupId,
                                     RtServers = group.ToList()
                                 }).ToList();
            }
            else
            {
                listServers2 = await ListRtServerByStoreAndStoreGroup();
            }
            listServers2.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(filters.ServerRt))
                {
                    x.RtServers = x.RtServers.Where(x => x.SzRtServerId.ToLower()
                                                         == filters.ServerRt.ToLower()).ToList();
                }
                if (!string.IsNullOrEmpty(filters.Error))
                {
                    x.RtServers = x.RtServers.Where(x => x.RtServerStatus.BOnErrorFlag.ToString().ToLower()
                                                                 .Contains(filters.Error.ToLower())).ToList();
                }
                var trn = new List<TransactionAffiliation>();
                Expression<Func<TransactionAffiliation, bool>> expressTrnFrom = null;
                if (!string.IsNullOrEmpty(filters.TransactionDateFrom))
                {
                    DateTime transactionDate = DateTime.ParseExact(
                        filters.TransactionDateFrom,
                        "dd-MM-yyyy",
                        System.Globalization.CultureInfo.InvariantCulture);
                    //expressTrnFrom=x=>x.DRtDateTime.HasValue && x.DRtDateTime.Value.Date >= transactionDate;
                    expressTrnFrom = x => (x.DRtDateTime.Value.Date != null ? x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date) >= transactionDate;
                }
                Expression<Func<TransactionAffiliation, bool>> expressTrnTo = null;
                if (!string.IsNullOrEmpty(filters.TransactionDateTo))
                {
                    DateTime transactionDate = DateTime.ParseExact(
                        filters.TransactionDateTo,
                        "dd-MM-yyyy",
                        System.Globalization.CultureInfo.InvariantCulture);

                    //x.RtServers.ForEach(x =>
                    //{
                    //    x.TransactionAffiliation = x.TransactionAffiliation
                    //    .Where(x => x.DRtDateTime.HasValue
                    //                    && x.DRtDateTime.Value.Date <= transactionDate).ToList();
                    //});

                    // expressTrnTo = x => x.DRtDateTime.HasValue && x.DRtDateTime.Value.Date <= transactionDate;
                    expressTrnTo = x => (x.DRtDateTime.Value.Date != null ? x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date) <= transactionDate;

                }

                Expression<Func<TransactionAffiliation, bool>> expressTrnPos = null;
                if (!string.IsNullOrEmpty(filters.PosWorkstationNmbr))
                {
                    //x.RtServers.ForEach(x =>
                    //{
                    //    x.TransactionAffiliation = x.TransactionAffiliation.Where(x => x.LPosWorkstationNmbr.ToString() == filters.Pos

                    //                                                                    ).ToList();
                    //});
                    expressTrnPos = x => x.LPosWorkstationNmbr.ToString() == filters.PosWorkstationNmbr;

                }

                Expression<Func<TransactionAffiliation, bool>> expressTrnClosureNmbr = null;

                if (!string.IsNullOrEmpty(filters.RtClosureNmbr))
                {
                    //x.RtServers.ForEach(srv =>
                    //{
                    //    srv.TransactionAffiliation = srv.TransactionAffiliation.Where(x => x.LRtClosureNmbr.ToString() == filters.LRtClosureNmbr).ToList();
                    //});
                    expressTrnClosureNmbr = x => x.LRtClosureNmbr.ToString() == filters.RtClosureNmbr;

                }

                Expression<Func<TransactionAffiliation, bool>> expressTrnDocumentNmbr = null;

                if (!string.IsNullOrEmpty(filters.RtDocumentNmbr))
                {
                    //x.RtServers.ForEach(srv =>
                    //{
                    //    srv.TransactionAffiliation = srv.TransactionAffiliation.Where(x => x.LRtDocumentNmbr.ToString() == filters.LRtDocumentNmbr).ToList();
                    //});
                    expressTrnDocumentNmbr = x => x.LRtDocumentNmbr.ToString() == filters.RtDocumentNmbr;


                }

                x.RtServers.ForEach(x =>
                {
                    var trn = _dbContext.TransactionAffiliation
                    .Where(x => x.SzRtServerId == filters.ServerRt);
                    if (expressTrnFrom != null)
                    {
                        trn = trn.Where(expressTrnFrom);

                    }
                    if (expressTrnTo != null)
                    {
                        trn = trn.Where(expressTrnTo);

                    }
                    if (expressTrnPos != null)
                    {
                        trn = trn.Where(expressTrnPos);

                    }
                    if (expressTrnClosureNmbr != null)
                    {
                        trn = trn.Where(expressTrnClosureNmbr);

                    }
                    if (expressTrnDocumentNmbr != null)
                    {
                        trn = trn.Where(expressTrnDocumentNmbr);

                    }
                    x.TransactionAffiliation = trn.ToList();

                });

            });

            return listServers2;


        }
    }
}