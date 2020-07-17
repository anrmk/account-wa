using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using AutoMapper;

using Core.Data.Dto;
using Core.Data.Entities;
using Core.Extension;
using Core.Services.Managers;

namespace Core.Services.Business {
    public interface ICompanyBusinessManager {
        Task<CompanyDto> GetCompany(long id);
        Task<List<CompanyDto>> GetCompanies();
        Task<Pager<CompanyDto>> GetCompanyPage(string search, string sort, string order, int offset = 0, int limit = 10);
        Task<CompanyDto> CreateCompany(CompanyDto dto);
        Task<CompanyDto> UpdateCompany(long id, CompanyDto dto);
        Task<bool> DeleteCompany(long id);

        Task<CompanyAddressDto> GetAddress(long id);
        Task<CompanyAddressDto> UpdateAddress(long companyId, CompanyAddressDto dto);

        Task<CompanySettingsDto> GetSettings(long id);
        Task<CompanySettingsDto> UpdateSettings(long companyId, CompanySettingsDto dto);

        Task<CompanyExportSettingsDto> GetExportSettings(long id);
        Task<List<CompanyExportSettingsDto>> GetAllExportSettings(long companyId);
        Task<CompanyExportSettingsDto> CreateExportSettings(CompanyExportSettingsDto dto);
        Task<CompanyExportSettingsDto> UpdateExportSettings(long id, CompanyExportSettingsDto dto);
        Task<bool> DeleteExportSettings(long id);

        Task<CompanyExportSettingsFieldDto> GetExportSettingsField(long id);
        Task<CompanyExportSettingsFieldDto> CreateExportSettingsField(CompanyExportSettingsFieldDto dto);
        Task<bool> DeleteExportSettingsField(long id);

        Task<CompanySummaryRangeDto> GetSummaryRange(long id);
        Task<List<CompanySummaryRangeDto>> GetSummaryRanges(long companyId);
        Task<CompanySummaryRangeDto> CreateSummaryRange(CompanySummaryRangeDto dto);
        Task<CompanySummaryRangeDto> UpdateSummaryRange(long id, CompanySummaryRangeDto dto);
        Task<bool> DeleteSummaryRange(long id);

        Task<List<CompanyRestrictedWordDto>> GetRestrictedWord(long restrictedWordId);
        Task<CompanyRestrictedWordDto> CreateRestrictedWord(CompanyRestrictedWordDto dto);
        Task<CompanyRestrictedWordDto> UpdateRestrictedWord(long id, CompanyRestrictedWordDto dto);
        Task<List<CompanyRestrictedWordDto>> UpdateRestrictedWords(long restrictedWordId, long?[] companiesId);
    }

    public class CompanyBusinessManager: BaseBusinessManager, ICompanyBusinessManager {
        private readonly IMapper _mapper;
        private readonly ICompanyManager _companyManager;
        private readonly ICompanyAddressMananger _companyAddressManager;
        private readonly ICompanySettingsManager _companySettingsManager;

        private readonly ICompanyExportSettingsManager _companyExportSettingsManager;
        private readonly ICompanyExportSettingsFieldManager _companyExportSettingsFieldManager;
        private readonly ICompanySummaryRangeManager _companySummaryManager;



        private readonly ICompanySettingsRestrictedWordManager _companySettingsRestrictedWordManager;
        private readonly ISettingsRestrictedWordManager _settingsRestrictedWordManager;

        public CompanyBusinessManager(IMapper mapper,
            ICompanyManager companyManager,
            ICompanyAddressMananger companyAddressManager,
            ICompanySettingsManager companySettingsManager,
            ICompanyExportSettingsManager companyExportSettingsManager,
            ICompanyExportSettingsFieldManager companyExportSettingsFieldManager,
            ICompanySummaryRangeManager companySummaryRangeManager,

        ICompanySettingsRestrictedWordManager companySettingsRestrictedWordManager,
            ISettingsRestrictedWordManager settingsRestrictedWordManager
            ) {
            _mapper = mapper;
            _companyManager = companyManager;
            _companyAddressManager = companyAddressManager;
            _companySettingsManager = companySettingsManager;
            _companyExportSettingsManager = companyExportSettingsManager;
            _companyExportSettingsFieldManager = companyExportSettingsFieldManager;
            _companySummaryManager = companySummaryRangeManager;

            _companySettingsRestrictedWordManager = companySettingsRestrictedWordManager;
            _settingsRestrictedWordManager = settingsRestrictedWordManager;
        }

