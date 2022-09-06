using DnSrtChecker.Models.ViewModels;
using DnSrtChecker.Models;
using DnSrtChecker.ModelsHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnSrtChecker.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.CompilerServices;
using System.Net;
using Microsoft.EntityFrameworkCore.Internal;
using System.Data.Entity.SqlServer;
using Microsoft.AspNetCore.Authentication;
using RazorEngineCore;
using System.ComponentModel;
using System.Linq.Expressions;
using DnSrtChecker.FiltersmodelBindRequest;
using System.Web.Mvc;
using DnSrtChecker.Models.ViewModels.ModelsFunctions;

namespace DnSrtChecker.Persistence
{
    public class TransactionRepository : ITransactionRepository
    {
        public readonly RT_ChecksContext _dbContext;
        private readonly ILogger _logger;
        private readonly IOptions<Properties> _properties;

        public TransactionRepository(RT_ChecksContext dbContext,
                                     ILogger<TransactionRepository> logger,
                                      IOptions<Properties> properties)
        {
            _dbContext = dbContext;
            _logger = logger;
            _properties = properties;
        }

        public bool AddTransaction(TransactionAffiliation transaction)
        {
            try
            {
                _dbContext.TransactionAffiliation.Add(transaction);
                _logger.LogDebug($"Transaction: {transaction.SzRtServerId} added successfully");
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error adding a new Transaction {transaction.SzRtDocumentId}  :{e.Message}");

                return false;
            }
        }


        public async Task<TransactionAffiliation> GetTransaction(string SzRtDocumentId, string SzRtServerId, 
            int LRetailStoreId, int LStoreGroupId)
        {
            //var baseQ = _dbContext.TransactionAffiliation.Where(x => x.LPosTaNmbr.Value.ToString() == transactionAffiliationId
            //                                                    && x.SzRtServerId == rtServerId
            //                                                   && x.LRetailStoreId == retailStoreId
            //                                                    && x.LStoreGroupId == storeGroupId
            //                                                    );

            var baseQ = _dbContext.TransactionAffiliation.Where(x => x.SzRtDocumentId == SzRtDocumentId
                                                    && x.SzRtServerId == SzRtServerId
                                                                && x.LRetailStoreId == LRetailStoreId
                                                                && x.LStoreGroupId == LStoreGroupId                                                                
                                                                );
            var res = baseQ.FirstOrDefault();
            await baseQ.AsQueryable().Include(x => x.RtServer).ThenInclude(x => x.TransactionRtError).LoadAsync();
            await baseQ.AsQueryable().Include(x => x.RtServer).ThenInclude(x => x.L).ThenInclude(x => x.LStoreGroup).LoadAsync();
            await baseQ.AsQueryable().Include(x => x.LTransactionMismatch).LoadAsync();
            await baseQ.AsQueryable().Include(x => x.TransactionDocument).LoadAsync();
            await baseQ.AsQueryable().Include(x => x.TransactionVat).LoadAsync();
            return res;
        }

        public Task<List<TransactionAffiliation>> ListTransaction()
        {
            return _dbContext.TransactionAffiliation
                                .Include(s => s.RtServer)
                                        .ThenInclude(err => err.TransactionRtError)
                                .Include(tm => tm.LTransactionMismatch)
                                .Include(td => td.TransactionDocument)
                                .Include(tv => tv.TransactionVat)
                                .ToListAsync();
        }

        public async Task<List<TransactionList>> GetTransactions(string UserName, string RtDeviceClosureDateTime, 
            string ServerRt, string RtDeviceID, string Store, string PosWorkstationNmbr,
            string TransactionCheckedFlag, string TransactionArchivedFlag,
            string HasMismatch, string RtNonCompliantFlag, string PosTaNmbr, 
            string RtClosureNmbr, string RtDocumentNmbr)
        {
            var transactionsList = new List<TransactionList>();
            try
            {                
                transactionsList = _dbContext.TransactionsByDate(UserName, RtDeviceClosureDateTime, ServerRt,
                RtDeviceID, Store, PosWorkstationNmbr, TransactionCheckedFlag,
                TransactionArchivedFlag, HasMismatch, RtNonCompliantFlag, PosTaNmbr, RtClosureNmbr, RtDocumentNmbr)
                .ToList();
            }
            catch (Exception)
            {

                throw;
            }
            
            return transactionsList;
            //TransactionsByDate(string RtDeviceClosureDateTime, string ServerRt,
            //string RtDeviceID, string Store, string PosWorkstationNmbr,
            //string TransactionCheckedFlag, string TransactionArchivedFlag, string UserName)
        }
        public Task<List<TransactionAffiliation>> ListTransactionsByServerRt(RtServer rtServer)
        {
            return _dbContext.TransactionAffiliation
                                .Where(srt => srt.SzRtServerId.Contains(rtServer.SzRtServerId)
                                          && srt.LRetailStoreId == rtServer.LRetailStoreId
                                          && srt.LStoreGroupId == rtServer.LStoreGroupId)
                                .Distinct().ToListAsync();
        }

        public Task<List<TransactionAffiliation>> ListTransactionByServerRt(RtServer rtServer)
        {
            return _dbContext.TransactionAffiliation
                                .Include(s => s.RtServer).ThenInclude(err => err.TransactionRtError)
                                .Include(tm => tm.LTransactionMismatch)
                                .Include(td => td.TransactionDocument)
                                .Include(tv => tv.TransactionVat)
                                .Where(srt => srt.SzRtServerId == rtServer.SzRtServerId)
                                .ToListAsync();
        }
        //For InitilaistNonCompliant
        public Task<List<TransactionAffiliation>> ListTransactionByServerRtTrn(RtServer rtServer)
        {
            return _dbContext.TransactionAffiliation
                                .Include(s => s.RtServer).ThenInclude(err => err.TransactionRtError)
                                .Where(srt => srt.SzRtServerId == rtServer.SzRtServerId
                                            && srt.LRetailStoreId == rtServer.LRetailStoreId
                                            && srt.LStoreGroupId == rtServer.LStoreGroupId)
                                .ToListAsync();
        }
        public Task<List<TransactionAffiliation>> ListTransactionNonCompliant()
        {
            return _dbContext.TransactionAffiliation
                                .Include(s => s.RtServer)
                                .Where(x => ((x.BRtNonCompliantFlag.HasValue && x.BRtNonCompliantFlag == true)
                                || (x.LTransactionMismatchId.HasValue && (x.LTransactionMismatchId == 1 || x.LTransactionMismatchId == 2)))
                                && ((!x.BTransactionCheckedFlag.HasValue || x.BTransactionCheckedFlag == false))
                                ).ToListAsync();
        }


        //public List<RtServer> ListRtServerNonCompliant()
        //{

        //    var errTable = _properties.Value.TransactionErrorTable.Select(x => x.Value).ToList();
        //    var tr = _dbContext.TransactionRtError.Include(x => x.RtServer).ToList()
        //                    .Where(x => errTable.Any(er => x.SzDescription.Contains(er)))
        //                    .Select(x => x.RtServer).Distinct().AsEnumerable();
        //    var TrnErrorTable = tr.Select(x => x.SzRtServerId).AsEnumerable();
        //    //var listRes = _dbContext.RtServer
        //    //                             .Include(x => x.RtServerStatus)
        //    //                             .Where(x => x.RtServerStatus.BWarningFlag == true)
        //    //                             .ToList();
        //    var baseQ = _dbContext.TransactionAffiliation
        //                        //.Include(s => s.RtServer)
        //                        //.ThenInclude(x => x.RtServerStatus)
        //                        // .ThenInclude(er => er.TransactionRtError)
        //                        .Where(x => (
        //                                (
        //                                                                        x.LTransactionMismatchId == 1
        //                                                                        ||
        //                                                                        x.LTransactionMismatchId == 2
        //                                                                        ||
        //                                                                        (x.LTransactionMismatchId == 3 && x.DRtDateTime.Value.Date < DateTime.Today)
        //                                                                        ||
        //                                                                         (x.LTransactionMismatchId == 4 && x.DPosDateTime.Value.Date < DateTime.Today)
        //                                                                         ||
        //                                                                         ((x.DPosTransactionTurnover.HasValue && x.DRtTransactionTurnover.HasValue)
        //                                                                            && x.DRtTransactionTurnover * (x.LRtReceivedTransactionCounter ?? 1) != x.DPosTransactionTurnover * (x.LPosReceivedTransactionCounter ?? 1))
        //                                                                        ||
        //                                                                        (TrnErrorTable.Any(er => er == x.SzRtServerId))
        //                                                                            )
        //                        && (x.BTransactionCheckedFlag != true)
        //                        )
        //                        ).Select(x => x.RtServer).Distinct();
        //    var listMismatch = baseQ.AsEnumerable();


        //    var listErrorTmp = _dbContext.TransactionAffiliation
        //                        // .Include(s => s.RtServer)
        //                        //.ThenInclude(er => er.TransactionRtError)
        //                        .Where(t => t.BRtNonCompliantFlag != false && t.BTransactionCheckedFlag != true
        //                        )
        //                        .Select(x => x.RtServer).Distinct().AsEnumerable();

        //    var listError = listErrorTmp.Intersect(tr).AsEnumerable();//.ToList();

        //    var listRes = listMismatch.Union(listError).ToList();
        //    return listRes;
        //}

        public List<RtServer> ListRtServerNonCompliant()
        {

            var listRes = _dbContext.RtServer
                .Include(x => x.RtServerStatus)
                .Where(x => x.RtServerStatus.BWarningFlag == true).ToList();
            return listRes;
        }



