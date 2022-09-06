using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;
using AutoMapper;
using DnSrtChecker.Data.RtTransactions;
using DnSrtChecker.FiltersmodelBindRequest;
using DnSrtChecker.Helpers;
using DnSrtChecker.Models;
using DnSrtChecker.Models.ViewModels;
using DnSrtChecker.Models.ViewModels.ModelsFunctions;
using DnSrtChecker.ModelsHelper;
using DnSrtChecker.Persistence;
using DnSrtChecker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RazorEngineCore;

namespace DnSrtChecker.Controllers
{
    [Authorize]
    public class TransactionsController : BasicController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoreRepository _storeRepository;
        private readonly IRtServerRepository _rtServerRepository;
        private readonly IMapper _mapper;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITransmissionRepository _transmissionRepository;
        private readonly ILogger<TransactionsController> _logger;
        private readonly EmailHelper _emailHelper;
        private readonly IOptions<Properties> _properties;
        private readonly List<string> _errorTable;
        private readonly string _flagViewTransactions;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly XmlSerializerHelper _xmlHelper;
        private readonly ITransactionRtErrorRepository _transactionRtErrorRepository;
        public TransactionsController(IUnitOfWork unitOfWork,
                                    IStoreRepository storeRepository,
                                    IRtServerRepository rtServerRepository,
                                    IMapper mapper,
                                    ITransactionRepository transactionRepository,
                                    ITransmissionRepository transmissionRepository,
                                    ILogger<TransactionsController> logger,
                                    EmailHelper emailHelper,
                                    IOptions<Properties> properties,
                                    UserManager<User> userManager,
                                    RoleManager<Role> roleManager,
                                    ITransactionRtErrorRepository transactionRtErrorRepository
                                    ) : base(userManager, roleManager)
        {
            _unitOfWork = unitOfWork;
            _storeRepository = storeRepository;
            _rtServerRepository = rtServerRepository;
            _mapper = mapper;
            _transactionRepository = transactionRepository;
            _transmissionRepository = transmissionRepository;
            _logger = logger;
            _emailHelper = emailHelper;
            _properties = properties;
            _userManager = userManager;
            _roleManager = roleManager;
            _transactionRtErrorRepository = transactionRtErrorRepository;
            _errorTable = properties.Value.TransactionErrorTable.Select(x => x.Value).ToList();
            _flagViewTransactions = properties.Value.FlagViewTransactions;
        }



        //public async Task<IActionResult> Index(string id, int retailStoreId, int storeGroupId)
        //{
        //    _logger.LogDebug("Start: First Access to Index Transactions");

        //    RtServer rtServer = await _rtServerRepository.GetRtServer(id, retailStoreId, storeGroupId);
        //    if (rtServer == null)
        //    {
        //        _logger.LogCritical($"Error: RT server not found:{rtServer.SzRtServerId}, with storeId: {rtServer.LRetailStoreId}, and storeGroupId:{rtServer.LStoreGroupId}");
        //        return NotFound();
        //    }
        //    var trnError = await _transactionRtErrorRepository.GetTransactionsRtErrorByServer(id);
        //    var trns = await _transactionRepository.ListTransactionsByServerRt(rtServer);
        //    rtServer.TransactionAffiliation = trns;
        //    rtServer.TransactionRtError = trnError;
        //    var listTransaction = new List<RtServersTransactionsViewModel>();
        //    var rtServerVM = _mapper.Map<RtServer, RtServerViewModel>(rtServer);
        //    var filters = new FiltersmodelBindingRequest();
        //    if (_flagViewTransactions.Equals("OnError"))
        //    {


        //        //filters.Conformity = "NonCompliant";
        //        //filters.IsChecked = "isNotChecked";

        //        listTransaction = await InitialListNonCompliant2(rtServer);
        //        if (listTransaction.Count() == 0 && listTransaction == null)
        //        {
        //            listTransaction = await InitialList(rtServer);
        //        }
        //        filters.ServerRt = id;
        //        filters.Store = retailStoreId.ToString();
        //        filters.StoreGroup = storeGroupId.ToString();
        //        filters.Conformity = null;
        //        filters.IsChecked = null;

        //    }
        //    else
        //    {
        //        listTransaction = await InitialList(rtServer);


        //    }
        //    filters.ServerRt = rtServerVM.SzRtServerId;
        //    filters.Store = rtServerVM.L.StoreNameComplet;
        //    filters.StoreGroup = rtServerVM.L.LStoreGroup.SzDescription;
        //    PopulateInputs(filters);

        //    _logger.LogDebug($"END: Return list of all RT Servers length:{listTransaction.Count}");
        //    return View(listTransaction);