        #region COMPANY
        public async Task<CompanyDto> GetCompany(long id) {
            var result = await _companyManager.FindInclude(id);
            return _mapper.Map<CompanyDto>(result);
        }

        public async Task<List<CompanyDto>> GetCompanies() {
            var result = await _companyManager.AllInclude();
            return _mapper.Map<List<CompanyDto>>(result.OrderBy(x => x.Name));
        }

        public async Task<Pager<CompanyDto>> GetCompanyPage(string search, string sort, string order, int offset = 0, int limit = 10) {
            Expression<Func<CompanyEntity, bool>> where = x =>
                   (true)
                   && (x.No.Contains(search) || x.Name.Contains(search));

            var sortby = sort ?? "No";

            string[] include = new string[] { "Address" };

            Tuple<List<CompanyEntity>, int> tuple = await _companyManager.Pager<CompanyEntity>(where, sortby, offset, limit, include);
            var list = tuple.Item1;
            var count = tuple.Item2;

            if(count == 0)
                return new Pager<CompanyDto>(new List<CompanyDto>(), 0, offset, limit);

            var page = (offset + limit) / limit;

            var result = _mapper.Map<List<CompanyDto>>(list);
            return new Pager<CompanyDto>(result, count, page, limit);
        }

        public async Task<CompanyDto> CreateCompany(CompanyDto dto) {
            var newEntity = _mapper.Map<CompanyEntity>(dto);
            newEntity.Address = new CompanyAddressEntity();
            newEntity.Settings = new CompanySettingsEntity();

            var entity = await _companyManager.Create(newEntity);
            return _mapper.Map<CompanyDto>(entity);
        }