        public async Task<List<TransactionByServerRtForTransmissions>> ListTransactionServerRt(string rtServerId, int storeId, int storeGroupId)
        {
            var baseQ = _dbContext.TransactionAffiliation
                                  .Include(x => x.TransactionVat).ThenInclude(x => x.SzVatCode)
                .Where(x => x.SzRtServerId == rtServerId
                    && x.DPosDateTime >= DateTime.Today.AddMonths(-1));
            var list = baseQ;
            // baseQ.Include(x => x.TransactionVat).ThenInclude(x=>x.SzVatCode).Load();

            var listRe = list.ToList()
                .Select(x => new
                {
                    x.SzRtServerId,
                    x.SzRtDeviceId,
                    x.LRtClosureNmbr,
                    x.DPosDateTime,
                    x.DPosTransactionTurnover,
                    x.LPosReceivedTransactionCounter,
                    x.DRtTransactionTurnover,
                    x.DRtDateTime,
                    x.LRtReceivedTransactionCounter,
                    x.LPosWorkstationNmbr,
                    x.BTransactionCheckedFlag,
                    TransactionVats = x.TransactionVat.ToList()
                });

            listRe.ToList().ForEach(x =>
            {
                x.TransactionVats.ForEach(v => { v.DPosGrossAmount = v.DPosGrossAmount * x.LPosReceivedTransactionCounter; });
            });
            var listRes = listRe.GroupBy(x => new
            {
                DPosDateTime = x.DPosDateTime != null ? x.DPosDateTime.Value.Date : x.DRtDateTime.Value.Date,
                szRtDeviceId = x.SzRtDeviceId,
                lRtClosureNmbr = x.LRtClosureNmbr,
                DRtDateTime = x.DRtDateTime != null ? x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date,

            },
                 (key, group) => new TransactionByServerRtForTransmissions
                 {
                     RtServerId = rtServerId,
                     RtDeviceId = key.szRtDeviceId,
                     ClosureNumber = key.lRtClosureNmbr,
                     DRtDateTime = key.DRtDateTime,
                     DPosDateTime = key.DPosDateTime,
                     LPosWorkstationNmbr = group.FirstOrDefault().LPosWorkstationNmbr.ToString(),
                     TotalTP = group.Sum(x => x.DPosTransactionTurnover * (x.LPosReceivedTransactionCounter ?? 1)),
                     TotalServerRT = group.Sum(x => x.DRtTransactionTurnover * (x.LRtReceivedTransactionCounter ?? 1)),
                     TransactionVatsEsen = group.ToList().SelectMany(x => x.TransactionVats).Where(x => x.SzVatCodeId == "ES ")
                                       .GroupBy(x => x.SzVatCode?.SzVatNature)
                                       .Select(x => new TransactionVatForTrasmissionsViewModel
                                       {
                                           SzVatNature = x.Key,
                                           DPosVatRate = x.FirstOrDefault().DPosVatRate,
                                           DPosGrossAmount = x.Sum(x => x.DPosGrossAmount),
                                           DRtGrossAmount = x.Sum(x => x.DRtGrossAmount)

                                       }
                                       ).ToList(),
                     TransactionVats = group.AsQueryable()
                                           .SelectMany(x => x.TransactionVats).Where(x => x.SzVatCodeId != "ES ")
                                           .AsQueryable()
                                           .GroupBy(x => x.DPosVatRate)
                                           .Select(x =>
                                                new TransactionVatForTrasmissionsViewModel
                                                {
                                                    DPosVatRate = x.Key,
                                                    DRtGrossAmount = x.Sum(x => x.DRtGrossAmount),
                                                    DPosGrossAmount = x.Sum(x => x.DPosGrossAmount)
                                                })
                                       .ToList()
                 })
               .Where(x => x.DRtDateTime < DateTime.Today && x.DPosDateTime < DateTime.Today)
               .ToList();

            listRes.ForEach(x =>
            {
                x.TransactionVats = x.TransactionVats.Union(x.TransactionVatsEsen).Distinct().ToList();
            });


            return listRes;

        }
        public async Task<List<TransactionByServerRtForTransmissions>> ListTransactionServerRtOfDay(string rtServerId, int storeId, int storeGroupId, DateTime? date)
        {
            var baseQ = _dbContext.TransactionAffiliation
                                  .Include(x => x.TransactionVat)
                                  .ThenInclude(x => x.SzVatCode)
                .Where(x => x.SzRtServerId == rtServerId
                    && x.DPosDateTime.Value.Date == date.Value.Date)
                .Select(x => new TransactionAffiliation
                {
                    SzRtDocumentId = x.SzRtDocumentId,
                    SzRtServerId = x.SzRtServerId,
                    SzRtDeviceId = x.SzRtDeviceId,
                    LRtClosureNmbr = x.LRtClosureNmbr,
                    DPosDateTime = x.DPosDateTime,
                    DPosTransactionTurnover = x.DPosTransactionTurnover,
                    LPosReceivedTransactionCounter = x.LPosReceivedTransactionCounter,
                    DRtTransactionTurnover = x.DRtTransactionTurnover,
                    DRtDateTime = x.DRtDateTime,
                    LRtReceivedTransactionCounter = x.LRtReceivedTransactionCounter,
                    LPosWorkstationNmbr = x.LPosWorkstationNmbr,
                    BTransactionCheckedFlag = x.BTransactionCheckedFlag,
                    TransactionVat = x.TransactionVat.Select(v =>
                                                               new TransactionVat
                                                               {
                                                                   DPosGrossAmount = v.DPosGrossAmount * x.LPosReceivedTransactionCounter,
                                                                   DPosNetAmount = v.DPosNetAmount,
                                                                   DPosVatAmount = v.DPosVatAmount,
                                                                   DPosVatRate = v.DPosVatRate,
                                                                   SzVatCode = v.SzVatCode,
                                                                   SzVatCodeId = v.SzVatCodeId,
                                                                   DRtGrossAmount = v.DRtGrossAmount,


                                                               }
                                                                ).ToList()
                }).ToList();



            var listRes = baseQ.GroupBy(x => new
            {
                DPosDateTime = x.DPosDateTime != null ? x.DPosDateTime.Value.Date : x.DRtDateTime.Value.Date,
                szRtDeviceId = x.SzRtDeviceId,
                lRtClosureNmbr = x.LRtClosureNmbr,
                DRtDateTime = x.DRtDateTime != null ? x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date,
            },
                 (key, group) => new TransactionByServerRtForTransmissions
                 {
                     RtServerId = rtServerId,
                     RtDeviceId = key.szRtDeviceId,
                     ClosureNumber = key.lRtClosureNmbr,
                     DRtDateTime = key.DRtDateTime,
                     DPosDateTime = key.DPosDateTime,
                     LPosWorkstationNmbr = group.FirstOrDefault().LPosWorkstationNmbr.ToString(),
                     TotalTP = group.Sum(x => x.DPosTransactionTurnover * (x.LPosReceivedTransactionCounter ?? 1)),
                     TotalServerRT = group.Sum(x => x.DRtTransactionTurnover * (x.LRtReceivedTransactionCounter ?? 1)),
                     TransactionVatsEsen = group.AsQueryable().SelectMany(x => x.TransactionVat).Where(x => x.SzVatCodeId == "ES " && x.SzVatCode != null)
                                       .GroupBy(x => x.SzVatCode.SzVatNature)
                                       .Select(x => new TransactionVatForTrasmissionsViewModel
                                       {
                                           SzVatNature = x.Key,
                                           DPosVatRate = x.FirstOrDefault().DPosVatRate,
                                           DPosGrossAmount = x.Sum(x => x.DPosGrossAmount),
                                           DRtGrossAmount = x.Sum(x => x.DRtGrossAmount)

                                       }
                                       ).ToList(),
                     TransactionVats = group.AsQueryable()
                                           .SelectMany(x => x.TransactionVat).Where(x => x.SzVatCodeId != "ES ")
                                           .AsQueryable()
                                           .GroupBy(x => x.DPosVatRate)
                                           .Select(x =>
                                                new TransactionVatForTrasmissionsViewModel
                                                {
                                                    DPosVatRate = x.Key,
                                                    DRtGrossAmount = x.Sum(x => x.DRtGrossAmount),
                                                    DPosGrossAmount = x.Sum(x => x.DPosGrossAmount)
                                                })
                                       .ToList()
                 })
               .Where(x => x.DRtDateTime < DateTime.Today && x.DPosDateTime < DateTime.Today)
               .ToList();

            listRes.ForEach(x =>
            {
                x.TransactionVats = x.TransactionVats.Union(x.TransactionVatsEsen).Distinct().ToList();
            });

            //var baseQ2 = _dbContext.TransactionAffiliation.Where(x => x.DPosDateTime.Value.Date >= DateTime.Today.AddMonths(-1)
            //                                                     && x.SzRtServerId == rtServerId).ToList();
            //var listGlobal = baseQ2.GroupBy(x => new
            //{
            //    DPosDateTime = x.DPosDateTime != null ? x.DPosDateTime.Value.Date : x.DRtDateTime.Value.Date,
            //    szRtDeviceId = x.SzRtDeviceId,
            //    lRtClosureNmbr = x.LRtClosureNmbr,
            //    DRtDateTime = x.DRtDateTime != null ? x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date,
            //},
            //    (key, group) => new TransactionByServerRtForTransmissions
            //    {
            //        RtServerId = rtServerId,
            //        RtDeviceId = key.szRtDeviceId,
            //        ClosureNumber = key.lRtClosureNmbr,
            //        DRtDateTime = key.DRtDateTime,
            //        DPosDateTime = group.Max(x => x.DPosDateTime).Value.Date,
            //        LPosWorkstationNmbr = group.FirstOrDefault().LPosWorkstationNmbr.ToString(),
            //        TotalTP = group.Sum(x => x.DPosTransactionTurnover * (x.LPosReceivedTransactionCounter ?? 1)),
            //        TotalServerRT = group.Sum(x => x.DRtTransactionTurnover * (x.LRtReceivedTransactionCounter ?? 1)),

            //    })
            //  .Where(x => x.DRtDateTime < DateTime.Today && x.DPosDateTime < DateTime.Today)
            //  .ToList();
            //listGlobal.ForEach(x =>
            //{
            //    var found = listRes.FirstOrDefault(s => s.RtDeviceId == x.RtDeviceId && s.ClosureNumber == x.ClosureNumber);
            //    if (found != null)
            //    {
            //        x.TransactionVats = found.TransactionVats;

            //    }
            //});

            //return listGlobal;
            return listRes;
        }

