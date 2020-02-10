using AutoMapper;

using Core.Data.Dto;

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
            //CreateMap<PeriodViewModel, Period>()
            //    .ForMember(d => d.Month, o => o.MapFrom(s => s.Value.Month))
            //    .ForMember(d => d.Year, o => o.MapFrom(s => s.Value.Year))
            //    .ReverseMap()
            //    .ForMember(d => d.Value, o => o.MapFrom(s => new DateTime(s.Year, s.Month, 1)))
            //    .ForMember(d => d.Display, o => o.Ignore());

            //CreateMap<CustomerViewModel, Customer>()
            //    .ReverseMap();


            //CreateMap<PaymentViewModel, Payment>()
            //    .ReverseMap()
            //    .ForMember(d => d.InvoiceId, o => o.MapFrom(s => s.Invoice != null ? s.Invoice.Id : 0));

            CreateMap<CompanyViewModel, CompanyDto>()
                .ForMember(d => d.Address, o => o.MapFrom(s => new CompanyAddressDto() { Id = s.AddressId, Address = s.Address, Address2 = s.Address2, City = s.City, State = s.State, ZipCode = s.ZipCode, Country = s.Country }))
                .ReverseMap()
                .ForMember(d => d.AddressId, o => o.MapFrom(s => (s.Address != null) ? s.Address.Id : (long?)null))
                .ForMember(d => d.Address, o => o.MapFrom(s => (s.Address != null) ? s.Address.Address : ""))
                .ForMember(d => d.Address2, o => o.MapFrom(s => (s.Address != null) ? s.Address.Address2 : ""))
                .ForMember(d => d.City, o => o.MapFrom(s => (s.Address != null) ? s.Address.City : ""))
                .ForMember(d => d.State, o => o.MapFrom(s => (s.Address != null) ? s.Address.State : ""))
                .ForMember(d => d.ZipCode, o => o.MapFrom(s => (s.Address != null) ? s.Address.ZipCode : ""))
                .ForMember(d => d.Country, o => o.MapFrom(s => (s.Address != null) ? s.Address.Country : ""));

            CreateMap<CompanyViewModelList, CompanyDto>()
                .ReverseMap()
                .ForMember(d => d.TotalCustomers, o => o.MapFrom(s => (s.Customers != null) ? s.Customers.Count : 0))
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
               .ForMember(d => d.Country, o => o.MapFrom(s => (s.Address != null) ? s.Address.Country : ""));

            CreateMap<CustomerListViewModel, CustomerDto>()
                .ReverseMap()
                .ForMember(d => d.Address, o => o.MapFrom(s => (s.Address != null) ? s.Address.ToString() : ""));

            CreateMap<InvoiceViewModel, InvoiceDto>()
                .ForMember(d => d.Customer, o => o.Ignore())
                .ForMember(d => d.Company, o => o.Ignore())
                .ForMember(d => d.Payment, o => o.Ignore())
                .ReverseMap()
                //.ForMember(d => d.CustomerName, o => o.MapFrom(s => s.Customer != null ? s.Customer.Name : ""))
                //.ForMember(d => d.PaymentAmount, o => o.MapFrom(s => s.Payment != null ? s.Payment.Amount : 0))
                //.ForMember(d => d.PaymentDate, o => o.MapFrom(s => s.Payment != null ? s.Payment.Date : (DateTime?)null))
                ;
            CreateMap<InvoiceListViewModel, InvoiceDto>()
                .ReverseMap()
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => (s.Company != null) ? s.Company.Name : ""))
                .ForMember(d => d.CustomerName, o => o.MapFrom(s => (s.Customer != null) ? s.Customer.Name : ""));

            CreateMap<PaymentViewModel, PaymentDto>()
                .ReverseMap();

            CreateMap<PaymentViewModelList, PaymentDto>()
                .ReverseMap();

            //CreateMap<ReportViewModel, ReportDto>().ReverseMap();
            //CreateMap<ReportDataViewModel, ReportDataDto>().ReverseMap();
        }
    }
}
