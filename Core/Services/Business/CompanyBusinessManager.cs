using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Core.Data.Dto;
using Core.Data.Entities;
using Core.Services.Managers;

namespace Core.Services.Business {
    public interface ICompanyBusinessManager {
        Task<CompanyDto> GetCompany(long id);
        Task<List<CompanyDto>> GetCompanies();

        Task<List<CompanyRestrictedWordDto>> GetRestrictedWord(long restrictedWordId);
        Task<CompanyRestrictedWordDto> CreateRestrictedWord(CompanyRestrictedWordDto dto);
        Task<CompanyRestrictedWordDto> UpdateRestrictedWord(long id, CompanyRestrictedWordDto dto);
        Task<List<CompanyRestrictedWordDto>> UpdateRestrictedWords(long restrictedWordId, long?[] companiesId);
    }

    public class CompanyBusinessManager: BaseBusinessManager, ICompanyBusinessManager {
        private readonly IMapper _mapper;
        private readonly ICompanyManager _companyManager;
        private readonly ICompanySettingsRestrictedWordManager _companySettingsRestrictedWordManager;
        private readonly ISettingsRestrictedWordManager _settingsRestrictedWordManager;

        public CompanyBusinessManager(IMapper mapper,
            ICompanyManager companyManager,
            ICompanySettingsRestrictedWordManager companySettingsRestrictedWordManager,
            ISettingsRestrictedWordManager settingsRestrictedWordManager
            ) {
            this._mapper = mapper;
            this._companyManager = companyManager;
            this._companySettingsRestrictedWordManager = companySettingsRestrictedWordManager;
            this._settingsRestrictedWordManager = settingsRestrictedWordManager;
        }

        public async Task<CompanyDto> GetCompany(long id) {
            var result = await _companyManager.FindInclude(id);
            return _mapper.Map<CompanyDto>(result);
        }

        public async Task<List<CompanyDto>> GetCompanies() {
            var result = await _companyManager.AllInclude();
            return _mapper.Map<List<CompanyDto>>(result.OrderBy(x => x.Name));
        }

        public async Task<List<CompanyRestrictedWordDto>> GetRestrictedWord(long restrictedWordId) {
            var entities = await _companySettingsRestrictedWordManager.FindByWordId(restrictedWordId);
            return _mapper.Map<List<CompanyRestrictedWordDto>>(entities);
        }

        public async Task<CompanyRestrictedWordDto> CreateRestrictedWord(CompanyRestrictedWordDto dto) {
            var entity = _mapper.Map<CompanyRestrictedWordEntity>(dto);
            entity = await _companySettingsRestrictedWordManager.Create(entity);

            return _mapper.Map<CompanyRestrictedWordDto>(entity);
        }

        public async Task<CompanyRestrictedWordDto> UpdateRestrictedWord(long id, CompanyRestrictedWordDto dto) {
            var entity = await _companySettingsRestrictedWordManager.Find(id);
            if(entity == null) {
                return null;
            }

            var newEntity = _mapper.Map(dto, entity);
            entity = await _companySettingsRestrictedWordManager.Update(newEntity);

            return _mapper.Map<CompanyRestrictedWordDto>(entity);
        }

        public async Task<List<CompanyRestrictedWordDto>> UpdateRestrictedWords(long restrictedWordId, long?[] companiesId) {
            var entities = await _companySettingsRestrictedWordManager.FindByWordId(restrictedWordId);

            //Создать
            var newCompanyRestrictedIds = companiesId.Where(x => !entities.Any(y => y.CompanyId == x)).ToList();
            var createCompanyRestrictedWordList = newCompanyRestrictedIds.Select(x => new CompanyRestrictedWordEntity() { CompanyId = x, RestrictedWordId = restrictedWordId });
            if(createCompanyRestrictedWordList.Count() > 0)
                createCompanyRestrictedWordList = await _companySettingsRestrictedWordManager.Create(createCompanyRestrictedWordList);

            //Удалить
            var removeCompanyRestrictedWordList = entities.Where(x => !companiesId.Any(y => y == x.CompanyId)).ToList();
            if(removeCompanyRestrictedWordList.Count() > 0)
                await _companySettingsRestrictedWordManager.Delete(removeCompanyRestrictedWordList.AsEnumerable());

            entities = await _companySettingsRestrictedWordManager.FindByWordId(restrictedWordId);

            return _mapper.Map<List<CompanyRestrictedWordDto>>(entities);
        }
    }
}
