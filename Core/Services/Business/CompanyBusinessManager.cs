using System.Collections.Generic;
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

        Task<CustomerDto> GetCustomer(long id);
        Task<List<CustomerDto>> GetCustomers();
        Task<CustomerDto> CreateCustomer(CustomerDto dto);
        Task<CustomerDto> UpdateCustomer(long id, CustomerDto dto);
        Task<bool> DeleteCustomer(long id);
    }

    public class CompanyBusinessManager: ICompanyBusinessManager {
        public readonly IMapper _mapper;
        public readonly ICompanyManager _companyManager;
        public readonly ICustomerManager _customerManager;

        public CompanyBusinessManager(IMapper mapper, ICompanyManager companyManager,
            ICustomerManager customerManager) {
            _mapper = mapper;
            _companyManager = companyManager;
            _customerManager = customerManager;
        }

        #region COMPANY

        public async Task<CompanyDto> GetCompany(long id) {
            var result = await _companyManager.FindInclude(id);
            return _mapper.Map<CompanyDto>(result);
        }

        public async Task<List<CompanyDto>> GetCompanies() {
            var result = await _companyManager.AllInclude();
            return _mapper.Map<List<CompanyDto>>(result);
        }

        public async Task<CompanyDto> CreateCompany(CompanyDto dto) {
            var entity = await _companyManager.Create(_mapper.Map<CompanyEntity>(dto));
            entity.Customers = new List<CustomerEntity>();

            foreach(long customerId in dto.Customers) {
                var customer = await _customerManager.Find(customerId);
                if(customer != null) {
                    customer.CompanyId = entity.Id;
                    await _customerManager.Update(customer);

                    entity.Customers.Add(customer);
                }
            };

            
            return _mapper.Map<CompanyDto>(entity);
        }

        public async Task<CompanyDto> UpdateCompany(long id, CompanyDto dto) {
            if(id != dto.Id) {
                return null;
            }
            var entity = await _companyManager.Find(id);
            if(entity == null) {
                return null;
            }
            entity = _mapper.Map(dto, entity);

            var customers = await _customerManager.FindByCompanyId(id);
            foreach(var c in customers) {
                c.CompanyId = null;
                await _customerManager.Update(c);
            }

            foreach(long customerId in dto.Customers) {
                var customer = await _customerManager.Find(customerId);
                if(customer != null) {
                    customer.CompanyId = entity.Id;
                    await _customerManager.Update(customer);
                    //entity.Customers.Add(customer);
                }
            };

            entity = await _companyManager.UpdateType(entity);
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

        #region CUSTOMER
        public async Task<CustomerDto> GetCustomer(long id) {
            var result = await _customerManager.FindInclude(id);
            return _mapper.Map<CustomerDto>(result);
        }

        public async Task<List<CustomerDto>> GetCustomers() {
            var result = await _customerManager.AllInclude();
            return _mapper.Map<List<CustomerDto>>(result);
        }

        public async Task<CustomerDto> CreateCustomer(CustomerDto dto) {
            var entity = _mapper.Map<CustomerEntity>(dto);
            entity = await _customerManager.Create(entity);
            return _mapper.Map<CustomerDto>(entity);
        }

        public async Task<CustomerDto> UpdateCustomer(long id, CustomerDto dto) {
            if(id != dto.Id) {
                return null;
            }
            var entity = await _customerManager.FindInclude(id);
            if(entity == null) {
                return null;
            }

            entity = await _customerManager.UpdateType(_mapper.Map(dto, entity));
            return _mapper.Map<CustomerDto>(entity);

        }

        public async Task<bool> DeleteCustomer(long id) {
            var entity = await _customerManager.FindInclude(id);
            if(entity == null) {
                return false;
            }
            int result = await _customerManager.Delete(entity);
            return result != 0;
        }
        #endregion
    }
}