        //}
        public async Task<IActionResult> Index(string RtServerId, int retailStoreId, int storeGroupId, DateTime dateFrom,
            string GroupDescription, string RetailDescription)
        {
            _logger.LogDebug("Start: First Access to Index Transactions");

            //RtServer rtServer = await _rtServerRepository.GetRtServer(id, retailStoreId, storeGroupId);
            //if (rtServer == null)
            //{
            //    _logger.LogCritical($"Error: RT server not found:{id}, with storeId: {retailStoreId}, and storeGroupId:{storeGroupId}");
            //    return NotFound();
            //}

            //var listTransaction = new List<RtServersTransactionsViewModel>();
            //var rtServerVM = _mapper.Map<RtServer, RtServerViewModel>(rtServer);
            var filters = new FiltersmodelBindingRequest();
            if (_flagViewTransactions.Equals("OnError"))
            {
                //filters.Conformity = "NonCompliant";
                //filters.IsChecked = "isNotChecked";

                //listTransaction = await InitialListNonCompliant2(rtServer, dateFrom);
                //if ((listTransaction.Count() == 0 && listTransaction == null )
                //    ||(listTransaction.Any(x=>x.RtServers.Any(c=>c.TransactionAffiliation!=null && c.TransactionAffiliation.Count()==0))))
                //{
                //    listTransaction = await InitialList2(rtServer, dateFrom);
                //}
                //filters.ServerRt = id;
                //filters.Store = retailStoreId.ToString();
                //filters.StoreGroup = storeGroupId.ToString();
                //filters.Conformity = null;
                //filters.IsChecked = null;

            }
            else
            {
                //listTransaction = await InitialList2(rtServer, dateFrom);
            }
            
            filters.ServerRt = RtServerId;
            filters.Store = RetailDescription;
            filters.StoreGroup = GroupDescription;
            filters.TransactionDateFrom = dateFrom.ToString("yyyy/MM/dd");
            filters.TransactionDateTo = dateFrom.ToString("yyyy/MM/dd");
            filters.StoreID = retailStoreId;
            filters.StoreGroupID = storeGroupId;
            PopulateInputs(filters);
            //sistemare il timeout qua
            List<TransactionList> listTransactions = new List<TransactionList>();
            bool timeOut = false;
            try
            {
                listTransactions = await _transactionRepository.GetTransactions(
               FiltersmodelBindingRequest.UserName,
               filters.TransactionDateFrom, filters.ServerRt, null,
              filters.StoreID.ToString(), filters.PosWorkstationNmbr, filters.IsChecked, filters.IsArchived,
               filters.NonCompliantOrHasMismatch, filters.NonCompliant, filters.PosTaNmbr, filters.RtClosureNmbr,
               filters.RtDocumentNmbr);
            }
            catch (Exception Ex)
            {
                Ex.ToString();
                timeOut = true;
            }
            var v = listTransactions.Where(x => x.BTransactionArchivedFlag == 1);
            ViewBag.TimeOut = timeOut;
            var list = _mapper.Map<List<TransactionList>, List<TransactionAffiliationViewModel>>(listTransactions);
            //var l = list.Where(x => x.LTransactionMismatchId == null);
            ViewBag.DateFrom = dateFrom.ToString("dd/MM/yyyy");
            //var ll = list.Select(x => x).Where(x => x.LTransactionMismatchId == 3);
            ViewBag.StoreID = retailStoreId;
            ViewBag.StoreGroupID = storeGroupId;

            _logger.LogDebug($"END: Return list of all RT Servers length:{listTransactions.Count}");
            return View(list);

        }
        public async Task<IActionResult> Filter(FiltersmodelBindingRequest filters, string StoreGroup,
            string store, int StoreGroupID, int storeID)
        {
            _logger.LogDebug("Start: Index Transactions");

            //var listTransaction = await FilterList(filters);
            //var listTransaction = await FilterList2(filters);
            PopulateInputs(filters);
            string nonCompliant;
            string hasMismatch;
            CompliantMismatchValue(filters.Conformity, out nonCompliant, out hasMismatch);
            string isArchived;
            string isChecked;
            CheckedArchivedValue(filters.IsCheckedOrArchived, out isArchived, out isChecked);
            List<TransactionList> listTransactions =  new List<TransactionList>();
            bool timeOut = false;
            try
            {
                listTransactions = await _transactionRepository.GetTransactions(
                FiltersmodelBindingRequest.UserName,
                Convert.ToDateTime(filters.TransactionDateFrom).ToString("yyyy/MM/dd"), filters.ServerRt, null,
                filters.StoreID.ToString(), filters.PosWorkstationNmbr, isChecked, isArchived,
                hasMismatch, nonCompliant, filters.PosTaNmbr,
                filters.RtClosureNmbr, filters.RtDocumentNmbr);
            }
            catch (Exception Ex)
            {
                Ex.ToString();
                timeOut = true;
                
            }
            ViewBag.TimeOut = timeOut;
            var list = _mapper.Map<List<TransactionList>, List<TransactionAffiliationViewModel>>(listTransactions);
            filters.StoreGroup = StoreGroup;
            filters.Store = store;
            PopulateInputs(filters);
            ViewBag.StoreID = storeID;
            ViewBag.StoreGroupID = StoreGroupID;
            ViewBag.DateFrom = Convert.ToDateTime(filters.TransactionDateFrom).ToString("dd/MM/yyyy");
            _logger.LogDebug($"END: Return list of all RT Servers length:{list.Count}");

            return View("Index", list);
        }

        public async Task<IActionResult> Details(
            string SzRtDocumentId, string SzRtServerId, int LRetailStoreId,
            int LStoreGroupId)
        {
            _logger.LogDebug("Start: Transaction Detail");

            var transaction = await _transactionRepository.GetTransaction(
                SzRtDocumentId, SzRtServerId, LRetailStoreId, LStoreGroupId);

            if (transaction == null)
            {
                return NotFound();
            }

            var transactionVM = _mapper.Map<TransactionAffiliation, TransactionAffiliationViewModel>(transaction);
            var counterPos = transactionVM.LPosReceivedTransactionCounter;
            var counterRt = transactionVM.LRtReceivedTransactionCounter;
            var rtServerError = new List<TransactionRtErrorViewModel>();
            if (transactionVM.SzRtDeviceId != null && transactionVM.LRtClosureNmbr != null && transactionVM.LRtDocumentNmbr != null)
            {
                rtServerError = _mapper.Map<List<TransactionRtError>, List<TransactionRtErrorViewModel>>(await _transactionRtErrorRepository.GetTransactionRtError(SzRtServerId, transactionVM.SzRtDeviceId, transactionVM.LRtClosureNmbr.Value, transactionVM.LRtDocumentNmbr.Value));
            }

            if (rtServerError != null)
            {
                transactionVM.RtServer.TransactionRtError = rtServerError;
            }
            foreach (var t in transactionVM.TransactionVat)
            {
                t.DPosGrossAmount = t.DPosGrossAmount * counterPos;
                t.DPosNetAmount = t.DPosNetAmount * counterPos;
                t.DPosVatAmount = t.DPosVatAmount * counterPos;
                t.DRtGrossAmount = t.DRtGrossAmount * counterRt;
                t.DRtNetAmount = t.DRtNetAmount * counterRt;
                t.DRtVatAmount = t.DRtVatAmount * counterRt;
            }
            if (transaction.SzRtDeviceId != null)
            {
                var transmission = await _transmissionRepository.GetTransmission(SzRtServerId, transaction.SzRtDeviceId ?? "", (transaction.DRtDateTime.HasValue
                                                                                           ? transaction.DRtDateTime.Value : transaction.DPosDateTime.Value));
                if (transmission != null)
                {
                    transactionVM.LRtServerOperationId = transmission.LRtServerOperationId;
                }
            }
            transactionVM.errorNonCompliant = transactionVM.RtServer.TransactionRtError
                .Where(c => c.SzRtDeviceId == transactionVM.SzRtDeviceId &&
                c.LRtDocumentNmbr == transactionVM.LRtDocumentNmbr).Count() > 0
                && _errorTable.Any(t => transactionVM.RtServer.TransactionRtError
                              .Where(c => c.SzRtDeviceId == transactionVM.SzRtDeviceId
                              && c.LRtDocumentNmbr == transactionVM.LRtDocumentNmbr)
                              .Any(err => err.SzDescription.StartsWith(t)))
                && transactionVM.BRtNonCompliantFlag == true;

            var xmlContent = transactionVM.TransactionDocument.Where(x => x.LDocumentTypeId == 2).Select(x => x.SzDocumentAttachment).FirstOrDefault();

            if (xmlContent != null)
            {
                var res = await BuildTemplateEngine(xmlContent);
                if (!string.IsNullOrEmpty(res))
                {
                    transactionVM.TransactionDocument.FirstOrDefault(x => x.LDocumentTypeId == 2)
                        .StringFromXmlDocument = res.ToString();
                }
            }

            var listLog = _mapper.Map<List<UserActivityLog>, List<UserActivityLogViewModel>>(await _transactionRepository.GetUserActivityByTransaction(transactionVM.SzRtDocumentId)).ToList();
            transactionVM.UserActivityLogViewModel = listLog.OrderByDescending(x => x.DUserActivityDateTime).ToList();
            _logger.LogDebug($"END: Transaction Detail DocumentId :{transactionVM.SzRtDocumentId}");

            return View(transactionVM);

        }
        public async Task<string> BuildTemplateEngine(string xmlContent)
        {
            _logger.LogDebug("Start: Prepare receipt Template");

            var xmlSerialize = XmlSerializerHelper.GetMemoryDetailFromXml(xmlContent);
            xmlSerialize.Receipt.SumTaxAmountVatTotals = xmlSerialize.Receipt.VatTotals.Sum(x => x.TaxAmount);
            string path = string.Format("{0}\\Templates\\DocumentAttachment.cshtml", Directory.GetCurrentDirectory());
            string template = await System.IO.File.ReadAllTextAsync(path);
            RazorEngine razorEngine = new RazorEngine();
            RazorEngineCompiledTemplate temp = razorEngine.Compile(template);
            string res = temp.Run(xmlSerialize);
            _logger.LogDebug($"END: Receipt: {res}");

            return res.ToString();
        }


