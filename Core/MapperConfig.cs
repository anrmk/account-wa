using AutoMapper;

using Core.Data.Dto;
using Core.Data.Entities;

namespace Core {
    public class MapperConfig: Profile {
        public MapperConfig() {
            CreateMap<CompanyDto, CompanyEntity>().ReverseMap();
            CreateMap<CompanyDtoList, CompanyEntity>()
                .ReverseMap()
                .ForMember(d => d.Address, o => o.MapFrom(s => s.Address != null ? s.Address.ToString() : "No Address"));
        }
    }
}
