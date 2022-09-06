using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DnSrtChecker.FiltersmodelBindRequest;
using DnSrtChecker.Helpers;
using DnSrtChecker.Models;
using DnSrtChecker.Models.ViewModels;
using DnSrtChecker.Models.ViewModels.ModelsFunctions;
using DnSrtChecker.ModelsHelper;
using DnSrtChecker.Persistence;
using DnSrtChecker.Services;
using DynamicExpresso;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RazorEngineCore;

namespace DnSrtChecker.Controllers
{
    [Authorize]
    public class TransmissionsController : BasicController
    {
        private readonly ITransmissionRepository _transmissionRepository;
        private readonly IRtServerRepository _rtServerRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IStoreGroupRepository _storeGroupRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ILogger<TransactionErrorsController> _logger;
        private readonly IMapper _mapper;
        private readonly IOptions<Properties> _properties;

        public TransmissionsController(ITransmissionRepository transmissionRepository,
                                                IRtServerRepository rtServerRepository,
                                                ITransactionRepository transactionRepository,
                                                IStoreRepository storeRepository,
                                                IStoreGroupRepository storeGroupRepository,
                                                IUnitOfWork unitOfWork,
                                                UserManager<User> userManager,
                                                RoleManager<Role> roleManager,
                                                IOptions<Properties> properties,
                                                ILogger<TransactionErrorsController> logger,
                                                IMapper mapper) : base(userManager, roleManager)
        {
            _transmissionRepository = transmissionRepository;
            _rtServerRepository = rtServerRepository;
            _transactionRepository = transactionRepository;
            _storeGroupRepository = storeGroupRepository;
            _properties = properties;
            _storeRepository = storeRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;

        }

        public async Task<ActionResult> Index(string id, int storeId, int storeGroupId, string date)
        {
            _logger.LogDebug("Start: Index Transmissions Controller");

            //var listView = await ListTrasmissionaAndTransactionsByDay22(id, storeId, storeGroupId,date);
            //ListTransmissionsByDay
            var listView = await ListTrasmissionaAndTransactionsByDay22(id, storeId, storeGroupId, date);
            //var listView = await ListTransmissionsByDay(id, storeId, storeGroupId, date);

            if (listView.Count == 0)
            {
                return RedirectToAction("Index", "Home", new FiltersmodelBindingRequest { ServerRt = id, Store = storeId.ToString(), StoreGroup = storeGroupId.ToString() });
            }
            ViewBag["ServerRt"] = id;
            ViewData["date"] = date ?? "";
            ViewData["storeId"] = storeId;
            ViewData["storeGroupId"] = storeGroupId;
            _logger.LogDebug($"END: Return list of all transmission grouped by DeviceId and TransmissionId length:{listView.Count}");
            return View(listView);
        }


