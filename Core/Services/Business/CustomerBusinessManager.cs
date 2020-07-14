using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Core.Data.Dto;
using Core.Services.Managers;

namespace Core.Services.Business {
    public interface ICustomerBusinessManager {
        Task<List<CustomerCreditUtilizedDto>> CreditUtilizedChangeStatus(long[] ids, bool isIgnored);
    }

    public class CustomerBusinessManager: BaseBusinessManager, ICustomerBusinessManager {
        private readonly IMapper _mapper;
        private readonly ICustomerManager _customerManager;
        private readonly ICustomerCreditUtilizedManager _customerCreditUtilizedManager;

        public CustomerBusinessManager(IMapper mapper, ICustomerManager customerManager,
            ICustomerCreditUtilizedManager customerCreditUtilizedManager
            ) {
            _mapper = mapper;
            _customerManager = customerManager;
            _customerCreditUtilizedManager = customerCreditUtilizedManager;
        }

        public async Task<List<CustomerCreditUtilizedDto>> CreditUtilizedChangeStatus(long[] ids, bool isIgnored) {
            var entities = await _customerCreditUtilizedManager.FindInclude(ids);
            if(entities == null || entities.Count == 0) {
                return null;
            }

            entities.ForEach(x => x.IsIgnored = isIgnored);
            var result = await _customerCreditUtilizedManager.Update(entities.AsEnumerable());
            var dtos = _mapper.Map<List<CustomerCreditUtilizedDto>>(result);
            return dtos;
        }
    }
}