        public async Task<List<TransactionByServerRtForTransmissions>> ListTransactionServerRtMonth(string rtServerId, int storeId, int storeGroupId, DateTime? date, int nmbrDays)
        {
            var dateInit = date.Value.Date;
            var dateEnd = date.Value.AddDays(1).Date;
            var baseQ = _dbContext.TransactionAffiliation
                .Where(x => x.SzRtServerId == rtServerId
                    && x.DPosDateTime >= dateInit
                    && x.DPosDateTime < dateEnd
                    )
                .Select(x => new TransactionAffiliation
                {
                    SzRtServerId = x.SzRtServerId,
                    SzRtDeviceId = x.SzRtDeviceId,
                    LRtClosureNmbr = x.LRtClosureNmbr,
                    DPosDateTime = x.DPosDateTime ?? x.DRtDateTime,
                    DPosTransactionTurnover = x.DPosTransactionTurnover * (x.LPosReceivedTransactionCounter ?? 1),
                    LPosReceivedTransactionCounter = x.LPosReceivedTransactionCounter,
                    DRtTransactionTurnover = x.DRtTransactionTurnover * (x.LRtReceivedTransactionCounter ?? 1),
                    DRtDateTime = x.DRtDateTime ?? x.DPosDateTime,
                    LRtReceivedTransactionCounter = x.LRtReceivedTransactionCounter,
                    LPosWorkstationNmbr = x.LPosWorkstationNmbr,
                    BTransactionCheckedFlag = x.BTransactionCheckedFlag,
                    TransactionVat = x.TransactionVat.Select(v =>
                                                               new TransactionVat
                                                               {
                                                                   DPosGrossAmount = v.DPosGrossAmount * x.LPosReceivedTransactionCounter,
                                                                   DPosNetAmount = v.DPosNetAmount,
                                                                   DPosVatAmount = v.DPosVatAmount,
                                                                   DPosVatRate = v.DPosVatRate,
                                                                   SzVatCode = v.SzVatCode,
                                                                   SzVatCodeId = v.SzVatCodeId,
                                                                   DRtGrossAmount = v.DRtGrossAmount,
                                                               }
                                                                ).ToList()
                }).ToList();



            var listRes = baseQ.Distinct().GroupBy(x => new
            {
                DPosDateTime = x.DPosDateTime.Value.Date,// != null ? x.DPosDateTime.Value.Date : x.DRtDateTime.Value.Date,
                szRtDeviceId = x.SzRtDeviceId,
                lRtClosureNmbr = x.LRtClosureNmbr,
                DRtDateTime = x.DRtDateTime.Value.Date,// != null ? x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date,
            },
                 (key, group) => new TransactionByServerRtForTransmissions
                 {
                     RtServerId = rtServerId,
                     RtDeviceId = key.szRtDeviceId,
                     ClosureNumber = key.lRtClosureNmbr,
                     DRtDateTime = key.DRtDateTime,
                     DPosDateTime = key.DPosDateTime,
                     LPosWorkstationNmbr = group.FirstOrDefault().LPosWorkstationNmbr.ToString(),
                     //TotalTP = group.Sum(x => x.DPosTransactionTurnover * (x.LPosReceivedTransactionCounter ?? 1)),
                     TotalTP = group.Sum(x => x.DPosTransactionTurnover),
                     // TotalServerRT = group.Sum(x => x.DRtTransactionTurnover * (x.LRtReceivedTransactionCounter ?? 1)),
                     TotalServerRT = group.Sum(x => x.DRtTransactionTurnover),
                     TransactionVatsEsen = group.AsQueryable().SelectMany(x => x.TransactionVat).Where(x => x.SzVatCodeId == "ES " && x.SzVatCode != null)
                                       .GroupBy(x => x.SzVatCode.SzVatNature)
                                       .Select(x => new TransactionVatForTrasmissionsViewModel
                                       {
                                           SzVatNature = x.Key,
                                           DPosVatRate = x.FirstOrDefault().DPosVatRate,
                                           DPosGrossAmount = x.Sum(x => x.DPosGrossAmount),
                                           DRtGrossAmount = x.Sum(x => x.DRtGrossAmount)
                                       }
                                       ),
                     TransactionVats = group.AsQueryable()
                                           .SelectMany(x => x.TransactionVat).Where(x => x.SzVatCodeId != "ES ")
                                           .AsQueryable()
                                           .GroupBy(x => x.DPosVatRate)
                                           .Select(x =>
                                                new TransactionVatForTrasmissionsViewModel
                                                {
                                                    DPosVatRate = x.Key,
                                                    DRtGrossAmount = x.Sum(x => x.DRtGrossAmount),
                                                    DPosGrossAmount = x.Sum(x => x.DPosGrossAmount)
                                                })

                 })
               .Where(x => x.DRtDateTime < DateTime.Today && x.DPosDateTime < DateTime.Today)
               .Distinct().ToList()
               ;
            //for (var i = 0; i < listRes.Count(); i++)
            //{
            //    var listVats = listRes[i].TransactionVats.Union(listRes[i].TransactionVatsEsen).Distinct().ToList();
            //    listRes[i].TransactionVats = listVats;
            //}
            listRes.ForEach(x =>
            {
                x.TransactionVats = x.TransactionVats.Union(x.TransactionVatsEsen).Distinct().ToList();
            });
            var initDate2 = date.Value.AddDays(-nmbrDays);
            var endDate2 = date.Value.Date;
            var baseQ2 = _dbContext.TransactionAffiliation.Where(x => x.DPosDateTime.Value.Date >= initDate2//.AddMonths(-1)
                                                                && x.DPosDateTime.Value.Date < endDate2
                                                                && x.SzRtServerId == rtServerId)
                                    .Select(x => new TransactionAffiliation
                                    {
                                        SzRtServerId = x.SzRtServerId
                                   ,
                                        LRetailStoreId = x.LRetailStoreId
                                   ,
                                        LStoreGroupId = x.LStoreGroupId
                                   ,
                                        LTransactionMismatchId = x.LTransactionMismatchId
                                   ,
                                        BRtNonCompliantFlag = x.BRtNonCompliantFlag
                                   ,
                                        BTransactionArchivedFlag = x.BTransactionArchivedFlag
                                   ,
                                        BTransactionCheckedFlag = x.BTransactionCheckedFlag
                                   ,
                                        DPosDateTime = x.DPosDateTime ?? x.DRtDateTime
                                   ,
                                        DRtDateTime = x.DRtDateTime ?? x.DPosDateTime
                                   ,
                                        DPosTransactionTurnover = x.DPosTransactionTurnover * (x.LPosReceivedTransactionCounter ?? 1)
                                   ,
                                        DRtTransactionTurnover = x.DRtTransactionTurnover * (x.LRtReceivedTransactionCounter ?? 1)
                                   ,
                                        LPosTaNmbr = x.LPosTaNmbr
                                   ,
                                        LPosWorkstationNmbr = x.LPosWorkstationNmbr
                                   ,
                                        LRtClosureNmbr = x.LRtClosureNmbr
                                   ,
                                        LRtDocumentNmbr = x.LRtDocumentNmbr
                                   ,
                                        LTransactionMismatch = x.LTransactionMismatch
                                   ,
                                        SzRtDeviceId = x.SzRtDeviceId
                                   ,
                                        SzTranscationCheckNote = x.SzTranscationCheckNote
                                   ,
                                        SzUserName = x.SzUserName

                                    })
                                    .ToList();
            var listGlobal = baseQ2.GroupBy(x => new
            {
                DPosDateTime = x.DPosDateTime.Value.Date,//DPosDateTime= x.DPosDateTime != null ? x.DPosDateTime.Value.Date : x.DRtDateTime.Value.Date,
                szRtDeviceId = x.SzRtDeviceId,
                lRtClosureNmbr = x.LRtClosureNmbr,
                DRtDateTime = x.DRtDateTime.Value.Date,//DRtDateTime= x.DRtDateTime != null ? x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date,
            },
                (key, group) => new TransactionByServerRtForTransmissions
                {
                    RtServerId = rtServerId,
                    RtDeviceId = key.szRtDeviceId,
                    ClosureNumber = key.lRtClosureNmbr,
                    DRtDateTime = key.DRtDateTime,
                    DPosDateTime = group.Max(x => x.DPosDateTime).Value.Date,
                    LPosWorkstationNmbr = group.FirstOrDefault().LPosWorkstationNmbr.ToString(),
                    TotalTP = group.Sum(x => x.DPosTransactionTurnover),
                    TotalServerRT = group.Sum(x => x.DRtTransactionTurnover),

                })
              .Where(x => x.DRtDateTime < DateTime.Today && x.DPosDateTime < DateTime.Today)
              .ToList()
              ;
            //listGlobal.ForEach(x =>
            //{
            //    var found = listRes.FirstOrDefault(s => s.RtDeviceId == x.RtDeviceId && s.ClosureNumber == x.ClosureNumber);
            //    if (found != null)
            //    {
            //        x.TransactionVats = found.TransactionVats;

            //    }
            //});
            listGlobal = listGlobal.Union(listRes).OrderBy(x => x.DPosDateTime).ToList();
            var test = listGlobal.OrderByDescending(x => x.DPosDateTime);
            return listGlobal.ToList();
            // return listRes;
        }
        public async Task<List<TransactionByServerRtForTransmissions>> ListTransactionServerRtMonth22(string rtServerId, int storeId, int storeGroupId, DateTime? date, int nmbrDays)
        {
            //
            var dateInit = date.Value.Date;
            var trnTotalsMonth = _dbContext.trnJustTotalsResult(rtServerId,
                dateInit.AddDays(-30).Date.ToString("yyyy-MM-dd"),
                dateInit.AddDays(1).Date.ToString("yyyy-MM-dd"))
                .ToList()
                .GroupBy(x => new { x.DPosDateTime.Value.Date, x.SzRtDeviceId })
                .Select(x => new TransactionByServerRtForTransmissions
                {
                    DPosDateTime = x.Key.Date,
                    RtDeviceId = x.Key.SzRtDeviceId,
                    ClosureNumberS = string.Join(" / ", x.Select(x => x.LRtClosureNmbr).Distinct()),
                    RtServerId = rtServerId,
                    TotalServerRT = x.Sum(x => x.TotalRT),
                    TotalTP = x.Sum(x => x.TotalTP)
                }).OrderByDescending(x => x.DPosDateTime)
                .AsEnumerable();
            var trnDetailLastDay = _dbContext.trnTotalsResult(rtServerId,
                                     dateInit.Date.ToString("yyyy-MM-dd"),
                                     dateInit.AddDays(1).Date.ToString("yyyy-MM-dd"))
                                .ToList()
                .GroupBy(x => new { x.DPosDateTime.Value.Date, x.SzRtDeviceId, x.SzVatNature })
                .Select(x => new TransactionByServerRtForTransmissions
                {
                    DPosDateTime = x.Key.Date,
                    RtDeviceId = x.Key.SzRtDeviceId,
                    RtServerId = rtServerId,
                    TransactionVats = x.Where(x => x.SzVatCodeId.ToLower().Trim() != "es")
                                     .GroupBy(x => x.DPosVatRate)
                                     .Select(x =>
                                         new TransactionVatForTrasmissionsViewModel
                                         {
                                             DPosVatRate = x.Key,
                                             DRtGrossAmount = x.Sum(x => x.DRtGrossAmountTotale),
                                             DPosGrossAmount = x.Sum(x => x.DPosGrossAmountTotale)
                                         }),
                    TransactionVatsEsen = x.Where(x => x.SzVatCodeId.ToLower().Trim() == "es" && x.SzVatCodeId != null)
                                       .GroupBy(x => x.SzVatNature)
                                       .Select(x => new TransactionVatForTrasmissionsViewModel
                                       {
                                           SzVatNature = x.Key,
                                           DPosVatRate = x.FirstOrDefault().DPosVatRate,
                                           DPosGrossAmount = x.Sum(x => x.DPosGrossAmountTotale),
                                           DRtGrossAmount = x.Sum(x => x.DRtGrossAmountTotale)
                                       }
                                       )
                })
                .GroupBy(x=>new { x.RtDeviceId,x.DPosDateTime.Value.Date})
                .Select(x=>new TransactionByServerRtForTransmissions
                {
                    DPosDateTime = x.Key.Date,
                    RtDeviceId = x.Key.RtDeviceId,
                    RtServerId = rtServerId,
                    TransactionVats = x.SelectMany(x=>x.TransactionVats).Union(x.SelectMany(x=>x.TransactionVatsEsen)),
                    TotalServerRT=x.Sum(x=>x.TotalServerRT),
                    TotalTP=x.Sum(x=>x.TotalTP)
                    
                })
                ;
            //foreach (var t in trnDetailLastDay)
            //{
            //    var foundtrn = trnDetailLastDay.Where(x => x.DPosDateTime.Value.Date == t.DPosDateTime.Value.Date
            //                        && x.RtDeviceId == t.RtDeviceId
            //                    );
            //    if (foundtrn != null)
            //    {
            //        t.TotalServerRT = trnDetailLastDay.Sum(x => x.TotalServerRT);
            //        t.TotalTP = trnDetailLastDay.Sum(x => x.TotalTP);
            //    }
            //    t.TransactionVats = t.TransactionVats.Union(t.TransactionVatsEsen);

            //}
            var listRes = trnTotalsMonth.Union(trnDetailLastDay);
            return listRes.ToList();
        }
        public List<TransactionByServerRtForTransmissions> ListTransactionServerRtAll(IEnumerable<SrvLastClosureDate> listdate)
        {


            var days = -Math.Abs(_properties.Value.HomeNmbrDays);

            //var listLastClosure = _dbContext.TransactionAffiliation
            //    .Where(x => x.DPosDateTime != null && x.DPosDateTime < DateTime.Today.Date)
            //                .AsQueryable()
            //                    .GroupBy(x =>new { x.SzRtServerId},(key,group)=>
            //                    new 
            //                    { 
            //                    srv=key.SzRtServerId,
            //                    MaxDposdatetime=group.Max(x=>x.DPosDateTime.Value.Date).Date
            //                    })
            //                    .AsEnumerable();

            var list = _dbContext.TransactionAffiliation
                                  //.Where(x => listLastClosure.Any(t => t.srv == x.SzRtServerId
                                  //    && t.MaxDposdatetime.Date == x.DPosDateTime.Value.Date))

                                  .Where(x =>
                                  listdate.AsEnumerable().Any(t => t.Srv == x.SzRtServerId && t.LastDate.Date == x.DPosDateTime.Value.Date)

                                  )
                                  //|| t.LastDate.Date == x.DRtDateTime.Value.Date)))
                                  .Select(x => new TransactionAffiliation
                                  {

                                      SzRtServerId = x.SzRtServerId,
                                      SzRtDeviceId = x.SzRtDeviceId,
                                      DPosDateTime = x.DPosDateTime,
                                      DPosTransactionTurnover = (x.DPosTransactionTurnover ?? 0) * (x.LPosReceivedTransactionCounter ?? 1),
                                      LPosReceivedTransactionCounter = x.LPosReceivedTransactionCounter,
                                      DRtDateTime = x.DRtDateTime,
                                      DRtTransactionTurnover = (x.DRtTransactionTurnover ?? 0) * (x.LRtReceivedTransactionCounter ?? 1),
                                      LRtReceivedTransactionCounter = x.LRtReceivedTransactionCounter
                                  }).AsEnumerable()
                                  .GroupBy(x => new
                                  {
                                      szRtServerId = x.SzRtServerId,
                                      szRtDeviceId = x.SzRtDeviceId,
                                      DPosDateTime = x.DPosDateTime != null ? x.DPosDateTime.Value.Date : x.DRtDateTime.Value.Date,
                                  },
                                                          (key, group)
                                     => new TransactionByServerRtForTransmissions
                                     {
                                         RtServerId = key.szRtServerId,
                                         RtDeviceId = key.szRtDeviceId,
                                         DPosDateTime = key.DPosDateTime,
                                         TotalTP = group.Sum(x => x.DPosTransactionTurnover),
                                         TotalServerRT = group.Sum(x => x.DRtTransactionTurnover)
                                     }).ToList();
            //var list = _dbContext.TransactionAffiliation
            //                  .Where(x => x.DPosDateTime != null
            //                  && x.DPosDateTime >= DateTime.Today.AddDays(days)
            //                  )
            //                  .AsNoTracking()
            //                  .AsQueryable()
            //                  .Select(x => new TransactionAffiliation
            //                  {
            //                      SzRtServerId = x.SzRtServerId,
            //                      SzRtDeviceId = x.SzRtDeviceId,
            //                      DPosDateTime = x.DPosDateTime,
            //                      DPosTransactionTurnover = x.DPosTransactionTurnover * x.LPosReceivedTransactionCounter ?? 1,
            //                      LPosReceivedTransactionCounter = x.LPosReceivedTransactionCounter,
            //                      DRtDateTime = x.DRtDateTime,
            //                      DRtTransactionTurnover = x.DRtTransactionTurnover * x.LRtReceivedTransactionCounter ?? 1,
            //                      LRtReceivedTransactionCounter = x.LRtReceivedTransactionCounter
            //                  })
            //                         .GroupBy(x => new
            //                         {
            //                             szRtServerId = x.SzRtServerId,
            //                             szRtDeviceId = x.SzRtDeviceId,
            //                             DPosDateTime = x.DPosDateTime != null ? x.DPosDateTime.Value.Date : x.DRtDateTime.Value.Date
            //                         },
            //                                              (key, group)
            //                         => new TransactionByServerRtForTransmissions
            //                         {
            //                             RtServerId = key.szRtServerId,
            //                             RtDeviceId = key.szRtDeviceId,
            //                             DPosDateTime = key.DPosDateTime,
            //                             TotalTP = group.Sum(x => x.DPosTransactionTurnover),
            //                             TotalServerRT = group.Sum(x => x.DRtTransactionTurnover)
            //                         }).Distinct().ToList();

            return list;
        }