        public async Task<ActionResult> InitialList(string id, int storeId, int storeGroupId, string date, 
            string GroupDescription, string RetailDescription, decimal DTotaleADE, decimal DTotalRT, decimal DTotalTP)
        {
            _logger.LogDebug("Start: Index Transmissions Controller");
            ViewData["ServerRt"] = id;
            ViewData["date"] = date ?? "";
            ViewData["storeId"] = storeId;
            ViewData["storeGroupId"] = storeGroupId;
            ViewData["RetailDescription"] = RetailDescription;
            ViewData["GroupDescription"] = GroupDescription;
            ViewData["DTotaleADE"] = DTotaleADE;
            ViewData["DTotalRT"] = DTotalRT;
            ViewData["DTotalTP"] = DTotalTP;
            ViewData["MismatchRTAdE"] = DTotalRT - DTotaleADE ;
            ViewData["MismatchTPAdE"] = DTotalTP - DTotaleADE ;

            /*
             * DTotaleADE = item.DTotalADE,
                                                   DTotalRT = item.DTotalRT,
                                                   DTotalTP = item.DTotalTP
             */
            //var listView = await ListTrasmissionaAndTransactionsByDay22(id, storeId, storeGroupId,date);
            //chiamare la stored procedure che sta creando Marco
            //in base al'utente far vedere certi server
            //se chiamo ListTransmissionByServerIdOfDay22 ??
            //var listView = await ListTransmissionsByDay(id, storeId, storeGroupId, date);
            //var listView = await ListTrasmissionaAndTransactionsByDay22(id, storeId, storeGroupId, date);
            //var listView = await _transmissionRepository.ListTransmissionByServerIdOfDay22(id, date);
            //var listView = await ListTrasmissionaAndTransactionsByDay22("88S25000616", 99, 1, date);
            List<TransmissionsList> listView = new List<TransmissionsList>();
            bool timeOut = false;
            try
            {
                listView = await ListTransmissionsByDay(FiltersmodelBindingRequest.UserName, id, storeId, storeGroupId, date);

            }
            catch (Exception Ex)
            {

                timeOut = true;
            }
            foreach (var item in listView)
            {
                item.DSaleAmountSum += listView
                    .Where(x=> x.szRtDeviceID == item.szRtDeviceID)
                    .Sum(x => x.SaleAmountD);

                item.DTotalADESum += listView
                    .Where(x => x.szRtDeviceID == item.szRtDeviceID)
                    .Sum(x => x.TotalADED);

                item.TotalTPSum += listView
                    .Where(x => x.szRtDeviceID == item.szRtDeviceID)
                    .Sum(x => x.TotalTPD);
            }
            var TotalTPDlIST = listView.Select(x => x).Where(x => x.TotalTPD != 0);
            /*
             Model.Where(x => x.szRtDeviceID
                                           == dev.Key).First().DSaleAmountSum += tv.SaleAmountD;
                                                Model.Where(x => x.szRtDeviceID
                                               == dev.Key).First().DReturnAmountSum += tv.ReturnAmountD;
                                                Model.Where(x => x.szRtDeviceID
                                               == dev.Key).First().DvoidAmountSum += tv.voidAmountD;
                                                Model.Where(x => x.szRtDeviceID
                                               == dev.Key).First().DVatAmountSum += tv.VatAmountD;
                                                Model.Where(x => x.szRtDeviceID
                                               == dev.Key).First().DTotalADESum += tv.TotalADED;
                                                Model.Where(x => x.szRtDeviceID
                                               == dev.Key).First().TotalTPSum += tv.TotalTPD;
             */
            //var deviecIDList = listView.Select(x => x.szRtDeviceID);
            //var transmissionListByDevice = listView.Select(x => x.lRtDeviceTransmissionID) 
            //var list = _mapper.Map<List<TransmissionsList>, List<TransmissionsByDayToIndexView>>(listView);
            var list = listView.GroupBy(x => x.szRtDeviceID).OrderBy(x => x.Key).ToList();



            if (listView.Count == 0)
            {
                return RedirectToAction("Index", "Home", new FiltersmodelBindingRequest { ServerRt = id, Store = storeId.ToString(), StoreGroup = storeGroupId.ToString() });
            }
            ViewData["date"] = Convert.ToDateTime(date).ToString("dd/MM/yyyy") ?? "";
            ViewData["timeOut"] = timeOut;
            _logger.LogDebug($"END: Return list of all transmission grouped by DeviceId and TransmissionId length:{listView.Count}");
            return View("Index", listView);
        }


