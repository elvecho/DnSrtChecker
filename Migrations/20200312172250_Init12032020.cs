using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DnSrtCheckerLib.Migrations
{
    public partial class Init12032020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "LOOKUP");

            migrationBuilder.EnsureSchema(
                name: "LOGGING");

            migrationBuilder.EnsureSchema(
                name: "TRX");

            migrationBuilder.EnsureSchema(
                name: "TRN");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            //migrationBuilder.CreateTable(
            //    name: "IndexMaintenance",
            //    schema: "LOGGING",
            //    columns: table => new
            //    {
            //        dDate = table.Column<DateTime>(type: "datetime", nullable: false),
            //        SzDbName = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
            //        SzSchemaName = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
            //        SzTableName = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
            //        szIndexName = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
            //        lPrePercFragmentation = table.Column<double>(nullable: true),
            //        szActionToDo = table.Column<string>(unicode: false, nullable: true),
            //        lPostPercFragmentation = table.Column<double>(nullable: true),
            //        dLastUpdateLocal = table.Column<DateTime>(type: "datetime", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_IndexMaintenance", x => new { x.dDate, x.SzDbName, x.SzSchemaName, x.SzTableName, x.szIndexName });
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Document_Type",
            //    schema: "LOOKUP",
            //    columns: table => new
            //    {
            //        lDocumentTypeID = table.Column<int>(nullable: false),
            //        szDescription = table.Column<string>(maxLength: 100, nullable: true),
            //        dLastUpdateLocal = table.Column<DateTime>(type: "datetime", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Document_Type", x => x.lDocumentTypeID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Store_Group",
            //    schema: "LOOKUP",
            //    columns: table => new
            //    {
            //        lStoreGroupID = table.Column<int>(nullable: false),
            //        szDescription = table.Column<string>(maxLength: 255, nullable: false),
            //        dLastUpdateLocal = table.Column<DateTime>(type: "datetime", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_StoreGroup", x => x.lStoreGroupID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Transaction_Mismatch",
            //    schema: "LOOKUP",
            //    columns: table => new
            //    {
            //        lTransactionMismatchID = table.Column<int>(nullable: false),
            //        szDescription = table.Column<string>(maxLength: 255, nullable: true),
            //        dLastUpdateLocal = table.Column<DateTime>(type: "datetime", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Transaction_Mismatch", x => x.lTransactionMismatchID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "RtServer_Transmission",
            //    schema: "TRX",
            //    columns: table => new
            //    {
            //        szRtServerID = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
            //        lRtServerOperationID = table.Column<int>(nullable: false),
            //        dLastUpdateLocal = table.Column<DateTime>(type: "datetime", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ServerRt_Transmission", x => new { x.szRtServerID, x.lRtServerOperationID });
            //    });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateTable(
            //    name: "Store",
            //    schema: "LOOKUP",
            //    columns: table => new
            //    {
            //        lRetailStoreID = table.Column<int>(nullable: false),
            //        lStoreGroupID = table.Column<int>(nullable: false),
            //        szDescription = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
            //        dLastUpdateLocal = table.Column<DateTime>(type: "datetime", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Store", x => new { x.lRetailStoreID, x.lStoreGroupID });
            //        table.ForeignKey(
            //            name: "FK_Store_#1",
            //            column: x => x.lStoreGroupID,
            //            principalSchema: "LOOKUP",
            //            principalTable: "Store_Group",
            //            principalColumn: "lStoreGroupID",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "RtServer_TransmissionDetail",
            //    schema: "TRX",
            //    columns: table => new
            //    {
            //        szRtServerID = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
            //        lRtServerOperationID = table.Column<int>(nullable: false),
            //        lRtDeviceTransmissionID = table.Column<int>(nullable: false),
            //        szRtDeviceID = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
            //        szRtDeviceType = table.Column<string>(unicode: false, fixedLength: true, maxLength: 2, nullable: true),
            //        szRtTransmissionFormat = table.Column<string>(unicode: false, fixedLength: true, maxLength: 5, nullable: true),
            //        dRtInactivityDateTimeFrom = table.Column<DateTime>(type: "datetime", nullable: true),
            //        dRtInactivityDateTimeTo = table.Column<DateTime>(type: "datetime", nullable: true),
            //        dRtDeviceClosureDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
            //        dLastUpdateLocal = table.Column<DateTime>(type: "datetime", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_RtServer_TransmissionDetail", x => new { x.szRtServerID, x.lRtServerOperationID, x.lRtDeviceTransmissionID, x.szRtDeviceID });
            //        table.ForeignKey(
            //            name: "FK_ServerRt_TransmissionDetail_#1",
            //            columns: x => new { x.szRtServerID, x.lRtServerOperationID },
            //            principalSchema: "TRX",
            //            principalTable: "RtServer_Transmission",
            //            principalColumns: new[] { "szRtServerID", "lRtServerOperationID" },
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "RtServer",
            //    schema: "LOOKUP",
            //    columns: table => new
            //    {
            //        szRtServerID = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
            //        lRetailStoreID = table.Column<int>(nullable: false),
            //        lStoreGroupID = table.Column<int>(nullable: false),
            //        szLocation = table.Column<string>(maxLength: 100, nullable: true),
            //        szIpAddress = table.Column<string>(maxLength: 100, nullable: true),
            //        szUsername = table.Column<string>(maxLength: 100, nullable: true),
            //        szPassword = table.Column<string>(maxLength: 100, nullable: true),
            //        bOnDutyFlag = table.Column<bool>(nullable: true),
            //        dLastUpdateLocal = table.Column<DateTime>(type: "datetime", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_RtServer", x => new { x.szRtServerID, x.lRetailStoreID, x.lStoreGroupID });
            //        table.ForeignKey(
            //            name: "FK_RtServer_#1",
            //            columns: x => new { x.lRetailStoreID, x.lStoreGroupID },
            //            principalSchema: "LOOKUP",
            //            principalTable: "Store",
            //            principalColumns: new[] { "lRetailStoreID", "lStoreGroupID" },
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "RtServer_TransmissionDetail_RtData",
            //    schema: "TRX",
            //    columns: table => new
            //    {
            //        lRtDataID = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        szRtServerID = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
            //        lRtServerOperationID = table.Column<int>(nullable: false),
            //        lRtDeviceTransmissionID = table.Column<int>(nullable: false),
            //        szRtDeviceID = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
            //        dVatRate = table.Column<decimal>(type: "decimal(6, 4)", nullable: false),
            //        dVatAmount = table.Column<decimal>(type: "decimal(15, 4)", nullable: true),
            //        szVatNature = table.Column<string>(unicode: false, fixedLength: true, maxLength: 3, nullable: true),
            //        szVatLegalReference = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
            //        bVatVentilation = table.Column<bool>(nullable: true),
            //        dSaleAmount = table.Column<decimal>(type: "decimal(15, 4)", nullable: true),
            //        dReturnAmount = table.Column<decimal>(type: "decimal(15, 4)", nullable: true),
            //        dVoidAmount = table.Column<decimal>(type: "decimal(15, 4)", nullable: true),
            //        dLastUpdateLocal = table.Column<DateTime>(type: "datetime", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ServerRT_TransmissionDetail_RtData", x => x.lRtDataID);
            //        table.ForeignKey(
            //            name: "FK_ServerRT_TransmissionDetail_RtData_#1",
            //            columns: x => new { x.szRtServerID, x.lRtServerOperationID, x.lRtDeviceTransmissionID, x.szRtDeviceID },
            //            principalSchema: "TRX",
            //            principalTable: "RtServer_TransmissionDetail",
            //            principalColumns: new[] { "szRtServerID", "lRtServerOperationID", "lRtDeviceTransmissionID", "szRtDeviceID" },
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "RtServer_TransmissionDetail_RtReport",
            //    schema: "TRX",
            //    columns: table => new
            //    {
            //        lRtReportID = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        szRtServerID = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
            //        lRtServerOperationID = table.Column<int>(nullable: false),
            //        lRtDeviceTransmissionID = table.Column<int>(nullable: false),
            //        szRtDeviceID = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
            //        dEventDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
            //        szEventType = table.Column<string>(unicode: false, fixedLength: true, maxLength: 2, nullable: true),
            //        szEventNote = table.Column<string>(maxLength: 255, nullable: true),
            //        dLastUpdateLocal = table.Column<DateTime>(type: "datetime", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ServerRt_Report", x => x.lRtReportID);
            //        table.ForeignKey(
            //            name: "FK_ServerRt_TransmissionDetail_RtReport_#1",
            //            columns: x => new { x.szRtServerID, x.lRtServerOperationID, x.lRtDeviceTransmissionID, x.szRtDeviceID },
            //            principalSchema: "TRX",
            //            principalTable: "RtServer_TransmissionDetail",
            //            principalColumns: new[] { "szRtServerID", "lRtServerOperationID", "lRtDeviceTransmissionID", "szRtDeviceID" },
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "RtServer_Status",
            //    schema: "LOOKUP",
            //    columns: table => new
            //    {
            //        szRtServerID = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
            //        lRetailStoreID = table.Column<int>(nullable: false),
            //        lStoreGroupID = table.Column<int>(nullable: false),
            //        bOnErrorFlag = table.Column<bool>(nullable: true),
            //        szErrorDescription = table.Column<string>(maxLength: 255, nullable: true),
            //        bVatVentilationFlag = table.Column<bool>(nullable: true),
            //        lLastClosureNmbr = table.Column<short>(nullable: true),
            //        szLastCloseResult = table.Column<string>(maxLength: 255, nullable: true),
            //        lMemoryAvailable = table.Column<short>(nullable: true),
            //        dGrandTotalAmount = table.Column<decimal>(type: "decimal(15, 4)", nullable: true),
            //        lPendingTransmissionNmbr = table.Column<short>(nullable: true),
            //        lPendingTransmissionDays = table.Column<short>(nullable: true),
            //        bRunningTransmissionFlag = table.Column<bool>(nullable: true),
            //        lTransmissionScheduleMinutesLeft = table.Column<short>(nullable: true),
            //        lTransmissionScheduleHoursRepeat = table.Column<short>(nullable: true),
            //        dtLastDateTimeRead = table.Column<DateTime>(type: "datetime", nullable: true),
            //        dtLastDateTimeCollected = table.Column<DateTime>(type: "datetime", nullable: true),
            //        dLastDateTimeTransactionsCollected = table.Column<DateTime>(name: "dLastDateTimeTransactionsCollected ", type: "datetime", nullable: true),
            //        dLastDateTimeTransmissionsCollected = table.Column<DateTime>(type: "datetime", nullable: true),
            //        dLastUpdateLocal = table.Column<DateTime>(type: "datetime", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ServerRt_Status", x => new { x.szRtServerID, x.lRetailStoreID, x.lStoreGroupID });
            //        table.ForeignKey(
            //            name: "FK_RtServer_Status_#1",
            //            columns: x => new { x.szRtServerID, x.lRetailStoreID, x.lStoreGroupID },
            //            principalSchema: "LOOKUP",
            //            principalTable: "RtServer",
            //            principalColumns: new[] { "szRtServerID", "lRetailStoreID", "lStoreGroupID" },
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Transaction_Affiliation",
            //    schema: "TRN",
            //    columns: table => new
            //    {
            //        lTransactionAffiliationID = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        szRtServerID = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
            //        lRetailStoreID = table.Column<int>(nullable: false),
            //        lStoreGroupID = table.Column<int>(nullable: false),
            //        dtBusinessDate = table.Column<DateTime>(type: "date", nullable: true),
            //        lPosWorkstationNmbr = table.Column<int>(nullable: true),
            //        lPosTaNmbr = table.Column<int>(nullable: true),
            //        dPosTransactionTurnover = table.Column<decimal>(type: "decimal(15, 4)", nullable: true),
            //        dtRtDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
            //        szRtDeviceID = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        lRtClosureNmbr = table.Column<int>(nullable: true),
            //        lRtDocumentNmbr = table.Column<int>(nullable: true),
            //        szRtDocumentID = table.Column<string>(unicode: false, maxLength: 64, nullable: true),
            //        dRtTransactionTurnover = table.Column<decimal>(type: "decimal(15, 4)", nullable: true),
            //        bRtNonCompliantFlag = table.Column<bool>(nullable: true),
            //        lTransactionMismatchID = table.Column<int>(nullable: true),
            //        bTransactionCheckedFlag = table.Column<bool>(nullable: true),
            //        szTranscationCheckNote = table.Column<string>(maxLength: 255, nullable: true),
            //        bTransactionArchivedFlag = table.Column<bool>(nullable: true),
            //        dLastUpdateLocal = table.Column<DateTime>(type: "datetime", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Transaction_Affiliation", x => x.lTransactionAffiliationID);
            //        table.ForeignKey(
            //            name: "FK_Transaction_Affiliation_#2",
            //            column: x => x.lTransactionMismatchID,
            //            principalSchema: "LOOKUP",
            //            principalTable: "Transaction_Mismatch",
            //            principalColumn: "lTransactionMismatchID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Transaction_Affiliation_#1",
            //            columns: x => new { x.szRtServerID, x.lRetailStoreID, x.lStoreGroupID },
            //            principalSchema: "LOOKUP",
            //            principalTable: "RtServer",
            //            principalColumns: new[] { "szRtServerID", "lRetailStoreID", "lStoreGroupID" },
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Transaction_RtError",
            //    schema: "TRN",
            //    columns: table => new
            //    {
            //        lRtErrorID = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        szRtServerID = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
            //        lRetailStoreID = table.Column<int>(nullable: false),
            //        lStoreGroupID = table.Column<int>(nullable: false),
            //        szRtDeviceID = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        dtRtDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
            //        lRtClosureNmbr = table.Column<int>(nullable: true),
            //        lRtDocumentNmbr = table.Column<int>(nullable: true),
            //        szDescription = table.Column<string>(maxLength: 255, nullable: true),
            //        dLastUpdateLocal = table.Column<DateTime>(type: "datetime", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Transaction_RtError", x => x.lRtErrorID);
            //        table.ForeignKey(
            //            name: "FK_Transaction_RtError_#1",
            //            columns: x => new { x.szRtServerID, x.lRetailStoreID, x.lStoreGroupID },
            //            principalSchema: "LOOKUP",
            //            principalTable: "RtServer",
            //            principalColumns: new[] { "szRtServerID", "lRetailStoreID", "lStoreGroupID" },
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Transaction_Document",
            //    schema: "TRN",
            //    columns: table => new
            //    {
            //        lTransactionDocumentID = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        lTransactionAffiliationID = table.Column<int>(nullable: true),
            //        lDocumentTypeID = table.Column<int>(nullable: true),
            //        szDocumentNote = table.Column<string>(maxLength: 255, nullable: true),
            //        szDocumentName = table.Column<string>(maxLength: 100, nullable: true),
            //        szDocumentAttachment = table.Column<string>(type: "xml", nullable: true),
            //        dLastUpdateLocal = table.Column<DateTime>(type: "datetime", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Transaction_Document", x => x.lTransactionDocumentID);
            //        table.ForeignKey(
            //            name: "FK_Transaction_Document_#1",
            //            column: x => x.lDocumentTypeID,
            //            principalSchema: "LOOKUP",
            //            principalTable: "Document_Type",
            //            principalColumn: "lDocumentTypeID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Transaction_Document_#2",
            //            column: x => x.lTransactionAffiliationID,
            //            principalSchema: "TRN",
            //            principalTable: "Transaction_Affiliation",
            //            principalColumn: "lTransactionAffiliationID",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Transaction_Vat",
            //    schema: "TRN",
            //    columns: table => new
            //    {
            //        lTransactionAffiliationID = table.Column<int>(nullable: false),
            //        szVatCodeID = table.Column<string>(unicode: false, maxLength: 5, nullable: false),
            //        dPosVatRate = table.Column<decimal>(type: "decimal(6, 4)", nullable: true),
            //        dPosGrossAmount = table.Column<decimal>(type: "decimal(15, 4)", nullable: true),
            //        dPosNetAmount = table.Column<decimal>(type: "decimal(15, 4)", nullable: true),
            //        dPosVatAmount = table.Column<decimal>(type: "decimal(15, 4)", nullable: true),
            //        dRtVatRate = table.Column<decimal>(type: "decimal(6, 4)", nullable: true),
            //        dRtGrossAmount = table.Column<decimal>(type: "decimal(15, 4)", nullable: true),
            //        dRtNetAmount = table.Column<decimal>(type: "decimal(15, 4)", nullable: true),
            //        dRtVatAmount = table.Column<decimal>(type: "decimal(15, 4)", nullable: true),
            //        bVatMismatchFlag = table.Column<bool>(nullable: true),
            //        bVatCheckedFlag = table.Column<bool>(nullable: true),
            //        dLastUpdateLocal = table.Column<DateTime>(type: "datetime", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Transaction_Vat", x => new { x.lTransactionAffiliationID, x.szVatCodeID });
            //        table.ForeignKey(
            //            name: "FK_Transaction_Vat_#1",
            //            column: x => x.lTransactionAffiliationID,
            //            principalSchema: "TRN",
            //            principalTable: "Transaction_Affiliation",
            //            principalColumn: "lTransactionAffiliationID",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            //migrationBuilder.CreateIndex(
            //    name: "IX_RtServer_lRetailStoreID_lStoreGroupID",
            //    schema: "LOOKUP",
            //    table: "RtServer",
            //    columns: new[] { "lRetailStoreID", "lStoreGroupID" });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Store_lStoreGroupID",
            //    schema: "LOOKUP",
            //    table: "Store",
            //    column: "lStoreGroupID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Transaction_Affiliation_lTransactionMismatchID",
            //    schema: "TRN",
            //    table: "Transaction_Affiliation",
            //    column: "lTransactionMismatchID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Transaction_Affiliation_szRtServerID_lRetailStoreID_lStoreGroupID",
            //    schema: "TRN",
            //    table: "Transaction_Affiliation",
            //    columns: new[] { "szRtServerID", "lRetailStoreID", "lStoreGroupID" });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Transaction_Document_lDocumentTypeID",
            //    schema: "TRN",
            //    table: "Transaction_Document",
            //    column: "lDocumentTypeID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Transaction_Document_lTransactionAffiliationID",
            //    schema: "TRN",
            //    table: "Transaction_Document",
            //    column: "lTransactionAffiliationID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Transaction_RtError_szRtServerID_lRetailStoreID_lStoreGroupID",
            //    schema: "TRN",
            //    table: "Transaction_RtError",
            //    columns: new[] { "szRtServerID", "lRetailStoreID", "lStoreGroupID" });

            //migrationBuilder.CreateIndex(
            //    name: "IDX_RtServer_Transmission_#1",
            //    schema: "TRX",
            //    table: "RtServer_Transmission",
            //    column: "dLastUpdateLocal");

            //migrationBuilder.CreateIndex(
            //    name: "IDX_RtServer_TransmissionDetail_#1",
            //    schema: "TRX",
            //    table: "RtServer_TransmissionDetail",
            //    column: "dLastUpdateLocal");

            //migrationBuilder.CreateIndex(
            //    name: "IDX_RtServer_TransmissionDetail_RtData_#1",
            //    schema: "TRX",
            //    table: "RtServer_TransmissionDetail_RtData",
            //    column: "dLastUpdateLocal");

            //migrationBuilder.CreateIndex(
            //    name: "IDX_RtServer_TransmissionDetail_RtData_#2",
            //    schema: "TRX",
            //    table: "RtServer_TransmissionDetail_RtData",
            //    columns: new[] { "szRtServerID", "lRtServerOperationID", "lRtDeviceTransmissionID", "szRtDeviceID" });

            //migrationBuilder.CreateIndex(
            //    name: "IDX_RtServer_TransmissionDetail_RtReport_#1",
            //    schema: "TRX",
            //    table: "RtServer_TransmissionDetail_RtReport",
            //    column: "dLastUpdateLocal");

            //migrationBuilder.CreateIndex(
            //    name: "IDX_RtServer_TransmissionDetail_RtReport_#2",
            //    schema: "TRX",
            //    table: "RtServer_TransmissionDetail_RtReport",
            //    columns: new[] { "szRtServerID", "lRtServerOperationID", "lRtDeviceTransmissionID", "szRtDeviceID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "IndexMaintenance",
                schema: "LOGGING");

            migrationBuilder.DropTable(
                name: "RtServer_Status",
                schema: "LOOKUP");

            migrationBuilder.DropTable(
                name: "Transaction_Document",
                schema: "TRN");

            migrationBuilder.DropTable(
                name: "Transaction_RtError",
                schema: "TRN");

            migrationBuilder.DropTable(
                name: "Transaction_Vat",
                schema: "TRN");

            migrationBuilder.DropTable(
                name: "RtServer_TransmissionDetail_RtData",
                schema: "TRX");

            migrationBuilder.DropTable(
                name: "RtServer_TransmissionDetail_RtReport",
                schema: "TRX");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Document_Type",
                schema: "LOOKUP");

            migrationBuilder.DropTable(
                name: "Transaction_Affiliation",
                schema: "TRN");

            migrationBuilder.DropTable(
                name: "RtServer_TransmissionDetail",
                schema: "TRX");

            migrationBuilder.DropTable(
                name: "Transaction_Mismatch",
                schema: "LOOKUP");

            migrationBuilder.DropTable(
                name: "RtServer",
                schema: "LOOKUP");

            migrationBuilder.DropTable(
                name: "RtServer_Transmission",
                schema: "TRX");

            migrationBuilder.DropTable(
                name: "Store",
                schema: "LOOKUP");

            migrationBuilder.DropTable(
                name: "Store_Group",
                schema: "LOOKUP");
        }
    }
}