        public bool RemoveTransaction(TransactionAffiliation transaction)
        {
            try
            {

                _logger.LogDebug($"Transaction: {transaction.SzRtDocumentId} is disabled successfully");
                _dbContext.TransactionAffiliation.Remove(transaction);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error adding a Transaction N° :{transaction.SzRtDocumentId}  :{e.Message}");

                return false;
            }
        }


        //public async Task<List<TrxRTServer>> ListServerWithTotalsTrnAndTrm()
        //{
        //    var days = -Math.Abs(_properties.Value.HomeNmbrDays);
        //    var listLastClosureDateBySrv = _dbContext.RtServerTransmissionDetail
        //        .Where(x => x.DRtDeviceClosureDateTime.Value.Date != DateTime.Now.Date)
        //        .Select(x => new { x.SzRtServerId, x.DRtDeviceClosureDateTime })
        //        .GroupBy(x => new { x.SzRtServerId }, (key, group) => new SrvLastClosureDate
        //        {
        //            Srv = key.SzRtServerId,
        //            LastDate = group.Max(x => x.DRtDeviceClosureDateTime.Value.Date),

        //        })
        //        .AsEnumerable();

        //    var listSrv = listLastClosureDateBySrv.ToList();
        //    var base1 = _dbContext.trxTotalsResult("it",
        //        DateTime.Today.AddDays(days).ToString("yyyy-MM-dd"), DateTime.Now.Date.ToString("yyyy-MM-dd")).AsEnumerable();
                                 