        public IActionResult GetListRtServersNoFilters(FiltersmodelBindingRequest filters)
        {
            _logger.LogDebug("Start: Reset all filters table");
            var filtersReset = ResetFilters();
            filtersReset.Store = filters.Store;
            filtersReset.StoreGroup = filters.StoreGroup;
            filtersReset.ServerRt = filters.ServerRt;
            filtersReset.TransactionDateTo = filters.TransactionDateTo;
            filtersReset.TransactionDateFrom = filters.TransactionDateFrom;
            filtersReset.StoreID = filters.StoreID;
            filtersReset.StoreGroupID = filters.StoreGroupID;
            _logger.LogDebug("End: Reset filters");
            return RedirectToAction("Filter", filtersReset);
        }

        public ActionResult RedirectToTransactionsWithFilters(string filtersJson)
        {
            var filters = JsonConvert.DeserializeObject<FiltersmodelBindingRequest>(filtersJson);
            return RedirectToAction("Index", filters);
        }
        public async Task<IEnumerable<TransactionAffiliationViewModel>> GetTransactionsCompliant(RtServer rtServer, IEnumerable<string> listDocumentsID)
        {
            
            var user = await GetCurrentuser();
            var roles = await _userManager.GetRolesAsync(user);
            var trn = await _transactionRepository.GetTransactionsCompliant(rtServer, listDocumentsID,_properties.Value.MaxTransactions);

            if (!roles.Any(x => x == "OperatorIT" ))
            {
                trn = trn.Where(x => x.LTransactionMismatchId != 3 && x.LTransactionMismatchId != 4);
            }
            var trnVM = _mapper.Map<IEnumerable<TransactionAffiliation>, IEnumerable<TransactionAffiliationViewModel>>(trn);
            return trnVM;
        }
        public async Task<IEnumerable<TransactionAffiliationViewModel>> GetTransactionsCompliant(string rtServer, IEnumerable<string> listDocumentsID,FiltersmodelBindingRequest filters)
        {
            var user = await GetCurrentuser();
            var roles = await _userManager.GetRolesAsync(user);

            // var trn = await _transactionRepository.GetTransactionsCompliant(rtServer, listDocumentsID, _properties.Value.MaxTransactions,filters);
            var trn = new List<TransactionAffiliation>();
            if (!roles.Any(x => x == "OperatorIT"))
            {
                trn = (await _transactionRepository.GetTransactionsCompliantOthers(rtServer, listDocumentsID, _properties.Value.MaxTransactions, filters)).ToList();
            }
            else
            {
                trn =( await _transactionRepository.GetTransactionsCompliant(rtServer, listDocumentsID, _properties.Value.MaxTransactions, filters)).ToList();


            }
            var trnlist = trn.ToList();
            var trnVM = _mapper.Map<IEnumerable<TransactionAffiliation>, IEnumerable<TransactionAffiliationViewModel>>(trnlist);
            return trnVM;
        }
        public async Task<List<RtServersTransactionsViewModel>> InitialList2(RtServer rtServer, string dateFrom)
        {
            _logger.LogDebug("Start: First access to Index Transactions ");

            var rtServerVM = _mapper.Map<RtServer, RtServerViewModel>(rtServer);
            //Get ListNonCompliant
            var listTrnNonCompliant = (await GetTransactionsNonCompliant(rtServer,dateFrom))
                                          .OrderByDescending(x => x.BRtNonCompliantFlag)
                                          .ThenByDescending(x => x.TransactionHasMismatch)
                                          .ThenByDescending(x => x.DRtDateTime); ;
            //Get List Transaction Compliant
            var listdocumentId = listTrnNonCompliant.Select(x => x.SzRtDocumentId);
            var listTrnCompliant = (await GetTransactionsCompliant(rtServer, listdocumentId));//.OrderByDescending(x => x.DRtDateTime).ToList() ;//.Except(listTrnNonCompliant).OrderByDescending(x => x.DRtDateTime).ToList();//(x => !listTrnNonCompliant.Contains(x)).OrderByDescending(x => x.DRtDateTime);
            //Union ListTransaction Non compliant and List transaction Compliant
            rtServerVM.TransactionAffiliation = listTrnNonCompliant.Union(listTrnCompliant).Take(_properties.Value.MaxTransactions).ToList();

            var listRtServers = new List<RtServerViewModel>();
            listRtServers.Add(rtServerVM);
            var listGroupedRtServers = new List<RtServersTransactionsViewModel>();
            listGroupedRtServers.Add(new RtServersTransactionsViewModel

            {
                LRetailStoreId = rtServer.LRetailStoreId,
                LStoreGroupId = rtServer.LStoreGroupId,
                RtServers = listRtServers
            });
            _logger.LogDebug($"END: List Transactions length: {listGroupedRtServers.Count()}");

            return listGroupedRtServers;
        }