        public async Task<CompanyDto> UpdateCompany(long id, CompanyDto dto) {
            var entity = await _companyManager.Find(id);
            if(entity == null) {
                return null;
            }
            var newEntity = _mapper.Map(dto, entity);
            entity = await _companyManager.Update(newEntity);

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
        #endregion

        #region COMPANY ADDRESS
        public async Task<CompanyAddressDto> GetAddress(long id) {
            var result = await _companyAddressManager.Find(id);
            return _mapper.Map<CompanyAddressDto>(result);
        }

        public async Task<CompanyAddressDto> UpdateAddress(long companyId, CompanyAddressDto dto) {
            var entity = await _companyAddressManager.Find(dto.Id);
            if(entity == null) {
                return null;
            }

            var newEntity = _mapper.Map(dto, entity);
            entity = await _companyAddressManager.Update(newEntity);
            return _mapper.Map<CompanyAddressDto>(entity);
        }
        #endregion

        #region COMPANY SETTINGS
        public async Task<CompanySettingsDto> GetSettings(long id) {
            var result = await _companySettingsManager.Find(id);
            return _mapper.Map<CompanySettingsDto>(result);
        }

        public async Task<CompanySettingsDto> UpdateSettings(long companyId, CompanySettingsDto dto) {
            var entity = await _companySettingsManager.Find(dto.Id);
            if(entity == null) {
                var companyEntity = await _companyManager.Find(companyId);
                if(companyEntity == null) {
                    return null;
                }
                if(!companyEntity.SettingsId.HasValue) {
                    entity = await _companySettingsManager.Create(_mapper.Map<CompanySettingsEntity>(dto));
                    companyEntity.SettingsId = entity.Id;
                    await _companyManager.Update(companyEntity);
                    return _mapper.Map<CompanySettingsDto>(entity);
                }
            }

            var newEntity = _mapper.Map(dto, entity);
            entity = await _companySettingsManager.Update(newEntity);
            return _mapper.Map<CompanySettingsDto>(entity);
        }
        #endregion

        #region COMPANY EXPORT SETTINGS
        public async Task<CompanyExportSettingsDto> GetExportSettings(long id) {
            var result = await _companyExportSettingsManager.FindInclude(id);
            return _mapper.Map<CompanyExportSettingsDto>(result);
        }

        public async Task<List<CompanyExportSettingsDto>> GetAllExportSettings(long companyId) {
            var result = await _companyExportSettingsManager.FindAllByCompanyId(companyId);
            return _mapper.Map<List<CompanyExportSettingsDto>>(result);
        }

        public async Task<CompanyExportSettingsDto> CreateExportSettings(CompanyExportSettingsDto dto) {
            var company = await _companyManager.Find(dto.CompanyId);
            if(company == null) {
                return null;
            }

            var newEntity = _mapper.Map<CompanyExportSettingsEntity>(dto);
            var entity = await _companyExportSettingsManager.Create(newEntity);
            return _mapper.Map<CompanyExportSettingsDto>(entity);
        }

        public async Task<CompanyExportSettingsDto> UpdateExportSettings(long id, CompanyExportSettingsDto dto) {
            var entity = await _companyExportSettingsManager.FindInclude(id);
            if(entity == null) {
                return null;
            }

            var newEntity = _mapper.Map(dto, entity);
            entity = await _companyExportSettingsManager.Update(newEntity);

            #region UPDATE FiELDS LIST
            foreach(var fieldDto in dto.Fields) {
                var field = await _companyExportSettingsFieldManager.Find(fieldDto.Id);
                if(field != null) {
                    var fieldEntity = _mapper.Map(fieldDto, field);
                    fieldEntity = await _companyExportSettingsFieldManager.Update(fieldEntity);
                }
            }
            #endregion

            return _mapper.Map<CompanyExportSettingsDto>(entity);
        }

        public async Task<bool> DeleteExportSettings(long id) {
            var entity = await _companyExportSettingsManager.FindInclude(id);
            if(entity == null) {
                return false;
            }
            int result = await _companyExportSettingsManager.Delete(entity);
            return result != 0;
        }
        #endregion

        #region EXPORT SETTINGS FIELD
        public async Task<CompanyExportSettingsFieldDto> GetExportSettingsField(long id) {
            var result = await _companyExportSettingsFieldManager.Find(id);
            return _mapper.Map<CompanyExportSettingsFieldDto>(result);
        }

        public async Task<CompanyExportSettingsFieldDto> CreateExportSettingsField(CompanyExportSettingsFieldDto dto) {
            var settings = await _companyExportSettingsManager.Find(dto.ExportSettingsId);
            if(settings == null) {
                return null;
            }

            var newEntity = _mapper.Map<CompanyExportSettingsFieldEntity>(dto);
            var entity = await _companyExportSettingsFieldManager.Create(newEntity);
            return _mapper.Map<CompanyExportSettingsFieldDto>(entity);
        }

        public async Task<bool> DeleteExportSettingsField(long id) {
            var entity = await _companyExportSettingsFieldManager.Find(id);
            if(entity == null) {
                return false;
            }
            int result = await _companyExportSettingsFieldManager.Delete(entity);
            return result != 0;
        }
        #endregion

        #region SUMMARY RANGE
        /// <summary>
        /// Получить ценовую группу по идунтификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<CompanySummaryRangeDto> GetSummaryRange(long id) {
            var result = await _companySummaryManager.Find(id);
            return _mapper.Map<CompanySummaryRangeDto>(result);
        }

        public async Task<List<CompanySummaryRangeDto>> GetSummaryRanges(long companyId) {
            var result = await _companySummaryManager.FindAllByCompanyId(companyId);
            return _mapper.Map<List<CompanySummaryRangeDto>>(result);
        }

        public async Task<CompanySummaryRangeDto> CreateSummaryRange(CompanySummaryRangeDto dto) {
            var company = await _companyManager.Find(dto.CompanyId);
            if(company == null) {
                return null;
            }
            var newEntity = _mapper.Map<CompanySummaryRangeEntity>(dto);

            var entity = await _companySummaryManager.Create(newEntity);
            return _mapper.Map<CompanySummaryRangeDto>(entity);
        }

        public async Task<CompanySummaryRangeDto> UpdateSummaryRange(long id, CompanySummaryRangeDto dto) {
            var entity = await _companySummaryManager.FindInclude(id);
            if(entity == null) {
                return null;
            }

            var newEntity = _mapper.Map(dto, entity);
            entity = await _companySummaryManager.Update(newEntity);

            return _mapper.Map<CompanySummaryRangeDto>(entity);
        }

        public async Task<bool> DeleteSummaryRange(long id) {
            var entity = await _companySummaryManager.FindInclude(id);
            if(entity == null) {
                return false;
            }
            int result = await _companySummaryManager.Delete(entity);
            return result != 0;
        }
        #endregion

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
