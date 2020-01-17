using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Core.Data.Dto;
using Core.Data.Entities;
using Core.Services.Managers;

namespace Core.Services.Business {
    public interface ICrudBusinessManager {
        Task<CompanyDto> GetCompany(long id);
        Task<List<CompanyDto>> GetCompanies();
        Task<CompanyDto> CreateCompany(CompanyDto dto);
        Task<CompanyDto> UpdateCompany(long id, CompanyDto dto);
        Task<bool> DeleteCompany(long id);

        Task<CustomerDto> GetCustomer(long id);
        Task<List<CustomerDto>> GetCustomers();
        Task<List<CustomerDto>> GetCustomers(long[] ids);
        Task<List<CustomerDto>> GetCustomers(long companyId);
        Task<CustomerDto> CreateCustomer(CustomerDto dto);
        Task<CustomerDto> UpdateCustomer(long id, CustomerDto dto);
        Task<bool> DeleteCustomer(long id);

        Task<InvoiceDto> GetInvoice(long id);
        Task<List<InvoiceDto>> GetInvoices();
        Task<List<InvoiceDto>> GetUnpaidInvoices(long customerId);
        Task<InvoiceDto> UpdateInvoice(long id, InvoiceDto dto);
        Task<InvoiceDto> CreateInvoice(InvoiceDto dto);
        Task<bool> DeleteInvoice(long id);

        Task<PaymentDto> GetPayment(long id);
        Task<List<PaymentDto>> GetPayments();
        Task<List<PaymentDto>> GetPaymentByInvoiceId(long id);
        Task<PaymentDto> CreatePayment(PaymentDto dto);
    }

    public class CrudBusinessManager: ICrudBusinessManager {
        public readonly IMapper _mapper;
        public readonly ICompanyManager _companyManager;
        public readonly ICustomerManager _customerManager;
        public readonly ICompanyCustomerManager _companyCustomersManager;
        public readonly IInvoiceManager _invoiceManager;
        public readonly IPaymentManager _paymentManager;

        public CrudBusinessManager(IMapper mapper, ICompanyManager companyManager,
            ICustomerManager customerManager, ICompanyCustomerManager companyCustomersManager,
            IInvoiceManager invoiceManager, IPaymentManager paymentManager) {
            _mapper = mapper;
            _companyManager = companyManager;
            _customerManager = customerManager;
            _companyCustomersManager = companyCustomersManager;
            _invoiceManager = invoiceManager;
            _paymentManager = paymentManager;
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
            var addedCustomers = dto.Customers.Select(x => new CompanyCustomerEntity() {
                CompanyId = entity.Id,
                CustomerId = x
            }).ToList();

            await _companyCustomersManager.Create(addedCustomers);
            entity.CompanyCustomers = addedCustomers;

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
            entity = await _companyManager.UpdateType(_mapper.Map(dto, entity));

            var ccitems = await _companyCustomersManager.FindAll(entity.Id);
            var ccIds = ccitems.Select(x => x.CustomerId).ToList();

            //Удалить тех, с кого убрали отметку
            var delteCustomers = ccitems.Where(x => !dto.Customers.Contains(x.CustomerId)).ToList();
            if(delteCustomers.Count() > 0)
                await _companyCustomersManager.Delete(delteCustomers);

            //Исключить выбранных в процессе добавления
            var addedCustomers = dto.Customers.Where(x => !ccIds.Contains(x)).Select(x => new CompanyCustomerEntity() {
                CompanyId = entity.Id,
                CustomerId = x
            }).ToList();


            await _companyCustomersManager.Create(addedCustomers);
            entity.CompanyCustomers = addedCustomers;

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

        public async Task<List<CustomerDto>> GetCustomers(long companyId) {
            var ccitems = await _companyCustomersManager.FindAll(companyId);
            if(ccitems.Count() > 0) {
                var ids = ccitems.Select(x => x.CustomerId).ToArray();
                var result = await _customerManager.FindAllInclude(ids);
                return _mapper.Map<List<CustomerDto>>(result);
            }
            return null;
        }

        public async Task<List<CustomerDto>> GetCustomers(long[] ids) {
            var result = await _customerManager.FindAllInclude(ids);
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

        #region INVOICE
        public async Task<InvoiceDto> GetInvoice(long id) {
            var result = await _invoiceManager.FindInclude(id);
            return _mapper.Map<InvoiceDto>(result);
        }

        public async Task<InvoiceDto> CreateInvoice(InvoiceDto dto) {
            var entity = _mapper.Map<InvoiceEntity>(dto);
            entity = await _invoiceManager.Create(entity);
            return _mapper.Map<InvoiceDto>(entity);
        }

        public async Task<List<InvoiceDto>> GetInvoices() {
            var result = await _invoiceManager.AllInclude();
            return _mapper.Map<List<InvoiceDto>>(result);
        }

        public async Task<List<InvoiceDto>> GetUnpaidInvoices(long customerId) {
            var result = await _invoiceManager.FindUnpaidByCustomerId(customerId);
            return _mapper.Map<List<InvoiceDto>>(result);
        }

        public async Task<bool> DeleteInvoice(long id) {
            var entity = await _invoiceManager.FindInclude(id);
            if(entity == null) {
                return false;
            }
            int result = await _invoiceManager.Delete(entity);
            return result != 0;
        }

        public async Task<InvoiceDto> UpdateInvoice(long id, InvoiceDto dto) {
            if(id != dto.Id) {
                return null;
            }
            var entity = await _invoiceManager.FindInclude(id);
            if(entity == null) {
                return null;
            }

            entity = await _invoiceManager.UpdateType(_mapper.Map(dto, entity));
            return _mapper.Map<InvoiceDto>(entity);
        }
        #endregion

        #region PAYMENT
        public async Task<PaymentDto> GetPayment(long id) {
            var result = await _paymentManager.FindInclude(id);
            return _mapper.Map<PaymentDto>(result);
        }
        public async Task<List<PaymentDto>> GetPayments() {
            var result = await _paymentManager.FindAllInclude();
            return _mapper.Map<List<PaymentDto>>(result);
        }

        public async Task<PaymentDto> CreatePayment(PaymentDto dto) {
            var item = await _invoiceManager.FindInclude(dto.InvoiceId ?? 0);
            if(item == null)
                return null;

            //if(item.Payment != null) {
            //    return _mapper.Map<PaymentDto>(item.Payment);
            //}

            var entity = await _paymentManager.Create(_mapper.Map<PaymentEntity>(dto));

            // item.PaymentId = entity.Id;
            // item = await _invoiceManager.UpdateType(item);

            return _mapper.Map<PaymentDto>(entity);
        }

        public async Task<List<PaymentDto>> GetPaymentByInvoiceId(long id) {
            var entity = await _paymentManager.FindByInvoiceId(id);
            return _mapper.Map<List<PaymentDto>>(entity);
        }
        #endregion
    }
}
