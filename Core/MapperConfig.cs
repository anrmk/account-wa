using System.Linq;

using AutoMapper;

using Core.Data.Dto;
using Core.Data.Dto.Nsi;
using Core.Data.Entities;
using Core.Data.Entities.Nsi;
using Core.Extension;

namespace Core {
    public class MapperConfig: Profile {
        public MapperConfig() {
            CreateMap<ApplicationUserEntity, ApplicationUserDto>().ReverseMap();
            CreateMap<UserProfileEntity, UserProfileDto>().ReverseMap();

            #region COMPANY
            CreateMap<CompanyDto, CompanyEntity>()
                .ForMember(d => d.Address, o => o.Ignore())
                .ForMember(d => d.Settings, o => o.Ignore())
                .ForMember(d => d.Customers, o => o.Ignore())
                .ForMember(d => d.SummaryRange, o => o.Ignore())
                .ReverseMap()
                ;
            CreateMap<CompanyAddressDto, CompanyAddressEntity>().ReverseMap();
            CreateMap<CompanySummaryRangeDto, CompanySummaryRangeEntity>().ReverseMap();
            CreateMap<CompanySettingsDto, CompanySettingsEntity>().ReverseMap();
            CreateMap<CompanyExportSettingsDto, CompanyExportSettingsEntity>()
                .ForMember(d => d.Fields, o => o.Ignore())
                .ReverseMap();
            CreateMap<CompanyExportSettingsFieldDto, CompanyExportSettingsFieldEntity>().ReverseMap();
            #endregion

            #region INVOICE
            CreateMap<InvoiceDto, InvoiceEntity>()
                .ForMember(d => d.Company, o => o.Ignore())
                .ForMember(d => d.Customer, o => o.Ignore())
                .ForMember(d => d.Payments, o => o.Ignore())
                .ReverseMap();

            #endregion

            #region CUSTOMER
            CreateMap<CustomerDto, CustomerEntity>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Company, o => o.Ignore())
                .ForMember(d => d.Type, o => o.Ignore())
                .ForMember(d => d.Invoices, o => o.Ignore())
                .ForMember(d => d.Activities, o => o.Ignore())
                .ForMember(d => d.TagLinks, o => o.Ignore())
                .ReverseMap()
                // .ForMember(d => d.AccountNumber, o => o.MapFrom(s => s.AccountNumber))
                .ForMember(d => d.TagsIds, o => o.MapFrom(s => s.TagLinks != null ? s.TagLinks.Select(r => r.TagId) : null))
                .ForMember(d => d.Tags, o => o.MapFrom(s => s.TagLinks != null ? s.TagLinks.Select(r => new CustomerTagDto() { Id = r.Tag.Id, Name = r.Tag.Name }) : null))
                //.ForMember(d => d.Tags, o => o.MapFrom(s => s.TagLinks != null ? s.TagLinks.Select(r => new CustomerTagDto() { Id = r.Tag.Id, Name = r.Tag.Name }) : null))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => s.Activities.IsActive()));
            ;
            CreateMap<CustomerAddressDto, CustomerAddressEntity>().ReverseMap();
            CreateMap<CustomerCreditUtilizedDto, CustomerCreditUtilizedEntity>().ReverseMap();
            CreateMap<CustomerCreditLimitDto, CustomerCreditLimitEntity>().ReverseMap();
            CreateMap<CustomerActivityDto, CustomerActivityEntity>().ReverseMap();
            CreateMap<CustomerTagDto, CustomerTagEntity>().ReverseMap();

            //CreateMap<CustomerDto, CustomerBulkEntity>()
            //    .ForMember(d => d.TagLinks, o => o.Ignore())
            //    .ReverseMap()
            //    .ForMember(d => d.TagsId, o => o.MapFrom(s => s.TagLinks.Select(r => r.TagId)))
            //    .ForMember(d => d.Tags, o => o.MapFrom(s => s.TagLinks.Select(r => new CustomerTagDto() { Id = r.Tag.Id, Name = r.Tag.Name})));
            #endregion

            #region PAYMENT
            CreateMap<PaymentDto, PaymentEntity>()
                .ForMember(d => d.Invoice, o => o.Ignore())
                .ReverseMap()
                .ForMember(d => d.InvoiceNo, o => o.MapFrom(s => s.Invoice != null ? s.Invoice.No : ""))
                .ForMember(d => d.CustomerId, o => o.MapFrom(s => s.Invoice != null ? s.Invoice.CustomerId : null));
            #endregion

            #region SAVED REPORT
            CreateMap<SavedReportDto, SavedReportEntity>()
                .ForMember(d => d.Fields, o => o.Ignore())
                .ForMember(d => d.Files, o => o.Ignore())
                .ReverseMap()
                ;
            CreateMap<SavedReportFieldDto, SavedReportFieldEntity>().ReverseMap();
            CreateMap<SavedReportFileDto, SavedReportFileEntity>().ReverseMap();
            #endregion

            #region NSI
            //CreateMap<ReportPeriodDto, ReportPeriodEntity>().ReverseMap();
            CreateMap<NsiDto, ReportFieldEntity>().ReverseMap();
            CreateMap<NsiDto, CustomerTypeEntity>().ReverseMap();
            #endregion
        }
    }
}