        //   var trm = base1.ToList();



        //   // //Commented 10/12/2020 da verificare
        //   // //var baseQ = _dbContext.RtServerTransmissionDetail
        //   // //                      .Where(x => x.DRtDeviceClosureDateTime.Value.Date != DateTime.Now.Date
        //   // //                               && x.DRtDeviceClosureDateTime >= DateTime.Today.AddDays(days)
        //   // //                    )
        //   // //                      .Select(x => new
        //   // //                      {
        //   // //                          SzRtServerId = x.SzRtServerId,
        //   // //                          BTransactionCheckedFlag = x.BTransactionCheckedFlag,
        //   // //                          DRtDeviceClosureDateTime = x.DRtDeviceClosureDateTime,
        //   // //                          SzRtDeviceId = x.SzRtDeviceId,
        //   // //                          totalADE = x.RtServerTransmissionDetailRtData.Sum(x => x.DSaleAmount + x.DVatAmount - x.DReturnAmount - x.DVoidAmount)
        //   // //                      }).AsEnumerable();

        //   // //var trm = baseQ
        //   // //.GroupBy(x => new { x.SzRtServerId }, (key, group)
        //   // //      => new
        //   // //      {
        //   // //          srv = key.SzRtServerId,
        //   // //          closureDate = group.Max(x => x.DRtDeviceClosureDateTime),
        //   // //          total =
        //   // //             group.Where(x => x.DRtDeviceClosureDateTime.Value.Date == group.Max(x => x.DRtDeviceClosureDateTime).Value.Date)
        //   // //             .GroupBy(x => new { x.DRtDeviceClosureDateTime.Value.Date }
        //   // //             )
        //   // //             .Select(t => new
        //   // //             {
        //   // //                 srv = t.Select(x => x.SzRtServerId).FirstOrDefault(),
        //   // //                 closureDate = t.FirstOrDefault().DRtDeviceClosureDateTime,
        //   // //                 IsChecked = t.Any(x => x.BTransactionCheckedFlag.HasValue && x.BTransactionCheckedFlag.Value),// t.Select(x => x.BTransactionCheckedFlag).ToList(),
        //   // //                         total = t.Sum(x => x.totalADE)// t.Sum(x => x.RtServerTransmissionDetailRtData.Sum(x => x.DSaleAmount + x.DVatAmount - x.DReturnAmount - x.DVoidAmount))

        //   // //                     }).ToList()


        //   // //      })
        //   // //.Select(x => new TransmissionsGroupedByDay
        //   // //{
        //   // //    SzRtServerId = x.srv,
        //   // //    OperationClosureDatetime = x.closureDate,
        //   // //    BTransactionCheckedFlag = x.total.Select(x => (bool?)x.IsChecked).ToList(),//x.total.SelectMany(x => x.IsChecked).ToList(),
        //   // //            TotalAmount = x.total.Sum(x => x.total)
        //   // //}).ToList();


        //   // //var trn22 = ListTransactionServerRtAll(listdate);
        //   // var trn = new List<TransactionByServerRtForTransmissions>();
        //   // var res = new List<TransactionByServerRtForTransmissions>();
        //   // for (var i = 0; i < listSrv.Count(); i++)
        //   // {
        //   //     var trnfound = _dbContext.trnJustTotalsResult(listSrv[i].Srv, listSrv[i].LastDate.Date.ToString("yyyy-MM-dd"), listSrv[i].LastDate.AddDays(1).Date.ToString("yyyy-MM-dd"))
        //   //         .Select(x =>
        //   //         new TransactionByServerRtForTransmissions
        //   //         {
        //   //             DPosDateTime = x.DPosDateTime,
        //   //             RtDeviceId = x.SzRtDeviceId,
        //   //             RtServerId = listSrv[i].Srv,
        //   //             TotalServerRT = x.TotalRT,
        //   //             TotalTP = x.TotalTP
        //   //         }
        //   //         ).AsEnumerable()
        //   //         ;
        //   //     res.AddRange(trnfound);

        //   // }

        //   // trn = res.GroupBy(x => new { x.RtServerId, x.DPosDateTime }, (key, group)
        //   //                                 => new TransactionByServerRtForTransmissions
        //   //                                 {
        //   //                                     DPosDateTime = key.DPosDateTime,
        //   //                                     RtServerId = key.RtServerId,
        //   //                                     TotalServerRT = group.Sum(x => x.TotalServerRT),
        //   //                                     TotalTP = group.Sum(x => x.TotalTP)
        //   //                                 }).ToList()
        //   //                              ;
        //   // var TRXcheckwithFlagChecked = _dbContext.RtServerTransmissionDetail
        //   //.Where(x => listLastClosureDateBySrv.Any(t => x.SzRtServerId == t.Srv && x.DRtDeviceClosureDateTime.Value.Date == t.LastDate.Date))
        //   //.Select(x => new { x.SzRtServerId, x.DRtDeviceClosureDateTime.Value.Date, x.BTransactionCheckedFlag })
        //   //.AsEnumerable()
        //   //.GroupBy(x => new { x.SzRtServerId, x.Date }).Select(x => new
        //   //{
        //   //    SzRtServerId = x.Key.SzRtServerId,
        //   //    Date = x.Key.Date,
        //   //    checkedFlag = x.Select(x => x.BTransactionCheckedFlag)
        //   //}).ToList();
        //   // // Matching TRN and TRX by Server
        //   // //trm.ForEach(t =>
        //   // //{

        //   // //    t.Total = 0.0m;
        //   // //    t.tp = 0.0m;
        //   // //    var trnfound = trn.Where(x => x.RtServerId == t.SzRtServerId
        //   // //                                          && x.DPosDateTime.Value.Date == t.OperationClosureDatetime.Value.Date
        //   // //                                          ).ToList();

        //   // //    if (trnfound != null && trnfound.Count() > 0)
        //   // //    {

        //   // //        t.TotalRtServer = trnfound.Sum(x => x.TotalServerRT);
        //   // //        t.TotalTP = trnfound.Sum(x => x.TotalTP);
        //   // //    }

        //   // //    //t.BTransactionCheckedFlag = listSrv.Where(x => x.Srv == t.SzRtServerId && x.LastDate.Date == t.OperationClosureDatetime.Value.Date).SelectMany(x => x.TRXCheckedFlag).ToList();
        //   // //    t.BTransactionCheckedFlag = TRXcheckwithFlagChecked.Where(x => x.SzRtServerId == t.SzRtServerId && x.Date.Date == t.OperationClosureDatetime.Value.Date).SelectMany(x => x.checkedFlag).ToList();
        //   // //});
        //   // //trm = trm.GroupBy(x => new { x.SzRtServerId })
        //   // //    .Select(x => new TransmissionsGroupedByDay
        //   // //    {
        //   // //        SzRtServerId = x.Key.SzRtServerId,
        //   // //        OperationClosureDatetime = x.FirstOrDefault().OperationClosureDatetime.Value,
        //   // //        TotalAmount = x.Sum(x => x.TotalAmount),
        //   // //        TotalRtServer = x.Sum(x => x.TotalRtServer),
        //   // //        TotalTP = x.Sum(x => x.TotalTP),
        //   // //        BTransactionCheckedFlag = x.SelectMany(x => x.BTransactionCheckedFlag).ToList()
        //   // //    })
        //   // //    .OrderBy(x => x.SzRtServerId).ToList();
        //    return trm;

        //}
        ////public async Task<List<TransmissionsGroupedByDay>> ListServerWithTotalsTrnAndTrm()
        ////{



        ////    var baseQ2 = _dbContext.RtServerTransmissionDetail
        ////        //.Include(x => x.RtServerTransmissionDetailRtData)
        ////        .Where(x => x.DRtDeviceClosureDateTime.Value.Date != DateTime.Now.Date)
        ////         ;
        ////    var trmlist = baseQ2;
        ////    baseQ2.Include(x => x.RtServerTransmissionDetailRtData)
        ////       .Select(x => new RtServerTransmissionDetail
        ////       {
        ////           SzRtServerId = x.SzRtServerId,
        ////           DRtDeviceClosureDateTime = x.DRtDeviceClosureDateTime,
        ////           RtServerTransmissionDetailRtData = x.RtServerTransmissionDetailRtData,
        ////           SzRtDeviceId = x.SzRtDeviceId
        ////       }).Load();
        ////    //var baseQ = _dbContext.RtServerTransmissionDetail
        ////    //                      .Where(x => x.DRtDeviceClosureDateTime.Value.Date != DateTime.Now.Date
        ////    //                                                            // && x.DRtDeviceClosureDateTime>=DateTime.Today.AddMonths(-1)
        ////    //                                                            );
        ////    //var trmlist = baseQ;
        ////    //baseQ.Include(x => x.RtServerTransmissionDetailRtData)
        ////    //  .Select(x => new RtServerTransmissionDetail
        ////    //  {
        ////    //      SzRtServerId = x.SzRtServerId,
        ////    //      DRtDeviceClosureDateTime = x.DRtDeviceClosureDateTime,
        ////    //      RtServerTransmissionDetailRtData = x.RtServerTransmissionDetailRtData,
        ////    //      SzRtDeviceId = x.SzRtDeviceId
        ////    //  })
        ////    //  .Load();

