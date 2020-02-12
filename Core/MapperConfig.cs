using System.Linq;

using AutoMapper;

using Core.Data.Dto;
using Core.Data.Entities;
using Core.Extension;

namespace Core {
    public class MapperConfig: Profile {
        public MapperConfig() {
            CreateMap<ApplicationUserEntity, ApplicationUserDto>().ReverseMap();
            CreateMap<UserProfileEntity, UserProfileDto>().ReverseMap();

            CreateMap<CompanyDto, CompanyEntity>()
                .ForMember(d => d.Customers, o => o.Ignore())
                .ReverseMap()
                 .ForMember(d => d.Customers, o => o.MapFrom(s => s.Customers.Select(r => r.Id)));

            CreateMap<CompanyAddressDto, CompanyAddressEntity>().ReverseMap();
            CreateMap<CompanySummaryRangeDto, CompanySummaryRangeEntity>().ReverseMap();

            CreateMap<CustomerDto, CustomerEntity>()
                .ForMember(d => d.Activities, o => o.Ignore())
                .ReverseMap()
                .ForMember(d => d.IsActive, o => o.MapFrom(s => s.Activities.IsActive()));
            CreateMap<CustomerAddressDto, CustomerAddressEntity>().ReverseMap();
            CreateMap<CustomerDto, CustomerBulkEntity>().ReverseMap();
            
            CreateMap<InvoiceDto, InvoiceEntity>()
                .ForMember(d => d.Company, o => o.Ignore())
                .ForMember(d => d.Customer, o => o.Ignore())
                .ForMember(d => d.Payment, o => o.Ignore())
                .ReverseMap();

            CreateMap<PaymentDto, PaymentEntity>()
                .ForMember(d => d.Invoice, o => o.Ignore())
                .ReverseMap()
                .ForMember(d => d.InvoiceNo, o => o.MapFrom(s => s.Invoice != null ? s.Invoice.No : ""))
                .ForMember(d => d.CustomerId, o => o.MapFrom(s => s.Invoice != null ? s.Invoice.CustomerId : null));
        }
    }
}
