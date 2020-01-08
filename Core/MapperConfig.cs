using AutoMapper;

using Core.Data.Dto;
using Core.Data.Entities;

namespace Core {
    public class MapperConfig: Profile {
        public MapperConfig() {
            CreateMap<CompanyDto, CompanyEntity>().ReverseMap();
            CreateMap<CompanyAddressDto, CompanyAddressEntity>().ReverseMap();
            //.ForMember(d => d.Address, o => o.MapFrom(s => s.Address != null ? s.Address.Address : ""))
            //.ForMember(d => d.Address2, o => o.MapFrom(s => s.Address != null ? s.Address.Address2 : ""))
            //.ForMember(d => d.City, o => o.MapFrom(s => s.Address != null ? s.Address.City : ""))
            //.ForMember(d => d.State, o => o.MapFrom(s => s.Address != null ? s.Address.State : ""))
            //.ForMember(d => d.ZipCode, o => o.MapFrom(s => s.Address != null ? s.Address.ZipCode : ""))
            //.ForMember(d => d.Country, o => o.MapFrom(s => s.Address != null ? s.Address.Country : ""));

        }
    }
}
