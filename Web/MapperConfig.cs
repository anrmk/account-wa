using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Core.Data.Dto;
using Core.Data.Dto.Nsi;
using Core.Extension;

using Microsoft.Extensions.DependencyInjection;

using Web.ViewModels;

namespace Web {
    public class MapperConfig: Profile {
        public static void Register(IServiceCollection services) {
            var mapperConfig = new MapperConfiguration(mc => {
                mc.AddProfile(new Core.MapperConfig());
                mc.AddProfile(new MapperConfig());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        public MapperConfig() {
            #region COMPANY
            CreateMap<CompanyViewModel, CompanyDto>()
                .ForMember(d => d.Address, o => o.MapFrom(s => new CompanyAddressDto() { Id = s.AddressId, Address = s.Address, Address2 = s.Address2, City = s.City, State = s.State, ZipCode = s.ZipCode, Country = s.Country }))
                .ForMember(d => d.Settings, o => o.MapFrom(s => new CompanySettingsDto() { Id = s.SettingsId, RoundType = s.RoundType, SaveCreditValues = s.SaveCreditValues }))
                .ForMember(d => d.Customers, o => o.MapFrom(s => s.Customers.Select(x => new CustomerDto() { Id = x })))
                .ReverseMap()

                .ForMember(d => d.AddressId, o => o.MapFrom(s => (s.Address != null) ? s.Address.Id : 0))
                .ForMember(d => d.Address, o => o.MapFrom(s => (s.Address != null) ? s.Address.Address : ""))
                .ForMember(d => d.Address2, o => o.MapFrom(s => (s.Address != null) ? s.Address.Address2 : ""))
                .ForMember(d => d.City, o => o.MapFrom(s => (s.Address != null) ? s.Address.City : ""))
                .ForMember(d => d.State, o => o.MapFrom(s => (s.Address != null) ? s.Address.State : ""))
                .ForMember(d => d.Country, o => o.MapFrom(s => (s.Address != null) ? s.Address.Country : ""))
                .ForMember(d => d.ZipCode, o => o.MapFrom(s => (s.Address != null) ? s.Address.ZipCode : ""))

                .ForMember(d => d.SettingsId, o => o.MapFrom(s => (s.Settings != null) ? s.Settings.Id : 0))
                .ForMember(d => d.RoundType, o => o.MapFrom(s => (s.Settings != null) ? s.Settings.RoundType : 0))
                .ForMember(d => d.SaveCreditValues, o => o.MapFrom(s => (s.Settings != null) ? s.Settings.SaveCreditValues : false))

                .ForMember(d => d.Customers, o => o.MapFrom(s => s.Customers.Select(x => x.Id)));


            CreateMap<CompanyListViewModel, CompanyDto>()
                .ReverseMap()
                .ForMember(d => d.Address, o => o.MapFrom(s => (s.Address != null) ? s.Address.ToString() : ""));


            CreateMap<CompanyAddressViewModel, CompanyAddressDto>().ReverseMap();
            CreateMap<CompanySummaryRangeViewModel, CompanySummaryRangeDto>().ReverseMap();
            CreateMap<CompanySettingsViewModel, CompanySettingsDto>().ReverseMap();
            CreateMap<CompanyExportSettingsViewModel, CompanyExportSettingsDto>().ReverseMap();
            CreateMap<CompanyExportSettingsFieldViewModel, CompanyExportSettingsFieldDto>().ReverseMap();
            #endregion

            #region CUSTOMER
            CreateMap<CustomerListViewModel, CustomerDto>()
                .ForMember(d => d.Invoices, o => o.Ignore())
                .ForMember(d => d.Activities, o => o.Ignore())
                .ForMember(d => d.Tags, o => o.Ignore())
                .ReverseMap()
                .ForMember(d => d.CreditLimit, o => o.MapFrom(s => s.CreditLimit.HasValue ? s.CreditLimit.Value.ToCurrency() : ""))
                .ForMember(d => d.CreditUtilized, o => o.MapFrom(s => s.CreditUtilized.HasValue ? s.CreditUtilized.Value.ToCurrency() : ""))
                .ForMember(d => d.Tags, o => o.MapFrom(s => s.Tags))
                //.ForMember(d => d.Tags, o => o.MapFrom(s => string.Join(',', s.Tags)))
                .ForMember(d => d.Company, o => o.MapFrom(s => (s.Company != null) ? s.Company.Name : ""))
                .ForMember(d => d.Type, o => o.MapFrom(s => (s.Type != null) ? s.Type.Name : ""))
                .ForMember(d => d.Address, o => o.MapFrom(s => (s.Address != null) ? s.Address.ToString() : ""));

            CreateMap<CustomerViewModel, CustomerDto>()
                .ForMember(d => d.Tags, o => o.Ignore())
                .ForMember(d => d.Address, o => o.MapFrom(s => new CustomerAddressDto() { Id = s.AddressId, Address = s.Address, Address2 = s.Address2, City = s.City, State = s.State, ZipCode = s.ZipCode, Country = s.Country }))
                .ReverseMap()
                .ForMember(d => d.AddressId, o => o.MapFrom(s => (s.Address != null) ? s.Address.Id : (long?)null))
                .ForMember(d => d.Address, o => o.MapFrom(s => (s.Address != null) ? s.Address.Address : ""))
                .ForMember(d => d.Address2, o => o.MapFrom(s => (s.Address != null) ? s.Address.Address2 : ""))
                .ForMember(d => d.City, o => o.MapFrom(s => (s.Address != null) ? s.Address.City : ""))
                .ForMember(d => d.State, o => o.MapFrom(s => (s.Address != null) ? s.Address.State : ""))
                .ForMember(d => d.ZipCode, o => o.MapFrom(s => (s.Address != null) ? s.Address.ZipCode : ""))
                .ForMember(d => d.Country, o => o.MapFrom(s => (s.Address != null) ? s.Address.Country : ""));

            CreateMap<CustomerCreditLimitViewModel, CustomerCreditLimitDto>().ReverseMap();
            CreateMap<CustomerCreditUtilizedViewModel, CustomerCreditUtilizedDto>().ReverseMap();
            CreateMap<CustomerActivityViewModel, CustomerActivityDto>().ReverseMap();
            CreateMap<CustomerTagViewModel, CustomerTagDto>().ReverseMap();

            CreateMap<CustomerFilterViewModel, CustomerFilterDto>()
                //  .ForMember(d => d., o => o.MapFrom(s => string.IsNullOrEmpty(s.Periods) ? new List<string>() : s.Periods.Split(',', System.StringSplitOptions.RemoveEmptyEntries).ToList()))
                .ReverseMap()
            //.ForMember(d => d.Periods, o => o.MapFrom(s => string.Join(",", s.Periods)));
            ;
            CreateMap<CustomerImportCreditsViewModel, CustomerImportCreditsDto>().ReverseMap();
            CreateMap<CustomerRecheckViewModel, CustomerRecheckDto>().ReverseMap();

            #endregion

            #region INVOICE
            CreateMap<InvoiceViewModel, InvoiceDto>()
                .ForMember(d => d.Customer, o => o.Ignore())
                .ForMember(d => d.Company, o => o.Ignore())
                .ForMember(d => d.Payments, o => o.Ignore())
                .ReverseMap();

            CreateMap<InvoiceListViewModel, InvoiceDto>()
                .ReverseMap()
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name))
                .ForMember(d => d.CustomerName, o => o.MapFrom(s => s.Customer.Name))
                .ForMember(d => d.CustomerTags, o => o.MapFrom(s => s.Customer.Tags != null ? s.Customer.Tags.Select(x => x.Name).ToArray() : new string[] { }))
                .ForMember(d => d.CustomerCreatedDate, o => o.MapFrom(s => s.Customer.Activities != null ? s.Customer.Activities.OrderByDescending(x => x.CreatedDate).FirstOrDefault().CreatedDate.ToString() : ""))
                .ForMember(d => d.CustomerType, o => o.MapFrom(s => s.Customer.Type != null ? s.Customer.Type.Name : ""))
                .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount.ToCurrency()))
                .ForMember(d => d.Balance, o => o.MapFrom(s => s.Balance.ToCurrency()))
                .ForMember(d => d.PaymentAmount, o => o.MapFrom(s => s.Payments.TotalAmount().ToCurrency()))
                .ForMember(d => d.PaymentDate, o => o.MapFrom(s => s.Payments.LastPaymentDate()));

            CreateMap<InvoiceFilterViewModel, InvoiceFilterDto>()
                .ForMember(d => d.Periods, o => o.MapFrom(s => string.IsNullOrEmpty(s.Periods) ? new List<string>() : s.Periods.Split(',', System.StringSplitOptions.RemoveEmptyEntries).ToList()))
                .ReverseMap()
                .ForMember(d => d.Periods, o => o.MapFrom(s => string.Join(",", s.Periods)));
            ;
            #endregion

            #region PAYMENT
            CreateMap<PaymentViewModel, PaymentDto>()
                .ReverseMap();

            CreateMap<PaymentListViewModel, PaymentDto>()
                .ReverseMap()
                .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount.ToCurrency()))
                ;

            CreateMap<PaymentFilterViewModel, PaymentFilterDto>().ReverseMap();

            #endregion

            #region SAVED REPORT
            CreateMap<SavedReportViewModel, SavedReportDto>().ReverseMap();
            #endregion

            CreateMap<NsiViewModel, NsiDto>().ReverseMap();

            CreateMap<PagerFilterViewModel, PagerFilter>().ReverseMap();
        }
    }
}