        public async Task<List<RtServersTransactionsViewModel>> InitialList(RtServer rtServer)
        {
            _logger.LogDebug("Start: First access to Index Transactions ");

            var rtServerVM = _mapper.Map<RtServer, RtServerViewModel>(rtServer);
            var rtServerError = await _transactionRtErrorRepository.GetTransactionsRtErrorByServer(rtServer.SzRtServerId);
            // Get List Transactions non compliant
            var listTrnNonCompliant = rtServerVM.TransactionAffiliation.Where(
                x => (
                (rtServerError.Where(c => c.SzRtDeviceId == x.SzRtDeviceId &&
                                                          c.LRtDocumentNmbr == x.LRtDocumentNmbr)
                                              .Count() > 0
                 && _errorTable.Any(t => rtServerError.Where(c => c.SzRtDeviceId == x.SzRtDeviceId
                                                                                  && c.LRtDocumentNmbr == x.LRtDocumentNmbr)
                                                                        .Any(err => err.SzDescription.StartsWith(t)))
                 && x.BRtNonCompliantFlag == true)
                    ||
                x.TransactionHasMismatch == true
                )
                && x.BTransactionCheckedFlag != true)
                                          .OrderByDescending(x => x.BRtNonCompliantFlag)
                                          .ThenByDescending(x => x.TransactionHasMismatch)
                                          .ThenByDescending(x => x.DRtDateTime);

            foreach (var t in listTrnNonCompliant.ToList())
            {
                if (rtServerError.Where(c => c.SzRtDeviceId == t.SzRtDeviceId &&
                c.LRtDocumentNmbr == t.LRtDocumentNmbr).Count() > 0
                && _errorTable.Any(x => rtServerError
                              .Where(c => c.SzRtDeviceId == t.SzRtDeviceId
                              && c.LRtDocumentNmbr == t.LRtDocumentNmbr)
                              .Any(err => err.SzDescription.StartsWith(x)))
                && t.BRtNonCompliantFlag == true)

                { t.errorNonCompliant = true;}
            }

            //Get List Transaction Compliant
            var listTrnCompliant = rtServerVM.TransactionAffiliation.Except(listTrnNonCompliant).OrderByDescending(x => x.DRtDateTime).ToList();//(x => !listTrnNonCompliant.Contains(x)).OrderByDescending(x => x.DRtDateTime);
            //Union ListTransaction Non compliant and List transaction Compliant
            rtServerVM.TransactionAffiliation = listTrnNonCompliant.Union(listTrnCompliant).ToList().Take(_properties.Value.MaxTransactions).ToList();

            var listRtServers = new List<RtServerViewModel>();
            listRtServers.Add(rtServerVM);
            var listGroupedRtServers = new List<RtServersTransactionsViewModel>();
            listGroupedRtServers.Add(new RtServersTransactionsViewModel {
                LRetailStoreId = rtServer.LRetailStoreId,
                LStoreGroupId = rtServer.LStoreGroupId,
                RtServers = listRtServers
            });
            _logger.LogDebug($"END: List Transactions length: {listGroupedRtServers.Count()}");
            return listGroupedRtServers;
        }
        public async Task<List<RtServersTransactionsViewModel>> InitialListNonCompliant2(RtServer rtServer, string dateFrom)
        {
            _logger.LogDebug("Start: First access to Index Transactions ");
            var trnNoncompliant = await GetTransactionsNonCompliant(rtServer, dateFrom);
            var rtServerVM = _mapper.Map<RtServer, RtServerViewModel>(rtServer);
            // Get List Transactions non compliant
            var test = trnNoncompliant.OrderByDescending(x => x.BRtNonCompliantFlag)
                                                 .ThenByDescending(x => x.TransactionHasMismatch)
                                                 .ThenByDescending(x => x.DRtDateTime)
                                                 .Take(_properties.Value.MaxTransactions).ToList();
            rtServerVM.TransactionAffiliation = test;

            var listRtServers = new List<RtServerViewModel>();
            listRtServers.Add(rtServerVM);
            var listGroupedRtServers = new List<RtServersTransactionsViewModel>();
            listGroupedRtServers.Add(new RtServersTransactionsViewModel

            {
                LRetailStoreId = rtServer.LRetailStoreId,
                LStoreGroupId = rtServer.LStoreGroupId,
                RtServers = listRtServers
            });
            _logger.LogDebug($"END: List Transactions length: {listGroupedRtServers.Count()}");

            return listGroupedRtServers;
        }
        public async Task<List<RtServersTransactionsViewModel>> InitialListNonCompliant(RtServer rtServer)
        {
            _logger.LogDebug("Start: First access to Index Transactions ");

            var rtServerVM = _mapper.Map<RtServer, RtServerViewModel>(rtServer);
            var rtServerError = rtServer.TransactionRtError;// await _transactionRtErrorRepository.GetTransactionsRtErrorByServer(rtServer.SzRtServerId);
            // Get List Transactions non compliant
            var listTrnNonCompliant = rtServerVM.TransactionAffiliation
                .Where(x => ((
                //rtServerError.Where(c => c.SzRtDeviceId == x.SzRtDeviceId &&
                //                                          c.LRtDocumentNmbr == x.LRtDocumentNmbr)
                //                              .Count() > 0
                x.HasError && _errorTable.Any
                         ( t => rtServerError
                         .Where(c => c.SzRtDeviceId == x.SzRtDeviceId 
                         && c.LRtDocumentNmbr == x.LRtDocumentNmbr)
                         .Any(err => err.SzDescription.StartsWith(t)))
                 && x.BRtNonCompliantFlag == true)
                    ||
                x.TransactionHasMismatch == true
                )
                && x.BTransactionCheckedFlag != true);

            foreach (var t in listTrnNonCompliant)
            {
                if (rtServerError.Where(c => c.SzRtDeviceId == t.SzRtDeviceId &&
                                                         c.LRtDocumentNmbr == t.LRtDocumentNmbr)
                                             .Count() > 0
                       && _errorTable.Any(x => rtServerError.Where(c => c.SzRtDeviceId == t.SzRtDeviceId
                                                                                  && c.LRtDocumentNmbr == t.LRtDocumentNmbr)
                                                                        .Any(err => err.SzDescription.StartsWith(x)))


                && t.BRtNonCompliantFlag == true)
                {
                    t.errorNonCompliant = true;
                }
            }

            //Get List Transaction Compliant
            //Union ListTransaction Non compliant and List transaction Compliant
            rtServerVM.TransactionAffiliation = listTrnNonCompliant
                                                  .OrderByDescending(x => x.BRtNonCompliantFlag)
                                                 .ThenByDescending(x => x.TransactionHasMismatch)
                                                 .ThenByDescending(x => x.DRtDateTime.HasValue ? x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date)
                                                 .ToList()
                                                .Take(_properties.Value.MaxTransactions).ToList();

            var listRtServers = new List<RtServerViewModel>();
            listRtServers.Add(rtServerVM);
            var listGroupedRtServers = new List<RtServersTransactionsViewModel>();
            listGroupedRtServers.Add(new RtServersTransactionsViewModel

            {
                LRetailStoreId = rtServer.LRetailStoreId,
                LStoreGroupId = rtServer.LStoreGroupId,
                RtServers = listRtServers
            });
            _logger.LogDebug($"END: List Transactions length: {listGroupedRtServers.Count()}");

            return listGroupedRtServers;
        }
        public async Task<List<RtServersTransactionsViewModel>> FilterList(FiltersmodelBindingRequest filters)
        {
            _logger.LogDebug("Start: Apply filters to list Transactions");

            var listServers = await _rtServerRepository.FilteredList(filters);
            var listServersVM = _mapper.Map<List<ListServersGrouped>, List<RtServersTransactionsViewModel>>(listServers);
             var rtServerError = await _transactionRtErrorRepository.GetTransactionsRtErrorByServer(listServers.FirstOrDefault().RtServers.FirstOrDefault().SzRtServerId);
           // var rtServerError = await _transactionRtErrorRepository.GetTransactionsRtErrorByServer(filters.ServerRt);

            listServersVM.ForEach(x =>
            {
                x.RtServers.ForEach(x =>
                {
                    var listNonComplHasMismatch = x.TransactionAffiliation.Where(x => (rtServerError
                                               .Where(c => c.SzRtDeviceId == x.SzRtDeviceId && c.LRtDocumentNmbr == x.LRtDocumentNmbr).Count() > 0
                                                &&
                                               _errorTable.Any(t => rtServerError.Where(c => c.SzRtDeviceId == x.SzRtDeviceId
                                                                                    && c.LRtDocumentNmbr == x.LRtDocumentNmbr)
                                                                          .Any(err => err.SzDescription.StartsWith(t))
                          )
                                                  && x.BRtNonCompliantFlag == true
                                               ) &&
                                               x.TransactionHasMismatch == true)
                                                 .OrderByDescending(x => x.BRtNonCompliantFlag)
                                                 .OrderByDescending(x => x.TransactionHasMismatch)
                                                 .OrderByDescending(x => x.DRtDateTime)
                                                 .ToList();
                    var lisNonCompliantHasNotMismtach = x.TransactionAffiliation.Where(x => (rtServerError
                                                 .Where(c => c.SzRtDeviceId == x.SzRtDeviceId && c.LRtDocumentNmbr == x.LRtDocumentNmbr).Count() > 0
                                                  &&

                                                 _errorTable.Any(t => rtServerError.Where(c => c.SzRtDeviceId == x.SzRtDeviceId
                                                                                      && c.LRtDocumentNmbr == x.LRtDocumentNmbr)
                                                                            .Any(err => err.SzDescription.StartsWith(t))
                            )
                                                    && x.BRtNonCompliantFlag == true
                                                 ) &&
                                                 x.TransactionHasMismatch != true)
                                                                .OrderByDescending(x => x.BRtNonCompliantFlag)
                                                                .OrderByDescending(x => x.TransactionHasMismatch)
                                                                .OrderByDescending(x => x.DRtDateTime)
                                                                .ToList();

                    var listCompliantHasMismatch = x.TransactionAffiliation.Where(x => !(rtServerError
                                                 .Where(c => c.SzRtDeviceId == x.SzRtDeviceId && c.LRtDocumentNmbr == x.LRtDocumentNmbr).Count() > 0
                                                  &&

                                                 _errorTable.Any(t => rtServerError.Where(c => c.SzRtDeviceId == x.SzRtDeviceId
                                                                                      && c.LRtDocumentNmbr == x.LRtDocumentNmbr)
                                                                            .Any(err => err.SzDescription.StartsWith(t))
                            )
                                                    && x.BRtNonCompliantFlag == true
                                                 ) && x.TransactionHasMismatch == true)
                                                                .OrderByDescending(x => x.TransactionHasMismatch)
                                                                .OrderByDescending(x => x.BRtNonCompliantFlag)
                                                                .OrderByDescending(x => x.DRtDateTime)
                                                                .ToList();
                    var listCompliantHasNotMismatch = x.TransactionAffiliation.Where(x => !(rtServerError
                                                 .Where(c => c.SzRtDeviceId == x.SzRtDeviceId && c.LRtDocumentNmbr == x.LRtDocumentNmbr).Count() > 0
                                                  &&
                                                 _errorTable.Any(t => rtServerError.Where(c => c.SzRtDeviceId == x.SzRtDeviceId
                                                                                      && c.LRtDocumentNmbr == x.LRtDocumentNmbr)
                                                                            .Any(err => err.SzDescription.StartsWith(t))
                            )
                                                   && x.TransactionHasMismatch != true))
                                                            .OrderByDescending(x => x.BRtNonCompliantFlag)
                                                            .OrderByDescending(x => x.TransactionHasMismatch)
                                                            .OrderByDescending(x => x.DRtDateTime).Take(_properties.Value.MaxTransactions)
                                                            .ToList();


                    foreach (var t in listNonComplHasMismatch)
                    {
                        t.errorNonCompliant = true;
                    }
                    foreach (var t in lisNonCompliantHasNotMismtach)
                    {
                        t.errorNonCompliant = true;
                    }
                    switch (filters.Conformity)
                    {
                        case "CompliantHasNotMismatch":
                            x.TransactionAffiliation = listCompliantHasNotMismatch;
                            break;
                        case "CompliantHasMismatch":
                            x.TransactionAffiliation = listCompliantHasMismatch;
                            break;
                        case "NonCompliantHasNotMismtach":
                            x.TransactionAffiliation = lisNonCompliantHasNotMismtach;
                            break;
                        case "NonCompliantHasMismatch":
                            x.TransactionAffiliation = listNonComplHasMismatch;
                            break;
                        case "NonCompliantHasMismatchFromTRX":
                            x.TransactionAffiliation = listNonComplHasMismatch.Union(lisNonCompliantHasNotMismtach)
                                                                            .Union(listCompliantHasMismatch)
                                                                            .ToList();
                            break;
                        case "NonCompliant":
                            x.TransactionAffiliation = listNonComplHasMismatch.Union(lisNonCompliantHasNotMismtach)
                                                                              .Union(listCompliantHasMismatch)
                                                                              .ToList();
                            break;
                        default:
                            x.TransactionAffiliation = listNonComplHasMismatch
                                                        .Union(lisNonCompliantHasNotMismtach)
                                                        .Union(listCompliantHasMismatch)
                                                        .Union(listCompliantHasNotMismatch)
                                                        .ToList();
                            break;
                    }

                    if (!string.IsNullOrEmpty(filters.IsCheckedOrArchived))
                    {
                        var valueTest = filters.IsCheckedOrArchived;
                        if (valueTest == "isChecked")
                        {
                            x.TransactionAffiliation = x.TransactionAffiliation
                                                                              .Where(x => x.TransactionIsChecked == true).ToList();
                        }
                        if (valueTest == "isNotChecked")
                        {
                            x.TransactionAffiliation = x.TransactionAffiliation
                                                                              .Where(x => x.TransactionIsChecked == false).ToList();
                        }
                        if (valueTest == "isArchived")
                        {
                            x.TransactionAffiliation = x.TransactionAffiliation
                                                                              .Where(x => x.TransactionIsArchived == true).ToList();
                        }
                    }
                    x.TransactionAffiliation = x.TransactionAffiliation
                                       .Take(_properties.Value.MaxTransactions)
                                       .ToList();

                    if (string.IsNullOrEmpty(filters.ServerRt))
                    {
                        x.TransactionAffiliation = x.TransactionAffiliation.Take(100).ToList();
                    }

                });
            });

            _logger.LogDebug($"END: List Transactions filtered length: {listServersVM.Count()}");

            return listServersVM;
        }
        

