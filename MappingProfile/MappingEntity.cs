using DnSrtChecker.Helpers;
using DnSrtChecker.Models;
using DnSrtChecker.Models.ViewModels;
using DnSrtChecker.Models.ViewModels.ModelsFunctions;
using DnSrtChecker.ModelsHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DnSrtChecker.MappingProfile
{
    public class MappingEntity: AutoMapper.Profile
    {
        public MappingEntity()
        {
            CreateMap<User, UserViewModel>().ReverseMap();
            CreateMap<UserRole, UserRoleViewModel>().ReverseMap();
            CreateMap<Role, RoleViewModel>().ReverseMap();
            CreateMap<Store, StoreViewModel>().ReverseMap();
            CreateMap<StoreGroup, StoreGroupViewModel>().ReverseMap();
            //CreateMap<RtServer, RtServerViewModel>();
            CreateMap<RtServer, RtServerViewModel>().ReverseMap();
            CreateMap<RtServerStatus, RtServerStatusViewModel>().ReverseMap();
            CreateMap<DocumentType, DocumentTypeViewModel>().ReverseMap();
            CreateMap<TransactionAffiliation, TransactionAffiliationViewModel>().ReverseMap();
            CreateMap<TransactionDocument, TransactionDocumentViewModel>().ReverseMap();
            CreateMap<TransactionMismatch, TransactionMismatchViewModel>().ReverseMap();
            CreateMap<TransactionRtError, TransactionRtErrorViewModel>().ReverseMap();
            CreateMap<TransactionVat, TransactionVatViewModel>().ReverseMap();
            CreateMap<RtServerTransmissionDetail, RtServerTransmissionDetailViewModel>().ReverseMap();
            CreateMap<RtServerTransmissionDetailRtData, RtServerTransmissionDetailRtDataViewModel>().ReverseMap();
            CreateMap<RtServerTransmissionDetailRtReport, RtServerTransmissionDetailRtReportViewModel>().ReverseMap();
            CreateMap<ListServersGrouped, RtServersTransactionsViewModel>().ReverseMap();
            CreateMap<RtServer, RtServersHomeViewModel>();//.ReverseMap();
            CreateMap<TransmissionsGroupedByDay, TransmissionsAndTransactionsGroupedByDayVM>().ReverseMap();
            CreateMap<UserActivityLog, UserActivityLogViewModel>().ReverseMap();
            CreateMap<ListServersStatusHomeBYDay, ListServersStatusHomeBYDayViewModel> ().ReverseMap();
            CreateMap<RtServersHome, RtServersHomeViewModel> ().ReverseMap();
            CreateMap<TrxRTServer, RtServerViewModel>().ReverseMap();
            CreateMap<RtServerByUser, RtServerViewModel>().ReverseMap();
            CreateMap<TrxRTServer, RtServersHomeViewModel>().ReverseMap();
            CreateMap<TransactionList, TransactionAffiliationViewModel>();
            CreateMap<TransmissionsList, TransmissionsByDayToIndexView>();

        }
    }
}