        ////    //var t = trmlist.ToList().GroupBy(x => x.SzRtServerId, (key, group) =>
        ////    //new TransmissionsGroupedByDay
        ////    //{
        ////    //    SzRtServerId = key,
        ////    //    OperationClosureDatetime = group.Max(x => x.DRtDeviceClosureDateTime),
        ////    //    TotalAmount = group.GroupBy(x=>x.DRtDeviceClosureDateTime.Value.Date)
        ////    //                .OrderByDescending(x =>x.Key).FirstOrDefault()
        ////    //                .Sum(x=>x.RtServerTransmissionDetailRtData.Sum(x=>x.DSaleAmount+x.DVatAmount-x.DReturnAmount-x.DVoidAmount))

        ////    //}
        ////    //).ToList() ;

        ////    var trm = trmlist.ToList()
        ////             .Select(x => new { x.SzRtServerId, x.DRtDeviceClosureDateTime, x.RtServerTransmissionDetailRtData, x.SzRtDeviceId })
        ////             .GroupBy(x => new { x.SzRtServerId }, (key, group)
        ////                   => new
        ////                   {
        ////                       srv = key.SzRtServerId,
        ////                       closureDate = group.Max(x => x.DRtDeviceClosureDateTime),
        ////                       total =
        ////                          //veriosne corretta
        ////                          group
        ////                          .GroupBy(x => x.DRtDeviceClosureDateTime.Value.Date)
        ////                          .Where(x => x.Key.Date == group.Max(x => x.DRtDeviceClosureDateTime).Value.Date)
        ////                          .SelectMany(z => z.Select(x => new
        ////                          {
        ////                              srv = x.SzRtServerId,
        ////                              x.SzRtDeviceId,
        ////                              total = x.RtServerTransmissionDetailRtData
        ////                                                            .Sum(x => x.DSaleAmount + x.DVatAmount - x.DReturnAmount - x.DVoidAmount),

        ////                              closureDate = group.Max(x => x.DRtDeviceClosureDateTime)
        ////                          }))
        ////                        .ToList()
        ////                   }
        ////             )
        ////             .SelectMany(x => x.total)
        ////             .GroupBy(x => new { x.srv, x.closureDate }, (key, group)
        ////                 => new TransmissionsGroupedByDay
        ////                 {
        ////                     SzRtServerId = key.srv,
        ////                     OperationClosureDatetime = key.closureDate,
        ////                     // SzRtDeviceId = key.SzRtDeviceId,
        ////                     TotalAmount = group.Sum(x => x.total)
        ////                 })
        ////             //.Select(x => new TransmissionsGroupedByDay { SzRtServerId = x.srv, OperationClosureDatetime = x.closureDate, 
        ////             //                                             TotalAmount = x.total, SzRtDeviceId = x.SzRtDeviceId })
        ////             .ToList();
        ////    var trn = ListTransactionServerRtAll();

        ////    trm.ForEach(t =>
        ////    {
        ////        t.TotalRtServer = 0.0m;
        ////        t.TotalTP = 0.0m;
        ////        var trnfound = trn.Where(x => x.RtServerId == t.SzRtServerId
        ////                                              && x.DPosDateTime.Value.Date == t.OperationClosureDatetime.Value.Date
        ////                                              // && x.RtDeviceId == t.SzRtDeviceId
        ////                                              && x.RtDeviceId != null
        ////                                              ).ToList();

        ////        if (trnfound != null && trnfound.Count() > 0)
        ////        {
        ////            var trx = new TransmissionsGroupedByDay();
        ////            var tmp = trnfound.Where(x => x.RtDeviceId == t.SzRtDeviceId).ToList();

        ////            t.TotalRtServer = trnfound.Sum(x => x.TotalServerRT);
        ////            t.TotalTP = trnfound.Sum(x => x.TotalTP);
        ////        }


        ////    });
        ////    return trm.ToList();
        ////}