        public async Task<List<RtServersTransactionsViewModel>> FilterList2(FiltersmodelBindingRequest filters)
        {

            var user = await GetCurrentuser();
            var roles = await _userManager.GetRolesAsync(user);
           
            var listServers = await _rtServerRepository.FilteredList(filters);
            var listServersVM = _mapper.Map<List<ListServersGrouped>, List<RtServersTransactionsViewModel>>(listServers);
            var serverId = listServers.FirstOrDefault().RtServers.FirstOrDefault().SzRtServerId;
            var listMismatch =( await FilterListsTrn( _mapper.Map<IEnumerable<TransactionAffiliation>, IEnumerable<TransactionAffiliationViewModel>>(await GetTransactionsMismatch(serverId)),filters)).ToList();
            var listError =(await FilterListsTrn( _mapper.Map<IEnumerable<TransactionAffiliation>, IEnumerable<TransactionAffiliationViewModel>>(await GetTransactionsErrorComplete(serverId)),filters)).ToList();
            //var listUnion = listMismatch.Union(listError).ToList();
            var listDocumentToExlude = listError.Select(x => x.SzRtDocumentId).ToList();
                                   ;
            var listCompliantHasNotMismatch = 
                    (await GetTransactionsCompliant(serverId, listDocumentToExlude,filters))
                                                ; 
            //if (!roles.Any(x => x == "OperatorIT"))
            //{
            //    listCompliantHasNotMismatch = listCompliantHasNotMismatch.Where(x => (x.LTransactionMismatchId != 3 && x.DRtDateTime< DateTime.Today.Date)&& (x.LTransactionMismatchId != 4 && x.DPosDateTime<DateTime.Today.Date)).ToList();
            //}

            var listFiltered = FilterListsTrn(listMismatch, filters);
            listServersVM.ForEach(x =>
            {
                x.RtServers.ForEach(t =>
               {
                   var listNonCompliantHasMismatch = listMismatch.Intersect(listError).OrderByDescending(x => x.BRtNonCompliantFlag)
                                                .OrderByDescending(x => x.TransactionHasMismatch)
                                                .OrderByDescending(x => x.DRtDateTime.HasValue?x.DRtDateTime.Value.Date:x.DPosDateTime.Value.Date);
                   var listNonCompliantHasNotMismatch = listError.Except(listMismatch).OrderByDescending(x => x.BRtNonCompliantFlag)
                                                               .OrderByDescending(x => x.TransactionHasMismatch)
                                                .OrderByDescending(x => x.DRtDateTime.HasValue ? x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date);
                   var listCompliantHasMismatch = listMismatch.Except(listError).OrderByDescending(x => x.TransactionHasMismatch)
                                                               .OrderByDescending(x => x.BRtNonCompliantFlag)
                                                .OrderByDescending(x => x.DRtDateTime.HasValue ? x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date);

                   switch (filters.Conformity)
                   {
                       case "CompliantHasNotMismatch":
                           t.TransactionAffiliation = listCompliantHasNotMismatch.ToList();
                           break;
                       case "CompliantHasMismatch":
                           t.TransactionAffiliation = listCompliantHasMismatch.ToList();
                           break;
                       case "NonCompliantHasNotMismtach":
                           t.TransactionAffiliation = listNonCompliantHasNotMismatch.ToList();
                           break;
                       case "NonCompliantHasMismatch":
                           t.TransactionAffiliation = listNonCompliantHasMismatch.ToList();
                           break;
                       case "NonCompliantHasMismatchFromTRX":
                           t.TransactionAffiliation = listNonCompliantHasMismatch.Union(listNonCompliantHasNotMismatch)
                                                                           .Union(listCompliantHasMismatch)
                                                                           .ToList();
                           break;
                       case "NonCompliant":
                           t.TransactionAffiliation = listNonCompliantHasMismatch.Union(listNonCompliantHasNotMismatch)
                                                                             .Union(listCompliantHasMismatch)
                                                                             .ToList();
                           break;
                       default:
                           t.TransactionAffiliation = listNonCompliantHasMismatch
                                                       .Union(listNonCompliantHasNotMismatch)
                                                       .Union(listCompliantHasMismatch)
                                                       .Union(listCompliantHasNotMismatch)
                                                       .ToList();
                           break;
                   }
                   if (!string.IsNullOrEmpty(filters.IsCheckedOrArchived))
                   {
                       var valueTest = filters.IsCheckedOrArchived;
                       if (valueTest == "isChecked")
                       {
                           t.TransactionAffiliation = t.TransactionAffiliation
                                                                             .Where(x => x.TransactionIsChecked == true).ToList();
                       }
                       if (valueTest == "isNotChecked")
                       {
                           t.TransactionAffiliation = t.TransactionAffiliation
                                                                             .Where(x => x.TransactionIsChecked == false).ToList();
                       }
                       if (valueTest == "isArchived")
                       {
                           t.TransactionAffiliation = t.TransactionAffiliation
                                                                             .Where(x => x.TransactionIsArchived == true).ToList();
                       }
                   }
                   t.TransactionAffiliation = t.TransactionAffiliation
                                      .Take(_properties.Value.MaxTransactions)
                                      .ToList();

                   if (string.IsNullOrEmpty(filters.ServerRt))
                   {
                       t.TransactionAffiliation = t.TransactionAffiliation.Take(100).ToList();
                   }
               });
            });


            _logger.LogDebug($"END: List Transactions filtered length: {listServersVM.Count()}");

            return listServersVM;
        }

