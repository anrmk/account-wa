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

            //CreateMap<InvoiceViewModel, Invoice>()
            //    .ReverseMap()
            //    .ForMember(d => d.CustomerId, o => o.MapFrom(s => s.CustomerId))
            //    .ForMember(d => d.CustomerAccountNumber, o => o.MapFrom(s => s.Customer != null ? s.Customer.AccountNumber : ""))
            //    .ForMember(d => d.CustomerName, o => o.MapFrom(s => s.Customer != null ? s.Customer.BusinessName : ""))
            //    .ForMember(d => d.PaymentAmount, o => o.MapFrom(s => s.Payment != null ? s.Payment.Amount : 0))
            //    .ForMember(d => d.PaymentDate, o => o.MapFrom(s => s.Payment != null ? s.Payment.Date : (DateTime?)null))
            //    ;

            //CreateMap<PaymentViewModel, Payment>()
            //    .ReverseMap()
            //    .ForMember(d => d.InvoiceId, o => o.MapFrom(s => s.Invoice != null ? s.Invoice.Id : 0));

            CreateMap<CompanyViewModel, CompanyDto>().ReverseMap();
            CreateMap<CompanyViewModelList, CompanyDtoList>().ReverseMap();

        }
    }


}
