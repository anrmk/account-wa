﻿using System;
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
    public interface ICrudBusinessManager {
        Task<CompanyDto> GetCompany(long id);
        Task<List<CompanyDto>> GetCompanies();
        Task<Pager<CompanyDto>> GetCompanyPage(string search, string sort, string order, int offset = 0, int limit = 10);
        Task<CompanyDto> CreateCompany(CompanyDto dto);
        Task<CompanyDto> UpdateCompany(long id, CompanyDto dto);
        Task<bool> DeleteCompany(long id);

        Task<bool> GetCompanyExportSettings(long id);

        Task<CompanySummaryRangeDto> GetCompanySummeryRange(long id);
        Task<List<CompanySummaryRangeDto>> GetCompanyAllSummaryRange(long companyId);
        Task<CompanySummaryRangeDto> CreateCompanySummaryRange(CompanySummaryRangeDto dto);

        Task<CustomerDto> GetCustomer(long id);
        Task<List<CustomerDto>> GetCustomers();
        Task<Pager<CustomerDto>> GetCustomersPage(string search, string sort, string order, int offset = 0, int limit = 10);
        Task<List<CustomerDto>> GetUntiedCustomers(long? companyId);
        Task<List<CustomerDto>> GetCustomers(long[] ids);
        Task<List<CustomerDto>> GetCustomers(long companyId);
        Task<List<CustomerDto>> GetBulkCustomers(long companyId, DateTime from, DateTime to);
        Task<CustomerDto> CreateCustomer(CustomerDto dto);
        Task<CustomerDto> UpdateCustomer(long id, CustomerDto dto);
        Task<bool> DeleteCustomer(long id);

        Task<InvoiceDto> GetInvoice(long id);
        //Task<Pager<InvoiceDto>> GetInvoicePage(long? companyId, DateTime? date, int? dayePerPeriod, int? numberOfPeriod, long? period, string search, string sort, string order, int offset = 0, int limit = 10);
        Task<Pager<InvoiceDto>> GetInvoicePage(InvoiceFilterDto filter);

        Task<List<InvoiceDto>> GetUnpaidInvoices(long customerId);
        Task<List<InvoiceDto>> GetUnpaidInvoicesByCompanyId(long companyId, DateTime from, DateTime to);
        Task<InvoiceDto> UpdateInvoice(long id, InvoiceDto dto);
        Task<InvoiceDto> CreateInvoice(InvoiceDto dto);
        Task<List<InvoiceDto>> CreateInvoice(List<InvoiceDto> list);
        Task<bool> DeleteInvoice(long id);

        Task<PaymentDto> GetPayment(long id);
        Task<Pager<PaymentDto>> GetPaymentPages(string search, string sort, string order, int offset = 0, int limit = 10);
        Task<List<PaymentDto>> GetPaymentByInvoiceId(long id);
        Task<List<InvoiceDto>> GetInvoices(long[] ids);
        Task<PaymentDto> CreatePayment(PaymentDto dto);
        Task<List<PaymentDto>> CreatePayment(List<PaymentDto> list);
        Task<PaymentDto> UpdatePayment(long id, PaymentDto dto);
        Task<bool> DeletePayment(long id);
    }

    public class CrudBusinessManager: BaseBusinessManager, ICrudBusinessManager {
        public readonly IMapper _mapper;
        public readonly ICompanyManager _companyManager;
        public readonly ICompanyAddressMananger _companyAddressManager;
        public readonly ICompanySummaryRangeManager _companySummaryManager;
        public readonly ICustomerManager _customerManager;
        public readonly ICustomerActivityManager _customerActivityManager;
        public readonly IInvoiceManager _invoiceManager;
        public readonly IPaymentManager _paymentManager;
        public readonly IReportManager _reportManager;

        public readonly INsiBusinessManager _nsiBusinessManager;

        public CrudBusinessManager(IMapper mapper, ICompanyManager companyManager,
            ICompanyAddressMananger companyAddressManager,
            ICompanySummaryRangeManager companySummaryManager,
            ICustomerManager customerManager, ICustomerActivityManager customerActivityManager,
            IInvoiceManager invoiceManager, IPaymentManager paymentManager, IReportManager reportManager, INsiBusinessManager nsiBusinessManager) {
            _mapper = mapper;
            _companyManager = companyManager;
            _companyAddressManager = companyAddressManager;
            _companySummaryManager = companySummaryManager;
            _customerManager = customerManager;
            _customerActivityManager = customerActivityManager;
            _invoiceManager = invoiceManager;
            _paymentManager = paymentManager;
            _nsiBusinessManager = nsiBusinessManager;
            _reportManager = reportManager;
        }

        #region COMPANY
        public async Task<CompanyDto> GetCompany(long id) {
            var result = await _companyManager.FindInclude(id);
            return _mapper.Map<CompanyDto>(result);
        }

        public async Task<Pager<CompanyDto>> GetCompanyPage(string search, string sort, string order, int offset = 0, int limit = 10) {
            Expression<Func<CompanyEntity, bool>> wherePredicate = x =>
                   (true)
                   && (x.No.Contains(search) || x.Name.Contains(search));

            #region Sort
            var sortby = GetExpression<CompanyEntity>(sort ?? "Name");
            #endregion

            string[] include = new string[] { "Address" };

            Tuple<List<CompanyEntity>, int> tuple = await _companyManager.Pager<CompanyEntity>(wherePredicate, sortby, offset, limit, include);
            var list = tuple.Item1;
            var count = tuple.Item2;

            if(count == 0)
                return new Pager<CompanyDto>(new List<CompanyDto>(), 0, offset, limit);

            var page = (offset + limit) / limit;

            var result = _mapper.Map<List<CompanyDto>>(list);
            return new Pager<CompanyDto>(result, count, page, limit);
        }

        public async Task<List<CompanyDto>> GetCompanies() {
            var result = await _companyManager.AllInclude();
            var map = _mapper.Map<List<CompanyDto>>(result);
            return map;
        }

        public async Task<CompanyDto> CreateCompany(CompanyDto dto) {
            var entity = await _companyManager.Create(_mapper.Map<CompanyEntity>(dto));

            //TODO: Провверить работу создания и сохранения компании
            //var customers = await _customerManager.FindByIds(dto.Customers.Select(x => x.Id).ToArray());
            //customers.ForEach(x => x.CompanyId = entity.Id);
            //await _customerManager.Update(customers);
            //entity.Customers = customers;

            return _mapper.Map<CompanyDto>(entity);
        }

        public async Task<CompanyDto> UpdateCompany(long id, CompanyDto dto) {
            var entity = await _companyManager.FindInclude(id);
            if(entity == null) {
                return null;
            }
            var newEntity = _mapper.Map(dto, entity);
            entity = await _companyManager.Update(newEntity);

            #region UPDATE CUSTOMER LIST
            var customers = await _customerManager.FindByCompanyId(entity.Id);

            //list of customers to delete
            //TODO: Проверить работу функции редактирования компании
            var customersForDelete = customers.Where(x => !dto.Customers.Any(y => y.Id == x.Id)).ToList();
            customersForDelete.ForEach(x => x.CompanyId = null);

            //list of customres to insert
            var selectedCustomersIds = dto.Customers.Where(x => customers.Where(p => p.Id == x.Id).FirstOrDefault() == null).ToList();
            var customersForInsert = await _customerManager.FindByIds(selectedCustomersIds.Select(x => x.Id).ToArray());
            customersForInsert.ForEach(x => x.CompanyId = entity.Id);

            var customersUpdate = customersForDelete.Union(customersForInsert);
            await _customerManager.Update(customersUpdate);
            #endregion

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

        /// <summary>
        /// Получить ценовую группу по идунтификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<CompanySummaryRangeDto> GetCompanySummeryRange(long id) {
            var result = await _companySummaryManager.Find(id);
            return _mapper.Map<CompanySummaryRangeDto>(result);
        }

        /// <summary>
        /// Получить список все ценовых групп компании
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<List<CompanySummaryRangeDto>> GetCompanyAllSummaryRange(long companyId) {
            var result = await _companySummaryManager.FindAllByCompanyId(companyId);
            return _mapper.Map<List<CompanySummaryRangeDto>>(result);
        }

        public async Task<CompanySummaryRangeDto> CreateCompanySummaryRange(CompanySummaryRangeDto dto) {
            var company = await _companyManager.Find(dto.CompanyId);
            if(company == null) {
                return null;
            }
            var newEntity = _mapper.Map<CompanySummaryRangeEntity>(dto);

            var entity = await _companySummaryManager.Create(newEntity);
            return _mapper.Map<CompanySummaryRangeDto>(entity);
        }

        public async Task<bool> GetCompanyExportSettings(long id) {
            return false;
        }

        #endregion

        #region CUSTOMER
        public async Task<CustomerDto> GetCustomer(long id) {
            var result = await _customerManager.FindInclude(id);
            return _mapper.Map<CustomerDto>(result);
        }

        public async Task<Pager<CustomerDto>> GetCustomersPage(string search, string sort, string order, int offset = 0, int limit = 10) {
            Expression<Func<CustomerEntity, bool>> wherePredicate = x =>
                   (true)
                && (string.IsNullOrEmpty(search)
                    || x.Name.ToLower().Contains(search.ToLower())
                    || x.No.ToLower().Contains(search.ToLower())
                    || x.Description.ToLower().Contains(search.ToLower()));

            #region Sort
            Expression<Func<CustomerEntity, string>> orderPredicate = x => x.Id.ToString();
            #endregion

            string[] include = new string[] { "Company", "Address", "Activities" };

            Tuple<List<CustomerEntity>, int> tuple = await _customerManager.Pager<CustomerEntity>(wherePredicate, orderPredicate, offset, limit, include);
            var list = tuple.Item1;
            var count = tuple.Item2;

            if(count == 0)
                return new Pager<CustomerDto>(new List<CustomerDto>(), 0, offset, limit);

            var page = (offset + limit) / limit;

            var result = _mapper.Map<List<CustomerDto>>(list);
            return new Pager<CustomerDto>(result, count, page, limit);
        }

        public async Task<List<CustomerDto>> GetCustomers() {
            var result = await _customerManager.AllInclude();
            return _mapper.Map<List<CustomerDto>>(result);
        }

        public async Task<List<CustomerDto>> GetUntiedCustomers(long? companyId) {
            var result = await _customerManager.FindUntied(companyId);
            return _mapper.Map<List<CustomerDto>>(result);
        }

        public async Task<List<CustomerDto>> GetCustomers(long companyId) {
            var result = await _customerManager.FindByCompanyId(companyId);
            return _mapper.Map<List<CustomerDto>>(result);
        }

        public async Task<List<CustomerDto>> GetCustomers(long[] ids) {
            var result = await _customerManager.FindByIds(ids);
            return _mapper.Map<List<CustomerDto>>(result);
        }

        public async Task<List<CustomerDto>> GetBulkCustomers(long companyId, DateTime from, DateTime to) {
            var result = await _customerManager.FindBulks(companyId, from, to);
            return _mapper.Map<List<CustomerDto>>(result);
        }

        public async Task<CustomerDto> CreateCustomer(CustomerDto dto) {
            var entity = _mapper.Map<CustomerEntity>(dto);
            entity = await _customerManager.Create(entity);

            var activityDto = new CustomerActivityDto() {
                CustomerId = entity.Id,
                IsActive = false
            };

            //Make activity
            var activity = await _customerActivityManager.Create(_mapper.Map<CustomerActivityEntity>(activityDto));

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

            var newEntity = _mapper.Map(dto, entity);
            entity = await _customerManager.Update(newEntity);

            //Make activity
            var activityDto = new CustomerActivityDto() {
                CustomerId = entity.Id,
                IsActive = dto.IsActive,
                CreatedDate = DateTime.Now
            };
            var activity = await _customerActivityManager.Create(_mapper.Map<CustomerActivityEntity>(activityDto));

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
            var dto = _mapper.Map<InvoiceDto>(result);
            return dto;
        }

        public async Task<List<InvoiceDto>> GetInvoices(long[] ids) {
            var result = await _invoiceManager.FindByIds(ids);
            return _mapper.Map<List<InvoiceDto>>(result);
        }

        public async Task<InvoiceDto> CreateInvoice(InvoiceDto dto) {
            var entity = _mapper.Map<InvoiceEntity>(dto);
            entity = await _invoiceManager.Create(entity);
            return _mapper.Map<InvoiceDto>(entity);
        }

        public async Task<List<InvoiceDto>> CreateInvoice(List<InvoiceDto> list) {
            var entities = _mapper.Map<List<InvoiceEntity>>(list).AsEnumerable();
            entities = await _invoiceManager.Create(entities);
            return _mapper.Map<List<InvoiceDto>>(entities);
        }

        public async Task<Pager<InvoiceDto>> GetInvoicePage(InvoiceFilterDto filter) {
            DateTime? dateFrom = filter.Date?.AddDays(30 * filter.NumberOfPeriods * -1);
            var selectedPeriod = filter.PeriodId.HasValue ? await _nsiBusinessManager.GetReportPeriodById(filter.PeriodId.Value) : null;

            #region Sort
            Expression<Func<InvoiceEntity, string>> orderPredicate = filter.RandomSort ? x => Guid.NewGuid().ToString() : GetExpression<InvoiceEntity>(filter.Sort ?? "No");
            #endregion

            Tuple<List<InvoiceEntity>, int> tuple;

            if(filter.Date != null && dateFrom != null) {
                var invoices = await _reportManager.GetAgingInvoices(filter.CompanyId.Value, filter.Date.Value, 30, filter.NumberOfPeriods);
                invoices = invoices.Where(x =>
                    true &&
                    (selectedPeriod == null) ||
                    ((filter.Date.Value - x.DueDate).Days >= selectedPeriod.From
                        && (filter.Date.Value - x.DueDate).Days <= selectedPeriod.To
                        && (x.Subtotal * (1 + x.TaxRate / 100)) - (x.Payments?.Sum(x => x.Amount) ?? 0) > 0)
                ).OrderBy(x => filter.RandomSort ? Guid.NewGuid().ToString() : "No").ToList();

                var filterInvoices = invoices.Skip(filter.Offset ?? 0).Take(filter.Limit ?? invoices.Count()).ToList();

                tuple = new Tuple<List<InvoiceEntity>, int>(filterInvoices, invoices.Count());

            } else {
                #region Filter
                Expression<Func<InvoiceEntity, bool>> wherePredicate = x =>
                      (true)
                   && (string.IsNullOrEmpty(filter.Search) || (x.No.ToLower().Contains(filter.Search.ToLower()) || x.Customer.Name.ToLower().Contains(filter.Search.ToLower())))
                   && ((filter.CompanyId == null) || filter.CompanyId == x.CompanyId)

                #region MUST BE UNIVERSAL LINQ REQUEST
                   //&& ((filter.Date == null || dateFrom == null) || (/*x.DueDate >= dateFrom.Value &&*/ x.Date <= filter.Date.Value))
                   //&& ((filter.Date == null || dateFrom == null) || ((x.Payments.Count == 0) || (!x.Payments.Any(s => s.Date <= filter.Date.Value))))
                   //&& ((filter.Date == null || dateFrom == null || selectedPeriod == null) ||
                   //        (x.Date <= filter.Date.Value.AddDays((selectedPeriod.From + 30) * -1) &&
                   //        x.Date >= filter.Date.Value.AddDays((selectedPeriod.To + 30) * -1))
                   //)
                   /*&& ((filter.Date == null || dateFrom == null) || (x.Payments.Count == 0)

                   && (
                        (filter.Date == null || dateFrom == null) ||
                        ((x.Subtotal * (1 + x.TaxRate / 100) - (x.Payments.Count > 0 ? x.Payments.Sum(d => d.Amount) : 0 )) >= 0)
                        //(((x.Subtotal * (1 + x.TaxRate / 100)) - (x.Payments.Sum(d => (d.Date < filter.Date.Value ? 0 : d.Amount)))) < 0)
                   )

                    )*/
                   //: x.Payments.Sum(x => (x.Date < filter.Date.Value) ? 0 : x.Amount))) >= 0)
                   ;
                #endregion
                #endregion

                string[] include = new string[] { "Company", "Customer", "Payments" };

                tuple = await _invoiceManager.Pager<InvoiceEntity>(wherePredicate, orderPredicate, filter.Offset, filter.Limit, include);
            }

            var list = tuple.Item1;
            var count = tuple.Item2;

            if(count == 0)
                return new Pager<InvoiceDto>(new List<InvoiceDto>(), 0, filter.Offset, filter.Limit);

            var page = (filter.Offset + filter.Limit) / filter.Limit;

            var result = _mapper.Map<List<InvoiceDto>>(list);
            return new Pager<InvoiceDto>(result, count, page, filter.Limit);
        }

        public async Task<List<InvoiceDto>> GetUnpaidInvoices(long customerId) {
            var result = await _invoiceManager.FindUnpaidByCustomerId(customerId);
            return _mapper.Map<List<InvoiceDto>>(result);
        }

        public async Task<List<InvoiceDto>> GetUnpaidInvoicesByCompanyId(long companyId, DateTime from, DateTime to) {
            var result = await _invoiceManager.FindUnpaidByCompanyId(companyId, from, to);
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
            var entity1 = _mapper.Map(dto, entity);
            entity = await _invoiceManager.Update(entity1);
            return _mapper.Map<InvoiceDto>(entity);
        }
        #endregion

        #region PAYMENT
        public async Task<PaymentDto> GetPayment(long id) {
            var result = await _paymentManager.FindInclude(id);
            return _mapper.Map<PaymentDto>(result);
        }

        public async Task<Pager<PaymentDto>> GetPaymentPages(string search, string sort, string order, int offset, int limit) {
            Expression<Func<PaymentEntity, bool>> wherePredicate = x =>
                   (true)
                && (string.IsNullOrEmpty(search) || (x.No.ToLower().Contains(search.ToLower())) || (x.Amount.ToString().Contains(search.ToLower())));
            //&& (string.IsNullOrEmpty(search) || (x.Invoice..ToLower().Contains(search.ToLower())))
            //&& (string.IsNullOrEmpty(search) || (x.Description.ToLower().Contains(search.ToLower())));

            #region Sort
            Expression<Func<PaymentEntity, string>> orderPredicate = x => x.Id.ToString();
            #endregion

            string[] include = new string[] { "Invoice" };

            Tuple<List<PaymentEntity>, int> tuple = await _paymentManager.Pager<PaymentEntity>(wherePredicate, orderPredicate, offset, limit, include);
            var list = tuple.Item1;
            var count = tuple.Item2;

            if(count == 0)
                return new Pager<PaymentDto>(new List<PaymentDto>(), 0, offset, limit);

            var page = (offset + limit) / limit;

            var result = _mapper.Map<List<PaymentDto>>(list);
            return new Pager<PaymentDto>(result, count, page, limit);

            //var result = await _paymentManager.FindAllInclude();
            //return _mapper.Map<List<PaymentDto>>(result);
        }

        public async Task<PaymentDto> CreatePayment(PaymentDto dto) {
            var item = await _invoiceManager.FindInclude(dto.InvoiceId ?? 0);
            if(item == null)
                return null;

            var entity = await _paymentManager.Create(_mapper.Map<PaymentEntity>(dto));

            return _mapper.Map<PaymentDto>(entity);
        }

        public async Task<List<PaymentDto>> CreatePayment(List<PaymentDto> list) {
            var entities = _mapper.Map<List<PaymentEntity>>(list).AsEnumerable();
            entities = await _paymentManager.Create(entities);
            return _mapper.Map<List<PaymentDto>>(entities);
        }

        public async Task<List<PaymentDto>> GetPaymentByInvoiceId(long id) {
            var entity = await _paymentManager.FindByInvoiceId(id);
            return _mapper.Map<List<PaymentDto>>(entity);
        }

        public async Task<PaymentDto> UpdatePayment(long id, PaymentDto dto) {
            if(id != dto.Id) {
                return null;
            }
            var entity = await _paymentManager.FindInclude(id);
            if(entity == null) {
                return null;
            }
            entity = await _paymentManager.Update(_mapper.Map(dto, entity));
            return _mapper.Map<PaymentDto>(entity);
        }

        public async Task<bool> DeletePayment(long id) {
            var entity = await _paymentManager.FindInclude(id);
            if(entity == null) {
                return false;
            }
            int result = await _paymentManager.Delete(entity);
            return result != 0;
        }


        #endregion
    }
}
