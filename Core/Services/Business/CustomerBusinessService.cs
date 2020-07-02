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
    public interface ICustomerBusinessService {
        Task<CustomerSettingsRestrictedWordDto> GetSettingsRestrictedWord(long id);
        Task<List<CustomerSettingsRestrictedWordDto>> GetSettingsRestrictedWords();
        Task<Pager<CustomerSettingsRestrictedWordDto>> GetSettingsRestrictedWordPage(PagerFilterDto filter);
        Task<CustomerSettingsRestrictedWordDto> CreateSettingsRestrictedWord(CustomerSettingsRestrictedWordDto dto);
        Task<CustomerSettingsRestrictedWordDto> UpdateSettingsRestrictedWord(long id, CustomerSettingsRestrictedWordDto dto);
        Task<bool> DeleteSettingsRestrictedWord(long id);
    }

    public class CustomerBusinessService: BaseBusinessManager, ICustomerBusinessService {
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
        private readonly ICustomerSettingsRestrictedWordManager _customerSettingsRestrictedWordManager;


        public CustomerBusinessService(IMapper mapper,
       ICustomerManager customerManager,
       ICustomerActivityManager customerActivityManager,
       ICustomerCreditLimitManager customerCreditLimitManager,
       ICustomerCreditUtilizedManager customerCreditUtilizedManager,
       ICustomerCreditUtilizedSettingsManager customerCreditUtilizedSettingsManager,
       ICustomerTagManager customerTagManager,
       ICustomerTagLinkManager customerTagLinkManager,
       ICustomerTypeManager customerTypeManager,
       ICustomerRecheckManager customerRecheckManager,
       ICustomerSettingsRestrictedWordManager customerSettingsRestrictedWordManager
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
            _customerSettingsRestrictedWordManager = customerSettingsRestrictedWordManager;
        }

        public async Task<CustomerSettingsRestrictedWordDto> GetSettingsRestrictedWord(long id) {
            var entity = await _customerSettingsRestrictedWordManager.Find(id);
            return _mapper.Map<CustomerSettingsRestrictedWordDto>(entity);
        }

        public async Task<List<CustomerSettingsRestrictedWordDto>> GetSettingsRestrictedWords() {
            var entities = await _customerSettingsRestrictedWordManager.All();
            return _mapper.Map<List<CustomerSettingsRestrictedWordDto>>(entities);
        }

        public async Task<Pager<CustomerSettingsRestrictedWordDto>> GetSettingsRestrictedWordPage(PagerFilterDto filter) {
            var sortby = "Name";
            Expression<Func<CustomerSettingsRestrictedWordEntity, bool>> where = x =>
                  (true)
               && (string.IsNullOrEmpty(filter.Search) || x.Name.ToLower().Contains(filter.Search.ToLower()));

            var tuple = await _customerSettingsRestrictedWordManager.Pager<CustomerSettingsRestrictedWordEntity>(where, sortby, filter.Order.Equals("desc"), filter.Offset, filter.Limit);

            var list = tuple.Item1;
            var count = tuple.Item2;

            if(count == 0)
                return new Pager<CustomerSettingsRestrictedWordDto>(new List<CustomerSettingsRestrictedWordDto>(), 0, filter.Offset, filter.Limit);

            var page = (filter.Offset + filter.Limit) / filter.Limit;

            var result = _mapper.Map<List<CustomerSettingsRestrictedWordDto>>(list);
            return new Pager<CustomerSettingsRestrictedWordDto>(result, count, page, filter.Limit);
        }

        public async Task<CustomerSettingsRestrictedWordDto> CreateSettingsRestrictedWord(CustomerSettingsRestrictedWordDto dto) {
            var entity = await _customerSettingsRestrictedWordManager.Create(_mapper.Map<CustomerSettingsRestrictedWordEntity>(dto));
            return _mapper.Map<CustomerSettingsRestrictedWordDto>(entity);
        }

        public async Task<CustomerSettingsRestrictedWordDto> UpdateSettingsRestrictedWord(long id, CustomerSettingsRestrictedWordDto dto) {
            var entity = await _customerSettingsRestrictedWordManager.Find(id);
            if(entity == null) {
                return null;
            }
            entity = await _customerSettingsRestrictedWordManager.Update(_mapper.Map(dto, entity));
            return _mapper.Map<CustomerSettingsRestrictedWordDto>(entity);
        }

        public async Task<bool> DeleteSettingsRestrictedWord(long id) {
            var entity = await _customerSettingsRestrictedWordManager.Find(id);
            if(entity == null) {
                return false;
            }
            int result = await _customerSettingsRestrictedWordManager.Delete(entity);
            return result != 0;
        }
    }
}