        public async Task<JsonResult> GetTrxDay(string data)//string id,int storeId,int storeGroupId,string date)
        {
            var parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var id = parameters["id"];
            var storeId = Int32.Parse(parameters["storeId"]);
            var storeGroupId = Int32.Parse(parameters["storeGroupId"]);
            var date = parameters["date"];
            _logger.LogDebug("Start: Group List Trasmissions by day");
            var day = DateTime.ParseExact(
                       parameters["date"],
                       "dd-MM-yyyy",
                       System.Globalization.CultureInfo.InvariantCulture);
            var listTrx = await _transmissionRepository.ListTransmissionByServerIdOfDay22(id, date);
            //var listTrx = await _transmissionRepository.ListTransmissionByServerIdOfDay(parameters["id"], parameters["date"]);
            var listTransmissions = _mapper.Map<List<TransmissionsGroupedByDay>,
                                    List<TransmissionsAndTransactionsGroupedByDayVM>>
                                    (listTrx);
            // var listransaction =await _transactionRepository.ListTransactionServerRt(id, storeId, storeGroupId);
            var listransaction = await _transactionRepository.ListTransactionServerRtDay(id,storeId, storeGroupId, day);
            var listview = Enumerable.Empty<TransmissionsByDayToIndexView>();
            var rtserver = _rtServerRepository.GetRtServerStatus(id);
            listTransmissions.ForEach(t =>
            {
                var exist = listransaction.Where(x => x.RtDeviceId == t.SzRtDeviceId
                                && x.DPosDateTime.Value.Date == t.OperationClosureDatetime.Value.Date
                                 ).ToList();

                if (exist != null && exist.Count() > 0)
                {

                    t.TotalRtServer = exist.Sum(x => x.TotalServerRT);
                    t.TotalTP = exist.Sum(x => x.TotalTP);
                    t.TransactionVats = exist.AsQueryable().SelectMany(x => x.TransactionVats)
                     .GroupBy(x => new { x.DPosVatRate, x.SzVatNature }, (key, group) => new TransactionVatForTrasmissionsViewModel
                     {
                         DPosVatRate = key.DPosVatRate,
                         SzVatNature = key.SzVatNature,
                         DPosGrossAmount = group.Sum(x => x.DPosGrossAmount),
                         DRtGrossAmount = group.Sum(x => x.DRtGrossAmount)
                     }
                    )
                   .ToList()
                                             //.Where(x => x.DPosGrossAmount != 0)
                                             .OrderBy(x => x.DPosVatRate)
                                             .ToList();
                    t.LPosWorkstation = exist.Select(x => x.LPosWorkstationNmbr ?? "").FirstOrDefault();
                    var listTransactionsVat = new List<TransactionVatForTrasmissionsViewModel>();
                    foreach (var vat in t.TransactionVats)
                    {
                        var tmp = t.RtServerTransmissionsDetailRtData
                                   .Where(x => x.DVatRate.Equals(vat.DPosVatRate));// && x.SzVatNature==vat.SzVatNature);
                        if (tmp != null)
                        {
                            foreach (var vtmp in tmp)
                            {
                                vat.DVatRate = vtmp.DVatRate;
                                vat.DSaleAmount = vtmp.DSaleAmount;
                                vat.DVatAmount = vtmp.DVatAmount;
                                vat.DVoidAmount = vtmp.DVoidAmount;
                                vat.DReturnAmount = vtmp.DReturnAmount;
                                vat.SzVatNature = vtmp.SzVatNature;
                                listTransactionsVat.Add(vat);
                            }

                        }

                    }

                    t.TotalReturn = t.TransactionVats.Sum(x => x.DReturnAmount);
                    t.TotalVoid = t.TransactionVats.Sum(x => x.DVoidAmount);
                    t.TotalSale = t.TransactionVats.Sum(x => x.DSaleAmount);
                    t.TotalVat = t.TransactionVats.Sum(x => x.DVatAmount);

                    t.HasVentilation = rtserver.BVatVentilationFlag ?? false;
                    t.TransactionVats = listTransactionsVat.ToList();

                }


            });
            var listTrnHasNotTrm = listransaction.Where(x => !listTransmissions.Exists(t => t.OperationClosureDatetime.Value.Date == x.DPosDateTime.Value.Date
                                                                              && t.SzRtDeviceId == x.RtDeviceId
                                                                             ));
            foreach (var t in listTrnHasNotTrm)
            {
                var obj = new TransmissionsAndTransactionsGroupedByDayVM
                {
                    SzRtServerId = t.RtServerId,
                    SzRtDeviceId = t.RtDeviceId,
                    HasVentilation = rtserver.BVatVentilationFlag ?? false,
                    OperationClosureDatetime = t.DPosDateTime ?? t.DRtDateTime,
                    TotalTP = t.TotalTP,
                    TotalRtServer = t.TotalServerRT,
                    LPosWorkstation = t.LPosWorkstationNmbr,
                    TotalAmount = 0.0m,


                };
                listTransmissions.Add(obj);
            }
            var store = await _storeRepository.GetStore(storeId);
            var storeGroup = await _storeGroupRepository.GetStoreGroup(storeGroupId);
            var listnote = new List<string>();
            listnote.Add("");
            if (listTransmissions != null)
            {
                var listLog = (await _transmissionRepository.GetUserActivityByTransaction(id));
                var listLogVM = _mapper.Map<List<UserActivityLog>, List<UserActivityLogViewModel>>(listLog).ToList();
                listview = listTransmissions
                    .GroupBy(x => x.OperationClosureDatetime.Value.Date, (key, group) =>
                new TransmissionsByDayToIndexView
                {
                    SzRtServerId = id,
                    LRetailStoreId = store.SzDescription,
                    Store = _mapper.Map<Store, StoreViewModel>(store),
                    StoreGroupId = storeGroup.SzDescription,
                    OperationClosureDatetime = key.Date,
                    GTotalAmount = group.Sum(x => x.TotalAmount),
                    GTotalRtServer = group.Sum(x => x.TotalRtServer),
                    GTotalTP = group.Sum(x => x.TotalTP),
                    BTransactionCheckedFlag = group.Where(x => x.LRtServerOperationId != 0).Any(x => (x.MismatchRtAdE != 0 && x.MismatchTPAdE != 0)
                                                     && (x.BTransactionCheckedFlag != null && x.BTransactionCheckedFlag.All(c => c == true)))
                                              || group.Where(x => x.LRtServerOperationId != 0).All(x => x.MismatchTPAdE == 0 && x.MismatchRtAdE == 0),

                    BTransactionArchivedFlag = group.Where(x => x.LRtServerOperationId != 0).Any(x => (x.MismatchRtAdE != 0 && x.MismatchTPAdE != 0)
                                              && (x.BTransactionArchivedFlag != null && x.BTransactionArchivedFlag.All(c => c == true)))
                                              || group.Where(x => x.LRtServerOperationId != 0).All(x => x.MismatchTPAdE == 0 && x.MismatchRtAdE == 0),
                    //  SzTranscationCheckNote = group.Where(x => x.SzTranscationCheckNote != null).FirstOrDefault().SzTranscationCheckNote,
                    TransmissionsGroupedByDay = group.ToList(),
                    // UserActivityLogViewModel = listLogVM.Where(x => x.LRtServerOperationId == group.Max(x => x.LRtServerOperationId)).ToList()
                });
            }
            string path = string.Format("{0}\\Templates\\PartialIndex.cshtml", Directory.GetCurrentDirectory());

            string template = await System.IO.File.ReadAllTextAsync(path);
            RazorEngine razorEngine = new RazorEngine();
            RazorEngineCompiledTemplate temp = razorEngine.Compile(template);
            string res = temp.Run(listview);

            _logger.LogDebug($"END: Return list TransmissionsGrouped length: {listview.Count()}");

            return Json(new { data = res });
            //return  PartialView("PartialIndex", listview);
        }

