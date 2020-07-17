using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using Core.Data.Dto;
using Core.Data.Entities;
using Core.Services.Managers;

namespace Core.Services.Business {
    public interface ICustomerBusinessManager {
        Task<CustomerDto> GetCustomer(long id);
        Task<CustomerDto> GetCustomer(string no, long companyId);
        Task<List<CustomerDto>> GetCustomers();
        Task<List<CustomerDto>> GetCustomers(long companyId);


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

        public async Task<CustomerDto> GetCustomer(long id) {
            var result = await _customerManager.FindInclude(id);
            return _mapper.Map<CustomerDto>(result);
        }

        public async Task<CustomerDto> GetCustomer(string no, long companyId) {
            var result = await _customerManager.FindInclude(no, companyId);
            return _mapper.Map<CustomerDto>(result);
        }

        public async Task<List<CustomerDto>> GetCustomers() {
            var result = await _customerManager.AllInclude();
            return _mapper.Map<List<CustomerDto>>(result);
        }

        public async Task<List<CustomerDto>> GetCustomers(long companyId) {
            var result = await _customerManager.FindByCompanyId(companyId);
            return _mapper.Map<List<CustomerDto>>(result);
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