        private Task<IEnumerable<TransactionAffiliationViewModel>> FilterListsTrn(IEnumerable<TransactionAffiliationViewModel> list, FiltersmodelBindingRequest filters)
        {
            var trn = list.AsQueryable();
            if (list != null)
            {


                Expression<Func<TransactionAffiliationViewModel, bool>> expressTrnFrom = null;
                if (!string.IsNullOrEmpty(filters.TransactionDateFrom))
                {
                    DateTime transactionDate = DateTime.ParseExact(
                        filters.TransactionDateFrom,
                        "dd-MM-yyyy",
                        System.Globalization.CultureInfo.InvariantCulture);
                    expressTrnFrom = x => (x.DRtDateTime!= null ? x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date) >= transactionDate;
                }
                Expression<Func<TransactionAffiliationViewModel, bool>> expressTrnTo = null;
                if (!string.IsNullOrEmpty(filters.TransactionDateTo))
                {
                    DateTime transactionDate = DateTime.ParseExact(
                        filters.TransactionDateTo,
                        "dd-MM-yyyy",
                        System.Globalization.CultureInfo.InvariantCulture);

                    expressTrnTo = x => (x.DRtDateTime != null ? x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date) <= transactionDate;

                }

                Expression<Func<TransactionAffiliationViewModel, bool>> expressTrnPos = null;
                if (!string.IsNullOrEmpty(filters.PosWorkstationNmbr))
                {

                    expressTrnPos = x => x.LPosWorkstationNmbr.ToString() == filters.PosWorkstationNmbr;

                }

                Expression<Func<TransactionAffiliationViewModel, bool>> expressTrnClosureNmbr = null;

                if (!string.IsNullOrEmpty(filters.RtClosureNmbr))
                {

                    expressTrnClosureNmbr = x => x.LRtClosureNmbr.ToString() == filters.RtClosureNmbr;

                }

                Expression<Func<TransactionAffiliationViewModel, bool>> expressTrnDocumentNmbr = null;

                if (!string.IsNullOrEmpty(filters.RtDocumentNmbr))
                {

                    expressTrnDocumentNmbr = x => x.LRtDocumentNmbr.ToString() == filters.RtDocumentNmbr;
                }

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

            }
            return Task.FromResult(trn.AsEnumerable());
        }