        public async Task<List<TransmissionsByDayToIndexView>> ListTrasmissionaAndTransactionsByDay(string id, int storeId, int storeGroupId, string date)
        {
            _logger.LogDebug("Start: Group List Trasmissions by day");
            var listview = Enumerable.Empty<TransmissionsByDayToIndexView>();
            var lisRes = new List<TransmissionsByDayToIndexView>();
            var listTrx = await _transmissionRepository.ListTransmissionByServerIdByDay(id, date, _properties.Value.TrxNmbrDays);
            var listTransmissions = _mapper.Map<List<TransmissionsGroupedByDay>,
                                    List<TransmissionsAndTransactionsGroupedByDayVM>>
                                    (listTrx);
            var initDate = DateTime.Today;
            if (listTransmissions.Count() > 0 && listTransmissions != null)
            {
                initDate = listTransmissions.OrderByDescending(x => x.OperationClosureDatetime)
                                            .FirstOrDefault().OperationClosureDatetime.Value;


                //var listransaction = await _transactionRepository.ListTransactionServerRtMonth(id, storeId, storeGroupId, initDate, _properties.Value.TrxNmbrDays);
                var listransaction = await _transactionRepository.ListTransactionServerRtMonth(id, storeId, storeGroupId, initDate, _properties.Value.TrxNmbrDays);


                var rtserver = _rtServerRepository.GetRtServerStatus(id);
                listTransmissions.ForEach(t =>
                {
                    var exist = listransaction.Where(x => x.RtDeviceId == t.SzRtDeviceId
                                    && x.DPosDateTime.Value.Date == t.OperationClosureDatetime.Value.Date
                                     ).ToList();

                    if (exist != null && exist.Count() > 0)
                    {

                        t.TotalRtServer = exist.Sum(x => x.TotalServerRT);
                        t.TotalTP = exist.Sum(x => x.TotalTP);
                        var vats = exist.Where(x => x.TransactionVats != null);
                        if (vats.Count() > 0 && vats != null)
                        {
                            t.TransactionVats = exist.AsQueryable().Where(x => x.TransactionVats != null).SelectMany(x => x.TransactionVats)
                         .GroupBy(x => new { x.DPosVatRate, x.SzVatNature }, (key, group) =>
                          new TransactionVatForTrasmissionsViewModel
                          {
                              DPosVatRate = key.DPosVatRate,
                              SzVatNature = key.SzVatNature,
                              DPosGrossAmount = group.Sum(x => x.DPosGrossAmount),
                              DRtGrossAmount = group.Sum(x => x.DRtGrossAmount)
                          }
                        )
                       .ToList()
                                                 //.Where(x => x.DPosGrossAmount != 0)
                                                 .OrderBy(x => x.DPosVatRate)
                                                 .ToList();
                            t.LPosWorkstation = exist.Select(x => x.LPosWorkstationNmbr ?? "").FirstOrDefault();
                            var listTransactionsVat = new List<TransactionVatForTrasmissionsViewModel>();
                            foreach (var vat in t.TransactionVats)
                            {
                                var tmp = t.RtServerTransmissionsDetailRtData
                                           .Where(x => x.DVatRate.Equals(vat.DPosVatRate));// && x.SzVatNature==vat.SzVatNature);
                                if (tmp != null)
                                {
                                    foreach (var vtmp in tmp)
                                    {
                                        vat.DVatRate = vtmp.DVatRate;
                                        vat.DSaleAmount = vtmp.DSaleAmount;
                                        vat.DVatAmount = vtmp.DVatAmount;
                                        vat.DVoidAmount = vtmp.DVoidAmount;
                                        vat.DReturnAmount = vtmp.DReturnAmount;
                                        vat.SzVatNature = vtmp.SzVatNature;
                                        listTransactionsVat.Add(vat);
                                    }

                                }

                            }

                            t.TotalReturn = t.TransactionVats.Sum(x => x.DReturnAmount);
                            t.TotalVoid = t.TransactionVats.Sum(x => x.DVoidAmount);
                            t.TotalSale = t.TransactionVats.Sum(x => x.DSaleAmount);
                            t.TotalVat = t.TransactionVats.Sum(x => x.DVatAmount);

                            t.HasVentilation = rtserver.BVatVentilationFlag ?? false;
                            t.TransactionVats = listTransactionsVat.ToList();

                        }

                    }


                });
                var listTrnHasNotTrm = listransaction.Where(x => !listTransmissions.Exists(t => t.OperationClosureDatetime.Value.Date == x.DPosDateTime.Value.Date
                                                                                  && t.SzRtDeviceId == x.RtDeviceId
                                                                                 ));
                foreach (var trn in listTrnHasNotTrm)
                {
                    var obj = new TransmissionsAndTransactionsGroupedByDayVM
                    {
                        SzRtServerId = trn.RtServerId,
                        SzRtDeviceId = trn.RtDeviceId,
                        HasVentilation = rtserver.BVatVentilationFlag ?? false,
                        OperationClosureDatetime = trn.DPosDateTime ?? trn.DRtDateTime,
                        TotalTP = trn.TotalTP,
                        TotalRtServer = trn.TotalServerRT,
                        LPosWorkstation = trn.LPosWorkstationNmbr,
                        TotalAmount = 0.0m,


                    };
                    listTransmissions.Add(obj);
                }
                var store = await _storeRepository.GetStore(storeId);
                var storeGroup = await _storeGroupRepository.GetStoreGroup(storeGroupId);
                var listnote = new List<string>();
                listnote.Add("");
                if (listTransmissions != null)
                {
                    var listLog = (await _transmissionRepository.GetUserActivityByTransaction(id));
                    var listLogVM = _mapper.Map<List<UserActivityLog>, List<UserActivityLogViewModel>>(listLog).ToList();

                    listview = listTransmissions
                        .GroupBy(x => x.OperationClosureDatetime.Value.Date,
                        (key, group) =>
                        new TransmissionsByDayToIndexView
                        {
                            SzRtServerId = id,
                            LRetailStoreId = store.SzDescription,
                            Store = _mapper.Map<Store, StoreViewModel>(store),
                            StoreGroupId = storeGroup.SzDescription,
                            OperationClosureDatetime = key.Date,
                            GTotalAmount = group.Sum(x => x.TotalAmount),
                            GTotalRtServer = group.Sum(x => x.TotalRtServer),
                            GTotalTP = group.Sum(x => x.TotalTP),
                            BTransactionCheckedFlag = (group.Where(x => x.LRtServerOperationId != 0)
                                                           .Any(x => (x.MismatchRtAdE != 0 && x.MismatchTPAdE != 0)
                                                             && (x.BTransactionCheckedFlag != null
                                                                && x.BTransactionCheckedFlag.All(c => c == true)))
                                                      || group
                                                      .All(x => x.MismatchTPAdE == 0 && x.MismatchRtAdE == 0)),

                            BTransactionArchivedFlag = (group.Where(x => x.LRtServerOperationId != 0)
                                                            .Any(x => (x.MismatchRtAdE != 0 && x.MismatchTPAdE != 0)
                                                                 && (x.BTransactionArchivedFlag != null
                                                                 && x.BTransactionArchivedFlag.All(c => c == true)))
                                                      || group
                                                      .All(x => x.MismatchTPAdE == 0 && x.MismatchRtAdE == 0)),

                            SzTranscationCheckNote = group.Where(x => x.SzTranscationCheckNote != null)
                                                        .Select(x => x.SzTranscationCheckNote.FirstOrDefault()).ToList()
                                                        ?? new List<string> { "" },
                            TransmissionsGroupedByDay = group.ToList(),
                            UserActivityLogViewModel = listLogVM.Where(x => x.LRtServerOperationId
                                                                         == group.Max(x => x.LRtServerOperationId))
                                                                .ToList() ?? new List<UserActivityLogViewModel>()
                        });
                }
            }
            lisRes = listview.OrderByDescending(x => x.OperationClosureDatetime.Value.Date).ToList();

            _logger.LogDebug($"END: Return list TransmissionsGrouped length: {listview.Count()}");

            return lisRes;
        }


