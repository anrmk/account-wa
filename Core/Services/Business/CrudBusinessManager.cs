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
    public interface ICrudBusinessManager {
        #region COMPANY
        Task<CompanyDto> GetCompany(long id);
        Task<CustomerDto> GetCustomer(string no, long companyId);
        Task<List<CompanyDto>> GetCompanies();
        Task<Pager<CompanyDto>> GetCompanyPage(string search, string sort, string order, int offset = 0, int limit = 10);
        Task<CompanyDto> CreateCompany(CompanyDto dto);
        Task<CompanyDto> UpdateCompany(long id, CompanyDto dto);
        Task<bool> DeleteCompany(long id);

        Task<CompanyExportSettingsDto> GetCompanyExportSettings(long id);
        Task<List<CompanyExportSettingsDto>> GetCompanyAllExportSettings(long companyId);
        Task<CompanyExportSettingsDto> CreateCompanyExportSettings(CompanyExportSettingsDto dto);
        Task<CompanyExportSettingsDto> UpdateCompanyExportSettings(long id, CompanyExportSettingsDto dto);
        Task<bool> DeleteCompanyExportSettings(long id);

        Task<CompanyExportSettingsFieldDto> GetCompanyExportSettingsField(long id);
        Task<CompanyExportSettingsFieldDto> CreateCompanyExportSettingsField(CompanyExportSettingsFieldDto dto);
        Task<bool> DeleteCompanyExportSettingsField(long id);

        Task<CompanySummaryRangeDto> GetCompanySummeryRange(long id);
        Task<List<CompanySummaryRangeDto>> GetCompanyAllSummaryRange(long companyId);
        Task<CompanySummaryRangeDto> CreateCompanySummaryRange(CompanySummaryRangeDto dto);
        #endregion

        #region CUSTOMER
        Task<CustomerDto> GetCustomer(long id);
        Task<List<CustomerDto>> GetCustomers();
        Task<Pager<CustomerDto>> GetCustomersPage(PagerFilter filter);
        Task<List<CustomerDto>> GetUntiedCustomers(long? companyId);
        Task<List<CustomerDto>> GetCustomers(long[] ids);
        Task<List<CustomerDto>> GetCustomers(long companyId);
        Task<List<CustomerDto>> GetBulkCustomers(long companyId, DateTime from, DateTime to);
        Task<CustomerDto> CreateCustomer(CustomerDto dto);
        Task<List<CustomerDto>> CreateOrUpdateCustomer(List<CustomerDto> list, List<string> columns);
        Task<CustomerDto> UpdateCustomer(long id, CustomerDto dto);
        Task<bool> DeleteCustomer(long id);

        Task<List<CustomerActivityDto>> GetCustomerAllActivity(long customerId);
        Task<CustomerActivityDto> CreateCustomerActivity(CustomerActivityDto dto);

        #endregion

        #region INVOICE
        Task<InvoiceDto> GetInvoice(long id);
        //Task<Pager<InvoiceDto>> GetInvoicePage(long? companyId, DateTime? date, int? dayePerPeriod, int? numberOfPeriod, long? period, string search, string sort, string order, int offset = 0, int limit = 10);
        Task<Pager<InvoiceDto>> GetInvoicePage(InvoiceFilterDto filter);

        Task<List<InvoiceDto>> GetUnpaidInvoices(long customerId);
        Task<List<InvoiceDto>> GetUnpaidInvoicesByCompanyId(long companyId, DateTime from, DateTime to);
        Task<InvoiceDto> UpdateInvoice(long id, InvoiceDto dto);
        Task<InvoiceDto> CreateInvoice(InvoiceDto dto);
        Task<List<InvoiceDto>> CreateInvoice(List<InvoiceDto> list);
        Task<bool> DeleteInvoice(long id);
        #endregion

        #region PAYMENT
        Task<PaymentDto> GetPayment(long id);
        Task<Pager<PaymentDto>> GetPaymentPages(string search, string sort, string order, int offset = 0, int limit = 10);
        Task<List<PaymentDto>> GetPaymentByInvoiceId(long id);
        Task<List<InvoiceDto>> GetInvoices(long[] ids);
        Task<PaymentDto> CreatePayment(PaymentDto dto);
        Task<List<PaymentDto>> CreatePayment(List<PaymentDto> list);
        Task<PaymentDto> UpdatePayment(long id, PaymentDto dto);
        Task<bool> DeletePayment(long id);
        #endregion

        #region SAVED REPORT
        Task<SavedReportDto> GetSavedReport(long id);
        Task<List<SavedReportDto>> GetSavedReport(string userId);
        Task<List<SavedReportDto>> GetSavedReport(string userId, long companyId);
        Task<SavedReportDto> GetSavedReport(string userId, long companyId, DateTime date);
        Task<SavedReportDto> CreateSavedReport(SavedReportDto dto);
        Task<SavedReportDto> UpdateSavedReport(long id, SavedReportDto dto);
        Task<SavedReportFileDto> GetSavedFile(long id);
        #endregion
    }

    public class CrudBusinessManager: BaseBusinessManager, ICrudBusinessManager {
        private readonly IMapper _mapper;
        private readonly ICompanyManager _companyManager;
        private readonly ICompanyAddressMananger _companyAddressManager;
        private readonly ICompanySummaryRangeManager _companySummaryManager;
        private readonly ICompanyExportSettingsManager _companyExportSettingsManager;
        private readonly ICompanyExportSettingsFieldManager _companyExportSettingsFieldManager;
        private readonly ICustomerManager _customerManager;
        private readonly ICustomerActivityManager _customerActivityManager;
        private readonly IInvoiceManager _invoiceManager;
        private readonly IPaymentManager _paymentManager;
        private readonly IReportManager _reportManager;
        private readonly ISavedReportManager _savedReportManager;
        private readonly ISavedReportFieldManager _savedReportFieldManager;
        private readonly ISavedReportFileManager _savedReportFileManager;

        private readonly INsiBusinessManager _nsiBusinessManager;

        public CrudBusinessManager(IMapper mapper, ICompanyManager companyManager,
            ICompanyAddressMananger companyAddressManager,
            ICompanySummaryRangeManager companySummaryManager,
            ICompanyExportSettingsManager companyExportSettingsManager,
            ICompanyExportSettingsFieldManager companyExportSettingsFieldManager,
            ICustomerManager customerManager, ICustomerActivityManager customerActivityManager,
            IInvoiceManager invoiceManager, IPaymentManager paymentManager,
            IReportManager reportManager, ISavedReportManager savedReportManager, ISavedReportFieldManager savedReportFieldManager, ISavedReportFileManager savedReportFileManager,
            INsiBusinessManager nsiBusinessManager) {
            _mapper = mapper;
            _companyManager = companyManager;
            _companyAddressManager = companyAddressManager;
            _companySummaryManager = companySummaryManager;
            _companyExportSettingsManager = companyExportSettingsManager;
            _companyExportSettingsFieldManager = companyExportSettingsFieldManager;
            _customerManager = customerManager;
            _customerActivityManager = customerActivityManager;
            _invoiceManager = invoiceManager;
            _paymentManager = paymentManager;
            _nsiBusinessManager = nsiBusinessManager;

            _reportManager = reportManager;
            _savedReportManager = savedReportManager;
            _savedReportFieldManager = savedReportFieldManager;
            _savedReportFileManager = savedReportFileManager;
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
            var map = _mapper.Map<List<CompanyDto>>(result.OrderBy(x => x.Name));
            return map;
        }

        public async Task<CompanyDto> CreateCompany(CompanyDto dto) {
            var entity = await _companyManager.Create(_mapper.Map<CompanyEntity>(dto));
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

        #region COMPANY SUMMARY RANGE
        /// <summary>
        /// Получить ценовую группу по идунтификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<CompanySummaryRangeDto> GetCompanySummeryRange(long id) {
            var result = await _companySummaryManager.Find(id);
            return _mapper.Map<CompanySummaryRangeDto>(result);
        }

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
        #endregion

        #region COMPANY EXPORT SETTINGS
        public async Task<CompanyExportSettingsDto> GetCompanyExportSettings(long id) {
            var result = await _companyExportSettingsManager.FindInclude(id);
            return _mapper.Map<CompanyExportSettingsDto>(result);
        }

        public async Task<List<CompanyExportSettingsDto>> GetCompanyAllExportSettings(long companyId) {
            var result = await _companyExportSettingsManager.FindAllByCompanyId(companyId);
            return _mapper.Map<List<CompanyExportSettingsDto>>(result);
        }

        public async Task<CompanyExportSettingsDto> CreateCompanyExportSettings(CompanyExportSettingsDto dto) {
            var company = await _companyManager.Find(dto.CompanyId);
            if(company == null) {
                return null;
            }

            var newEntity = _mapper.Map<CompanyExportSettingsEntity>(dto);
            var entity = await _companyExportSettingsManager.Create(newEntity);
            return _mapper.Map<CompanyExportSettingsDto>(entity);
        }

        public async Task<CompanyExportSettingsDto> UpdateCompanyExportSettings(long id, CompanyExportSettingsDto dto) {
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

        public async Task<bool> DeleteCompanyExportSettings(long id) {
            var entity = await _companyExportSettingsManager.FindInclude(id);
            if(entity == null) {
                return false;
            }
            int result = await _companyExportSettingsManager.Delete(entity);
            return result != 0;
        }
        #endregion

        #region EXPORT SETTINGS FIELD
        public async Task<CompanyExportSettingsFieldDto> GetCompanyExportSettingsField(long id) {
            var result = await _companyExportSettingsFieldManager.Find(id);
            return _mapper.Map<CompanyExportSettingsFieldDto>(result);
        }

        public async Task<CompanyExportSettingsFieldDto> CreateCompanyExportSettingsField(CompanyExportSettingsFieldDto dto) {
            var settings = await _companyExportSettingsManager.Find(dto.ExportSettingsId);
            if(settings == null) {
                return null;
            }

            var newEntity = _mapper.Map<CompanyExportSettingsFieldEntity>(dto);
            var entity = await _companyExportSettingsFieldManager.Create(newEntity);
            return _mapper.Map<CompanyExportSettingsFieldDto>(entity);
        }

        public async Task<bool> DeleteCompanyExportSettingsField(long id) {
            var entity = await _companyExportSettingsFieldManager.Find(id);
            if(entity == null) {
                return false;
            }
            int result = await _companyExportSettingsFieldManager.Delete(entity);
            return result != 0;
        }
        #endregion

        #endregion

        #region CUSTOMER
        public async Task<CustomerDto> GetCustomer(long id) {
            var result = await _customerManager.FindInclude(id);
            return _mapper.Map<CustomerDto>(result);
        }

        public async Task<CustomerDto> GetCustomer(string no, long companyId) {
            var result = await _customerManager.FindInclude(no, companyId);
            return _mapper.Map<CustomerDto>(result);
        }

        public async Task<Pager<CustomerDto>> GetCustomersPage(PagerFilter filter) {
            //public async Task<Pager<CustomerDto>> GetCustomersPage(string search, string sort, string order, int offset = 0, int limit = 10) {
            Expression<Func<CustomerEntity, bool>> wherePredicate = x =>
               (true)
            && (string.IsNullOrEmpty(filter.Search)
                || x.Name.ToLower().Contains(filter.Search.ToLower())
                || x.No.ToLower().Contains(filter.Search.ToLower())
                || x.Description.ToLower().Contains(filter.Search.ToLower()));

            #region Sort
            Expression<Func<CustomerEntity, string>> orderPredicate = x => x.Id.ToString();
            #endregion

            string[] include = new string[] { "Company", "Address", "Activities" };

            Tuple<List<CustomerEntity>, int> tuple = await _customerManager.Pager<CustomerEntity>(wherePredicate, orderPredicate, filter.Offset, filter.Limit, include);
            var list = tuple.Item1;
            var count = tuple.Item2;

            if(count == 0)
                return new Pager<CustomerDto>(new List<CustomerDto>(), 0, filter.Offset, filter.Limit);

            var page = (filter.Offset + filter.Limit) / filter.Limit;

            var result = _mapper.Map<List<CustomerDto>>(list);
            return new Pager<CustomerDto>(result, count, page, filter.Limit);
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
                IsActive = dto.IsActive
            };

            //Make activity
            var activity = await _customerActivityManager.Create(_mapper.Map<CustomerActivityEntity>(activityDto));

            return _mapper.Map<CustomerDto>(entity);
        }

        public async Task<List<CustomerDto>> CreateOrUpdateCustomer(List<CustomerDto> list, List<string> columns) {
            var updateCustomerList = new List<CustomerEntity>();
            var createCustomerList = new List<CustomerEntity>();

            var updateActivityList = new List<CustomerActivityEntity>();
            var createActivityList = new List<CustomerActivityEntity>();

            foreach(var dto in list) {
                var entity = await _customerManager.FindInclude(dto.No, dto.CompanyId ?? 0);
                if(entity != null) {
                    //UPDATE CUSTOMER
                    foreach(var column in columns) {
                        var entityProp = entity.GetType().GetProperty(column);
                        var dtoProp = dto.GetType().GetProperty(column);

                        if(entityProp != null && dtoProp != null && entityProp.PropertyType?.BaseType != typeof(EntityBase<long>)) {
                            var value = dtoProp.GetValue(dto);
                            entityProp.SetValue(entity, value);
                        }
                    }

                    //UPDATE CUSTOMER ADDRESS
                    foreach(var column in columns) {
                        var entityProp = entity.Address?.GetType().GetProperty(column);
                        var dtoProp = dto.Address?.GetType().GetProperty(column);
                        if(entityProp != null && dtoProp != null && entityProp.PropertyType?.BaseType != typeof(EntityBase<long>)) {
                            var value = dtoProp.GetValue(dto.Address);
                            entityProp.SetValue(entity.Address, value);
                        }
                    }

                    updateCustomerList.Add(entity);

                    //UPDATE ACTIVITY
                    if(columns.Contains("CreatedDate")) {
                        var activityList = await _customerActivityManager.FindByCustomerId(entity.Id);
                        if(activityList != null && activityList.Count > 0) {
                            var activity = activityList.FirstOrDefault();
                            activity.CreatedDate = entity.CreatedDate;
                            updateActivityList.Add(activity);
                        } else {
                            createActivityList.Add(new CustomerActivityEntity() {
                                Customer = entity,
                                CustomerId = entity.Id,
                                IsActive = true,
                                CreatedDate = entity.CreatedDate,
                            });
                        }
                    }
                }
            }

            if(updateCustomerList.Count() != 0) {
                await _customerManager.Update(updateCustomerList.AsEnumerable());
            }

            if(updateActivityList.Count() != 0) {
                await _customerActivityManager.Update(updateActivityList.AsEnumerable());
            }

            if(createActivityList.Count() != 0) {
                await _customerActivityManager.Create(createActivityList.AsEnumerable());
            }

            //CREATE CUSTOMERS
            //Здесь неправильно добавляется дата для Activity
            var exceptList = list.Where(x => !updateCustomerList.Any(y => y.No == x.No && y.CompanyId == x.CompanyId));
            if(exceptList.Count() != 0) {
                var items = await _customerManager.Create(_mapper.Map<List<CustomerEntity>>(exceptList).AsEnumerable());

                if(columns.Contains("CreatedDate")) {
                    var activities = items.Select(x => new CustomerActivityEntity() {
                        Customer = x,
                        CustomerId = x.Id,
                        IsActive = true,
                        CreatedDate = x.CreatedDate
                    });
                    await _customerActivityManager.Create(activities);
                }
            }

            return list;
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

            ////Make activity
            //var activityDto = new CustomerActivityDto() {
            //    CustomerId = entity.Id,
            //    IsActive = dto.IsActive,
            //    CreatedDate = DateTime.Now
            //};
            //var activity = await _customerActivityManager.Create(_mapper.Map<CustomerActivityEntity>(activityDto));

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

        public async Task<List<CustomerActivityDto>> GetCustomerAllActivity(long customerId) {
            var result = await _customerActivityManager.FindByCustomerId(customerId);
            return _mapper.Map<List<CustomerActivityDto>>(result);
        }

        public async Task<CustomerActivityDto> CreateCustomerActivity(CustomerActivityDto dto) {
            var company = await _customerManager.Find(dto.CustomerId);
            if(company == null) {
                return null;
            }
            var newEntity = _mapper.Map<CustomerActivityEntity>(dto);

            var entity = await _customerActivityManager.Create(newEntity);
            return _mapper.Map<CustomerActivityDto>(entity);
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

            Tuple<List<InvoiceEntity>, int> tuple;

            if(filter.Date != null && dateFrom != null) {
                var invoices = await _reportManager.GetAgingInvoices(filter.CompanyId.Value, filter.Date.Value, 30, filter.NumberOfPeriods);
                invoices = invoices.Where(x =>
                    true && (filter.From.HasValue ? (filter.Date.Value - x.DueDate).Days >= filter.From : true)
                         && (filter.To.HasValue ? (filter.Date.Value - x.DueDate).Days <= filter.To.Value : true)
                         && (x.Subtotal * (1 + x.TaxRate / 100)) - (x.Payments?.Sum(x => x.Amount) ?? 0) > 0

                /*
                (selectedPeriod == null) || (
                  (filter.Date.Value - x.DueDate).Days >= selectedPeriod.From
                  && (filter.Date.Value - x.DueDate).Days <= selectedPeriod.To
                  && (x.Subtotal * (1 + x.TaxRate / 100)) - (x.Payments?.Sum(x => x.Amount) ?? 0) > 0
                )*/
                ).OrderBy(x => filter.RandomSort ? Guid.NewGuid().ToString() : "No").ToList();

                var filterInvoices = invoices.Skip(filter.Offset ?? 0).Take(filter.Limit ?? invoices.Count()).ToList();
                tuple = new Tuple<List<InvoiceEntity>, int>(filterInvoices, invoices.Count());

            } else {
                #region Sort
                Expression<Func<InvoiceEntity, string>> orderPredicate = filter.RandomSort ? x => Guid.NewGuid().ToString() : GetExpression<InvoiceEntity>(filter.Sort ?? "No");
                #endregion

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

        #region SAVED REPORT
        public async Task<SavedReportDto> GetSavedReport(long id) {
            var entity = await _savedReportManager.Find(id);
            return _mapper.Map<SavedReportDto>(entity);
        }

        public async Task<List<SavedReportDto>> GetSavedReport(string userId) {
            var entity = await _savedReportManager.FindAllByUserId(userId);
            return _mapper.Map<List<SavedReportDto>>(entity);
        }

        public async Task<List<SavedReportDto>> GetSavedReport(string userId, long companyId) {
            var entity = await _savedReportManager.FindAllByUserAndCompanyId(userId, companyId);
            return _mapper.Map<List<SavedReportDto>>(entity);
        }

        public async Task<SavedReportDto> GetSavedReport(string userId, long companyId, DateTime date) {
            var entity = await _savedReportManager.FindInclude(new Guid(userId), companyId, date);
            return _mapper.Map<SavedReportDto>(entity);
        }

        public async Task<SavedReportDto> CreateSavedReport(SavedReportDto dto) {
            var item = _mapper.Map<SavedReportEntity>(dto);
            var entity = await _savedReportManager.FindInclude(dto.ApplicationUserId, dto.CompanyId ?? 0, dto.Date);
            if(entity != null) {
                //REMOVE ALL FIELDS
                await _savedReportFieldManager.Delete(entity.Fields.AsEnumerable());

                //REMOVE ALL FILES
                await _savedReportFileManager.Delete(entity.Files.AsEnumerable());
            } else {
                entity = await _savedReportManager.Create(item);
            }

            var fieldEntity = _mapper.Map<List<SavedReportFieldEntity>>(dto.Fields);
            fieldEntity.ForEach(x => x.ReportId = entity.Id);
            var savedFieldEntity = await _savedReportFieldManager.Create(fieldEntity.AsEnumerable());

            var fileEntity = _mapper.Map<List<SavedReportFileEntity>>(dto.Files);
            fileEntity.ForEach(x => x.ReportId = entity.Id);
            var savedFileEntity = await _savedReportFileManager.Create(fileEntity.AsEnumerable());

            return _mapper.Map<SavedReportDto>(entity);
        }

        public async Task<SavedReportDto> UpdateSavedReport(long id, SavedReportDto dto) {
            var entity = await _savedReportManager.Find(id);
            if(entity == null) {
                return null;
            }
            //var mapentity = _mapper.Map(dto, entity);
            entity.IsPublished = dto.IsPublished;
            
            entity = await _savedReportManager.Update(entity);
            return _mapper.Map<SavedReportDto>(entity);
        }

        public async Task<SavedReportFileDto> GetSavedFile(long id) {
            var entity = await _savedReportFileManager.Find(id);
            return _mapper.Map<SavedReportFileDto>(entity);
        }

        #endregion
    }
}
