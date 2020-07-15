using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Core.Data.Dto;
using Core.Data.Entities;
using Core.Services.Managers;

namespace Core.Services.Business {
    public interface ICustomerBusinessManager {
        Task<List<CustomerCreditUtilizedDto>> UpdateOrCreateCreditUtilized(List<CustomerCreditUtilizedDto> credits);
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

        public async Task<List<CustomerCreditUtilizedDto>> UpdateOrCreateCreditUtilized(List<CustomerCreditUtilizedDto> credits) {
            var list = new List<CustomerCreditUtilizedDto>();
            foreach(var credit in credits) {
                var entity = _mapper.Map<CustomerCreditUtilizedEntity>(credit);

                if(credit.Id == 0) {
                    entity.CreatedDate = credit.NewCreatedDate;
                    entity = await _customerCreditUtilizedManager.Create(entity);
                } else
                    entity = await _customerCreditUtilizedManager.Update(entity);

                list.Add(_mapper.Map<CustomerCreditUtilizedDto>(entity));
            }
            return list;
        }
    }
}
