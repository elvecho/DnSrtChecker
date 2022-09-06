using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DnSrtChecker.Models;
using DnSrtChecker.Persistence;
using AutoMapper;
using DnSrtChecker.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DnSrtChecker.FiltersmodelBindRequest;
using DnSrtChecker.Services;
using Microsoft.AspNetCore.Identity;
using DnSrtChecker.ModelsHelper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.RegularExpressions;
using Serilog;
using DnSrtChecker.Models.ViewModels.ModelsFunctions;
using Microsoft.Extensions.Caching.Memory;

namespace DnSrtChecker.Controllers
{
    [Authorize]
    public class HomeController : BasicController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoreRepository _storeRepository;
        private readonly IRtServerRepository _rtServerRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITransmissionRepository _transmissionRepository;
        private readonly IMapper _mapper;
        private readonly IOptions<Properties> _properties;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private IMemoryCache _memoryCache;
        public HomeController(
            IUnitOfWork unitOfWork,
            IStoreRepository storeRepository,
            IRtServerRepository rtServerRepository,
            ITransactionRepository transactionRepository,
            ITransmissionRepository transmissionRepository,
            IMapper mapper,
            IOptions<Properties> properties,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IMemoryCache memoryCache,
            ILogger<HomeController> logger) : base(userManager, roleManager)
        {
            _unitOfWork = unitOfWork;
            _storeRepository = storeRepository;
            _rtServerRepository = rtServerRepository;
            _transactionRepository = transactionRepository;
            _transmissionRepository = transmissionRepository;
            _properties = properties;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<ActionResult> Index(FiltersmodelBindingRequest filters)
        {
            _logger.LogDebug("Start: Index HomeController");
            List<RtServerViewModel> list = new List<RtServerViewModel>();
            
            var user = await GetCurrentuser();
            var roles = await _userManager.GetRolesAsync(user);
            var listRtServer = new List<TrxRTServer>();
            FiltersmodelBindingRequest.UserName = user.UserName;
           

            var firstAccess = false;
            if (AnyFilterNotEmpty(filters))
            {
                if (string.IsNullOrEmpty(filters.TransmissionDateFrom))
                {
                    firstAccess = true;
                    filters.TransmissionDateFrom = (DateTime.Now).AddDays(-3).ToString("dd-MM-yyyy");
                }
                if (string.IsNullOrEmpty(filters.TransmissionDateTo))
                {
                    firstAccess = true;
                    filters.TransmissionDateTo = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");
                }
            }
            decimal DTotalRTSum = 0;
            decimal DTotalADESum = 0;
            decimal DTotalTPSum = 0;
            bool timeOut = false;

            if (!string.IsNullOrEmpty(filters.TransmissionDateFrom) && !string.IsNullOrEmpty(filters.TransmissionDateTo))
            {
                List<TrxRTServer> listFiltered = new List<TrxRTServer>();
                try
                {
                    listFiltered = await FilterList(filters);

                }
                catch (Exception Ex)
                {
                    Ex.ToString();
                    timeOut = true;
                }
                if (firstAccess)
                {
                    filters.TransmissionDateFrom = "";
                    filters.TransmissionDateTo = "";
                }
                ViewData["TimeOut"] = timeOut;
                PopulateInputs(filters);
                _logger.LogDebug($"END: Return list of all RT Servers length:{listFiltered.Count()}");

                list = _mapper.Map<List<TrxRTServer>, List<RtServerViewModel>>(listFiltered);             
                
                
            }
            else
            {
                //var listsrvHasOneTRNNOnMismatch = _transactionRepository.ListSrvHasOneMsmatch();
                try
                {
                    listRtServer = await FilterList(filters);
                    //var prova = listRtServer.Where(x => x.LRetailStoreId == 187);

                }
                catch (Exception Ex)
                {
                    Ex.ToString();
                    timeOut = true;
                }
                ViewData["TimeOut"] = timeOut;
                PopulateInputs(filters);
                ViewBag.transmissionDateFrom = "";
                ViewBag.transmissionDateTo = "";
                _logger.LogDebug($"END: Return list of all RT Servers length:{listRtServer.Count()}");
                //RtServersHomeViewModel
                //return View("Index", _mapper.Map<List<TrxRTServer>, List<RtServerViewModel>>(listRtServer));
                list = _mapper.Map<List<TrxRTServer>, List<RtServerViewModel>>(listRtServer);              
               
            }
            if (filters.Store != "0" && filters.Store != null && list != null)
            {
                DTotalRTSum = list.Sum(x => x.DTotalRT);
                DTotalADESum = list.Sum(x => x.DTotalADE);
                DTotalTPSum = list.Sum(x => x.DTotalTP);
            }
            ViewData["DTotalRTSum"] = DTotalRTSum;
            ViewData["DTotalADESum"] = DTotalADESum;
            ViewData["DTotalTPSum"] = DTotalTPSum;
            return View("Index", list/*.OrderByDescending(x => x.DRtDeviceClosureDateTime)*/);
        }

        public IActionResult GetListRtServersNoFilters()
        {
            _logger.LogDebug("Start: Reset all filters table");

            return RedirectToAction("Index", ResetFilters());
        }
        /**
        *Input:filters strings
        *Output:listFiltered
        **/


        public async Task<List<TrxRTServer>> FilterList(FiltersmodelBindingRequest filters)
        {

            _logger.LogDebug("Start: Filter ListRTServer and order list result by compliant and mismatch");
            ////List Server RT which has status
            //var listRtServerActive = await _rtServerRepository.ListRtServerStatusHome();
            //// List Server RT has any transaction non compliant: BWarningFlag=true
            //var listTrnNonCompliant = _transactionRepository.ListRtServerNonCompliant();
            List<TrxRTServer> listFiltered;
            
            listFiltered = await _rtServerRepository.ListRtServerStatusNew(filters);


           
            _logger.LogDebug($"END: list result length : {listFiltered.Count()}");

            return listFiltered;
        }

        public async Task<IEnumerable<ListServersStatusHomeBYDayViewModel>> FilterListByDate(FiltersmodelBindingRequest filters)
        {
            _logger.LogDebug("Start: Filter ListRTServer and order list result by compliant and mismatch");

            var listFiltered = await _rtServerRepository.ListRtServerStatusHomeFiltered(filters);
            var listFilteredVM = _mapper.Map<List<ListServersStatusHomeBYDay>, List<ListServersStatusHomeBYDayViewModel>>(listFiltered);
            return listFilteredVM;
        }
        public async Task<IEnumerable<ListServersStatusHomeBYDayViewModel>> FilterListByDate2(FiltersmodelBindingRequest filters)
        {
            _logger.LogDebug("Start: Filter ListRTServer and order list result by compliant and mismatch");
            var listFiltered = await ListRtServersStatusHome22(filters);
            var listFilteredVM = _mapper.Map<List<ListServersStatusHomeBYDay>, List<ListServersStatusHomeBYDayViewModel>>(listFiltered);
            return listFilteredVM;
        }
        public async Task<List<ListServersStatusHomeBYDay>> ListRtServersStatusHome(FiltersmodelBindingRequest filters)
        {
            var _erroTable = _properties.Value.TransactionErrorTable.Select(x => x.Value);
            var user = await GetCurrentuser();
            var roles = await _userManager.GetRolesAsync(user);

            DateTime trxDateFrom = DateTime.ParseExact(
                 filters.TransmissionDateFrom,
                 "dd-MM-yyyy",
                 System.Globalization.CultureInfo.InvariantCulture);

            DateTime trxDateTo = DateTime.ParseExact(
                       filters.TransmissionDateTo,
                       "dd-MM-yyyy",
                       System.Globalization.CultureInfo.InvariantCulture);
            var servers = _rtServerRepository.ListRTServersFiltered(filters);
            var listSrv = servers.Select(x => x.SzRtServerId);
            var trx = (await _transmissionRepository.ListTransmissionAllServersNonCompliantByDate(listSrv, trxDateFrom, trxDateTo)).ToList();
            IEnumerable<ListServersStatusHomeBYDay> trn;
            if (!roles.Any(x => x == "OperatorIT" || x == "OperatorAdmin"))
            {
                trn = await _rtServerRepository.ListTransactionsByDayForHome2(listSrv, trxDateFrom, trxDateTo, _erroTable);
            }
            else
            {
                trn = await _rtServerRepository.ListTransactionsByDayForHome3(listSrv, trxDateFrom, trxDateTo, _erroTable);
            }

            trx.ForEach(x =>
            {
                var listserverHome = new List<RtServersHome>();

                x.ListRtServersHome.ToList().ForEach(srv =>
                {
                    var totalADE = srv.TotalADE ?? 0.00m;
                    var foundDate = trn.Any(trn => trn.OperationClosureDatetime.Value.Date == x.OperationClosureDatetime.Value.Date);

                    if (foundDate)
                    {
                        var foundsrv = trn.Where(t => t.OperationClosureDatetime.Value.Date == x.OperationClosureDatetime.Value.Date)
                         .SelectMany(x => x.ListRtServersHome.Where(tmp => tmp.SzRtServerId == srv.SzRtServerId));
                        if (foundsrv != null && foundsrv.Count() > 0)
                        {
                            var totalTP = foundsrv.Sum(x => x.TotalTP ?? 0.00m);
                            var totalRT = foundsrv.Sum(x => x.TotalRT ?? 0.00m);
                            if (totalADE != totalTP)
                            {
                                foreach (var s in foundsrv)
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
                                    rtServer.TrasnmissionError = totalADE.Equals(totalTP);
                                    rtServer.DateLastClosure = x.OperationClosureDatetime;
                                    rtServer.TransmissionChecked = srv.TransmissionChecked;
                                    listserverHome.Add(rtServer);
                                }
                            }


                        }
                    }

                });
                x.ListRtServersHome = listserverHome.ToList()
                                          .GroupBy(x => new { x.SzRtServerId, x.LRetailStoreId, x.LStoreGroupId }, (key, group)
                                          => new RtServersHome
                                          {
                                              SzRtServerId = key.SzRtServerId,
                                              L = group.FirstOrDefault().L,
                                              LRetailStoreId = key.LRetailStoreId,
                                              LStoreGroupId = key.LStoreGroupId,
                                              BOnDutyFlag = group.FirstOrDefault().BOnDutyFlag,
                                              BOnError = group.FirstOrDefault().BOnError,
                                              NonCompliant = group.Any(x => x.NonCompliant == true),// !group.All(x => x.NonCompliant != true),
                                              NonCompliantOrHasMismatch = (!group.All(x => x.NonCompliant != true)).ToString(),
                                              RtServerStatus = group.FirstOrDefault().RtServerStatus,
                                              TotalTP = group.Sum(x => x.TotalTP ?? 0.00m),
                                              TotalRT = group.Sum(x => x.TotalTP ?? 0.00m),
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
                trx.ForEach(x =>
                {

                    x.ListRtServersHome = x.ListRtServersHome.Where(x => x.NonCompliant == Noncompliant).ToList();
                });
            }
            return trx.ToList();

        }
        public async Task<List<ListServersStatusHomeBYDay>> ListRtServersStatusHome22(FiltersmodelBindingRequest filters)
        {
            var _erroTable = _properties.Value.TransactionErrorTable.Select(x => x.Value);
            var user = await GetCurrentuser();
            var roles = await _userManager.GetRolesAsync(user);

            DateTime trxDateFrom = DateTime.ParseExact(
                 filters.TransmissionDateFrom,
                 "dd-MM-yyyy",
                 System.Globalization.CultureInfo.InvariantCulture);

            DateTime trxDateTo = DateTime.ParseExact(
                       filters.TransmissionDateTo,
                       "dd-MM-yyyy",
                       System.Globalization.CultureInfo.InvariantCulture);
            var servers = _rtServerRepository.ListRTServersFiltered(filters);
            var listSrv = servers.Select(x => x.SzRtServerId).ToList();
            IEnumerable<RtServersHome> trn;

            trn = await _rtServerRepository.ListTransactionsByDayForHome22(listSrv, trxDateFrom, trxDateTo, _erroTable);

            var trnList = trn.ToList();
            var listserverHome = new List<RtServersHome>();
            var trx = servers.AsEnumerable();

            foreach (var t in trx)
            {
                var totalADE = t.TotalADE ?? 0.00m;
                var foundTRNbyDate = trnList.Where(x => x.DateLastClosure.Value.Date == t.DateLastClosure.Value.Date &&
                                                x.SzRtServerId == t.SzRtServerId);
                if (foundTRNbyDate != null && foundTRNbyDate.Count() > 0)
                {

                    var totalTP = foundTRNbyDate.Sum(x => x.TotalTP ?? 0.00m);
                    var totalRT = foundTRNbyDate.Sum(x => x.TotalRT ?? 0.00m);
                    if (!(totalADE == totalTP && totalADE == totalRT))
                    {
                        foreach (var s in foundTRNbyDate)
                        {
                            RtServersHome rtServer = new RtServersHome();
                            //rtServer = srv;
                            rtServer.SzRtServerId = t.SzRtServerId;
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
                            rtServer.TrasnmissionError = totalADE.Equals(totalTP) && totalADE.Equals(totalRT);
                            rtServer.DateLastClosure = t.DateLastClosure;
                            rtServer.TransmissionChecked = false;
                            listserverHome.Add(rtServer);
                        }
                    }
                }
            }
            var res = listserverHome.GroupBy(x => x.DateLastClosure.Value.Date)
                                      .Select(x => new ListServersStatusHomeBYDay
                                      {
                                          OperationClosureDatetime = x.Key,
                                          ListRtServersHome = x.GroupBy(t => new { t.SzRtServerId, t.LRetailStoreId, t.LStoreGroupId })
                                       .Select(r => new RtServersHome
                                       {
                                           SzRtServerId = r.Key.SzRtServerId,
                                           L = r.FirstOrDefault().L,
                                           LRetailStoreId = r.Key.LRetailStoreId,
                                           LStoreGroupId = r.Key.LStoreGroupId,
                                           BOnDutyFlag = r.FirstOrDefault().BOnDutyFlag,
                                           BOnError = r.FirstOrDefault().BOnError,
                                           NonCompliant = r.Any(x => x.NonCompliant == true),// !group.All(x => x.NonCompliant != true),
                                           NonCompliantOrHasMismatch = (!r.All(x => x.NonCompliant != true)).ToString(),
                                           RtServerStatus = r.FirstOrDefault().RtServerStatus,
                                           TotalTP = r.Sum(x => x.TotalTP ?? 0.00m),
                                           TotalRT = r.Sum(x => x.TotalRT ?? 0.00m),
                                           TotalADE = r.Sum(x => x.TotalADE ?? 0.00m),
                                           TransmissionChecked = !r.Any(x => x.TransmissionChecked != true),
                                           TrasnmissionError = r.Sum(x => x.TotalADE).Equals(r.Sum(x => x.TotalTP)) && r.Sum(x => x.TotalADE).Equals(r.Sum(x => x.TotalRT)),
                                           DateLastClosure = r.FirstOrDefault().DateLastClosure

                                       }).OrderByDescending(x => x.TrasnmissionError).ThenByDescending(x => x.NonCompliant)
                                      }).OrderByDescending(x => x.OperationClosureDatetime).AsEnumerable();

            if (!string.IsNullOrEmpty(filters.NonCompliant))
            {
                var Noncompliant = bool.Parse(filters.NonCompliant);
                foreach (var x in res)
                {
                    x.ListRtServersHome = x.ListRtServersHome.Where(x => x.NonCompliant == Noncompliant);

                }
            }
            return res.ToList();

        }

        public async Task<PartialViewResult> GetListOfDay(FiltersmodelBindingRequest filters)
        {
            _logger.LogDebug("Start: Get ListServer with mismatch for selected Day");
            filters.TransmissionDateFrom = filters.DayFilter;
            filters.TransmissionDateTo = filters.DayFilter;
            var listFiltered = ListRtServersStatusHome22(filters);
            var listRes = _mapper.Map<IEnumerable<RtServersHome>, IEnumerable<RtServersHomeViewModel>>((await listFiltered).SelectMany(x => x.ListRtServersHome));
            return PartialView("ListRtServersHomebyDay", listRes);
        }

        public async Task<List<ListServersStatusHomeBYDayViewModel>> InitFilterListByDay(FiltersmodelBindingRequest filters)
        {
            var listFiltered = _rtServerRepository.ListServersHomeFilteredByDay(filters);
            var listRes = _mapper.Map<List<ListServersStatusHomeBYDay>, List<ListServersStatusHomeBYDayViewModel>>(listFiltered);
            return listRes;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public void PopulateInputs(FiltersmodelBindingRequest filters)
        {
            _logger.LogDebug("Start: Populate all filter Inputs.");
            ViewBag.serverRt = filters.ServerRt;
            ViewBag.store = filters.Store;
            ViewBag.storeGroup = filters.StoreGroup;
            ViewBag.transmissionDateFrom = filters.TransmissionDateFrom ?? "";
            ViewBag.transmissionDateTo = filters.TransmissionDateTo ?? "";

            PopulateStatusDropDownList(filters.Status == null ? "" : filters.Status);
            PopulateErrorDropDownList(filters.Error == null ? "" : filters.Error);
            PopulateNonCompliantDropDownList(filters.NonCompliant == null ? "" : filters.NonCompliant);
            _logger.LogDebug("END: End populate Inputs.");

        }
        public bool AnyFilterNotEmpty(FiltersmodelBindingRequest filters)
        {
            var res = false;
            if (!string.IsNullOrEmpty(filters.Status))
                res = true;
            if (!string.IsNullOrEmpty(filters.Error))
                res = true;

            if (!string.IsNullOrEmpty(filters.NonCompliant))
                res = true;
            if (!string.IsNullOrEmpty(filters.Store))
                res = true;
            if (!string.IsNullOrEmpty(filters.StoreGroup))
                res = true;
            if (!string.IsNullOrEmpty(filters.ServerRt))
                res = true;
            return res;
        }
    }
}