        //==========================================================
        public async Task<List<TransmissionsList>> ListTransmissionsByDay(string userName, string id, int storeId, int storeGroupId, string date)
        {
            var TransmissionsByDayList = await _transmissionRepository.GetTransmissionsByDay(userName, id, storeId, storeGroupId, date);
            return TransmissionsByDayList;
        }
        //==========================================================
        public async Task<List<TransmissionsByDayToIndexView>> ListTrasmissionaAndTransactionsByDay22(string id, int storeId, int storeGroupId, string date)
        {
            _logger.LogDebug("Start: Group List Trasmissions by day");
            var listview = Enumerable.Empty<TransmissionsByDayToIndexView>();
            var lisRes = new List<TransmissionsByDayToIndexView>();
            //Da commentare e cambiare il flusso a chiedere il risultato della Stored procedure
            // as TransmissionsByDayToIndexView
            var listTrx = await _transmissionRepository.ListTransmissionByServerIdByDay22(id, date, _properties.Value.TrxNmbrDays);//_properties.Value.TrxNmbrDays
            var listTransmissions = _mapper.Map<List<TransmissionsGroupedByDay>,
                                    List<TransmissionsAndTransactionsGroupedByDayVM>>
                                    (listTrx);
            var initDate = DateTime.Today;
            if (listTransmissions.Count() > 0 && listTransmissions != null)
            {
                initDate = listTransmissions.OrderByDescending(x => x.OperationClosureDatetime)
                                            .FirstOrDefault().OperationClosureDatetime.Value;


                var listransaction = await _transactionRepository.ListTransactionServerRtMonth22(id, storeId, storeGroupId, initDate, _properties.Value.TrxNmbrDays);//_properties.Value.TrxNmbrDays


                var rtserver = _rtServerRepository.GetRtServerStatus(id);
                listTransmissions.ForEach(t =>
                {
                    var found = listransaction.Where(x => x.RtDeviceId == t.SzRtDeviceId
                                    && x.DPosDateTime.Value.Date == t.OperationClosureDatetime.Value.Date
                                     ).AsEnumerable();

                    if (found != null && found.Count() > 0)
                    {

                        t.TotalRtServer = found.Sum(x => x.TotalServerRT);
                        t.TotalTP = found.Sum(x => x.TotalTP);
                        var vats = found.Where(x => x.TransactionVats != null);
                        if (vats.Count() > 0 && vats != null)
                        {
                            //t.TransactionVats = found.AsQueryable().Where(x => x.TransactionVats != null).SelectMany(x => x.TransactionVats)
                            t.TransactionVats = vats.SelectMany(x => x.TransactionVats)
                             .GroupBy(x => new { x.DPosVatRate, x.SzVatNature }, (key, group) =>
                              new TransactionVatForTrasmissionsViewModel
                              {
                                  DPosVatRate = key.DPosVatRate,
                                  SzVatNature = key.SzVatNature,
                                  DPosGrossAmount = group.Sum(x => x.DPosGrossAmount),
                                  DRtGrossAmount = group.Sum(x => x.DRtGrossAmount)
                              }
                        )
                       .AsEnumerable()

                                                 .OrderBy(x => x.DPosVatRate)
                                                 .ToList();
                            t.LPosWorkstation = found.Select(x => x.LPosWorkstationNmbr ?? "").FirstOrDefault();
                            var listTransactionsVat = new List<TransactionVatForTrasmissionsViewModel>();
                            foreach (var vat in t.TransactionVats)
                            {
                                var tmp = t.RtServerTransmissionsDetailRtData
                                           .Where(x => x.DVatRate.Equals(vat.DPosVatRate));// && x.SzVatNature==vat.SzVatNature);
                                if (tmp != null)
                                {
                                    foreach (var vtmp in tmp)
                                    {
                                        vat.DVatRate = vtmp.DVatRate;
                                        vat.DSaleAmount = vtmp.DSaleAmount;
                                        vat.DVatAmount = vtmp.DVatAmount;
                                        vat.DVoidAmount = vtmp.DVoidAmount;
                                        vat.DReturnAmount = vtmp.DReturnAmount;
                                        vat.SzVatNature = vtmp.SzVatNature;
                                        listTransactionsVat.Add(vat);
                                    }
                                }
                            }
                            t.TotalReturn = t.TransactionVats.Sum(x => x.DReturnAmount);
                            t.TotalVoid = t.TransactionVats.Sum(x => x.DVoidAmount);
                            t.TotalSale = t.TransactionVats.Sum(x => x.DSaleAmount);
                            t.TotalVat = t.TransactionVats.Sum(x => x.DVatAmount);

                            t.HasVentilation = rtserver.BVatVentilationFlag ?? false;
                            t.TransactionVats = listTransactionsVat.ToList();

                        }
                    }
                });
                var listTrnHasNotTrm = listransaction.Where(x => !listTransmissions.Exists(t => t.OperationClosureDatetime.Value.Date == x.DPosDateTime.Value.Date
                                                                                  && t.SzRtDeviceId == x.RtDeviceId
                                                                                 ));
                foreach (var tt in listTrnHasNotTrm)
                {
                    var obj = new TransmissionsAndTransactionsGroupedByDayVM
                    {
                        SzRtServerId = tt.RtServerId,
                        SzRtDeviceId = tt.RtDeviceId,
                        HasVentilation = rtserver.BVatVentilationFlag ?? false,
                        OperationClosureDatetime = tt.DPosDateTime ?? tt.DRtDateTime,
                        TotalTP = tt.TotalTP,
                        TotalRtServer = tt.TotalServerRT,
                        LPosWorkstation = tt.LPosWorkstationNmbr,
                        TotalAmount = 0.0m,


                    };
                    listTransmissions.Add(obj);
                }
                var store = await _storeRepository.GetStore(storeId);
                var storeGroup = await _storeGroupRepository.GetStoreGroup(storeGroupId);
                var listnote = new List<string>();
                listnote.Add("");
                if (listTransmissions != null)
                {
                    var listLog = (await _transmissionRepository.GetUserActivityByTransaction(id));
                    var listLogVM = _mapper.Map<List<UserActivityLog>, List<UserActivityLogViewModel>>(listLog).ToList();

                    listview = listTransmissions
                        .GroupBy(x => x.OperationClosureDatetime.Value.Date,
                        (key, group) =>
                        new TransmissionsByDayToIndexView
                        {
                            SzRtServerId = id,
                            LRetailStoreId = store.SzDescription,
                            Store = _mapper.Map<Store, StoreViewModel>(store),
                            StoreGroupId = storeGroup.SzDescription,
                            OperationClosureDatetime = key.Date,
                            GTotalAmount = group.Sum(x => x.TotalAmount),
                            GTotalRtServer = group.Sum(x => x.TotalRtServer),
                            GTotalTP = group.Sum(x => x.TotalTP),
                            BTransactionCheckedFlag = (group.Where(x => x.LRtServerOperationId != 0)
                                                           .Any(x => (x.MismatchRtAdE != 0 && x.MismatchTPAdE != 0)
                                                             && (x.BTransactionCheckedFlag != null
                                                                && x.BTransactionCheckedFlag.All(c => c == true)))
                                                      || group
                                                      .All(x => x.MismatchTPAdE == 0 && x.MismatchRtAdE == 0)),

                            BTransactionArchivedFlag = (group.Where(x => x.LRtServerOperationId != 0)
                                                            .Any(x => (x.MismatchRtAdE != 0 && x.MismatchTPAdE != 0)
                                                                 && (x.BTransactionArchivedFlag != null
                                                                 && x.BTransactionArchivedFlag.All(c => c == true)))
                                                      || group
                                                      .All(x => x.MismatchTPAdE == 0 && x.MismatchRtAdE == 0)),

                            SzTranscationCheckNote = group.Where(x => x.SzTranscationCheckNote != null)
                                                        .Select(x => x.SzTranscationCheckNote.FirstOrDefault()).ToList()
                                                        ?? new List<string> { "" },
                            TransmissionsGroupedByDay = group.ToList(),
                            UserActivityLogViewModel = listLogVM.Where(x => x.LRtServerOperationId
                                                                         == group.Max(x => x.LRtServerOperationId))
                                                                .ToList() ?? new List<UserActivityLogViewModel>()
                        });
                }
            }
            lisRes = listview.Where(x => x.OperationClosureDatetime.Value.Date <= Convert.ToDateTime(date)
                && x.OperationClosureDatetime.Value.Date >= Convert.ToDateTime(date).AddDays(-3))
                .OrderByDescending(x => x.OperationClosureDatetime.Value.Date).ToList();
            //lisRes = listview.Where(x => x.OperationClosureDatetime.Value.ToString("dd-MM-yyyy") == date).ToList();

            //var lisRes = await _transmissionRepository..trxTotalsResult(filters.UserName, dateInit.Value.Date.ToString("yyyy-MM-dd"),
            //    dateTo.Value.Date.ToString("yyyy-MM-dd"), filters.ServerRt, filters.Store,
            //    filters.StoreGroup, filters.Status, filters.Error, filters.NonCompliant)
            //    .ToListAsync();
            _logger.LogDebug($"END: Return list TransmissionsGrouped length: {listview.Count()}");

            return lisRes;
        }

