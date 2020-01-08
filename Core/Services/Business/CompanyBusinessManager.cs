using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Core.Data.Dto;
using Core.Data.Entities;
using Core.Services.Managers;

namespace Core.Services.Business {
    public interface ICompanyBusinessManager {
        Task<CompanyDto> GetCompany(long id);
        Task<List<CompanyDto>> GetCompanies();
        Task<CompanyDto> CreateCompany(CompanyDto dto);
        Task<CompanyDto> UpdateCompany(long id, CompanyDto dto);
        Task<bool> DeleteCompany(long id);
    }

    public class CompanyBusinessManager : ICompanyBusinessManager {
        public readonly IMapper _mapper;
        public readonly ICompanyManager _companyManager;

        public CompanyBusinessManager(IMapper mapper, ICompanyManager companyManager) {
            _mapper = mapper;
            _companyManager = companyManager;
        }

        public async Task<CompanyDto> GetCompany(long id) {
            var result = await _companyManager.FindInclude(id);
            return _mapper.Map<CompanyDto>(result);
        }

        public async Task<List<CompanyDto>> GetCompanies() {
            var result = await _companyManager.AllInclude();
            return _mapper.Map<List<CompanyDto>>(result);
        }

        public async Task<CompanyDto> CreateCompany(CompanyDto dto) {
            var entity = _mapper.Map<CompanyEntity>(dto);
            entity = await _companyManager.Create(entity);
            return _mapper.Map<CompanyDto>(entity);
        }

        public async Task<CompanyDto> UpdateCompany(long id, CompanyDto dto) {
            if(id != dto.Id) {
                return null;
            }
            var entity = await _companyManager.FindInclude(id);
            if(entity == null) {
                return null;
            }

            entity =  await _companyManager.UpdateType(_mapper.Map(dto, entity));
            return _mapper.Map<CompanyDto>(entity);

        }

        public async Task<bool> DeleteCompany(long id) {
            var entity = await _companyManager.FindInclude(id);
            if(entity == null) {
                return false;
            }
            int result = await _companyManager.Delete(entity);
            return result != 0;
        }
    }
}
