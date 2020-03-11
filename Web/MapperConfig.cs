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
                .ForMember(d => d.Customers, o => o.MapFrom(s => s.Customers.Select(x => new CustomerDto() { Id = x })))
                .ReverseMap()

                .ForMember(d => d.AddressId, o => o.MapFrom(s => (s.Address != null) ? s.Address.Id : 0))
                .ForMember(d => d.Address, o => o.MapFrom(s => (s.Address != null) ? s.Address.Address : ""))
                .ForMember(d => d.Address2, o => o.MapFrom(s => (s.Address != null) ? s.Address.Address2 : ""))
                .ForMember(d => d.City, o => o.MapFrom(s => (s.Address != null) ? s.Address.City : ""))
                .ForMember(d => d.State, o => o.MapFrom(s => (s.Address != null) ? s.Address.State : ""))
                .ForMember(d => d.Country, o => o.MapFrom(s => (s.Address != null) ? s.Address.Country : ""))
                .ForMember(d => d.ZipCode, o => o.MapFrom(s => (s.Address != null) ? s.Address.ZipCode : ""))

                .ForMember(d => d.Customers, o => o.MapFrom(s => s.Customers.Select(x => x.Id)));


            CreateMap<CompanyListViewModel, CompanyDto>()
                .ReverseMap()
                .ForMember(d => d.Address, o => o.MapFrom(s => (s.Address != null) ? s.Address.ToString() : ""));

            CreateMap<CompanySummaryRangeViewModel, CompanySummaryRangeDto>();

            CreateMap<CompanyExportSettingsViewModel, CompanyExportSettingsDto>().ReverseMap();
            CreateMap<CompanyExportSettingsFieldViewModel, CompanyExportSettingsFieldDto>().ReverseMap();
            #endregion

            #region CUSTOMER
            CreateMap<CustomerListViewModel, CustomerDto>()
                .ForMember(d => d.Invoices, o => o.Ignore())
                .ForMember(d => d.Activities, o => o.Ignore())
                .ReverseMap()
                .ForMember(d => d.Company, o => o.MapFrom(s => (s.Company != null) ? s.Company.Name : ""))
                .ForMember(d => d.Type, o => o.MapFrom(s => (s.Type != null) ? s.Type.Name : ""))
                .ForMember(d => d.Address, o => o.MapFrom(s => (s.Address != null) ? s.Address.ToString() : ""));

            CreateMap<CustomerViewModel, CustomerDto>()
               .ForMember(d => d.Address, o => o.MapFrom(s => new CustomerAddressDto() { Id = s.AddressId, Address = s.Address, Address2 = s.Address2, City = s.City, State = s.State, ZipCode = s.ZipCode, Country = s.Country }))
               .ReverseMap()
               .ForMember(d => d.AddressId, o => o.MapFrom(s => (s.Address != null) ? s.Address.Id : (long?)null))
               .ForMember(d => d.Address, o => o.MapFrom(s => (s.Address != null) ? s.Address.Address : ""))
               .ForMember(d => d.Address2, o => o.MapFrom(s => (s.Address != null) ? s.Address.Address2 : ""))
               .ForMember(d => d.City, o => o.MapFrom(s => (s.Address != null) ? s.Address.City : ""))
               .ForMember(d => d.State, o => o.MapFrom(s => (s.Address != null) ? s.Address.State : ""))
               .ForMember(d => d.ZipCode, o => o.MapFrom(s => (s.Address != null) ? s.Address.ZipCode : ""))
               .ForMember(d => d.Country, o => o.MapFrom(s => (s.Address != null) ? s.Address.Country : ""))
               ;

            CreateMap<CustomerActivityViewModel, CustomerActivityDto>().ReverseMap();

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
                .ForMember(d => d.Amount, o => o.MapFrom(s => (s.Subtotal * (1 + s.TaxRate / 100)).ToString("0.##")))
                .ForMember(d => d.PaymentAmount, o => o.MapFrom(s => s.Payments.TotalAmount()))
                .ForMember(d => d.PaymentDate, o => o.MapFrom(s => s.Payments.LastPaymentDate()));

            CreateMap<InvoiceFilterViewModel, InvoiceFilterDto>().ReverseMap();
            #endregion

            #region PAYMENT
            CreateMap<PaymentViewModel, PaymentDto>()
                .ReverseMap();

            CreateMap<PaymentViewModelList, PaymentDto>()
                .ReverseMap();

            #endregion

            CreateMap<NsiViewModel, NsiDto>().ReverseMap();

            CreateMap<PagerFilterViewModel, PagerFilter>().ReverseMap();
        }
    }
}
