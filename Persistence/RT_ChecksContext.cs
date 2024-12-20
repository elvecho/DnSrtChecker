﻿using System.IO;
using System.Linq;
using DnSrtChecker.Models;
using DnSrtChecker.Models.ViewModels;
using DnSrtChecker.Models.ViewModels.ModelsFunctions;
using DnSrtChecker.ModelsHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DnSrtChecker.Persistence
{
    public partial class RT_ChecksContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>,
                                                           UserRole, IdentityUserLogin<string>,
                                                           IdentityRoleClaim<string>,
                                                           IdentityUserToken<string>>
    {
        public IQueryable<TrxGroupedByDay> trxResult(string srv, string dfrom, string dto) =>
        Set<TrxGroupedByDay>().FromSqlInterpolated($"select * from TRX.[01_query_01_01_Totals]({srv},{dfrom},{dto})");

        //Home page
        public IQueryable<TrxRTServer> trxTotalsResult(string Username, string dfrom, string dto, string ServerRt = null, string Store = null, string StoreGroup = null,
         string Status = null, string Error = null, string Warning = null) => 
            Set<TrxRTServer>().
            FromSqlInterpolated($"SELECT  *, case when  TotalADE = TotalRT and TotalRT = TotalTP then 0 else 1 end as TrasnmissionError from TRX.[TotalNegozio_Trasmissioni_BYDATE]({dfrom},{dto}, {ServerRt} , {Store} , {StoreGroup} , {Status} , {Error}, {Warning},{Username}) order by TrasnmissionError  DESC, LRetailStoreID ");

        //Lista dei server by user
        public IQueryable<RtServerByUser> rtServerByUser(string Username) =>
            Set<RtServerByUser>().
            FromSqlInterpolated($"Select * from TRX.[RtServerParam]({Username})");
        
        public IQueryable<TransactionList> TransactionsByDate(string UserName, string RtDeviceClosureDateTime, string ServerRt, string RtDeviceID=null, string Store=null, string PosWorkstationNmbr = null, string TransactionCheckedFlag = null, string TransactionArchivedFlag = null, string HasMismatch = null, string RtNonCompliantFlag = null, string PosTaNmbr = null, string RtClosureNmbr = null, string RtDocumentNmbr = null) =>
           Set<TransactionList>().FromSqlInterpolated($"select * from TRX.[Dettagli_Transazioni]({RtDeviceClosureDateTime},{ServerRt},{RtDeviceID},{Store},{PosWorkstationNmbr},{PosTaNmbr},{RtClosureNmbr},{RtDocumentNmbr},{TransactionCheckedFlag},{TransactionArchivedFlag},{HasMismatch},{RtNonCompliantFlag},{UserName}) order by bRtNonCompliantFlag Desc,  lTransactionMismatchID Desc, lPosWorkstationNmbr , lPosTaNmbr");

        public IQueryable<TransmissionsList> TrasmissionsByDayToV(string userName, string id, int storeId, int storeGroupId, string date) =>
        Set<TransmissionsList>().FromSqlInterpolated($"select * from [TRX].[TotaCasseNegozio_Trasmissioni_BYDATE]({date},{date},{storeId},{userName})");

        public IQueryable<TRNTotalsByDaysAndServer> trnTotalsResult(string srv,string dfrom, string dto) =>
        Set<TRNTotalsByDaysAndServer>().FromSqlInterpolated($"select * from TRX.[01_query_02_Totals]({srv},{dfrom},{dto})");

        public IQueryable<TRNTotalsByServer> trnJustTotalsResult(string srv, string dfrom, string dto) =>
        Set<TRNTotalsByServer>().FromSqlInterpolated($"select * from TRX.[01_query_03_Totals]({srv},{dfrom},{dto})");

        public IQueryable<TRNTotalsByDaysAndServer> trxTotalsResult22(string srv, string dfrom, string dto) =>
        Set<TRNTotalsByDaysAndServer>().FromSqlInterpolated($"select * from TRX.[02_query_01_Totals]({srv},{dfrom},{dto})");


        public RT_ChecksContext(DbContextOptions<RT_ChecksContext> options)
           : base(options)
        {
            
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TrxGroupedByDay>().HasNoKey().ToView(null);
            modelBuilder.Entity<TrxRTServer>().HasNoKey().ToView(null);
            modelBuilder.Entity<TRXTotalsByDays>().HasNoKey().ToView(null);
            modelBuilder.Entity<TRNTotalsByDaysAndServer>().HasNoKey().ToView(null);
            modelBuilder.Entity<TRNTotalsByServer>().HasNoKey().ToView(null);
            modelBuilder.Entity<TransactionList>().HasNoKey().ToView(null);
            modelBuilder.Entity<TransmissionsList>().HasNoKey().ToView(null);
            modelBuilder.Entity<CampiDaMarco>().HasNoKey().ToView(null);
            modelBuilder.Entity<RtServerByUser>().HasNoKey().ToView(null);
            //modelBuilder.Entity<TransmissionsByDayToIndexView>().HasNoKey().ToView(null);

            modelBuilder.Entity<UserRole>(userRole =>
             {
                 userRole.HasKey(ur => new { ur.UserId, ur.RoleId });
                 userRole.HasOne(ur => ur.Role)
                 .WithMany(r => r.UserRoles)
                 .HasForeignKey(ur => ur.RoleId)
                 .IsRequired();

                 userRole.HasOne(ur => ur.User)
                     .WithMany(r => r.UserRoles)
                     .HasForeignKey(ur => ur.UserId)
                     .IsRequired();
             });

            modelBuilder.Entity<DirTree>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK__DirTree__3214EC06F3561F45")
                    .IsClustered(false);

                entity.ToTable("DirTree", "LOAD");

                entity.HasAnnotation("SqlServer:MemoryOptimized", true);

                entity.Property(e => e.SubDirectory)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.HasKey(e => e.LDocumentTypeId);

                entity.ToTable("Document_Type", "LOOKUP");

                entity.HasIndex(e => e.DLastUpdateLocal)
                    .HasName("IDX_Document_Type_#1");

                entity.HasIndex(e => e.SzDescription)
                    .HasName("IDX_Document_Type_#2");

                entity.Property(e => e.LDocumentTypeId)
                    .HasColumnName("lDocumentTypeID")
                    .ValueGeneratedNever();

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.SzDescription)
                    .HasColumnName("szDescription")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<IndexMaintenance>(entity =>
            {
                entity.HasKey(e => new { e.DDate, e.SzSchemaName, e.SzTableName, e.SzIndexName });

                entity.ToTable("IndexMaintenance", "LOGGING");

                entity.HasIndex(e => e.DLastUpdateLocal)
                    .HasName("IDX_IndexMaintenance_#1");

                entity.Property(e => e.DDate)
                    .HasColumnName("dDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.SzSchemaName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SzTableName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SzIndexName)
                    .HasColumnName("szIndexName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.LPostPercFragmentation).HasColumnName("lPostPercFragmentation");

                entity.Property(e => e.LPrePercFragmentation).HasColumnName("lPrePercFragmentation");

                entity.Property(e => e.SzActionToDo)
                    .HasColumnName("szActionToDo")
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MailActivityLog>(entity =>
            {
                entity.HasKey(e => e.LMailActivityLogId);

                entity.ToTable("MailActivityLog", "LOGGING");

                entity.Property(e => e.LMailActivityLogId).HasColumnName("lMailActivityLogID");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.DMailSentDateTime)
                    .HasColumnName("dMailSentDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.SzBody)
                    .HasColumnName("szBody")
                    .IsUnicode(false);

                entity.Property(e => e.SzRecipients)
                    .IsRequired()
                    .HasColumnName("szRecipients")
                    .IsUnicode(false);

                entity.Property(e => e.SzSubject)
                    .HasColumnName("szSubject")
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MailRecipient>(entity =>
            {
                entity.HasKey(e => e.LStoreGroupId);

                entity.ToTable("Mail_Recipient", "LOOKUP");

                entity.Property(e => e.LStoreGroupId)
                    .HasColumnName("lStoreGroupID")
                    .ValueGeneratedNever();

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.SzRecipients)
                    .IsRequired()
                    .HasColumnName("szRecipients")
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RtServer>(entity =>
            {
                entity.HasKey(e => new { e.SzRtServerId, e.LRetailStoreId, e.LStoreGroupId });

                entity.ToTable("RtServer", "LOOKUP");

                //entity.HasIndex(e => e.DLastUpdateLocal)
                //    .HasName("IDX_RtServer_#1");

                entity.Property(e => e.SzRtServerId)
                    .HasColumnName("szRtServerID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LRetailStoreId).HasColumnName("lRetailStoreID");

                entity.Property(e => e.LStoreGroupId).HasColumnName("lStoreGroupID");

                entity.Property(e => e.BOnDutyFlag).HasColumnName("bOnDutyFlag");

                //entity.Property(e => e.DLastUpdateLocal)
                //    .HasColumnName("dLastUpdateLocal")
                //    .HasColumnType("datetime");

                entity.Property(e => e.SzIpAddress)
                    .HasColumnName("szIpAddress")
                    .HasMaxLength(100);

                entity.Property(e => e.SzLocation)
                    .HasColumnName("szLocation")
                    .HasMaxLength(100);

                entity.Property(e => e.SzPassword)
                    .HasColumnName("szPassword")
                    .HasMaxLength(100);

                entity.Property(e => e.SzUsername)
                    .HasColumnName("szUsername")
                    .HasMaxLength(100);

                entity.HasOne(d => d.L)
                    .WithMany(p => p.RtServer)
                    .HasForeignKey(d => new { d.LRetailStoreId, d.LStoreGroupId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RtServer_#1");
            });

            modelBuilder.Entity<RtServerStatus>(entity =>
            {
                entity.HasKey(e => new { e.SzRtServerId, e.LRetailStoreId, e.LStoreGroupId })
                    .HasName("PK_ServerRt_Status");

                entity.ToTable("RtServer_Status", "LOOKUP");

                entity.HasIndex(e => e.DLastDateTimeTransactionsCollected)
                    .HasName("IDX_RtServer_Status_#3");

                entity.HasIndex(e => e.DLastDateTimeTransmissionsCollected)
                    .HasName("IDX_RtServer_Status_#2");

                entity.HasIndex(e => e.DLastUpdateLocal)
                    .HasName("IDX_RtServer_Status_#1");

                entity.Property(e => e.SzRtServerId)
                    .HasColumnName("szRtServerID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LRetailStoreId).HasColumnName("lRetailStoreID");

                entity.Property(e => e.LStoreGroupId).HasColumnName("lStoreGroupID");

                entity.Property(e => e.BOnErrorFlag).HasColumnName("bOnErrorFlag");

                entity.Property(e => e.BRunningTransmissionFlag).HasColumnName("bRunningTransmissionFlag");

                entity.Property(e => e.BVatVentilationFlag).HasColumnName("bVatVentilationFlag");

                entity.Property(e => e.BWarningFlag).HasColumnName("bWarningFlag");

                entity.Property(e => e.DGrandTotalAmount)
                    .HasColumnName("dGrandTotalAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DLastDateTimeCollected)
                    .HasColumnName("dLastDateTimeCollected")
                    .HasColumnType("datetime");

                entity.Property(e => e.DLastDateTimeRead)
                    .HasColumnName("dLastDateTimeRead")
                    .HasColumnType("datetime");

                entity.Property(e => e.DLastDateTimeTransactionsCollected)
                    .HasColumnName("dLastDateTimeTransactionsCollected")
                    .HasColumnType("datetime");

                entity.Property(e => e.DLastDateTimeTransmissionsCollected)
                    .HasColumnName("dLastDateTimeTransmissionsCollected")
                    .HasColumnType("datetime");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.LLastClosureNmbr).HasColumnName("lLastClosureNmbr");

                entity.Property(e => e.LMemoryAvailable).HasColumnName("lMemoryAvailable");

                entity.Property(e => e.LPendingTransmissionDays).HasColumnName("lPendingTransmissionDays");

                entity.Property(e => e.LPendingTransmissionNmbr).HasColumnName("lPendingTransmissionNmbr");

                entity.Property(e => e.LTransmissionScheduleHoursRepeat).HasColumnName("lTransmissionScheduleHoursRepeat");

                entity.Property(e => e.LTransmissionScheduleMinutesLeft).HasColumnName("lTransmissionScheduleMinutesLeft");

                entity.Property(e => e.SzErrorDescription)
                    .HasColumnName("szErrorDescription")
                    .HasMaxLength(255);

                entity.Property(e => e.SzLastCloseResult)
                    .HasColumnName("szLastCloseResult")
                    .HasMaxLength(255);

                entity.HasOne(d => d.RtServer)
                    .WithOne(p => p.RtServerStatus)
                    .HasForeignKey<RtServerStatus>(d => new { d.SzRtServerId, d.LRetailStoreId, d.LStoreGroupId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RtServer_Status_#1");
            });

            modelBuilder.Entity<RtServerTransmission>(entity =>
            {
                entity.HasKey(e => new { e.SzRtServerId, e.LRtServerOperationId })
                    .HasName("PK_ServerRt_Transmission");

                entity.ToTable("RtServer_Transmission", "TRX");

                entity.HasIndex(e => e.DLastUpdateLocal)
                    .HasName("IDX_RtServer_Transmission_#1");

                entity.Property(e => e.SzRtServerId)
                    .HasColumnName("szRtServerID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LRtServerOperationId).HasColumnName("lRtServerOperationID");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<RtServerTransmissionDetail>(entity =>
            {
                entity.HasKey(e => new { e.SzRtServerId, e.LRtServerOperationId, e.LRtDeviceTransmissionId, e.SzRtDeviceId });

                entity.ToTable("RtServer_TransmissionDetail", "TRX");

                entity.HasIndex(e => e.DLastUpdateLocal)
                    .HasName("IDX_RtServer_TransmissionDetail_#1");

                entity.HasIndex(e => e.DRtDeviceClosureDateTime)
                    .HasName("IDX_TRtServer_TransmissionDetail_#2");

                entity.Property(e => e.SzRtServerId)
                    .HasColumnName("szRtServerID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LRtServerOperationId).HasColumnName("lRtServerOperationID");

                entity.Property(e => e.LRtDeviceTransmissionId).HasColumnName("lRtDeviceTransmissionID");

                entity.Property(e => e.SzRtDeviceId)
                    .HasColumnName("szRtDeviceID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BTransactionArchivedFlag).HasColumnName("bTransactionArchivedFlag");

                entity.Property(e => e.BTransactionCheckedFlag).HasColumnName("bTransactionCheckedFlag");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.DRtDeviceClosureDateTime)
                    .HasColumnName("dRtDeviceClosureDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.DRtInactivityDateTimeFrom)
                    .HasColumnName("dRtInactivityDateTimeFrom")
                    .HasColumnType("datetime");

                entity.Property(e => e.DRtInactivityDateTimeTo)
                    .HasColumnName("dRtInactivityDateTimeTo")
                    .HasColumnType("datetime");

                entity.Property(e => e.SzRtDeviceType)
                    .HasColumnName("szRtDeviceType")
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.SzRtTransmissionFormat)
                    .HasColumnName("szRtTransmissionFormat")
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.SzTranscationCheckNote)
                    .HasColumnName("szTranscationCheckNote")
                    .HasMaxLength(255);

                entity.Property(e => e.SzUserName)
                    .HasColumnName("szUserName")
                    .HasMaxLength(256);

                entity.HasOne(d => d.RtServerTransmission)
                    .WithMany(p => p.RtServerTransmissionDetail)
                    .HasForeignKey(d => new { d.SzRtServerId, d.LRtServerOperationId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServerRt_TransmissionDetail_#1");
            });

            modelBuilder.Entity<RtServerTransmissionDetailRtData>(entity =>
            {
                entity.HasKey(e => e.LRtDataId)
                    .HasName("PK_ServerRT_TransmissionDetail_RtData");

                entity.ToTable("RtServer_TransmissionDetail_RtData", "TRX");

                entity.HasIndex(e => e.DLastUpdateLocal)
                    .HasName("IDX_RtServer_TransmissionDetail_RtData_#1");

                entity.HasIndex(e => new { e.SzRtServerId, e.LRtServerOperationId, e.LRtDeviceTransmissionId, e.SzRtDeviceId })
                    .HasName("IDX_RtServer_TransmissionDetail_RtData_#2");

                entity.Property(e => e.LRtDataId).HasColumnName("lRtDataID");

                entity.Property(e => e.BVatVentilation).HasColumnName("bVatVentilation");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.DReturnAmount)
                    .HasColumnName("dReturnAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DSaleAmount)
                    .HasColumnName("dSaleAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DVatAmount)
                    .HasColumnName("dVatAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DVatRate)
                    .HasColumnName("dVatRate")
                    .HasColumnType("decimal(6, 4)");

                entity.Property(e => e.DVoidAmount)
                    .HasColumnName("dVoidAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.LRtDeviceTransmissionId).HasColumnName("lRtDeviceTransmissionID");

                entity.Property(e => e.LRtServerOperationId).HasColumnName("lRtServerOperationID");

                entity.Property(e => e.SzRtDeviceId)
                    .IsRequired()
                    .HasColumnName("szRtDeviceID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SzRtServerId)
                    .IsRequired()
                    .HasColumnName("szRtServerID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SzVatLegalReference)
                    .HasColumnName("szVatLegalReference")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SzVatNature)
                    .HasColumnName("szVatNature")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.RtServerTransmissionDetail)
                    .WithMany(p => p.RtServerTransmissionDetailRtData)
                    .HasForeignKey(d => new { d.SzRtServerId, d.LRtServerOperationId, d.LRtDeviceTransmissionId, d.SzRtDeviceId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServerRT_TransmissionDetail_RtData_#1");
            });

            modelBuilder.Entity<RtServerTransmissionDetailRtReport>(entity =>
            {
                entity.HasKey(e => e.LRtReportId)
                    .HasName("PK_ServerRt_Report");

                entity.ToTable("RtServer_TransmissionDetail_RtReport", "TRX");

                entity.HasIndex(e => e.DLastUpdateLocal)
                    .HasName("IDX_RtServer_TransmissionDetail_RtReport_#1");

                entity.HasIndex(e => new { e.SzRtServerId, e.LRtServerOperationId, e.LRtDeviceTransmissionId, e.SzRtDeviceId })
                    .HasName("IDX_RtServer_TransmissionDetail_RtReport_#2");

                entity.Property(e => e.LRtReportId).HasColumnName("lRtReportID");

                entity.Property(e => e.DEventDateTime)
                    .HasColumnName("dEventDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.LRtDeviceTransmissionId).HasColumnName("lRtDeviceTransmissionID");

                entity.Property(e => e.LRtServerOperationId).HasColumnName("lRtServerOperationID");

                entity.Property(e => e.SzEventNote)
                    .HasColumnName("szEventNote")
                    .HasMaxLength(255);

                entity.Property(e => e.SzEventType)
                    .HasColumnName("szEventType")
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.SzRtDeviceId)
                    .IsRequired()
                    .HasColumnName("szRtDeviceID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SzRtServerId)
                    .IsRequired()
                    .HasColumnName("szRtServerID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.RtServerTransmissionDetail)
                    .WithMany(p => p.RtServerTransmissionDetailRtReport)
                    .HasForeignKey(d => new { d.SzRtServerId, d.LRtServerOperationId, d.LRtDeviceTransmissionId, d.SzRtDeviceId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServerRt_TransmissionDetail_RtReport_#1");
            });

            modelBuilder.Entity<RtServerVat>(entity =>
            {
                entity.HasKey(e => e.SzVatCodeId)
                    .HasName("PK_RtServerVAT");

                entity.ToTable("RtServer_VAT", "LOOKUP");

                entity.HasIndex(e => e.DLastUpdateLocal)
                    .HasName("IDX_RtServerVAT_#1");

                entity.Property(e => e.SzVatCodeId)
                    .HasColumnName("szVatCodeID")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.SzDescription)
                    .HasColumnName("szDescription")
                    .HasMaxLength(100);

                entity.Property(e => e.SzVatNature)
                    .IsRequired()
                    .HasColumnName("szVatNature")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasKey(e => new { e.LRetailStoreId, e.LStoreGroupId });

                entity.ToTable("Store", "LOOKUP");

                entity.HasIndex(e => e.DLastUpdateLocal)
                    .HasName("IDX_Store_#1");

                entity.Property(e => e.LRetailStoreId).HasColumnName("lRetailStoreID");

                entity.Property(e => e.LStoreGroupId).HasColumnName("lStoreGroupID");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.SzDescription)
                    .IsRequired()
                    .HasColumnName("szDescription")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.LStoreGroup)
                    .WithMany(p => p.Store)
                    .HasForeignKey(d => d.LStoreGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Store_#1");
            });

            modelBuilder.Entity<StoreGroup>(entity =>
            {
                entity.HasKey(e => e.LStoreGroupId)
                    .HasName("PK_StoreGroup");

                entity.ToTable("Store_Group", "LOOKUP");

                entity.HasIndex(e => e.DLastUpdateLocal)
                    .HasName("IDX_Store_Group_#1");

                entity.Property(e => e.LStoreGroupId)
                    .HasColumnName("lStoreGroupID")
                    .ValueGeneratedNever();

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.SzDescription)
                    .IsRequired()
                    .HasColumnName("szDescription")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<TransactionAffiliation>(entity =>
            {
                entity.HasKey(e => e.SzRtDocumentId);

                entity.ToTable("Transaction_Affiliation", "TRN");

                entity.HasIndex(e => e.DLastUpdateLocal)
                    .HasName("IDX_Transaction_Affiliation_#1");

                entity.HasIndex(e => e.LTransactionMismatchId)
                    .HasName("IDX_Transaction_Affiliation_#3");

                entity.HasIndex(e => new { e.DPosDateTime, e.DRtDateTime })
                    .HasName("IDX_Transaction_Affiliation_#4");

                entity.HasIndex(e => new { e.SzRtServerId, e.DPosDateTime })
                    .HasName("IDX_Transaction_Affiliation_#2");

                entity.Property(e => e.SzRtDocumentId)
                    .HasColumnName("szRtDocumentID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.BRtNonCompliantFlag).HasColumnName("bRtNonCompliantFlag");

                entity.Property(e => e.BTransactionArchivedFlag).HasColumnName("bTransactionArchivedFlag");

                entity.Property(e => e.BTransactionCheckedFlag).HasColumnName("bTransactionCheckedFlag");

                entity.Property(e => e.DBusinessDate)
                    .HasColumnName("dBusinessDate")
                    .HasColumnType("date");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.DPosDateTime)
                    .HasColumnName("dPosDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.DPosTransactionTurnover)
                    .HasColumnName("dPosTransactionTurnover")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DRtDateTime)
                    .HasColumnName("dRtDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.DRtTransactionTurnover)
                    .HasColumnName("dRtTransactionTurnover")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.LPosReceivedTransactionCounter).HasColumnName("lPosReceivedTransactionCounter");

                entity.Property(e => e.LPosTaNmbr).HasColumnName("lPosTaNmbr");

                entity.Property(e => e.LPosWorkstationNmbr).HasColumnName("lPosWorkstationNmbr");

                entity.Property(e => e.LRetailStoreId).HasColumnName("lRetailStoreID");

                entity.Property(e => e.LRtClosureNmbr).HasColumnName("lRtClosureNmbr");

                entity.Property(e => e.LRtDocumentNmbr).HasColumnName("lRtDocumentNmbr");

                entity.Property(e => e.LRtReceivedTransactionCounter).HasColumnName("lRtReceivedTransactionCounter");

                entity.Property(e => e.LStoreGroupId).HasColumnName("lStoreGroupID");

                entity.Property(e => e.LTransactionMismatchId).HasColumnName("lTransactionMismatchID");

                entity.Property(e => e.SzRtDeviceId)
                    .HasColumnName("szRtDeviceID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SzRtServerId)
                    .IsRequired()
                    .HasColumnName("szRtServerID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SzTranscationCheckNote)
                    .HasColumnName("szTranscationCheckNote")
                    .HasMaxLength(255);

                entity.Property(e => e.SzUserName)
                    .HasColumnName("szUserName")
                    .HasMaxLength(256)
                    .IsFixedLength();

                entity.HasOne(d => d.LTransactionMismatch)
                    .WithMany(p => p.TransactionAffiliation)
                    .HasForeignKey(d => d.LTransactionMismatchId)
                    .HasConstraintName("FK_Transaction_Affiliation_#2");

                entity.HasOne(d => d.RtServer)
                    .WithMany(p => p.TransactionAffiliation)
                    .HasForeignKey(d => new { d.SzRtServerId, d.LRetailStoreId, d.LStoreGroupId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_Affiliation_#1");
            });

            modelBuilder.Entity<TransactionDocument>(entity =>
            {
                entity.HasKey(e => e.LTransactionDocumentId);

                entity.ToTable("Transaction_Document", "TRN");

                entity.HasIndex(e => e.DLastUpdateLocal)
                    .HasName("IDX_Transaction_Vat_#1");

                entity.HasIndex(e => e.SzRtDocumentId)
                    .HasName("IDX_Transaction_Document_#3");

                entity.HasIndex(e => new { e.LTransactionDocumentId, e.LDocumentTypeId })
                    .HasName("IDX_Transaction_Document_#2");

                entity.Property(e => e.LTransactionDocumentId).HasColumnName("lTransactionDocumentID");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.LDocumentTypeId).HasColumnName("lDocumentTypeID");

                entity.Property(e => e.SzDocumentAttachment)
                    .HasColumnName("szDocumentAttachment")
                    .HasColumnType("xml");

                entity.Property(e => e.SzDocumentAttachmentTxt).HasColumnName("szDocumentAttachmentTxt");

                entity.Property(e => e.SzDocumentName)
                    .HasColumnName("szDocumentName")
                    .HasMaxLength(100);

                entity.Property(e => e.SzDocumentNote)
                    .HasColumnName("szDocumentNote")
                    .HasMaxLength(255);

                entity.Property(e => e.SzRtDocumentId)
                    .IsRequired()
                    .HasColumnName("szRtDocumentID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.HasOne(d => d.LDocumentType)
                    .WithMany(p => p.TransactionDocument)
                    .HasForeignKey(d => d.LDocumentTypeId)
                    .HasConstraintName("FK_Transaction_Document_#1");

                entity.HasOne(d => d.SzRtDocument)
                    .WithMany(p => p.TransactionDocument)
                    .HasForeignKey(d => d.SzRtDocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_Document_#2");
            });

            modelBuilder.Entity<TransactionMismatch>(entity =>
            {
                entity.HasKey(e => e.LTransactionMismatchId);

                entity.ToTable("Transaction_Mismatch", "LOOKUP");

                entity.HasIndex(e => e.DLastUpdateLocal)
                    .HasName("IDX_Transaction_Mismatch_#1");

                entity.Property(e => e.LTransactionMismatchId)
                    .HasColumnName("lTransactionMismatchID")
                    .ValueGeneratedNever();

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.SzDescription)
                    .HasColumnName("szDescription")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<TransactionRtError>(entity =>
            {
                entity.HasKey(e => e.LRtErrorId);

                entity.ToTable("Transaction_RtError", "TRN");

                entity.HasIndex(e => e.DLastUpdateLocal)
                    .HasName("IDX_Transaction_RtError_#1");

                entity.Property(e => e.LRtErrorId).HasColumnName("lRtErrorID");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.DRtDateTime)
                    .HasColumnName("dRtDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.LRetailStoreId).HasColumnName("lRetailStoreID");

                entity.Property(e => e.LRtClosureNmbr).HasColumnName("lRtClosureNmbr");

                entity.Property(e => e.LRtDocumentNmbr).HasColumnName("lRtDocumentNmbr");

                entity.Property(e => e.LStoreGroupId).HasColumnName("lStoreGroupID");

                entity.Property(e => e.SzDescription)
                    .HasColumnName("szDescription")
                    .HasMaxLength(255);

                entity.Property(e => e.SzRtDeviceId)
                    .HasColumnName("szRtDeviceID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SzRtServerId)
                    .IsRequired()
                    .HasColumnName("szRtServerID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.RtServer)
                    .WithMany(p => p.TransactionRtError)
                    .HasForeignKey(d => new { d.SzRtServerId, d.LRetailStoreId, d.LStoreGroupId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_RtError_#1");
            });

            modelBuilder.Entity<TransactionVat>(entity =>
            {
                entity.HasKey(e => new { e.SzRtDocumentId, e.SzVatCodeId });

                entity.ToTable("Transaction_Vat", "TRN");

                entity.HasIndex(e => e.SzRtDocumentId)
                    .HasName("IDX_Transaction_Vat_#1");

                entity.Property(e => e.SzRtDocumentId)
                    .HasColumnName("szRtDocumentID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.SzVatCodeId)
                    .HasColumnName("szVatCodeID")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.BVatCheckedFlag).HasColumnName("bVatCheckedFlag");

                entity.Property(e => e.BVatMismatchFlag).HasColumnName("bVatMismatchFlag");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.DPosGrossAmount)
                    .HasColumnName("dPosGrossAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DPosNetAmount)
                    .HasColumnName("dPosNetAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DPosVatAmount)
                    .HasColumnName("dPosVatAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DPosVatRate)
                    .HasColumnName("dPosVatRate")
                    .HasColumnType("decimal(6, 4)");

                entity.Property(e => e.DRtGrossAmount)
                    .HasColumnName("dRtGrossAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DRtNetAmount)
                    .HasColumnName("dRtNetAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DRtVatAmount)
                    .HasColumnName("dRtVatAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DRtVatRate)
                    .HasColumnName("dRtVatRate")
                    .HasColumnType("decimal(6, 4)");

                entity.HasOne(d => d.SzRtDocument)
                    .WithMany(p => p.TransactionVat)
                    .HasForeignKey(d => d.SzRtDocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_Vat_#1");

                entity.HasOne(d => d.SzVatCode)
                    .WithMany(p => p.TransactionVat)
                    .HasForeignKey(d => d.SzVatCodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_Vat_#2");
            });

            modelBuilder.Entity<UserActivity>(entity =>
            {
                entity.HasKey(e => e.LUserActivityId);

                entity.ToTable("UserActivity", "LOOKUP");

                entity.HasIndex(e => e.DLastUpdateLocal)
                    .HasName("IDX_UserActivity_#1");

                entity.Property(e => e.LUserActivityId).HasColumnName("lUserActivityID");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.SzDescription)
                    .IsRequired()
                    .HasColumnName("szDescription")
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserActivityLog>(entity =>
            {
                entity.HasKey(e => e.LUserActivityLogId);

                entity.ToTable("UserActivityLog", "LOGGING");

                entity.HasIndex(e => e.DLastUpdateLocal)
                    .HasName("IDX_UserActivityLog_#1");

                entity.HasIndex(e => e.SzRtDocumentColumn)
                    .HasName("IDX_UserActivityLog_#4");

                entity.HasIndex(e => e.SzRtDocumentId)
                    .HasName("IDX_UserActivityLog_#3");

                entity.HasIndex(e => e.SzUserName)
                    .HasName("IDX_UserActivityLog_#2");

                entity.Property(e => e.LUserActivityLogId).HasColumnName("lUserActivityLogID");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.DUserActivityDateTime)
                    .HasColumnName("dUserActivityDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.LRtDeviceTransmissionId).HasColumnName("lRtDeviceTransmissionID");

                entity.Property(e => e.LRtServerOperationId).HasColumnName("lRtServerOperationID");

                entity.Property(e => e.LUserActivityId).HasColumnName("lUserActivityID");

                entity.Property(e => e.SzNewValue)
                    .HasColumnName("szNewValue")
                    .HasMaxLength(255);

                entity.Property(e => e.SzOldValue)
                    .HasColumnName("szOldValue")
                    .HasMaxLength(255);

                entity.Property(e => e.SzRtDeviceId)
                    .HasColumnName("szRtDeviceID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SzRtDocumentColumn)
                    .HasColumnName("szRtDocumentColumn")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SzRtDocumentId)
                    .HasColumnName("szRtDocumentID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.SzRtServerId)
                    .HasColumnName("szRtServerID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SzTablename)
                    .HasColumnName("szTablename")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SzUserName)
                    .IsRequired()
                    .HasColumnName("szUserName")
                    .HasMaxLength(256);

                entity.HasOne(d => d.LUserActivity)
                    .WithMany(p => p.UserActivityLog)
                    .HasForeignKey(d => d.LUserActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserActivityLog_#1");
            });

            modelBuilder.Entity<Xmltransactions>(entity =>
            {
                entity.ToTable("XMLTransactions", "LOAD");

                entity.Property(e => e.BodyTransavtion)
                    .IsRequired()
                    .HasColumnType("xml");

                entity.Property(e => e.DtBusinessDate)
                    .HasColumnName("dtBusinessDate")
                    .HasColumnType("date");

                entity.Property(e => e.LPosTaNmbr).HasColumnName("lPosTaNmbr");

                entity.Property(e => e.LPosWorkstationNmbr).HasColumnName("lPosWorkstationNmbr");

                entity.Property(e => e.LRetailStoreId).HasColumnName("lRetailStoreID");

                entity.Property(e => e.LStoreGroupId).HasColumnName("lStoreGroupID");

                entity.Property(e => e.LoadedDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<XmlwithOpenXml>(entity =>
            {
                entity.ToTable("XMLwithOpenXML", "LOAD");

                entity.Property(e => e.LoadedDateTime).HasColumnType("datetime");

                entity.Property(e => e.Xmldata)
                    .IsRequired()
                    .HasColumnName("XMLData")
                    .HasColumnType("xml");
            });
        }
        public virtual DbSet<DirTree> DirTree { get; set; }
        public virtual DbSet<DocumentType> DocumentType { get; set; }
        public virtual DbSet<IndexMaintenance> IndexMaintenance { get; set; }
        public virtual DbSet<MailActivityLog> MailActivityLog { get; set; }
        public virtual DbSet<MailRecipient> MailRecipient { get; set; }
        public virtual DbSet<RtServer> RtServer { get; set; }
        public virtual DbSet<RtServerStatus> RtServerStatus { get; set; }
        public virtual DbSet<RtServerTransmission> RtServerTransmission { get; set; }
        public virtual DbSet<RtServerTransmissionDetail> RtServerTransmissionDetail { get; set; }
        public virtual DbSet<RtServerTransmissionDetailRtData> RtServerTransmissionDetailRtData { get; set; }
        public virtual DbSet<RtServerTransmissionDetailRtReport> RtServerTransmissionDetailRtReport { get; set; }
        public virtual DbSet<RtServerVat> RtServerVat { get; set; }
        public virtual DbSet<Store> Store { get; set; }
        public virtual DbSet<StoreGroup> StoreGroup { get; set; }
        public virtual DbSet<TransactionAffiliation> TransactionAffiliation { get; set; }
        public virtual DbSet<TransactionDocument> TransactionDocument { get; set; }
        public virtual DbSet<TransactionMismatch> TransactionMismatch { get; set; }
        public virtual DbSet<TransactionRtError> TransactionRtError { get; set; }
        public virtual DbSet<TransactionVat> TransactionVat { get; set; }
        public virtual DbSet<UserActivity> UserActivity { get; set; }
        public virtual DbSet<UserActivityLog> UserActivityLog { get; set; }
        public virtual DbSet<Xmltransactions> Xmltransactions { get; set; }
        public virtual DbSet<XmlwithOpenXml> XmlwithOpenXml { get; set; }

    }    
}