        public async Task<IEnumerable<TransactionAffiliation>> GetTransactionsErrorComplete(string rtServer)
        {
            var trnAllError = (await _transactionRtErrorRepository
          .GetTransactionsRtErrorByServer2(rtServer));
            trnAllError = trnAllError.Where(x => _errorTable.Any(t => x.SzDescription.StartsWith(t))).ToList();

            var trnerror = (await _transactionRepository.GetTransactionsError(rtServer));
            var trnerrorNnComp = trnerror.Where(x =>
            (
                (trnAllError != null
                    && trnAllError.Any(c => c.SzRtDeviceId == x.SzRtDeviceId
                    && c.LRtDocumentNmbr == x.LRtDocumentNmbr)
                                        )
                // questa riga verra dicommentata se la riga 140 dell'index.cshtml View
                //sara dicommentata e anche nel detail view la riga 234 dovvrebbe essere decommentata
                // tutto questo per poter visulizzare i led di conformita rossi se:
                //1.flag BRtNonCompliantFlag=true
                // e
                // 1.1. errorNoncompliantflag= true
                //      oppure
                // 1.2. hasmistamch=true
                &&
                x.BRtNonCompliantFlag == true)
                );

 
            return trnerrorNnComp;
        }

        public async Task<IEnumerable<TransactionAffiliation>> GetTransactionsMismatch(string rtServer)
        {
            var user = await GetCurrentuser();
            var roles = await _userManager.GetRolesAsync(user);
            var trnsmismatch = (await _transactionRepository.GetTransactionsMismtach(rtServer));

            if (!roles.Any(x => x == "OperatorIT"))
            {
                trnsmismatch = trnsmismatch.Where(x => x.LTransactionMismatchId != 3 && x.LTransactionMismatchId != 4);
            }
            return trnsmismatch;
        }
        public async Task<IEnumerable<TransactionAffiliation>> GetTransactionsMismatch(RtServer rtServer)
        {
            var user = await GetCurrentuser();
            var roles = await _userManager.GetRolesAsync(user);
            var trnsmismatch = (await _transactionRepository.GetTransactionsMismtach(rtServer));

            if (!roles.Any(x => x == "OperatorIT"))
            {
            trnsmismatch = trnsmismatch.Where(x => x.LTransactionMismatchId != 3 && x.LTransactionMismatchId != 4);
            //View Just trn have missing RT Totals     
            //trnsmismatch = trnsmismatch.Where(x => x.LTransactionMismatchId != 3);
            }
            return trnsmismatch;
        }
        public async Task<IEnumerable<TransactionAffiliationViewModel>> GetTransactionsNonCompliant(RtServer rtServer, string dateFrom)
        {
         

            //var trnAllError = (await _transactionRtErrorRepository
            //    .GetTransactionsRtErrorByServer(rtServer.SzRtServerId, _errorTable.AsEnumerable())).ToList();
            var trnAllError = (await _transactionRtErrorRepository
           .GetTransactionsRtErrorByServer2(rtServer.SzRtServerId)).Where(x =>
           x.DLastUpdateLocal.HasValue && x.DLastUpdateLocal.Value.ToString("dd-MM-yyyy") 
           == dateFrom);
           // trnAllError = trnAllError.Where(x => _errorTable.Any(t => x.SzDescription.StartsWith(t))).ToList();
            var test=trnAllError.Where(x => _errorTable.Any(t => x.SzDescription.StartsWith(t))).ToList();

            var trnerror = (await _transactionRepository.GetTransactionsError(rtServer));
            
            var trnerrorNnComp = trnerror.AsParallel().Where(x =>
            (
                 (test != null
                    &&
                    test.Any(c => c.SzRtDeviceId == x.SzRtDeviceId 
                    && c.LRtDocumentNmbr == x.LRtDocumentNmbr
                    && c.LRtClosureNmbr==x.LRtClosureNmbr
                    )
                                        )
                &&
                x.BRtNonCompliantFlag == true
                )
                &&
               x.BTransactionCheckedFlag != true
                ).ToList();
            var trnErrorsVM = _mapper.Map<IEnumerable<TransactionAffiliation>, IEnumerable<TransactionAffiliationViewModel>>(trnerrorNnComp.AsEnumerable());
            foreach (var trn in trnErrorsVM)
            {
                trn.errorNonCompliant = true;
            }

           var trnMimsatchVM = _mapper.Map<IEnumerable<TransactionAffiliation>, IEnumerable<TransactionAffiliationViewModel>>(await GetTransactionsMismatch(rtServer));

            var res = trnErrorsVM.Union(trnMimsatchVM).AsEnumerable();
            return res;
        }