        public Task<List<UserActivityLog>> GetUserActivityByTransaction(string idTransaction)
        {
            return _dbContext.UserActivityLog
                      .Where(x => x.SzRtDocumentId == idTransaction)
                      .ToListAsync();
        }
        //test
        public async Task<List<TransactionByServerRtForTransmissions>> ListTransactionServerRtDay(string rtServerId, int storeId, int storeGroupId, DateTime? date)
        {
            var baseQ = _dbContext.TransactionAffiliation
                                  .Include(x => x.TransactionVat)
                                  .ThenInclude(x => x.SzVatCode)
                .Where(x => x.SzRtServerId == rtServerId
                    && x.DPosDateTime.Value.Date == date.Value.Date

                    )
                .Select(x => new TransactionAffiliation
                {
                    SzRtDocumentId = x.SzRtDocumentId,
                    SzRtServerId = x.SzRtServerId,
                    SzRtDeviceId = x.SzRtDeviceId,
                    LRtClosureNmbr = x.LRtClosureNmbr,
                    DPosDateTime = x.DPosDateTime,
                    DPosTransactionTurnover = x.DPosTransactionTurnover,
                    LPosReceivedTransactionCounter = x.LPosReceivedTransactionCounter,
                    DRtTransactionTurnover = x.DRtTransactionTurnover,
                    DRtDateTime = x.DRtDateTime,
                    LRtReceivedTransactionCounter = x.LRtReceivedTransactionCounter,
                    LPosWorkstationNmbr = x.LPosWorkstationNmbr,
                    BTransactionCheckedFlag = x.BTransactionCheckedFlag,
                    TransactionVat = x.TransactionVat.Select(v =>
                                                               new TransactionVat
                                                               {
                                                                   DPosGrossAmount = v.DPosGrossAmount * x.LPosReceivedTransactionCounter,
                                                                   DPosNetAmount = v.DPosNetAmount,
                                                                   DPosVatAmount = v.DPosVatAmount,
                                                                   DPosVatRate = v.DPosVatRate,
                                                                   SzVatCode = v.SzVatCode,
                                                                   SzVatCodeId = v.SzVatCodeId,
                                                                   DRtGrossAmount = v.DRtGrossAmount,


                                                               }
                                                                ).ToList()
                }).ToList();


            var listRes = baseQ.GroupBy(x => new
            {
                DPosDateTime = x.DPosDateTime != null ? x.DPosDateTime.Value.Date : x.DRtDateTime.Value.Date,
                szRtDeviceId = x.SzRtDeviceId,
                lRtClosureNmbr = x.LRtClosureNmbr,
                DRtDateTime = x.DRtDateTime != null ? x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date,
            },
                 (key, group) => new TransactionByServerRtForTransmissions
                 {
                     RtServerId = rtServerId,
                     RtDeviceId = key.szRtDeviceId,
                     ClosureNumber = key.lRtClosureNmbr,
                     DRtDateTime = key.DRtDateTime,
                     DPosDateTime = key.DPosDateTime,
                     LPosWorkstationNmbr = group.FirstOrDefault().LPosWorkstationNmbr.ToString(),
                     TotalTP = group.Sum(x => x.DPosTransactionTurnover * (x.LPosReceivedTransactionCounter ?? 1)),
                     TotalServerRT = group.Sum(x => x.DRtTransactionTurnover * (x.LRtReceivedTransactionCounter ?? 1)),
                     TransactionVatsEsen = group.AsQueryable().SelectMany(x => x.TransactionVat).Where(x => x.SzVatCodeId.ToLower().Trim() == "es" && x.SzVatCode != null)
                                       .GroupBy(x => x.SzVatCode.SzVatNature)
                                       .Select(x => new TransactionVatForTrasmissionsViewModel
                                       {
                                           SzVatNature = x.Key,
                                           DPosVatRate = x.FirstOrDefault().DPosVatRate,
                                           DPosGrossAmount = x.Sum(x => x.DPosGrossAmount),
                                           DRtGrossAmount = x.Sum(x => x.DRtGrossAmount)

                                       }
                                       ).ToList(),
                     TransactionVats = group.AsQueryable()
                                           .SelectMany(x => x.TransactionVat).Where(x => x.SzVatCodeId.ToLower().Trim() != "es")
                                           .AsQueryable()
                                           .GroupBy(x => x.DPosVatRate)
                                           .Select(x =>
                                                new TransactionVatForTrasmissionsViewModel
                                                {
                                                    DPosVatRate = x.Key,
                                                    DRtGrossAmount = x.Sum(x => x.DRtGrossAmount),
                                                    DPosGrossAmount = x.Sum(x => x.DPosGrossAmount)
                                                })
                                       .ToList()
                 })
               .Where(x => x.DRtDateTime < DateTime.Today && x.DPosDateTime < DateTime.Today)
               .ToList();

            listRes.ForEach(x =>
            {
                x.TransactionVats = x.TransactionVats.Union(x.TransactionVatsEsen).Distinct().ToList();
            });
            return listRes;
        }
        public Task<IEnumerable<TransactionAffiliation>> GetTransactionsMismtach(RtServer rtserver)
        {
            //var lastUpdate = Convert.ToDateTime(dateFrom);
            var trn = _dbContext.TransactionAffiliation
                                .Where(x => x.SzRtServerId == rtserver.SzRtServerId
                                &&
                                x.LRetailStoreId == rtserver.LRetailStoreId
                                &&
                                x.LStoreGroupId == rtserver.LStoreGroupId
                                &&
                                (
                                x.LTransactionMismatchId.HasValue
                                   && (
                                   (x.LTransactionMismatchId == 1 || x.LTransactionMismatchId == 2)
                                    || (x.LTransactionMismatchId == 3 && x.DRtDateTime.Value.Date < DateTime.Today.Date)
                                    || (x.LTransactionMismatchId == 4 && x.DPosDateTime.Value.Date < DateTime.Today.Date)
                                    || (
                                    (x.LTransactionMismatchId != 3 && x.LTransactionMismatchId != 4) &&
                                    (((x.DPosTransactionTurnover ?? 0) * (x.LPosReceivedTransactionCounter ?? 1)) != ((x.DRtTransactionTurnover ?? 0) * (x.LRtReceivedTransactionCounter ?? 1))))
                                       && ((!x.DPosDateTime.HasValue || x.DPosDateTime.Value.Date < DateTime.Today.Date)
                                       && (!x.DRtDateTime.HasValue || x.DRtDateTime.Value.Date < DateTime.Today.Date))
                                   )
                                   && x.BTransactionCheckedFlag != true
                                   )
                                  );
            return Task.FromResult(trn.AsEnumerable());
        }
        public Task<IEnumerable<TransactionAffiliation>> GetTransactionsMismtach(string rtserver)
        {
            var trn = _dbContext.TransactionAffiliation
                                .Where(x =>
                                x.SzRtServerId == rtserver &&  
                                (x.LTransactionMismatchId.HasValue
                                   && (
                                   (x.LTransactionMismatchId == 1 || x.LTransactionMismatchId == 2)
                                    || (x.LTransactionMismatchId == 3 && x.DRtDateTime.Value.Date < DateTime.Today.Date)
                                    || (x.LTransactionMismatchId == 4 && x.DPosDateTime.Value.Date < DateTime.Today.Date)
                                    || (
                                    (x.LTransactionMismatchId != 3 && x.LTransactionMismatchId != 4) &&
                                    (((x.DPosTransactionTurnover ?? 0) * (x.LPosReceivedTransactionCounter ?? 1)) != ((x.DRtTransactionTurnover ?? 0) * (x.LRtReceivedTransactionCounter ?? 1))))
                                       && ((!x.DPosDateTime.HasValue || x.DPosDateTime.Value.Date < DateTime.Today.Date)
                                       && (!x.DRtDateTime.HasValue || x.DRtDateTime.Value.Date < DateTime.Today.Date))
                                   )
                                   //&& x.BTransactionCheckedFlag != true
                                   )
                                  );
            return Task.FromResult(trn.AsEnumerable());
        }
        public Task<IEnumerable<TransactionAffiliation>> GetTransactionsError(RtServer rtserver)
        {
            var trn = _dbContext.RtServer
                .Include(x => x.TransactionRtError)
                .Include(x => x.TransactionAffiliation)
                .Where(x => x.SzRtServerId == rtserver.SzRtServerId
                       && x.LRetailStoreId == rtserver.LRetailStoreId
                       && x.LStoreGroupId == rtserver.LStoreGroupId
                       )
                .SelectMany(x =>

                    x.TransactionAffiliation
                    .Where(t => x.TransactionRtError
                    .Any(r => r.LRtDocumentNmbr == t.LRtDocumentNmbr && r.SzRtDeviceId == t.SzRtDeviceId)
                    )
                )
                .AsEnumerable();

            return Task.FromResult(trn);
        }
        public Task<IEnumerable<TransactionAffiliation>> GetTransactionsError(string rtserver)
        {
            var trn = _dbContext.RtServer
                .Include(x => x.TransactionRtError)
                .Include(x => x.TransactionAffiliation)
                .Where(x => x.SzRtServerId == rtserver
                       )
                .SelectMany(x =>

                    x.TransactionAffiliation
                    .Where(t => x.TransactionRtError
                    .Any(r => r.LRtDocumentNmbr == t.LRtDocumentNmbr && r.SzRtDeviceId == t.SzRtDeviceId)
                    )
                )
                .AsEnumerable();

            return Task.FromResult(trn);
        }
        public Task<IEnumerable<TransactionAffiliation>> GetTransactionsCompliant(RtServer rtserver, IEnumerable<string> noncompliant, int trnNmbr)
        {

            //var   trn = _dbContext.TransactionAffiliation
            //           .Where(x => x.SzRtServerId == rtserver.SzRtServerId
            //          && x.LRetailStoreId == rtserver.LRetailStoreId
            //          && x.LStoreGroupId == rtserver.LStoreGroupId
            //          && !noncompliant.Any(t=>t==x.SzRtDocumentId)
            //         ).OrderByDescending(x => x.DRtDateTime).Take(trnNmbr).AsEnumerable();

            var trn = _dbContext.TransactionAffiliation
                      .Where(x =>
                                x.SzRtServerId == rtserver.SzRtServerId
                                  && x.LRetailStoreId == rtserver.LRetailStoreId
                                          && x.LStoreGroupId == rtserver.LStoreGroupId
                                            &&
                                !(
                                x.LTransactionMismatchId.HasValue
                                   && (
                                   (x.LTransactionMismatchId == 1 || x.LTransactionMismatchId == 2)
                                    || (x.LTransactionMismatchId == 3 && x.DRtDateTime.Value.Date < DateTime.Today.Date)
                                    || (x.LTransactionMismatchId == 4 && x.DPosDateTime.Value.Date < DateTime.Today.Date)
                                    || (
                                    (x.LTransactionMismatchId != 3 && x.LTransactionMismatchId != 4) &&
                                    (((x.DPosTransactionTurnover ?? 0) * (x.LPosReceivedTransactionCounter ?? 1)) != ((x.DRtTransactionTurnover ?? 0) * (x.LRtReceivedTransactionCounter ?? 1))))
                                       && ((!x.DPosDateTime.HasValue || x.DPosDateTime.Value.Date < DateTime.Today.Date)
                                       && (!x.DRtDateTime.HasValue || x.DRtDateTime.Value.Date < DateTime.Today.Date))
                                   )
                                   ) && (noncompliant != null && !noncompliant.Any(t => t == x.SzRtDocumentId))

                                  )

                       .OrderByDescending(x => x.DRtDateTime).Take(trnNmbr)
                       .AsEnumerable();

            return Task.FromResult(trn);


        }
        public Task<IEnumerable<TransactionAffiliation>> GetTransactionsCompliant(string rtserver, IEnumerable<string> noncompliant, int trnNmbr, FiltersmodelBindingRequest filters)
        {

            Expression<Func<TransactionAffiliation, bool>> expressTrnFrom = null;
            if (!string.IsNullOrEmpty(filters.TransactionDateFrom))
            {
                DateTime transactionDate = DateTime.ParseExact(
                    filters.TransactionDateFrom,
                    "dd-MM-yyyy",
                    System.Globalization.CultureInfo.InvariantCulture);
                expressTrnFrom = x => (x.DRtDateTime != null ? x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date) >= transactionDate;
            }
            Expression<Func<TransactionAffiliation, bool>> expressTrnTo = null;
            if (!string.IsNullOrEmpty(filters.TransactionDateTo))
            {
                DateTime transactionDate = DateTime.ParseExact(
                    filters.TransactionDateTo,
                    "dd-MM-yyyy",
                    System.Globalization.CultureInfo.InvariantCulture);

                expressTrnTo = x => (x.DRtDateTime != null ? x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date) <= transactionDate;

            }

            Expression<Func<TransactionAffiliation, bool>> expressTrnPos = null;
            if (!string.IsNullOrEmpty(filters.PosWorkstationNmbr))
            {
                Int16.TryParse(filters.PosWorkstationNmbr, out var i);
                expressTrnPos = x => x.LPosWorkstationNmbr == i;

            }

            Expression<Func<TransactionAffiliation, bool>> expressTrnClosureNmbr = null;

            if (!string.IsNullOrEmpty(filters.RtClosureNmbr))
            {
                Int32.TryParse(filters.RtClosureNmbr, out var i);
                expressTrnClosureNmbr = x => x.LRtClosureNmbr == i;

            }

            Expression<Func<TransactionAffiliation, bool>> expressTrnDocumentNmbr = null;

            if (!string.IsNullOrEmpty(filters.RtDocumentNmbr))
            {

                expressTrnDocumentNmbr = x => x.LRtDocumentNmbr.ToString() == filters.RtDocumentNmbr;
            }


            var trnn = _dbContext.TransactionAffiliation.Where(x => x.SzRtServerId == rtserver
              &&
                           !(
                           x.LTransactionMismatchId.HasValue
                              && (
                              (x.LTransactionMismatchId == 1 || x.LTransactionMismatchId == 2)
                               || (x.LTransactionMismatchId == 3 && x.DRtDateTime < DateTime.Today.Date)
                               || (x.LTransactionMismatchId == 4 && x.DPosDateTime < DateTime.Today.Date)
                               || (
                               (x.LTransactionMismatchId != 3 && x.LTransactionMismatchId != 4) &&
                               (((x.DPosTransactionTurnover ?? 0) * (x.LPosReceivedTransactionCounter ?? 1)) != ((x.DRtTransactionTurnover ?? 0) * (x.LRtReceivedTransactionCounter ?? 1))))
                                  && ((!x.DPosDateTime.HasValue || x.DPosDateTime.Value.Date < DateTime.Today.Date)
                                  && (!x.DRtDateTime.HasValue || x.DRtDateTime.Value.Date < DateTime.Today.Date))
                              )
                              )
                              && !noncompliant.Any(t => t == x.SzRtDocumentId)

                );
            if (expressTrnFrom != null)
            {
                trnn = trnn.Where(expressTrnFrom);
            }
            if (expressTrnTo != null)
            {
                trnn = trnn.Where(expressTrnTo);
            }
            if (expressTrnPos != null)
            {
                trnn = trnn.Where(expressTrnPos);

            }
            if (expressTrnClosureNmbr != null)
            {
                trnn = trnn.Where(expressTrnClosureNmbr);

            }
            if (expressTrnDocumentNmbr != null)
            {
                trnn = trnn.Where(expressTrnDocumentNmbr);

            }

            var trn = trnn.OrderByDescending(x => x.DRtDateTime.HasValue ? x.DRtDateTime : x.DPosDateTime)
                   .Take(trnNmbr)
                    ;

            //var trn = _dbContext.TransactionAffiliation.Where(x => x.SzRtServerId == rtserver
            //       &&
            //                    !(
            //                    x.LTransactionMismatchId.HasValue
            //                       && (
            //                       (x.LTransactionMismatchId == 1 || x.LTransactionMismatchId == 2)
            //                        || (x.LTransactionMismatchId == 3 && x.DRtDateTime < DateTime.Today.Date)
            //                        || (x.LTransactionMismatchId == 4 && x.DPosDateTime < DateTime.Today.Date)
            //                        || (
            //                        (x.LTransactionMismatchId != 3 && x.LTransactionMismatchId != 4) &&
            //                        (((x.DPosTransactionTurnover ?? 0) * (x.LPosReceivedTransactionCounter ?? 1)) != ((x.DRtTransactionTurnover ?? 0) * (x.LRtReceivedTransactionCounter ?? 1))))
            //                           && ((!x.DPosDateTime.HasValue || x.DPosDateTime.Value.Date < DateTime.Today.Date)
            //                           && (!x.DRtDateTime.HasValue || x.DRtDateTime.Value.Date < DateTime.Today.Date))
            //                       )
            //                       ) && !noncompliant.Any(t => t == x.SzRtDocumentId)

            //         ).AsEnumerable()
            //         .OrderByDescending(x=>x.DRtDateTime??x.DPosDateTime)
            //         .Take(trnNmbr);


            return Task.FromResult(trn.AsEnumerable());


        }
        public Task<IEnumerable<TransactionAffiliation>> GetTransactionsCompliantOthers(string rtserver, IEnumerable<string> noncompliant, int trnNmbr, FiltersmodelBindingRequest filters)
        {

            Expression<Func<TransactionAffiliation, bool>> expressTrnFrom = null;
            if (!string.IsNullOrEmpty(filters.TransactionDateFrom))
            {
                DateTime transactionDate = DateTime.ParseExact(
                    filters.TransactionDateFrom,
                    "dd-MM-yyyy",
                    System.Globalization.CultureInfo.InvariantCulture);
                expressTrnFrom = x => (x.DRtDateTime != null ? x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date) >= transactionDate;
            }
            Expression<Func<TransactionAffiliation, bool>> expressTrnTo = null;
            if (!string.IsNullOrEmpty(filters.TransactionDateTo))
            {
                DateTime transactionDate = DateTime.ParseExact(
                    filters.TransactionDateTo,
                    "dd-MM-yyyy",
                    System.Globalization.CultureInfo.InvariantCulture);

                expressTrnTo = x => (x.DRtDateTime != null ? x.DRtDateTime.Value.Date : x.DPosDateTime.Value.Date) <= transactionDate;

            }

            Expression<Func<TransactionAffiliation, bool>> expressTrnPos = null;
            if (!string.IsNullOrEmpty(filters.PosWorkstationNmbr))
            {
                Int16.TryParse(filters.PosWorkstationNmbr, out var i);
                expressTrnPos = x => x.LPosWorkstationNmbr == i;

            }

            Expression<Func<TransactionAffiliation, bool>> expressTrnClosureNmbr = null;

            if (!string.IsNullOrEmpty(filters.RtClosureNmbr))
            {
                Int32.TryParse(filters.RtClosureNmbr, out var i);
                expressTrnClosureNmbr = x => x.LRtClosureNmbr == i;

            }

            Expression<Func<TransactionAffiliation, bool>> expressTrnDocumentNmbr = null;

            if (!string.IsNullOrEmpty(filters.RtDocumentNmbr))
            {

                expressTrnDocumentNmbr = x => x.LRtDocumentNmbr.ToString() == filters.RtDocumentNmbr;
            }


            var trnn = _dbContext.TransactionAffiliation.Where(x => x.SzRtServerId == rtserver
              &&
                           !(
                           x.LTransactionMismatchId.HasValue
                              && (
                              (x.LTransactionMismatchId == 1 || x.LTransactionMismatchId == 2)
                               || (x.LTransactionMismatchId == 3 && x.DRtDateTime < DateTime.Today.Date)
                               || (x.LTransactionMismatchId == 4 && x.DPosDateTime < DateTime.Today.Date)
                               || (
                               (x.LTransactionMismatchId != 3 && x.LTransactionMismatchId != 4) &&
                               (((x.DPosTransactionTurnover ?? 0) * (x.LPosReceivedTransactionCounter ?? 1)) != ((x.DRtTransactionTurnover ?? 0) * (x.LRtReceivedTransactionCounter ?? 1))))
                                  && ((!x.DPosDateTime.HasValue || x.DPosDateTime.Value.Date < DateTime.Today.Date)
                                  && (!x.DRtDateTime.HasValue || x.DRtDateTime.Value.Date < DateTime.Today.Date))
                              )
                              )
                              && !noncompliant.Any(t => t == x.SzRtDocumentId)

                );
            if (expressTrnFrom != null)
            {
                trnn = trnn.Where(expressTrnFrom);
            }
            if (expressTrnTo != null)
            {
                trnn = trnn.Where(expressTrnTo);
            }
            if (expressTrnPos != null)
            {
                trnn = trnn.Where(expressTrnPos);

            }
            if (expressTrnClosureNmbr != null)
            {
                trnn = trnn.Where(expressTrnClosureNmbr);

            }
            if (expressTrnDocumentNmbr != null)
            {
                trnn = trnn.Where(expressTrnDocumentNmbr);

            }
            var xxx = trnn.ToList();
            var trn = trnn.OrderByDescending(x => x.DRtDateTime.HasValue ? x.DRtDateTime : x.DPosDateTime)
                   .Take(trnNmbr)
                    ;

            //var trn = _dbContext.TransactionAffiliation.Where(x => x.SzRtServerId == rtserver
            //       &&
            //                    !(
            //                    x.LTransactionMismatchId.HasValue
            //                       && (
            //                       (x.LTransactionMismatchId == 1 || x.LTransactionMismatchId == 2)
            //                        || (x.LTransactionMismatchId == 3 && x.DRtDateTime < DateTime.Today.Date)
            //                        || (x.LTransactionMismatchId == 4 && x.DPosDateTime < DateTime.Today.Date)
            //                        || (
            //                        (x.LTransactionMismatchId != 3 && x.LTransactionMismatchId != 4) &&
            //                        (((x.DPosTransactionTurnover ?? 0) * (x.LPosReceivedTransactionCounter ?? 1)) != ((x.DRtTransactionTurnover ?? 0) * (x.LRtReceivedTransactionCounter ?? 1))))
            //                           && ((!x.DPosDateTime.HasValue || x.DPosDateTime.Value.Date < DateTime.Today.Date)
            //                           && (!x.DRtDateTime.HasValue || x.DRtDateTime.Value.Date < DateTime.Today.Date))
            //                       )
            //                       ) && !noncompliant.Any(t => t == x.SzRtDocumentId)

            //         ).AsEnumerable()
            //         .OrderByDescending(x=>x.DRtDateTime??x.DPosDateTime)
            //         .Take(trnNmbr);


            return Task.FromResult(trn.AsEnumerable());


        }
        public List<string> ListSrvHasOneMsmatch()
        {
            var list = _dbContext.TransactionAffiliation
                .Where(x => (
                (x.LTransactionMismatchId == 3 && x.DRtDateTime < DateTime.Today.Date)
                ||
             (x.LTransactionMismatchId == 4 && x.DPosDateTime < DateTime.Today.Date)
             )
             && x.BTransactionCheckedFlag != true).Select(x => x.SzRtServerId).Distinct().ToList();
            return list;
        }
        public List<TransactionAffiliation> ListSrvHasMsmatchTPAndRT(DateTime dFrom, DateTime dTo)
        {
            var list = _dbContext.TransactionAffiliation
                .Where(x =>
                (
                (x.LTransactionMismatchId == 3 && x.DRtDateTime < dTo.AddDays(1).Date && x.DRtDateTime >= dFrom.Date)
             //||
             //(x.LTransactionMismatchId == 4 && x.DPosDateTime < DateTime.Today.Date)
             )
             && x.BTransactionCheckedFlag != true).Select(x => new TransactionAffiliation { SzRtServerId = x.SzRtServerId, DRtDateTime = x.DRtDateTime }).Distinct().ToList();
            return list;
        }

        Task<List<TransmissionsGroupedByDay>> ITransactionRepository.ListServerWithTotalsTrnAndTrm()
        {
            //to do 
            //trasformare il resultato della transaction ricevuta e passarla al view.
            throw new NotImplementedException();
        }
    }
}
