using System.Linq;
using AutoMapper;

using Core.Data.Dto;
using Core.Data.Entities;

namespace Core {
    public class MapperConfig: Profile {
        public MapperConfig() {
            CreateMap<CompanyDto, CompanyEntity>()
                .ForMember(d => d.Customers, o => o.Ignore())
                .ReverseMap()
                .ForMember(d => d.Customers, o => o.MapFrom(s => s.Customers.Select(x => x.Id) ?? null));

            CreateMap<CompanyAddressDto, CompanyAddressEntity>().ReverseMap();

            CreateMap<CustomerDto, CustomerEntity>().ReverseMap();
            CreateMap<CustomerAddressDto, CustomerAddressEntity>().ReverseMap();
        }
    }
}