        public List<TransmissionGroupedByTRXAndDevice> GroupedList(List<RtServerTransmissionDetail> list)
        {
            List<TransmissionGroupedByTRXAndDevice> listTransmissions = list
                .GroupBy(x => new { x.LRtDeviceTransmissionId, x.SzRtDeviceId }, (key, group) => new
                {
                    key1 = key.LRtDeviceTransmissionId,
                    key2 = key.SzRtDeviceId,
                    result = _mapper.Map<List<RtServerTransmissionDetail>, List<RtServerTransmissionDetailViewModel>>(group.ToList())
                }).Select(x => new TransmissionGroupedByTRXAndDevice
                {
                    DeviceId = x.key2,
                    TransmissionId = x.key1,
                    RtServerTransmissionsDetail = x.result
                }).ToList();
            listTransmissions.ForEach(x =>
            {
                x.RtServerTransmissionsDetail.ForEach(x =>
                {
                    x.RtServerTransmissionDetailRtReport = x.RtServerTransmissionDetailRtReport.OrderByDescending(x => x.DEventDateTime).ToList();
                });
            });
            return listTransmissions;
        }
        public async Task<IActionResult> IsChecked(string transmission)
        {
            _logger.LogDebug("Start: Change SzTranscationCheckNote value");

            var parameters = JsonConvert.DeserializeObject<TransmissionCheckdeOrArchived>(transmission);
            var dateTrx = DateTime.ParseExact(
                      parameters.DRtDeviceClosureDateTime,
                      "dd-MM-yyyy",
                      System.Globalization.CultureInfo.InvariantCulture);
            var getTransmission = await _transmissionRepository.GetTransmission(parameters.RtServerId, dateTrx);
            if (getTransmission == null)
            {
                return NotFound();
            }

            if (getTransmission.Any(x => x.BTransactionCheckedFlag != parameters.IsChecked))
            {
                getTransmission.ForEach(x =>
                {
                    if (x.BTransactionCheckedFlag != parameters.IsChecked)
                    {
                        x.BTransactionCheckedFlag = parameters.IsChecked;
                    }
                    x.SzUserName = User.Identity.Name;
                }
                );
            }

            try
            {
                _unitOfWork.UpdateRangeAsync(getTransmission);
                await _unitOfWork.CompleteAsync();
                _logger.LogDebug($"END: Change BTransactionCheckedFlag status to all Transmission for day: {getTransmission.Select(x => x.DRtDeviceClosureDateTime)}");

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                _logger.LogCritical("END: There is an error in changing BTransactionCheckedFlag status ");

                return Json(new { success = false });
            }
        }

