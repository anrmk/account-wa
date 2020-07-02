using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using AutoMapper;

using Core.Data.Dto;
using Core.Data.Entities;
using Core.Extension;
using Core.Services.Managers;

namespace Core.Services.Business {
    public interface ISettingsBusinessService {
        Task<SettingsRestrictedWordDto> GetRestrictedWord(long id);
        Task<List<SettingsRestrictedWordDto>> GetRestrictedWords();
        Task<Pager<SettingsRestrictedWordDto>> GetRestrictedWordPage(PagerFilterDto filter);
        Task<SettingsRestrictedWordDto> CreateRestrictedWord(SettingsRestrictedWordDto dto);
        Task<SettingsRestrictedWordDto> UpdateRestrictedWord(long id, SettingsRestrictedWordDto dto);
        Task<bool> DeleteRestrictedWord(long id);
    }

    public class SettingsBusinessService: BaseBusinessManager, ISettingsBusinessService {
        private readonly IMapper _mapper;
        private readonly ICustomerManager _customerManager;
        private readonly ICustomerActivityManager _customerActivityManager;
        private readonly ICustomerCreditLimitManager _customerCreditLimitManager;
        private readonly ICustomerCreditUtilizedManager _customerCreditUtilizedManager;
        private readonly ICustomerCreditUtilizedSettingsManager _customerCreditUtilizedSettingsManager;
        private readonly ICustomerTagManager _customerTagManager;
        private readonly ICustomerTagLinkManager _customerTagLinkManager;
        private readonly ICustomerTypeManager _customerTypeManager;
        private readonly ICustomerRecheckManager _customerRecheckManager;
        private readonly ISettingsRestrictedWordManager _settingsRestrictedWordManager;


        public SettingsBusinessService(IMapper mapper,
       ICustomerManager customerManager,
       ICustomerActivityManager customerActivityManager,
       ICustomerCreditLimitManager customerCreditLimitManager,
       ICustomerCreditUtilizedManager customerCreditUtilizedManager,
       ICustomerCreditUtilizedSettingsManager customerCreditUtilizedSettingsManager,
       ICustomerTagManager customerTagManager,
       ICustomerTagLinkManager customerTagLinkManager,
       ICustomerTypeManager customerTypeManager,
       ICustomerRecheckManager customerRecheckManager,
       ISettingsRestrictedWordManager settingsRestrictedWordManager
            ) {
            _mapper = mapper;

            _customerManager = customerManager;
            _customerActivityManager = customerActivityManager;
            _customerCreditLimitManager = customerCreditLimitManager;
            _customerCreditUtilizedManager = customerCreditUtilizedManager;
            _customerCreditUtilizedSettingsManager = customerCreditUtilizedSettingsManager;
            _customerTagManager = customerTagManager;
            _customerTagLinkManager = customerTagLinkManager;
            _customerTypeManager = customerTypeManager;
            _customerRecheckManager = customerRecheckManager;

            _settingsRestrictedWordManager = settingsRestrictedWordManager;
        }

        public async Task<SettingsRestrictedWordDto> GetRestrictedWord(long id) {
            var entity = await _settingsRestrictedWordManager.Find(id);
            return _mapper.Map<SettingsRestrictedWordDto>(entity);
        }

        public async Task<List<SettingsRestrictedWordDto>> GetRestrictedWords() {
            var entities = await _settingsRestrictedWordManager.All();
            return _mapper.Map<List<SettingsRestrictedWordDto>>(entities);
        }

        public async Task<Pager<SettingsRestrictedWordDto>> GetRestrictedWordPage(PagerFilterDto filter) {
            var sortby = "Name";
            Expression<Func<SettingsRestrictedWordEntity, bool>> where = x =>
                  (true)
               && (string.IsNullOrEmpty(filter.Search) || x.Name.ToLower().Contains(filter.Search.ToLower()));

            var tuple = await _settingsRestrictedWordManager.Pager<SettingsRestrictedWordEntity>(where, sortby, filter.Order.Equals("desc"), filter.Offset, filter.Limit);

            var list = tuple.Item1;
            var count = tuple.Item2;

            if(count == 0)
                return new Pager<SettingsRestrictedWordDto>(new List<SettingsRestrictedWordDto>(), 0, filter.Offset, filter.Limit);

            var page = (filter.Offset + filter.Limit) / filter.Limit;

            var result = _mapper.Map<List<SettingsRestrictedWordDto>>(list);
            return new Pager<SettingsRestrictedWordDto>(result, count, page, filter.Limit);
        }

        public async Task<SettingsRestrictedWordDto> CreateRestrictedWord(SettingsRestrictedWordDto dto) {
            var entity = await _settingsRestrictedWordManager.Create(_mapper.Map<SettingsRestrictedWordEntity>(dto));
            return _mapper.Map<SettingsRestrictedWordDto>(entity);
        }

        public async Task<SettingsRestrictedWordDto> UpdateRestrictedWord(long id, SettingsRestrictedWordDto dto) {
            var entity = await _settingsRestrictedWordManager.Find(id);
            if(entity == null) {
                return null;
            }
            entity = await _settingsRestrictedWordManager.Update(_mapper.Map(dto, entity));
            return _mapper.Map<SettingsRestrictedWordDto>(entity);
        }

        public async Task<bool> DeleteRestrictedWord(long id) {
            var entity = await _settingsRestrictedWordManager.Find(id);
            if(entity == null) {
                return false;
            }
            int result = await _settingsRestrictedWordManager.Delete(entity);
            return result != 0;
        }
    }
}