        public async Task<IActionResult> IsArchived(string transaction)
        {
            _logger.LogDebug("Start: Change BTransactionArchivedFlag status");

            var parameters = JsonConvert.DeserializeObject<TransactionCheckedOrArchived>(transaction);

            var getTransaction = 
                await _transactionRepository.GetTransaction(parameters.IdTransaction, parameters.RtServerId,
                                                                        parameters.RetailStoreId,
                                                                        parameters.StoreGroupId);
            if (getTransaction == null)
            {
                return NotFound();
            }
            if (getTransaction.BTransactionArchivedFlag != parameters.IsArchived)
            {
                getTransaction.BTransactionArchivedFlag = parameters.IsArchived;
                getTransaction.SzUserName = User.Identity.Name;
            }

            try
            {
                _unitOfWork.UpdateAsync(getTransaction);
                await _unitOfWork.CompleteAsync();

                _logger.LogDebug($"END: BTransactionArchivedFlag value changed to {getTransaction.BTransactionArchivedFlag}");

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                _logger.LogCritical("END: There is an error in changing BTransactionArchivedFlag status ");

                return Json(new { success = false });
            }


        }

        public async Task<IActionResult> NoteEdit(string transaction)
        {
            _logger.LogDebug("Start: Change SzTranscationCheckNote value");

            var parameters = JsonConvert.DeserializeObject<TransactionCheckedOrArchived>(transaction);

            var getTransaction = await _transactionRepository.GetTransaction(parameters.IdTransaction, parameters.RtServerId,
                                                                        parameters.RetailStoreId, parameters.StoreGroupId);
            if (getTransaction == null)
            {
                return NotFound();
            }
            if (getTransaction.SzTranscationCheckNote != parameters.CheckNote)
            {
                getTransaction.SzTranscationCheckNote = parameters.CheckNote;
                getTransaction.SzUserName = User.Identity.Name;

            }
            try
            {
                _unitOfWork.UpdateAsync(getTransaction);
                await _unitOfWork.CompleteAsync();
                _logger.LogDebug($"END: Change SzTranscationCheckNote value to : {getTransaction.SzTranscationCheckNote}");

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                _logger.LogCritical("END: There is an error in changing BTransactionArchivedFlag status ");

                return Json(new { success = false });
            }

        }

        public async Task<IActionResult> IsChecked(string transaction)
        {
            _logger.LogDebug("Start: Change SzTranscationCheckNote value");

            var parameters = JsonConvert.DeserializeObject<TransactionCheckedOrArchived>(transaction);

            var getTransaction = await _transactionRepository.GetTransaction(parameters.IdTransaction, parameters.RtServerId,
                                                                        parameters.RetailStoreId, parameters.StoreGroupId);
            if (getTransaction == null)
            {
                return NotFound();
            }

            if (getTransaction.BTransactionCheckedFlag != parameters.IsChecked)
            {
                getTransaction.BTransactionCheckedFlag = parameters.IsChecked;
                if (getTransaction.TransactionVat.Count > 0)
                {
                    getTransaction.TransactionVat.ToList().ForEach(x =>
                    {
                        x.BVatCheckedFlag = parameters.IsChecked;
                    });
                }
                getTransaction.SzUserName = User.Identity.Name;
            }

            try
            {
                _unitOfWork.UpdateAsync(getTransaction);
                await _unitOfWork.CompleteAsync();
                _logger.LogDebug($"END: Change BTransactionCheckedFlag status to : {getTransaction.BTransactionCheckedFlag}");

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                _logger.LogCritical("END: There is an error in changing BTransactionCheckedFlag status ");

                return Json(new { success = false });
            }
        }

        public void PopulateInputs(FiltersmodelBindingRequest filters)
        {
            _logger.LogDebug("Start: Populate all filter Inputs.");

            ViewBag.storeGroup = string.IsNullOrEmpty(filters.StoreGroup) ? "" : filters.StoreGroup;
            ViewBag.store = string.IsNullOrEmpty(filters.Store) ? "" : filters.Store;
            ViewBag.serverRt = string.IsNullOrEmpty(filters.ServerRt) ? "" : filters.ServerRt;
            ViewBag.pos = string.IsNullOrEmpty(filters.PosWorkstationNmbr) ? "" : filters.PosWorkstationNmbr;
            ViewBag.transactionDateFrom = filters.TransactionDateFrom;
            ViewBag.transactionDateTo = filters.TransactionDateTo;
            ViewBag.storeGroupID = filters.StoreGroupID;
            ViewBag.storeID = filters.StoreID;

            ViewBag.RtClosureNmbr = filters.RtClosureNmbr;
            ViewBag.RtDocumentNmbr = filters.RtDocumentNmbr;
            ViewBag.PosTaNmbr = filters.PosTaNmbr;
            
            PopulateNonCompliantOrHasMismatchDropDownList(string.IsNullOrEmpty(filters.NonCompliantOrHasMismatch) ? "" : filters.NonCompliantOrHasMismatch);
            PopulateisCheckedDropDownList(string.IsNullOrEmpty(filters.IsChecked) ? "" : filters.IsChecked);
            PopulateisCheckedOrArchivedDropDownList(string.IsNullOrEmpty(filters.IsCheckedOrArchived) ? "" : filters.IsCheckedOrArchived);
            PopulateConformityDropDownList(string.IsNullOrEmpty(filters.Conformity) ? "" : filters.Conformity);
            _logger.LogDebug("END: End populate Inputs.");

        }

        public void CompliantMismatchValue(string Conformity, out string nonCompliant, out string hasMismatch)
        {
            nonCompliant = null;
            hasMismatch = null;
            if (!string.IsNullOrEmpty(Conformity))
            {
                switch (Conformity)
                {
                    case "CompliantHasNotMismatch":
                        nonCompliant = "false";
                        hasMismatch = "false";
                        break;
                    case "CompliantHasMismatch":
                        nonCompliant = "false";
                        hasMismatch = "true";
                        break;
                    case "NonCompliantHasNotMismtach":
                        nonCompliant = "true";
                        hasMismatch = "false";
                        break;
                    case "NonCompliantHasMismatch":
                        nonCompliant = "true";
                        hasMismatch = "true";
                        break;
                }

            }
        }
        public void CheckedArchivedValue(string IsCheckedOrArchived, out string isArchived, out string isChecked)
        {
            isArchived = null;
            isChecked = null;
            if (!string.IsNullOrEmpty(IsCheckedOrArchived))
            {
                switch (IsCheckedOrArchived)
                {
                    case "isChecked":
                        isChecked = "true";
                        break;
                    case "isNotChecked":
                        isChecked = "false";
                        break;
                    case "isArchived":
                        isArchived = "true";
                        break;
                }

            }
        }

        private async Task BuildDocumentReceipt(
            string SzRtDocumentId, string SzRtServerId,
            int LRetailStoreId, int LStoreGroupId)
        {
            _logger.LogDebug("Start: Get transaction document.");

            var document = (await _transactionRepository.GetTransaction(SzRtDocumentId, SzRtServerId,
                LRetailStoreId, LStoreGroupId)).TransactionDocument.FirstOrDefault(x => x.LDocumentTypeId == 2);
            _logger.LogDebug($"END: Document:{document}.");

        }


    }
}