        public async Task<IActionResult> IsArchived(string transmission)
        {
            _logger.LogDebug("Start: Change SzTranscationCheckNote value");

            var parameters = JsonConvert.DeserializeObject<TransmissionCheckdeOrArchived>(transmission);
            var dateTrx = DateTime.ParseExact(
                      parameters.DRtDeviceClosureDateTime,
                      "dd-MM-yyyy",
                      System.Globalization.CultureInfo.InvariantCulture);
            var getTransmission = await _transmissionRepository.GetTransmission(parameters.RtServerId, dateTrx);
            if (getTransmission == null)
            {
                return NotFound();
            }

            if (getTransmission.Any(x => x.BTransactionArchivedFlag != parameters.IsArchived))
            {
                getTransmission.ForEach(x =>
                {
                    if (x.BTransactionArchivedFlag != parameters.IsArchived)
                    {
                        x.BTransactionArchivedFlag = parameters.IsArchived;
                    }
                    x.SzUserName = User.Identity.Name;
                }
                );
            }

            try
            {
                _unitOfWork.UpdateRangeAsync(getTransmission);
                await _unitOfWork.CompleteAsync();
                _logger.LogDebug($"END: Change BTransactionArchivedFlag status to all Transmission for day: {getTransmission.Select(x => x.DRtDeviceClosureDateTime)}");

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                _logger.LogCritical("END: There is an error in changing BTransactionArchivedFlag status ");

                return Json(new { success = false });
            }
        }

