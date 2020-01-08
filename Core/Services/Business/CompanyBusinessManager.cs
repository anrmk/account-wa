using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Core.Data.Dto;
using Core.Services.Managers;

namespace Core.Services.Business {
    public interface ICompanyBusinessManager {
        Task<List<CompanyDtoList>> GetCompanies();
    }

    public class CompanyBusinessManager : ICompanyBusinessManager {
        public readonly IMapper _mapper;
        public readonly ICompanyManager _companyManager;

        public CompanyBusinessManager(IMapper mapper, ICompanyManager companyManager) {
            _mapper = mapper;
            _companyManager = companyManager;
        }

        public async Task<List<CompanyDtoList>> GetCompanies() {
            var result = await _companyManager.All();
            return _mapper.Map<List<CompanyDtoList>>(result);
        }
    }
}
