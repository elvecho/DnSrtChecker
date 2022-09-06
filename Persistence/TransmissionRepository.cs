using DnSrtChecker.Models;
using DnSrtChecker.Models.ViewModels;
using DnSrtChecker.Models.ViewModels.ModelsFunctions;
using DnSrtChecker.ModelsHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DnSrtChecker.Persistence
{
    public class TransmissionRepository : ITransmissionRepository
    {
        public readonly RT_ChecksContext _dbContext;

        public TransmissionRepository(RT_ChecksContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool AddTransmission(RtServerTransmission transmission)
        {
            try
            {
                _dbContext.RtServerTransmission.Add(transmission);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<RtServerTransmissionDetail> GetTransmission(int RtServerOperationId, string rtServerId)
        {
            return await _dbContext.RtServerTransmissionDetail
                .Include(x => x.RtServerTransmissionDetailRtData)
                .Include(x => x.RtServerTransmissionDetailRtReport)
                .FirstOrDefaultAsync(x => x.LRtServerOperationId == RtServerOperationId
                                    && x.SzRtServerId == rtServerId);
        }
        public async Task<RtServerTransmissionDetail> GetTransmission(string rtServerId, string deviceId, DateTime dPosDateTime)
        {
            return _dbContext.RtServerTransmissionDetail
                            .FirstOrDefault(x => x.SzRtServerId == rtServerId
                                                    && x.SzRtDeviceId == deviceId
                                                    && x.DRtDeviceClosureDateTime.Value.Date == dPosDateTime.Date);

        }
        public async Task<List<RtServerTransmissionDetail>> GetTransmission(string rtServerId, DateTime dPosDateTime)
        {
            return await _dbContext.RtServerTransmissionDetail
                            .Where(x => x.SzRtServerId == rtServerId
                                                    && x.DRtDeviceClosureDateTime.Value.Date == dPosDateTime.Date).ToListAsync();

        }

        public async Task<List<RtServerTransmissionDetail>> ListTransmission()
        {
            return await _dbContext.RtServerTransmissionDetail
                 .Include(x => x.RtServerTransmissionDetailRtData)
                 .Include(x => x.RtServerTransmissionDetailRtReport).ToListAsync();



        }

        public async Task<List<RtServerTransmissionDetail>> ListTransmissionByServerId(string rtServer)
        {
            return (await _dbContext.RtServerTransmissionDetail
                 .Include(x => x.RtServerTransmissionDetailRtData).Include(x => x.RtServerTransmissionDetailRtReport)
                 .ToListAsync()).Where(x => x.SzRtServerId == rtServer).ToList();
        }


        public async Task<List<TransmissionsGroupedByDay>> ListTransmissionByServerIdByDay(string rtServer, string date,int nmbrDays)
        {
            var days = -Math.Abs(nmbrDays);
            var initDate = new DateTime();
            if (date == "" || date == null)
            {
                initDate = DateTime.Today;
            }
            else
            {
                initDate = DateTime.ParseExact(
                       date,
                        "dd-MM-yyyy",
                        System.Globalization.CultureInfo.InvariantCulture).AddDays(1);
            }
            var baseQ = _dbContext.RtServerTransmissionDetail.Where(x => x.SzRtServerId.Contains(rtServer)
                                                     && x.DRtDeviceClosureDateTime < initDate.Date
                                                     && x.DRtDeviceClosureDateTime.Value.Date >= initDate.AddDays(days).Date)
                                    .Select(x => new RtServerTransmissionDetail
                                    {
                                        SzRtServerId = x.SzRtServerId
                                        ,
                                        SzRtDeviceId = x.SzRtDeviceId
                                        ,
                                        BTransactionArchivedFlag = x.BTransactionArchivedFlag
                                        ,
                                        BTransactionCheckedFlag = x.BTransactionCheckedFlag
                                        ,
                                        DRtDeviceClosureDateTime = x.DRtDeviceClosureDateTime
                                        ,
                                        LRtDeviceTransmissionId = x.LRtDeviceTransmissionId
                                        ,
                                        LRtServerOperationId = x.LRtServerOperationId
                                        ,
                                        SzTranscationCheckNote = x.SzTranscationCheckNote
                                        ,
                                        SzUserName = x.SzUserName
                                        ,
                                        RtServerTransmissionDetailRtData = x.RtServerTransmissionDetailRtData
                                                                            .Select(x => new RtServerTransmissionDetailRtData
                                                                            {
                                                                                BVatVentilation = x.BVatVentilation
                                                                                ,
                                                                                DReturnAmount = x.DReturnAmount
                                                                                ,
                                                                                DSaleAmount = x.DSaleAmount
                                                                                ,
                                                                                DVatAmount = x.DVatAmount
                                                                                ,
                                                                                DVatRate = x.DVatRate
                                                                                ,
                                                                                DVoidAmount = x.DVoidAmount
                                                                                ,
                                                                                LRtDeviceTransmissionId = x.LRtDeviceTransmissionId
                                                                                ,
                                                                                LRtServerOperationId = x.LRtServerOperationId
                                                                                ,
                                                                                SzRtDeviceId = x.SzRtDeviceId
                                                                                ,
                                                                                SzRtServerId = x.SzRtServerId
                                                                                ,
                                                                                SzVatNature = x.SzVatNature
                                                                            }).ToList()
                                        ,
                                        RtServerTransmissionDetailRtReport = x.RtServerTransmissionDetailRtReport
                                                                             .Select(x => new RtServerTransmissionDetailRtReport
                                                                             {
                                                                                 SzRtServerId = x.SzRtServerId
                                                                                ,
                                                                                 DEventDateTime = x.DEventDateTime
                                                                                ,
                                                                                 LRtDeviceTransmissionId = x.LRtDeviceTransmissionId
                                                                                ,
                                                                                 LRtServerOperationId = x.LRtServerOperationId
                                                                                ,
                                                                                 SzEventNote = x.SzEventNote
                                                                                ,
                                                                                 SzRtDeviceId = x.SzRtDeviceId
                                                                             }).ToList()
                                    });
            var listTrsm = baseQ;
            // baseQ.Include(x => x.RtServerTransmissionDetailRtData).Load();
            //baseQ.Include(x => x.RtServerTransmissionDetailRtReport).Load();
            var listRes = listTrsm.ToList()
                                .GroupBy(x => new { x.DRtDeviceClosureDateTime.Value.Date, x.SzRtDeviceId },
                                 (key, group) => new TransmissionsGroupedByDay
                                 {
                                     OperationClosureDatetime = key.Date,
                                     SzRtDeviceId = key.SzRtDeviceId,
                                     SzRtServerId = rtServer,
                                     LRtDeviceTransmissionIdS =string.Join(" / ", group.Select(x => x.LRtDeviceTransmissionId).Distinct()),
                                     LRtServerOperationIdS = string.Join(" / ",group.Select(x => x.LRtServerOperationId).Distinct()),
                                     BTransactionCheckedFlag = group.Select(x => x.BTransactionCheckedFlag).ToList(),
                                     BTransactionArchivedFlag = group.Select(x => x.BTransactionArchivedFlag).ToList(),
                                     SzTranscationCheckNote = group.Select(x => x.SzTranscationCheckNote).ToList(),
                                     SzUserName = group.Select(x => x.SzUserName).ToList(),
                                     TotalAmount = group.Sum(x => x.RtServerTransmissionDetailRtData
                                                        .Sum(x => x.DSaleAmount + x.DVatAmount
                                                                  - x.DReturnAmount - x.DVoidAmount)),
                                     TotalRtServer = 0,
                                     TotalTP = 0,
                                     RtServerTransmissionsDetailRtData = group.AsQueryable()
                                     .SelectMany(x => x.RtServerTransmissionDetailRtData)
                                                       .GroupBy(x => new { x.DVatRate, x.SzVatNature }, (key, group) =>
                                                         new RtServerTransmissionDetailRtData
                                                         {
                                                           BVatVentilation = group.FirstOrDefault().BVatVentilation,
                                                           DReturnAmount = group.Sum(x => x.DReturnAmount),
                                                           DSaleAmount = group.Sum(x => x.DSaleAmount),
                                                           DVatAmount = group.Sum(x => x.DVatAmount),
                                                           DVatRate = key.DVatRate,
                                                           DVoidAmount = group.Sum(x => x.DVoidAmount),
                                                           LRtDataId = group.FirstOrDefault().LRtDataId,
                                                           LRtDeviceTransmissionId = group.FirstOrDefault().LRtDeviceTransmissionId,
                                                           LRtServerOperationId = group.FirstOrDefault().LRtServerOperationId,
                                                           SzRtDeviceId = group.FirstOrDefault().SzRtDeviceId,
                                                           SzRtServerId = group.FirstOrDefault().SzRtServerId,
                                                           SzVatNature = key.SzVatNature
                                                         }).ToList(),
                                     RtServerTransmissionsDetailRtReport = group.ToList()
                                                                                                          .SelectMany(x => x.RtServerTransmissionDetailRtReport)
                                                                                                          .ToList()

                                 }
                    ).OrderBy(x => x.SzRtDeviceId).ToList();
            //var test = listRes.OrderByDescending(x => x.OperationClosureDatetime);
            return listRes;
        }       

        public async Task<List<TransmissionsGroupedByDay>> ListTransmissionByServerIdByDay22(string rtServer, string date, int nmbrDays)
        {
            var days = -Math.Abs(nmbrDays);
            var initDate = new DateTime();
            if (date == "" || date == null)
            {
                initDate = DateTime.Today;
            }
            else
            {
                initDate = DateTime.ParseExact(
                       date,
                        "dd-MM-yyyy",
                        System.Globalization.CultureInfo.InvariantCulture).AddDays(1);
            }
            //Get TRX Details, RTDATA, RTReport
            var base1 = _dbContext.RtServerTransmissionDetail
                                .Where(x => x.SzRtServerId.Contains(rtServer)
                                && x.DRtDeviceClosureDateTime < initDate.Date
                                && x.DRtDeviceClosureDateTime.Value.Date == DateTime.ParseExact(date,"dd-MM-yyyy",
                                System.Globalization.CultureInfo.InvariantCulture)
                                //&& x.DRtDeviceClosureDateTime.Value.Date >= initDate.AddDays(days).Date
                                )
                                .Select(x =>
                                new RtServerTransmissionDetail
                                {
                                    SzRtServerId = x.SzRtServerId,
                                    SzRtDeviceId = x.SzRtDeviceId,
                                    BTransactionArchivedFlag = x.BTransactionArchivedFlag,
                                    BTransactionCheckedFlag = x.BTransactionCheckedFlag,
                                    DRtDeviceClosureDateTime = x.DRtDeviceClosureDateTime,
                                    LRtDeviceTransmissionId = x.LRtDeviceTransmissionId,
                                    LRtServerOperationId = x.LRtServerOperationId,
                                    SzTranscationCheckNote = x.SzTranscationCheckNote,
                                    SzUserName = x.SzUserName,
                                    RtServerTransmissionDetailRtData = x.RtServerTransmissionDetailRtData
                                                                            .Select(x => new RtServerTransmissionDetailRtData
                                                                            {
                                                                                BVatVentilation = x.BVatVentilation,
                                                                            }).ToList(),
                                    RtServerTransmissionDetailRtReport = x.RtServerTransmissionDetailRtReport
                                                                             .Select(x => new RtServerTransmissionDetailRtReport
                                                                             {
                                                                                 DEventDateTime = x.DEventDateTime,
                                                                                 SzEventNote = x.SzEventNote
                                                                             }).ToList()
                                }
                                )
                                .AsEnumerable()
                                .GroupBy(x=>new { x.DRtDeviceClosureDateTime.Value.Date, x.SzRtDeviceId})
                                .Select(x => new TransmissionsGroupedByDay
                                {
                                    OperationClosureDatetime = x.Key.Date,
                                    SzRtDeviceId = x.Key.SzRtDeviceId,
                                    SzRtServerId = rtServer,
                                    LRtDeviceTransmissionIdS = string.Join(" / ",x.Select(t=>t.LRtDeviceTransmissionId).Distinct()),
                                    LRtServerOperationIdS =  string.Join(" / ", x.Select(t => t.LRtServerOperationId).Distinct()),
                                    BTransactionCheckedFlag = x.Select(x => x.BTransactionCheckedFlag).ToList(),
                                    BTransactionArchivedFlag = x.Select(x => x.BTransactionArchivedFlag).ToList(),
                                    SzTranscationCheckNote = x.Select(x => x.SzTranscationCheckNote).ToList(),
                                    SzUserName = x.Select(x => x.SzUserName).ToList(),
                                    TotalRtServer = 0,
                                    TotalTP = 0,
                                    RtServerTransmissionsDetailRtData = x.AsQueryable()
                                     .SelectMany(x => x.RtServerTransmissionDetailRtData).ToList(),
                                    RtServerTransmissionsDetailRtReport = x.AsQueryable()
                                     .SelectMany(x => x.RtServerTransmissionDetailRtReport).ToList()
                                }
                                ).ToList();
                                ;
            //var baseTot = _dbContext.trxResult(rtServer
            //                                , initDate.AddDays(days).Date.ToString("yyyy-MM-dd")
            //                                , initDate.Date.ToString("yyyy-MM-dd")
            //                               ).ToList()
            //                        ;
            var baseTot = _dbContext.trxResult(rtServer
                                            , initDate.Date.ToString("yyyy-MM-dd")
                                            , initDate.Date.ToString("yyyy-MM-dd")
                                           ).ToList()
                                    ;
            //var baseTot = _dbContext.trxTotalsResult2(initDate.AddDays(days).Date.ToString("yyyy-MM-dd")
            //                               , initDate.Date.ToString("yyyy-MM-dd")
            //                              )
            //                       .ToList();

            foreach (var t in base1)
            {
                var found = baseTot.Where(x => x.DRtDeviceClosureDateTime.Value.Date == t.OperationClosureDatetime.Value.Date
                                              && x.SzRtDeviceId == t.SzRtDeviceId);
                if(found!=null && found.Count()>0)
                {
                    t.TotalAmount = found.Sum(x => x.SumValue);
                    t.RtServerTransmissionsDetailRtData = found
                        .Select(x => new RtServerTransmissionDetailRtData
                        {
                            BVatVentilation = t.RtServerTransmissionsDetailRtData.FirstOrDefault().BVatVentilation,
                            DReturnAmount = x.dReturnAmount,
                            DSaleAmount = x.dSaleAmount,
                            DVatAmount = x.dVatAmount,
                            DVatRate = x.DVatRate,
                            DVoidAmount = x.dVoidAmount,
                            SzRtDeviceId = x.SzRtDeviceId,
                            SzVatNature = x.SzVatNature,
                            SzRtServerId = rtServer
                        }).ToList();
                        
                }
            }
            var listRes = base1.OrderBy(x=>x.SzRtDeviceId).ToList();
            //var test = listRes.OrderByDescending(x => x.OperationClosureDatetime);
            return listRes;
        }
        public async Task<List<TransmissionsGroupedByDay>> ListTransmissionByServerIdDay(string rtServer, string date)
        {
            var day = DateTime.ParseExact(
                       date,
                       "dd-MM-yyyy",
                       System.Globalization.CultureInfo.InvariantCulture);

            var list = _dbContext.RtServerTransmissionDetail.Include(x => x.RtServerTransmissionDetailRtData)
                                                  .Include(x => x.RtServerTransmissionDetailRtReport)
                                                  .ToList()
                                                  .Where(x => x.SzRtServerId == rtServer && x.DRtDeviceClosureDateTime.Value.Date == day.Date)
                                                  .GroupBy(x => new { x.DRtDeviceClosureDateTime.Value.Date })
                                                  .OrderByDescending(x => x.Key)
                                                  .FirstOrDefault()
                                                  .GroupBy(x => x.SzRtDeviceId)
                                                  .Select(x => new TransmissionsGroupedByDay
                                                  {
                                                      OperationClosureDatetime = x.Max(x => x.DRtDeviceClosureDateTime),
                                                      SzRtDeviceId = x.Select(x => x.SzRtDeviceId).FirstOrDefault(),
                                                      SzRtServerId = x.Select(x => x.SzRtServerId).FirstOrDefault(),
                                                      LRtDeviceTransmissionId = x.Max(x => x.LRtDeviceTransmissionId),
                                                      LRtServerOperationId = x.Max(x => x.LRtServerOperationId),
                                                      BTransactionCheckedFlag = x.Select(x => x.BTransactionCheckedFlag).ToList(),
                                                      BTransactionArchivedFlag = x.Select(x => x.BTransactionArchivedFlag).ToList(),
                                                      SzTranscationCheckNote = x.Select(x => x.SzTranscationCheckNote).ToList(),
                                                      SzUserName = x.Select(x => x.SzUserName).ToList(),
                                                      TotalAmount = x.Sum(x => x.RtServerTransmissionDetailRtData.Sum(x => x.DSaleAmount + x.DVatAmount
                                                                                                                        - x.DVoidAmount - x.DReturnAmount)),
                                                      TotalRtServer = 0,
                                                      TotalTP = 0

                                                  })
                                                 ;
            var listGlobal = _dbContext.RtServerTransmissionDetail
                                .Include(x => x.RtServerTransmissionDetailRtData)
                                .Where(x => x.SzRtServerId == rtServer && x.DRtDeviceClosureDateTime.Value.Date >= DateTime.Today.AddMonths(-1))

                                .ToList()
                                .GroupBy(x => new { x.DRtDeviceClosureDateTime.Value.Date, x.SzRtDeviceId },
                                 (key, group) => new TransmissionsGroupedByDay
                                 {
                                     OperationClosureDatetime = key.Date,
                                     SzRtDeviceId = key.SzRtDeviceId,
                                     SzRtServerId = rtServer,
                                     LRtDeviceTransmissionId = group.Max(x => x.LRtDeviceTransmissionId),
                                     LRtServerOperationId = group.Max(x => x.LRtServerOperationId),
                                     BTransactionCheckedFlag = group.Select(x => x.BTransactionCheckedFlag).ToList(),
                                     BTransactionArchivedFlag = group.Select(x => x.BTransactionArchivedFlag).ToList(),
                                     SzTranscationCheckNote = group.Select(x => x.SzTranscationCheckNote).ToList(),
                                     SzUserName = group.Select(x => x.SzUserName).ToList(),
                                     TotalAmount = group.Sum(x => x.RtServerTransmissionDetailRtData
                                                        .Sum(x => x.DSaleAmount + x.DVatAmount
                                                                  - x.DReturnAmount - x.DVoidAmount)),
                                     TotalRtServer = 0,
                                     TotalTP = 0

                                 }
                                 ).ToList()
                               ;// .OrderBy(x => x.SzRtDeviceId)
            listGlobal.ForEach(x =>
            {
                if (x.OperationClosureDatetime.Value.Date == list.FirstOrDefault().OperationClosureDatetime.Value.Date)
                {
                    x = list.FirstOrDefault();
                }
            });
            return listGlobal.OrderByDescending(x => x.OperationClosureDatetime.Value.Date).ToList();
        }

        //public async Task<List<TransmissionsGroupedByDay>> ListTransmissionByServerIdOfDay(string rtServer, string date)
        //{
        //    var day = DateTime.ParseExact(
        //               date,
        //               "dd-MM-yyyy",
        //               System.Globalization.CultureInfo.InvariantCulture);

        //    var list = _dbContext.RtServerTransmissionDetail
        //                                          .Include(x => x.RtServerTransmissionDetailRtData)
        //                                          .Include(x => x.RtServerTransmissionDetailRtReport)
        //                                          .Where(x => x.SzRtServerId == rtServer && x.DRtDeviceClosureDateTime.Value.Date == day.Date)
        //                                          .ToList()
        //                                          .GroupBy(x => new { x.DRtDeviceClosureDateTime.Value.Date, x.SzRtDeviceId })
        //                                          //.OrderByDescending(x => x.Key)
        //                                          //.FirstOrDefault()
        //                                          //.GroupBy(x => x.SzRtDeviceId)
        //                                          .Select(x => new TransmissionsGroupedByDay
        //                                          {
        //                                              OperationClosureDatetime = x.Max(x => x.DRtDeviceClosureDateTime),
        //                                              SzRtDeviceId = x.Select(x => x.SzRtDeviceId).FirstOrDefault(),
        //                                              SzRtServerId = x.Select(x => x.SzRtServerId).FirstOrDefault(),
        //                                              LRtDeviceTransmissionId = x.Max(x => x.LRtDeviceTransmissionId),
        //                                              LRtServerOperationId = x.Max(x => x.LRtServerOperationId),
        //                                              BTransactionCheckedFlag = x.Select(x => x.BTransactionCheckedFlag).ToList(),
        //                                              BTransactionArchivedFlag = x.Select(x => x.BTransactionArchivedFlag).ToList(),
        //                                              SzTranscationCheckNote = x.Select(x => x.SzTranscationCheckNote).ToList(),
        //                                              SzUserName = x.Select(x => x.SzUserName).ToList(),
        //                                              TotalAmount = x.Sum(x => x.RtServerTransmissionDetailRtData.Sum(x => x.DSaleAmount + x.DVatAmount
        //                                                                                                                - x.DVoidAmount - x.DReturnAmount)),
        //                                              TotalRtServer = 0,
        //                                              TotalTP = 0,
        //                                              RtServerTransmissionsDetailRtData = x.SelectMany(v => v.RtServerTransmissionDetailRtData).ToList(),
        //                                              RtServerTransmissionsDetailRtReport = x.SelectMany(v => v.RtServerTransmissionDetailRtReport).ToList()

        //                                          })
        //                                         ;

        //    return list.ToList();
        //}
        public async Task<List<TransmissionsGroupedByDay>> ListTransmissionByServerIdOfDay(string rtServer, string date)
        {
            var day = DateTime.ParseExact(
                          date,
                          "dd-MM-yyyy",
                          System.Globalization.CultureInfo.InvariantCulture);
            var baseQ = _dbContext.RtServerTransmissionDetail.Where(x => x.SzRtServerId.Contains(rtServer)
                                                     && x.DRtDeviceClosureDateTime.Value.Date==day.Date
                                                    )
                                    .Select(x => new RtServerTransmissionDetail
                                    {
                                        SzRtServerId = x.SzRtServerId
                                        ,
                                        SzRtDeviceId = x.SzRtDeviceId
                                        ,
                                        BTransactionArchivedFlag = x.BTransactionArchivedFlag
                                        ,
                                        BTransactionCheckedFlag = x.BTransactionCheckedFlag
                                        ,
                                        DRtDeviceClosureDateTime = x.DRtDeviceClosureDateTime
                                        ,
                                        LRtDeviceTransmissionId = x.LRtDeviceTransmissionId
                                        ,
                                        LRtServerOperationId = x.LRtServerOperationId
                                        ,
                                        SzTranscationCheckNote = x.SzTranscationCheckNote
                                        ,
                                        SzUserName = x.SzUserName
                                        ,
                                        RtServerTransmissionDetailRtData = x.RtServerTransmissionDetailRtData
                                                                            .Select(x => new RtServerTransmissionDetailRtData
                                                                            {
                                                                                BVatVentilation = x.BVatVentilation
                                                                                ,
                                                                                DReturnAmount = x.DReturnAmount
                                                                                ,
                                                                                DSaleAmount = x.DSaleAmount
                                                                                ,
                                                                                DVatAmount = x.DVatAmount
                                                                                ,
                                                                                DVatRate = x.DVatRate
                                                                                ,
                                                                                DVoidAmount = x.DVoidAmount
                                                                                ,
                                                                                LRtDeviceTransmissionId = x.LRtDeviceTransmissionId
                                                                                ,
                                                                                LRtServerOperationId = x.LRtServerOperationId
                                                                                ,
                                                                                SzRtDeviceId = x.SzRtDeviceId
                                                                                ,
                                                                                SzRtServerId = x.SzRtServerId
                                                                                ,
                                                                                SzVatNature = x.SzVatNature
                                                                            }).ToList()
                                        ,
                                        RtServerTransmissionDetailRtReport = x.RtServerTransmissionDetailRtReport
                                                                             .Select(x => new RtServerTransmissionDetailRtReport
                                                                             {
                                                                                 SzRtServerId = x.SzRtServerId
                                                                                ,
                                                                                 DEventDateTime = x.DEventDateTime
                                                                                ,
                                                                                 LRtDeviceTransmissionId = x.LRtDeviceTransmissionId
                                                                                ,
                                                                                 LRtServerOperationId = x.LRtServerOperationId
                                                                                ,
                                                                                 SzEventNote = x.SzEventNote
                                                                                ,
                                                                                 SzRtDeviceId = x.SzRtDeviceId
                                                                             }).ToList()
                                    });
            var listTrsm = baseQ;
            // baseQ.Include(x => x.RtServerTransmissionDetailRtData).Load();
            //baseQ.Include(x => x.RtServerTransmissionDetailRtReport).Load();
            var listRes = listTrsm.ToList()
                                .GroupBy(x => new { x.DRtDeviceClosureDateTime.Value.Date, x.SzRtDeviceId },
                                 (key, group) => new TransmissionsGroupedByDay
                                 {
                                     OperationClosureDatetime = key.Date,
                                     SzRtDeviceId = key.SzRtDeviceId,
                                     SzRtServerId = rtServer,
                                     LRtDeviceTransmissionId = group.Max(x => x.LRtDeviceTransmissionId),
                                     LRtDeviceTransmissionIdS = string.Join(" / ",group.Select(x => x.LRtDeviceTransmissionId).Distinct()),
                                     LRtServerOperationIdS = string.Join(" / ",group.Select(x => x.LRtServerOperationId).Distinct()),
                                     LRtServerOperationId = group.Max(x => x.LRtServerOperationId),
                                     BTransactionCheckedFlag = group.Select(x => x.BTransactionCheckedFlag).ToList(),
                                     BTransactionArchivedFlag = group.Select(x => x.BTransactionArchivedFlag).ToList(),
                                     SzTranscationCheckNote = group.Select(x => x.SzTranscationCheckNote).ToList(),
                                     SzUserName = group.Select(x => x.SzUserName).ToList(),
                                     TotalAmount = group.Sum(x => x.RtServerTransmissionDetailRtData
                                                        .Sum(x => x.DSaleAmount + x.DVatAmount
                                                                  - x.DReturnAmount - x.DVoidAmount)),
                                     TotalRtServer = 0,
                                     TotalTP = 0,
                                     RtServerTransmissionsDetailRtData = group.AsQueryable()
                                     .SelectMany(x => x.RtServerTransmissionDetailRtData)
                                                                                                                      .GroupBy(x => new { x.DVatRate, x.SzVatNature }, (key, group) =>
                                                                                                                          new RtServerTransmissionDetailRtData
                                                                                                                          {
                                                                                                                              BVatVentilation = group.FirstOrDefault().BVatVentilation,
                                                                                                                              DReturnAmount = group.Sum(x => x.DReturnAmount),
                                                                                                                              DSaleAmount = group.Sum(x => x.DSaleAmount),
                                                                                                                              DVatAmount = group.Sum(x => x.DVatAmount),
                                                                                                                              DVatRate = key.DVatRate,
                                                                                                                              DVoidAmount = group.Sum(x => x.DVoidAmount),
                                                                                                                              LRtDataId = group.FirstOrDefault().LRtDataId,
                                                                                                                              LRtDeviceTransmissionId = group.FirstOrDefault().LRtDeviceTransmissionId,
                                                                                                                              LRtServerOperationId = group.FirstOrDefault().LRtServerOperationId,
                                                                                                                              SzRtDeviceId = group.FirstOrDefault().SzRtDeviceId,
                                                                                                                              SzRtServerId = group.FirstOrDefault().SzRtServerId,
                                                                                                                              SzVatNature = key.SzVatNature
                                                                                                                          }).ToList(),
                                     RtServerTransmissionsDetailRtReport = group.ToList()
                                                                                                          .SelectMany(x => x.RtServerTransmissionDetailRtReport)
                                                                                                          .ToList()

                                 }
                    ).OrderBy(x => x.SzRtDeviceId).ToList();
            var test = listRes.OrderByDescending(x => x.OperationClosureDatetime);
            return listRes;
        }

        public async Task<List<TransmissionsGroupedByDay>> ListTransmissionByServerIdOfDay22(string rtServer, string date)
        {
            var day = DateTime.ParseExact(
                          date,
                          "dd-MM-yyyy",
                          System.Globalization.CultureInfo.InvariantCulture);

            var base1 = _dbContext.RtServerTransmissionDetail
                                .Where(x => x.SzRtServerId.Contains(rtServer)
                                && x.DRtDeviceClosureDateTime >= day.Date
                                && x.DRtDeviceClosureDateTime < day.AddDays(1).Date
                                )
                                .Select(x =>
                                new RtServerTransmissionDetail
                                {
                                    SzRtServerId = x.SzRtServerId,
                                    SzRtDeviceId = x.SzRtDeviceId,
                                    BTransactionArchivedFlag = x.BTransactionArchivedFlag,
                                    BTransactionCheckedFlag = x.BTransactionCheckedFlag,
                                    DRtDeviceClosureDateTime = x.DRtDeviceClosureDateTime,
                                    LRtDeviceTransmissionId = x.LRtDeviceTransmissionId,
                                    LRtServerOperationId = x.LRtServerOperationId,
                                    SzTranscationCheckNote = x.SzTranscationCheckNote,
                                    SzUserName = x.SzUserName,
                                    RtServerTransmissionDetailRtData = x.RtServerTransmissionDetailRtData
                                                                            .Select(x => new RtServerTransmissionDetailRtData
                                                                            {
                                                                                BVatVentilation = x.BVatVentilation,
                                                                            }).ToList(),
                                    RtServerTransmissionDetailRtReport = x.RtServerTransmissionDetailRtReport
                                                                             .Select(x => new RtServerTransmissionDetailRtReport
                                                                             {
                                                                                 DEventDateTime = x.DEventDateTime,
                                                                                 SzEventNote = x.SzEventNote
                                                                             }).ToList()
                                }
                                )
                                .AsEnumerable()
                                .GroupBy(x => new { x.DRtDeviceClosureDateTime.Value.Date, x.SzRtDeviceId })
                                .Select(x => new TransmissionsGroupedByDay
                                {
                                    OperationClosureDatetime = x.Key.Date,
                                    SzRtDeviceId = x.Key.SzRtDeviceId,
                                    SzRtServerId = rtServer,
                                    LRtDeviceTransmissionIdS = string.Join(" / ", x.Select(t => t.LRtDeviceTransmissionId).Distinct()),
                                    LRtServerOperationIdS = string.Join(" / ", x.Select(t => t.LRtServerOperationId).Distinct()),
                                    BTransactionCheckedFlag = x.Select(x => x.BTransactionCheckedFlag).ToList(),
                                    BTransactionArchivedFlag = x.Select(x => x.BTransactionArchivedFlag).ToList(),
                                    SzTranscationCheckNote = x.Select(x => x.SzTranscationCheckNote).ToList(),
                                    SzUserName = x.Select(x => x.SzUserName).ToList(),
                                    TotalRtServer = 0,
                                    TotalTP = 0,
                                    TotalAmount=0,
                                    RtServerTransmissionsDetailRtData = x.AsQueryable()
                                     .SelectMany(x => x.RtServerTransmissionDetailRtData).ToList(),

                                    RtServerTransmissionsDetailRtReport = x.AsQueryable()
                                     .SelectMany(x => x.RtServerTransmissionDetailRtReport).ToList()

                                }
                                ).ToList();
            ;
            var baseTot = _dbContext.trxResult(rtServer
                                            , day.Date.ToString("yyyy-MM-dd")
                                            , day.AddDays(1).Date.ToString("yyyy-MM-dd")
                                           )
                .ToList()
                ;
            foreach (var tt in base1)
            {
                var found = baseTot.Where(x => x.DRtDeviceClosureDateTime.Value.Date == tt.OperationClosureDatetime.Value.Date
                                              && x.SzRtDeviceId == tt.SzRtDeviceId);
                if (found != null && found.Count()>0)
                {
                    tt.TotalAmount = found.Sum(x => x.SumValue);

                    tt.RtServerTransmissionsDetailRtData = found
                        .Select(x => new RtServerTransmissionDetailRtData
                        {
                            BVatVentilation = tt.RtServerTransmissionsDetailRtData.FirstOrDefault().BVatVentilation,
                            DReturnAmount = x.dReturnAmount,
                            DSaleAmount = x.dSaleAmount,
                            DVatAmount = x.dVatAmount,
                            DVatRate = x.DVatRate,
                            DVoidAmount = x.dVoidAmount,
                            SzRtDeviceId = x.SzRtDeviceId,
                            SzVatNature = x.SzVatNature,
                            SzRtServerId = rtServer
                        }).ToList();

                }
            }
            var listRes = base1.OrderBy(x => x.SzRtDeviceId).ToList();
            //var test = listRes.OrderByDescending(x => x.OperationClosureDatetime);


            /////////////////////////////////////////////////////////////////////////////

          
            return listRes;
        }
        public async Task<List<TransmissionsGroupedByDay>> ListTransmissionByServerOfDay(string rtServer, DateTime date)
        {
            var list = _dbContext.RtServerTransmissionDetail.Include(x => x.RtServerTransmissionDetailRtData)
                                                  .Include(x => x.RtServerTransmissionDetailRtReport)
                                                  .Where(x => x.SzRtServerId == rtServer && x.DRtDeviceClosureDateTime.Value.Date == date.Date)
                                                  .GroupBy(x => new { x.DRtDeviceClosureDateTime.Value.Date })
                                                  .OrderByDescending(x => x.Key).FirstOrDefault()
                                                  .GroupBy(x => x.SzRtDeviceId)
                                                  .Select(x => new TransmissionsGroupedByDay
                                                  {
                                                      OperationClosureDatetime = x.Max(x => x.DRtDeviceClosureDateTime),
                                                      SzRtDeviceId = x.Select(x => x.SzRtDeviceId).FirstOrDefault(),
                                                      SzRtServerId = x.Select(x => x.SzRtServerId).FirstOrDefault(),
                                                      LRtDeviceTransmissionId = x.Max(x => x.LRtDeviceTransmissionId),
                                                      LRtServerOperationId = x.Max(x => x.LRtServerOperationId),
                                                      BTransactionCheckedFlag = x.Select(x => x.BTransactionCheckedFlag).ToList(),
                                                      BTransactionArchivedFlag = x.Select(x => x.BTransactionArchivedFlag).ToList(),
                                                      SzTranscationCheckNote = x.Select(x => x.SzTranscationCheckNote).ToList(),
                                                      SzUserName = x.Select(x => x.SzUserName).ToList(),
                                                      TotalAmount = x.Sum(x => x.RtServerTransmissionDetailRtData.Sum(x => x.DSaleAmount + x.DVatAmount
                                                                                                                        - x.DVoidAmount - x.DReturnAmount)),
                                                      TotalRtServer = 0,
                                                      TotalTP = 0

                                                  })
                                                 ;
            var listGlobal = _dbContext.RtServerTransmissionDetail
                                .Include(x => x.RtServerTransmissionDetailRtData)
                                .GroupBy(x => new { x.DRtDeviceClosureDateTime.Value.Date, x.SzRtDeviceId },
                                 (key, group) => new TransmissionsGroupedByDay
                                 {
                                     OperationClosureDatetime = key.Date,
                                     SzRtDeviceId = key.SzRtDeviceId,
                                     SzRtServerId = rtServer,
                                     LRtDeviceTransmissionId = group.Max(x => x.LRtDeviceTransmissionId),
                                     LRtServerOperationId = group.Max(x => x.LRtServerOperationId),
                                     BTransactionCheckedFlag = group.Select(x => x.BTransactionCheckedFlag).ToList(),
                                     BTransactionArchivedFlag = group.Select(x => x.BTransactionArchivedFlag).ToList(),
                                     SzTranscationCheckNote = group.Select(x => x.SzTranscationCheckNote).ToList(),
                                     SzUserName = group.Select(x => x.SzUserName).ToList(),
                                     TotalAmount = group.Sum(x => x.RtServerTransmissionDetailRtData
                                                        .Sum(x => x.DSaleAmount + x.DVatAmount
                                                                  - x.DReturnAmount - x.DVoidAmount)),
                                     TotalRtServer = 0,
                                     TotalTP = 0

                                 }
                                 )
                                .OrderBy(x => x.SzRtDeviceId).ToList();

            listGlobal.ForEach(x =>
            {
                if (x.OperationClosureDatetime.Value.Date == list.FirstOrDefault().OperationClosureDatetime.Value.Date)
                {
                    x = list.FirstOrDefault();
                }
            });
            return listGlobal.OrderByDescending(x => x.OperationClosureDatetime.Value.Date).ToList();
        }


        public bool RemoveTransmission(RtServerTransmission transmission)
        {
            try
            {
                _dbContext.RtServerTransmission.Remove(transmission);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public List<RtServerTransmissionDetail> GetTrasmissionRecapLastClosure()
        {
            return _dbContext.RtServerTransmissionDetail
                             .Include(x => x.RtServerTransmissionDetailRtData)
                             .ToList();
        }

        public Task<List<UserActivityLog>> GetUserActivityByTransaction(string rtserverId)
        {
            return _dbContext.UserActivityLog
                      .Where(x => x.SzRtServerId == rtserverId && x.SzTablename.Contains("RtServer_TransmissionDetail"))
                      .ToListAsync();
        }
        public Task<IEnumerable<ListServersStatusHomeBYDay>> ListTransmissionAllServersNonCompliantByDate(IEnumerable<string> list, DateTime trxDateFrom, DateTime trxDateTo)
        {
            var baseQ = _dbContext.RtServerTransmissionDetail.Where(x => x.DRtDeviceClosureDateTime >= trxDateFrom.Date
                                                                       && x.DRtDeviceClosureDateTime < trxDateTo.AddDays(1).Date
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
                                                              })  ;
            return Task.FromResult(baseQ);
        }

       
        public async Task<List<TransmissionsList>> GetTransmissionsByDay(string userName, string id, int storeId, int storeGroupId, string date)
        {
            var transmissionsByDayV = new List<TransmissionsList>();
            try
            {
                transmissionsByDayV = await _dbContext.TrasmissionsByDayToV(userName, id, storeId, storeGroupId, date).ToListAsync();

            }
            catch (Exception)
            { }
            return transmissionsByDayV;
        }
    }
}