        public async Task<IActionResult> NoteEdit(string transmission)
        {
            _logger.LogDebug("Start: Change SzTranscationCheckNote value");

            var parameters = JsonConvert.DeserializeObject<TransmissionCheckdeOrArchived>(transmission);
            var dateTrx = DateTime.ParseExact(
                      parameters.DRtDeviceClosureDateTime,
                      "dd-MM-yyyy",
                      System.Globalization.CultureInfo.InvariantCulture);
            var getTransmission = await _transmissionRepository.GetTransmission(parameters.RtServerId, dateTrx);
            if (getTransmission == null)
            {
                return NotFound();
            }

            if (getTransmission.Any(x => x.SzTranscationCheckNote != parameters.CheckNote))
            {
                getTransmission.ForEach(x =>
                {
                    if (x.SzTranscationCheckNote != parameters.CheckNote)
                    {
                        x.SzTranscationCheckNote = parameters.CheckNote;
                    }
                    x.SzUserName = User.Identity.Name;
                }
                );
            }

            try
            {
                _unitOfWork.UpdateRangeAsync(getTransmission);
                await _unitOfWork.CompleteAsync();
                _logger.LogDebug($"END: Change BTransactionArchivedFlag status to all Transmission for day: {getTransmission.Select(x => x.DRtDeviceClosureDateTime)}");

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                _logger.LogCritical("END: There is an error in changing BTransactionArchivedFlag status ");

                return Json(new { success = false });
            }
        }

    }
}