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

using Microsoft.EntityFrameworkCore.Internal;

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

        Task<CompanyAddressDto> GetCompanyAddress(long id);
        Task<CompanySettingsDto> CreateCompanyAddress(CompanyAddressDto dto);
        Task<CompanyAddressDto> UpdateCompanyAddress(long companyId, CompanyAddressDto dto);

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
        Task<CompanySummaryRangeDto> UpdateCompanySummaryRange(long id, CompanySummaryRangeDto dto);
        Task<bool> DeleteCompanySummaryRange(long id);

        Task<CompanySettingsDto> GetCompanySettings(long id);
        Task<CompanySettingsDto> CreateCompanySettings(CompanySettingsDto dto);
        Task<CompanySettingsDto> UpdateCompanySettings(long companyId, CompanySettingsDto dto);
        #endregion

        #region CUSTOMER
        Task<CustomerDto> GetCustomer(long id);
        Task<List<CustomerDto>> GetCustomers();
        Task<Pager<CustomerDto>> GetCustomersPage(CustomerFilterDto filter);
        Task<List<CustomerDto>> GetUntiedCustomers(long? companyId);
        Task<List<CustomerDto>> GetCustomers(long[] ids);
        Task<List<CustomerDto>> GetCustomers(long companyId);
        //Task<List<CustomerDto>> GetBulkCustomers(long companyId, DateTime from, DateTime to);
        Task<CustomerDto> CreateCustomer(CustomerDto dto);
        Task<List<CustomerDto>> CreateOrUpdateCustomer(List<CustomerDto> list, List<string> columns);
        Task<List<CustomerImportCreditsDto>> CreateOrUpdateCustomerCredits(List<CustomerImportCreditsDto> list, List<string> columns);
        Task<CustomerDto> UpdateCustomer(long id, CustomerDto dto);
        Task<bool> DeleteCustomer(long id);

        //Activity
        Task<List<CustomerActivityDto>> GetCustomerAllActivity(long customerId);
        Task<CustomerActivityDto> CreateCustomerActivity(CustomerActivityDto dto);

        //Credit Limit
        Task<CustomerCreditLimitDto> GetCustomerCreditLimit(long id);
        Task<List<CustomerCreditLimitDto>> GetCustomerCreditLimits(long customerId);
        Task<CustomerCreditLimitDto> CreateCustomerCreditLimit(CustomerCreditLimitDto dto);
        Task<CustomerCreditLimitDto> UpdateCustomerCreditLimit(long id, CustomerCreditLimitDto dto);
        Task<bool> DeleteCustomerCreditLimit(long id);

        //Credit Utilized
        Task<CustomerCreditUtilizedDto> GetCustomerCreditUtilized(long id);
        Task<List<CustomerCreditUtilizedDto>> GetCustomerCreditUtilizeds(long customerId);
        Task<CustomerCreditUtilizedDto> CreateCustomerCreditUtilized(CustomerCreditUtilizedDto dto);
        Task<CustomerCreditUtilizedDto> UpdateCustomerCreditUtilized(long id, CustomerCreditUtilizedDto dto);
        Task<bool> DeleteCustomerCreditUtilized(long id);

        //Tags
        Task<List<CustomerTagDto>> GetCustomerTags();
        Task<Pager<CustomerTagDto>> GetCustomerTags(PagerFilter filter);
        Task<CustomerTagDto> GetCustomerTag(long id);
        Task<CustomerTagDto> CreateCustomerTag(CustomerTagDto dto);
        Task<CustomerTagDto> UpdateCustomerTag(long id, CustomerTagDto dto);
        Task<bool> DeleteCustomerTag(long id);

        Task<List<CustomerTagDto>> GetCustomerTags(long customerId);
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
        Task<bool> DeleteInvoice(long[] ids);
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
        Task<bool> DeleteSavedReport(long id);
        Task<SavedReportFileDto> GetSavedFile(long id);
        #endregion
    }

    public class CrudBusinessManager: BaseBusinessManager, ICrudBusinessManager {
        private readonly IMapper _mapper;
        private readonly ICompanyManager _companyManager;
        private readonly ICompanyAddressMananger _companyAddressManager;
        private readonly ICompanySettingsManager _companySettingsManager;
        
        private readonly ICompanySummaryRangeManager _companySummaryManager;
        private readonly ICompanyExportSettingsManager _companyExportSettingsManager;
        private readonly ICompanyExportSettingsFieldManager _companyExportSettingsFieldManager;

        private readonly ICustomerManager _customerManager;
        private readonly ICustomerActivityManager _customerActivityManager;
        private readonly ICustomerCreditLimitManager _customerCreditLimitManager;
        private readonly ICustomerCreditUtilizedManager _customerCreditUtilizedManager;
        private readonly ICustomerTagManager _customerTagManager;
        private readonly ICustomerTagLinkManager _customerTagLinkManager;

        private readonly IInvoiceManager _invoiceManager;
        private readonly IPaymentManager _paymentManager;
        private readonly IReportManager _reportManager;
        private readonly ISavedReportManager _savedReportManager;
        private readonly ISavedReportFieldManager _savedReportFieldManager;
        private readonly ISavedReportFileManager _savedReportFileManager;

        private readonly INsiBusinessManager _nsiBusinessManager;

        public CrudBusinessManager(IMapper mapper, ICompanyManager companyManager,
            ICompanyAddressMananger companyAddressManager,
            ICompanySettingsManager companySettingsManager,
            ICompanySummaryRangeManager companySummaryManager,
            ICompanyExportSettingsManager companyExportSettingsManager,
            ICompanyExportSettingsFieldManager companyExportSettingsFieldManager,
            ICustomerManager customerManager, ICustomerActivityManager customerActivityManager, ICustomerCreditLimitManager customerCreditLimitManager, ICustomerCreditUtilizedManager customerCreditUtilizedManager, ICustomerTagManager customerTagManager, ICustomerTagLinkManager customerTagLinkManager,
            IInvoiceManager invoiceManager, IPaymentManager paymentManager,
            IReportManager reportManager, ISavedReportManager savedReportManager, ISavedReportFieldManager savedReportFieldManager, ISavedReportFileManager savedReportFileManager,
            INsiBusinessManager nsiBusinessManager) {
            _mapper = mapper;
            _companyManager = companyManager;
            _companyAddressManager = companyAddressManager;
            _companySettingsManager = companySettingsManager;
            _companySummaryManager = companySummaryManager;
            _companyExportSettingsManager = companyExportSettingsManager;
            _companyExportSettingsFieldManager = companyExportSettingsFieldManager;

            _customerManager = customerManager;
            _customerActivityManager = customerActivityManager;
            _customerCreditLimitManager = customerCreditLimitManager;
            _customerCreditUtilizedManager = customerCreditUtilizedManager;
            _customerTagManager = customerTagManager;
            _customerTagLinkManager = customerTagLinkManager;

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
            var sortby = sort ?? "Name";
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

            #region UPDATE CUSTOMER LIST
            //var customers = await _customerManager.FindByCompanyId(entity.Id);

            ////list of customers to delete
            //var customersForDelete = customers.Where(x => !dto.Customers.Any(y => y.Id == x.Id)).ToList();
            //customersForDelete.ForEach(x => x.CompanyId = null);

            ////list of customres to insert
            //var selectedCustomersIds = dto.Customers.Where(x => customers.Where(p => p.Id == x.Id).FirstOrDefault() == null).ToList();
            //var customersForInsert = await _customerManager.FindByIds(selectedCustomersIds.Select(x => x.Id).ToArray());
            //customersForInsert.ForEach(x => x.CompanyId = entity.Id);

            //var customersUpdate = customersForDelete.Union(customersForInsert);
            //await _customerManager.Update(customersUpdate);
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

        #region COMPANY ADDRESS
        public async Task<CompanyAddressDto> GetCompanyAddress(long id) {
            var result = await _companyAddressManager.Find(id);
            return _mapper.Map<CompanyAddressDto>(result);
        }

        public async Task<CompanySettingsDto> CreateCompanyAddress(CompanyAddressDto dto) {
            var settings = await _companyAddressManager.Find(dto.Id);
            if(settings == null) {
                return null;
            }

            var newEntity = _mapper.Map<CompanyAddressEntity>(dto);
            var entity = await _companyAddressManager.Create(newEntity);
            return _mapper.Map<CompanySettingsDto>(entity);
        }

        public async Task<CompanyAddressDto> UpdateCompanyAddress(long companyId, CompanyAddressDto dto) {
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
        public async Task<CompanySettingsDto> GetCompanySettings(long id) {
            var result = await _companySettingsManager.Find(id);
            return _mapper.Map<CompanySettingsDto>(result);
        }

        public async Task<CompanySettingsDto> CreateCompanySettings(CompanySettingsDto dto) {
            var settings = await _companySettingsManager.Find(dto.Id);
            if(settings == null) {
                return null;
            }

            var newEntity = _mapper.Map<CompanySettingsEntity>(dto);
            var entity = await _companySettingsManager.Create(newEntity);
            return _mapper.Map<CompanySettingsDto>(entity);
        }

        public async Task<CompanySettingsDto> UpdateCompanySettings(long companyId, CompanySettingsDto dto) {
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

        public async Task<CompanySummaryRangeDto> UpdateCompanySummaryRange(long id, CompanySummaryRangeDto dto) {
            var entity = await _companySummaryManager.FindInclude(id);
            if(entity == null) {
                return null;
            }

            var newEntity = _mapper.Map(dto, entity);
            entity = await _companySummaryManager.Update(newEntity);

            return _mapper.Map<CompanySummaryRangeDto>(entity);
        }

        public async Task<bool> DeleteCompanySummaryRange(long id) {
            var entity = await _companySummaryManager.FindInclude(id);
            if(entity == null) {
                return false;
            }
            int result = await _companySummaryManager.Delete(entity);
            return result != 0;
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

        public async Task<Pager<CustomerDto>> GetCustomersPage(CustomerFilterDto filter) {
            Tuple<List<CustomerEntity>, int> tuple;
            List<string> recheckFilter = new List<string>();

            #region Sort
            //var sortby = filter.RandomSort ? x => Guid.NewGuid().ToString() : GetExpression<CustomerEntity>(filter.Sort ?? "No");
            var sortby = filter.RandomSort ? "" : filter.Sort ?? "No";
            #endregion

            if(filter.DateFrom.HasValue && filter.DateTo.HasValue) {
                var customers = await _customerManager.FindBulks(filter.CompanyId ?? 0, filter.DateFrom.Value, filter.DateTo.Value);
                recheckFilter = customers.GroupBy(x => x.Recheck).Select(x => x.Key.ToString()).ToList();

                var createdMonth = filter.CreatedDate?.Month;
                var createdYear = filter.CreatedDate?.Year;

                customers = customers.Where(x => (true)
                    && (string.IsNullOrEmpty(filter.Search)
                       || x.Name.ToLower().Contains(filter.Search.ToLower())
                       || x.No.ToLower().Contains(filter.Search.ToLower())
                    )
                    && ((filter.TagsIds == null || filter.TagsIds.Count == 0) 
                        || x.TagLinks.Where(y => filter.TagsIds.Contains(y.TagId)).Count() > 0 || (filter.TagsIds.Contains(0) && x.TagLinks.Count == 0))
                    && ((filter.TypeIds == null || filter.TypeIds.Count == 0) || filter.TypeIds.Contains(x.TypeId))
                    && ((filter.Recheck == null || filter.Recheck.Count == 0) || filter.Recheck.Contains(x.Recheck))
                    && ((createdMonth == null) || x.CreatedDate.Month == createdMonth)
                    && ((createdYear == null) || x.CreatedDate.Year == createdYear)
                   ).ToList();

                //TODO: BUG FIX
                customers = string.IsNullOrEmpty(sortby) ?
                    customers.OrderBy(x => Guid.NewGuid().ToString()).ToList() :
                    SortExtension.OrderByDynamic(customers.AsQueryable(), sortby, filter.Order.Equals("desc")).ToList();
                //customers = filter.Order.Equals("desc") ? customers.OrderByDescending(sortby.Compile()).ToList() : customers.OrderBy(sortby.Compile()).ToList();
                //SortExtension.OrderByDynamic(query, order, descSort)

                //var filteredCustomers = customers.Skip(filter.Offset ?? 0).Take(filter.Limit ?? customers.Count()).ToList();
                var filteredCustomers = customers.Skip(filter.Offset).Take(filter.Limit).ToList();

                tuple = new Tuple<List<CustomerEntity>, int>(filteredCustomers, customers.Count());
            } else {
                Expression<Func<CustomerEntity, bool>> wherePredicate = x =>
                    (true)
                    && (string.IsNullOrEmpty(filter.Search)
                        || x.Name.ToLower().Contains(filter.Search.ToLower())
                        || x.No.ToLower().Contains(filter.Search.ToLower())
                    )
                    && ((filter.CompanyId == null) || filter.CompanyId == x.CompanyId);

                string[] include = new string[] { "Company", "Address", "Activities", "TagLinks", "TagLinks.Tag", "CreditLimits", "CreditUtilizeds" };

                tuple = await _customerManager.Pager<CustomerEntity>(wherePredicate, sortby, filter.Order.Equals("desc"), filter.Offset, filter.Limit, include);
            }

            var list = tuple.Item1;
            var count = tuple.Item2;

            if(count == 0)
                return new Pager<CustomerDto>(new List<CustomerDto>(), 0, filter.Offset, filter.Limit);

            var page = (filter.Offset + filter.Limit) / filter.Limit;

            var result = _mapper.Map<List<CustomerDto>>(list);
            var pager = new Pager<CustomerDto>(result, count, page, filter.Limit);
            pager.Filter.Add("Recheck", recheckFilter);

            return pager;
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

        //public async Task<List<CustomerDto>> GetBulkCustomers(long companyId, DateTime from, DateTime to) {
        //    var result = await _customerManager.FindBulks(companyId, from, to);
        //    return _mapper.Map<List<CustomerDto>>(result);
        //}

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
            var updateCustomerEntityList = new List<CustomerEntity>();
            //var createCustomerList = new List<CustomerEntity>();

            var updateActivitEntityyList = new List<CustomerActivityEntity>();
            var createActivityEntityList = new List<CustomerActivityEntity>();

            var removeTagLinkList = new List<CustomerTagLinkEntity>();
            var createTagLinkList = new List<CustomerTagLinkEntity>();

            var customerTags = await _customerTagManager.All();

            #region UPDATE CUSTOMERS
            foreach(var dto in list) {
                var customerEntity = await _customerManager.FindInclude(dto.No, dto.CompanyId ?? 0);
                if(customerEntity != null) {
                    //UPDATE CUSTOMER
                    foreach(var column in columns) {
                        var entityProp = customerEntity.GetType().GetProperty(column);
                        var dtoProp = dto.GetType().GetProperty(column);

                        if(entityProp != null && dtoProp != null && entityProp.PropertyType?.BaseType != typeof(EntityBase<long>)) {
                            var value = dtoProp.GetValue(dto);
                            entityProp.SetValue(customerEntity, value);
                        }
                    }

                    //UPDATE CUSTOMER ADDRESS
                    foreach(var column in columns) {
                        var entityProp = customerEntity.Address?.GetType().GetProperty(column);
                        var dtoProp = dto.Address?.GetType().GetProperty(column);
                        if(entityProp != null && dtoProp != null && entityProp.PropertyType?.BaseType != typeof(EntityBase<long>)) {
                            var value = dtoProp.GetValue(dto.Address);
                            entityProp.SetValue(customerEntity.Address, value);
                        }
                    }

                    updateCustomerEntityList.Add(customerEntity);

                    //UPDATE ACTIVITY
                    if(columns.Contains("CreatedDate")) {
                        var activityEntityList = await _customerActivityManager.FindByCustomerId(customerEntity.Id);
                        if(activityEntityList != null && activityEntityList.Count > 0) {
                            var activityEntity = activityEntityList.FirstOrDefault();
                            activityEntity.CreatedDate = dto.CreatedDate;
                            updateActivitEntityyList.Add(activityEntity);
                        } else {
                            createActivityEntityList.Add(new CustomerActivityEntity() {
                                CustomerId = customerEntity.Id,
                                IsActive = true,
                                CreatedDate = dto.CreatedDate,
                            });
                        }
                    }

                    //CREATE/UPDATE TAGS
                    if(columns.Contains("TagsIds")) {
                        var tagLinkEntityList = await _customerTagLinkManager.FindByCustomerId(customerEntity.Id);

                        var createTagLinkIds = dto.TagsIds.Where(x => !tagLinkEntityList.Any(y => y.TagId == x.Value)).ToList();
                        var createTagLinksEntity = customerTags.Where(x => createTagLinkIds.Contains(x.Id))
                            .Select(x => new CustomerTagLinkEntity() {
                                CustomerId = customerEntity.Id,
                                TagId = x.Id,
                            }).ToList();

                        if(createTagLinksEntity.Count > 0)
                            createTagLinkList.AddRange(createTagLinksEntity);

                        var removeTagLinkEntity = tagLinkEntityList.Where(x => !dto.TagsIds.Any(y => y.Value == x.TagId)).ToList();
                        if(removeTagLinkEntity.Count > 0)
                            removeTagLinkList.AddRange(removeTagLinkEntity);
                    }
                } else {
                    //Возможно здесь необходимо будет описать функции создания новых CUSTOMERs
                    //выявим в процессе тестирования
                }
            }

            if(updateCustomerEntityList.Count() != 0) {
                await _customerManager.Update(updateCustomerEntityList.AsEnumerable());
            }

            if(updateActivitEntityyList.Count() != 0) {
                await _customerActivityManager.Update(updateActivitEntityyList.AsEnumerable());
            }

            if(createActivityEntityList.Count() != 0) {
                await _customerActivityManager.Create(createActivityEntityList.AsEnumerable());
            }

            if(createTagLinkList.Count() != 0) {
                await _customerTagLinkManager.Create(createTagLinkList.AsEnumerable());
            }
            if(removeTagLinkList.Count() != 0) {
                await _customerTagLinkManager.Delete(removeTagLinkList.AsEnumerable());
            }
            #endregion


            #region CREATE CUSTOMERS
            createActivityEntityList.Clear(); // Очистить список активности
            createTagLinkList.Clear(); // Очистить список тэгов

            var exceptList = list.Where(x => !updateCustomerEntityList.Any(y => y.No == x.No && y.CompanyId == x.CompanyId));
            foreach(var dto in exceptList) {
                var customerEntity = await _customerManager.Create(_mapper.Map<CustomerEntity>(dto));
                if(customerEntity != null) {

                    //CREATE ACTIVITY
                    if(columns.Contains("CreatedDate")) {
                        createActivityEntityList.Add(new CustomerActivityEntity() {
                            CustomerId = customerEntity.Id,
                            IsActive = true,
                            CreatedDate = dto.CreatedDate,
                        });
                    }

                    //CREATE TAGS
                    if(columns.Contains("TagsIds")) {
                        createTagLinkList.AddRange(customerTags.Where(x => dto.TagsIds.Contains(x.Id)).Select(x => new CustomerTagLinkEntity() {
                            CustomerId = customerEntity.Id,
                            TagId = x.Id,
                        }));
                    }
                }
            }

            if(createActivityEntityList.Count() != 0) {
                await _customerActivityManager.Create(createActivityEntityList.AsEnumerable());
            }

            if(createTagLinkList.Count() != 0) {
                await _customerTagLinkManager.Create(createTagLinkList.AsEnumerable());
            }
            #endregion

            return list;
        }


        public async Task<List<CustomerImportCreditsDto>> CreateOrUpdateCustomerCredits(List<CustomerImportCreditsDto> list, List<string> columns) {
            var updateCreditLimitEntityList = new List<CustomerCreditLimitEntity>();
            var createCreditLimitEntityList = new List<CustomerCreditLimitEntity>();

            var updateCreditUtilizedEntityList = new List<CustomerCreditUtilizedEntity>();
            var createCreditUtilizedEntityList = new List<CustomerCreditUtilizedEntity>();

            foreach(var dto in list) {
                var customerEntity = await _customerManager.FindInclude(dto.No, dto.CompanyId ?? 0);
                if(customerEntity != null) {
                    //CREATE CREDIT LIMITS
                    if(columns.Contains("CreditLimit") && dto.CreditLimit.HasValue) {
                        var creditLimitEntityList = await _customerCreditLimitManager.FindAllByCustomerId(customerEntity.Id);
                        var creditLimitEntity = creditLimitEntityList.Where(x => x.CreatedDate == dto.CreatedDate).FirstOrDefault(); //найти схожую запись

                        if(creditLimitEntity != null) { //если имеется - обновить
                            creditLimitEntity.Value = dto.CreditLimit;
                            updateCreditLimitEntityList.Add(creditLimitEntity);
                        } else {
                            var doublicate = creditLimitEntityList.Where(x => x.Value == dto.CreditLimit && x.CreatedDate < dto.CreatedDate)
                                .FirstOrDefault(); //найти предыдущю запись
                            if(doublicate == null) {
                                createCreditLimitEntityList.Add(new CustomerCreditLimitEntity() {
                                    CustomerId = customerEntity.Id,
                                    Value = dto.CreditLimit,
                                    CreatedDate = dto.CreatedDate,
                                });
                            }
                        }
                    }

                    //CREATE CREDIT LIMITS
                    if(columns.Contains("CreditUtilized") && dto.CreditUtilized.HasValue) {
                        var creditUtilizedEntityList = await _customerCreditUtilizedManager.FindAllByCustomerId(customerEntity.Id);
                        var creditUtilizedEntity = creditUtilizedEntityList.Where(x => x.CreatedDate == dto.CreatedDate).FirstOrDefault(); //найти схожую запись

                        if(creditUtilizedEntity != null) {
                            creditUtilizedEntity.Value = dto.CreditUtilized;
                            updateCreditUtilizedEntityList.Add(creditUtilizedEntity);
                        } else {
                            var doublicate = creditUtilizedEntityList.Where(x => x.Value == dto.CreditUtilized && x.CreatedDate < dto.CreatedDate)
                               .FirstOrDefault(); //найти предыдущю запись
                            if(doublicate == null) {
                                createCreditUtilizedEntityList.Add(new CustomerCreditUtilizedEntity() {
                                    CustomerId = customerEntity.Id,
                                    Value = dto.CreditUtilized,
                                    CreatedDate = dto.CreatedDate,
                                });
                            }

                        }
                    }



                    //UPDATE CUSTOMER
                    //foreach(var column in columns) {
                    //    var entityProp = customerEntity.GetType().GetProperty(column);
                    //    var dtoProp = dto.GetType().GetProperty(column);

                    //    if(entityProp != null && dtoProp != null && entityProp.PropertyType?.BaseType != typeof(EntityBase<long>)) {
                    //        var value = dtoProp.GetValue(dto);
                    //        entityProp.SetValue(customerEntity, value);
                    //    }
                    //}


                } else {
                    //Возможно здесь необходимо будет описать функции создания новых CUSTOMERs
                    //выявим в процессе тестирования
                }
            }

            if(updateCreditLimitEntityList.Count() != 0) {
                await _customerCreditLimitManager.Update(updateCreditLimitEntityList.AsEnumerable());
            }
            if(createCreditLimitEntityList.Count() != 0) {
                await _customerCreditLimitManager.Create(createCreditLimitEntityList.AsEnumerable());
            }

            if(updateCreditUtilizedEntityList.Count() != 0) {
                await _customerCreditUtilizedManager.Update(updateCreditUtilizedEntityList.AsEnumerable());
            }
            if(createCreditUtilizedEntityList.Count() != 0) {
                await _customerCreditUtilizedManager.Create(createCreditUtilizedEntityList.AsEnumerable());
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


            #region UPDATE CUSTOMER LIST
            var entityTagLinks = entity.TagLinks;

            //list of tags to delete
            var removeTag = entityTagLinks.Where(x => !dto.TagsIds.Contains(x.TagId));
            await _customerTagLinkManager.Delete(removeTag);


            //list of customres to insert
            var selectedCustomersIds = dto.TagsIds.Where(x => entityTagLinks.Where(p => p.TagId == x).FirstOrDefault() == null).ToList();
            var createTag = selectedCustomersIds.Select(x => new CustomerTagLinkEntity() {
                CustomerId = entity.Id,
                TagId = x
            });

            await _customerTagLinkManager.Create(createTag);
            #endregion

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

        #region ACTIVITY
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

        #region CREDIT LIMIT
        public async Task<CustomerCreditLimitDto> GetCustomerCreditLimit(long id) {
            var result = await _customerCreditLimitManager.Find(id);
            return _mapper.Map<CustomerCreditLimitDto>(result);
        }

        public async Task<List<CustomerCreditLimitDto>> GetCustomerCreditLimits(long customerId) {
            var result = await _customerCreditLimitManager.FindAllByCustomerId(customerId);
            return _mapper.Map<List<CustomerCreditLimitDto>>(result);
        }

        public async Task<CustomerCreditLimitDto> CreateCustomerCreditLimit(CustomerCreditLimitDto dto) {
            var customer = await _customerManager.Find(dto.CustomerId);
            if(customer == null) {
                return null;
            }
            var newEntity = _mapper.Map<CustomerCreditLimitEntity>(dto);

            var entity = await _customerCreditLimitManager.Create(newEntity);
            return _mapper.Map<CustomerCreditLimitDto>(entity);
        }

        public async Task<CustomerCreditLimitDto> UpdateCustomerCreditLimit(long id, CustomerCreditLimitDto dto) {
            if(id != dto.Id) {
                return null;
            }
            var entity = await _customerCreditLimitManager.Find(id);
            if(entity == null) {
                return null;
            }

            var newEntity = _mapper.Map(dto, entity);
            entity = await _customerCreditLimitManager.Update(newEntity);

            return _mapper.Map<CustomerCreditLimitDto>(entity);
        }

        public async Task<bool> DeleteCustomerCreditLimit(long id) {
            var entity = await _customerCreditLimitManager.FindInclude(id);
            if(entity == null) {
                return false;
            }
            int result = await _customerCreditLimitManager.Delete(entity);
            return result != 0;
        }
        #endregion

        #region CREDIT UTILIZED
        public async Task<CustomerCreditUtilizedDto> GetCustomerCreditUtilized(long id) {
            var result = await _customerCreditUtilizedManager.Find(id);
            return _mapper.Map<CustomerCreditUtilizedDto>(result);
        }

        public async Task<List<CustomerCreditUtilizedDto>> GetCustomerCreditUtilizeds(long customerId) {
            var result = await _customerCreditUtilizedManager.FindAllByCustomerId(customerId);
            return _mapper.Map<List<CustomerCreditUtilizedDto>>(result);
        }

        public async Task<CustomerCreditUtilizedDto> CreateCustomerCreditUtilized(CustomerCreditUtilizedDto dto) {
            var company = await _customerManager.Find(dto.CustomerId);
            if(company == null) {
                return null;
            }
            var newEntity = _mapper.Map<CustomerCreditUtilizedEntity>(dto);

            var entity = await _customerCreditUtilizedManager.Create(newEntity);
            return _mapper.Map<CustomerCreditUtilizedDto>(entity);
        }

        public async Task<CustomerCreditUtilizedDto> UpdateCustomerCreditUtilized(long id, CustomerCreditUtilizedDto dto) {
            if(id != dto.Id) {
                return null;
            }
            var entity = await _customerCreditUtilizedManager.Find(id);
            if(entity == null) {
                return null;
            }

            var newEntity = _mapper.Map(dto, entity);
            entity = await _customerCreditUtilizedManager.Update(newEntity);

            return _mapper.Map<CustomerCreditUtilizedDto>(entity);
        }

        public async Task<bool> DeleteCustomerCreditUtilized(long id) {
            var entity = await _customerCreditUtilizedManager.FindInclude(id);
            if(entity == null) {
                return false;
            }
            int result = await _customerCreditUtilizedManager.Delete(entity);
            return result != 0;
        }
        #endregion

        #region TAGS
        public async Task<List<CustomerTagDto>> GetCustomerTags() {
            var result = await _customerTagManager.All();
            return _mapper.Map<List<CustomerTagDto>>(result);
        }

        public async Task<Pager<CustomerTagDto>> GetCustomerTags(PagerFilter filter) {
            Expression<Func<CustomerTagEntity, bool>> wherePredicate = x =>
               (true)
            && (string.IsNullOrEmpty(filter.Search)
                || x.Name.ToLower().Contains(filter.Search.ToLower()));

            #region Sort
            var sortBy = "Id";
            #endregion

            string[] include = new string[] { };

            Tuple<List<CustomerTagEntity>, int> tuple = await _customerTagManager.Pager<CustomerTagEntity>(wherePredicate, sortBy, filter.Offset, filter.Limit, include);
            var list = tuple.Item1;
            var count = tuple.Item2;

            if(count == 0)
                return new Pager<CustomerTagDto>(new List<CustomerTagDto>(), 0, filter.Offset, filter.Limit);

            var page = (filter.Offset + filter.Limit) / filter.Limit;

            var result = _mapper.Map<List<CustomerTagDto>>(list);
            return new Pager<CustomerTagDto>(result, count, page, filter.Limit);
        }

        public async Task<CustomerTagDto> GetCustomerTag(long id) {
            var result = await _customerTagManager.Find(id);
            return _mapper.Map<CustomerTagDto>(result);
        }

        public async Task<CustomerTagDto> CreateCustomerTag(CustomerTagDto dto) {
            var entity = _mapper.Map<CustomerTagEntity>(dto);
            entity = await _customerTagManager.Create(entity);

            return _mapper.Map<CustomerTagDto>(entity);
        }

        public async Task<CustomerTagDto> UpdateCustomerTag(long id, CustomerTagDto dto) {
            if(id != dto.Id) {
                return null;
            }

            var entity = await _customerTagManager.Find(id);
            if(entity == null) {
                return null;
            }

            var newEntity = _mapper.Map(dto, entity);
            entity = await _customerTagManager.Update(newEntity);

            return _mapper.Map<CustomerTagDto>(entity);
        }

        public async Task<bool> DeleteCustomerTag(long id) {
            var entity = await _customerTagManager.Find(id);
            if(entity == null) {
                return false;
            }
            int result = await _customerTagManager.Delete(entity);
            return result != 0;
        }
        #endregion

        #region TAGS LINK
        public async Task<List<CustomerTagDto>> GetCustomerTags(long customerId) {
            var tagList = new List<CustomerTagEntity>();

            var result = await _customerTagLinkManager.FindByCustomerId(customerId);
            if(result != null)
                tagList = result.Select(x => x.Tag).ToList();

            return _mapper.Map<List<CustomerTagDto>>(tagList);
        }

        #endregion

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
            var totalAmount = 0m;

            #region Sort
            var sortby = filter.RandomSort ? "" : filter.Sort ?? "No";
            #endregion

            if(filter.Date != null && dateFrom != null) {
                var invoices = await _reportManager.GetAgingInvoices(filter.CompanyId.Value, filter.Date.Value, 30, filter.NumberOfPeriods);

                //Поиск в разрезе периода
                if(filter.Periods.Count > 0) {
                    List<InvoiceEntity> newList = new List<InvoiceEntity>();

                    foreach(var p in filter.Periods) {
                        //var dash = p.Contains('+') ? p.LastIndexOf('+') : p.LastIndexOf('-');
                        int filterFrom, filterTo;

                        if(p.Contains('+')) {
                            var dash = p.LastIndexOf('+');
                            if(dash != -1 && int.TryParse(p.Substring(0, dash), out filterFrom)) {
                                newList.AddRange(invoices.Where(x =>
                                true && ((filter.Date.Value - x.DueDate).Days >= filterFrom)
                                     && (x.Subtotal * (1 + x.TaxRate / 100)) - (x.Payments?.Sum(x => x.Amount) ?? 0) > 0
                                ));
                            }
                        } else if(p.Contains('-')) {
                            var dash = p.LastIndexOf('-');
                            if(dash != -1 && int.TryParse(p.Substring(0, dash), out filterFrom) && int.TryParse(p.Substring(dash + 1), out filterTo)) {
                                newList.AddRange(invoices.Where(x =>
                                true && ((filter.Date.Value - x.DueDate).Days >= filterFrom)
                                     && ((filter.Date.Value - x.DueDate).Days <= filterTo)
                                     && (x.Subtotal * (1 + x.TaxRate / 100)) - (x.Payments?.Sum(x => x.Amount) ?? 0) > 0
                                ));
                            }
                        }

                        
                    }
                    invoices = newList;
                }

                //Поиск в разрезе заказчиков с задолжностями более 2х сумм
                if(filter.MoreThanOne) {
                    var dublCustomers = invoices.GroupBy(x => x.Customer.No)
                              .Where(g => g.Count() > 1)
                              .Select(y => y.Key)
                              .ToList();
                    invoices = invoices.Where(x => dublCustomers.Contains(x.Customer.No)).ToList();
                }

                invoices = invoices.Where(x =>
                    true
                    && ((filter.TypeId == null) || filter.TypeId == x.Customer.TypeId)
                    && ((filter.DateFrom == null) || filter.DateFrom <= x.Date)
                    && ((filter.DateTo == null) || filter.DateTo >= x.Date)
                ).ToList();

                //TODO: BUG FIX
                invoices  = string.IsNullOrEmpty(sortby) ?
                    invoices.OrderBy(x => Guid.NewGuid().ToString()).ToList() :
                    SortExtension.OrderByDynamic(invoices.AsQueryable(), sortby, filter.Order.Equals("desc")).ToList();

                //invoices = filter.Order.Equals("desc") ? invoices.OrderByDescending(sortby.Compile()).ToList() : invoices.OrderBy(sortby.Compile()).ToList();

                totalAmount = invoices.Sum(x => x.Subtotal);

                //var filterInvoices = invoices.Skip(filter.Offset ?? 0).Take(filter.Limit ?? invoices.Count()).ToList();
                var filterInvoices = invoices.Skip(filter.Offset).Take(filter.Limit).ToList();
                tuple = new Tuple<List<InvoiceEntity>, int>(filterInvoices, invoices.Count());
            } else {
                #region Filter
                Expression<Func<InvoiceEntity, bool>> wherePredicate = x =>
                      (true)
                   && (string.IsNullOrEmpty(filter.Search)
                        || (x.No.ToLower().Contains(filter.Search.ToLower())
                        || x.Customer.Name.ToLower().Contains(filter.Search.ToLower()))
                        || x.Subtotal.ToString().Equals(filter.Search.ToLower())
                        )
                   && ((filter.CompanyId == null) || filter.CompanyId == x.CompanyId)
                   && ((filter.CustomerId == null) || filter.CustomerId == x.CustomerId)
                   && ((filter.TypeId == null) || filter.TypeId == x.Customer.TypeId)
                   && ((filter.DateFrom == null) || filter.DateFrom <= x.Date)
                   && ((filter.DateTo == null) || filter.DateTo >= x.Date);
                #endregion

                string[] include = new string[] { "Company", "Customer", "Customer.Type", "Customer.Activities",  "Payments" };

                totalAmount = (await _invoiceManager.Filter(wherePredicate)).Sum(x => x.Subtotal);

                tuple = await _invoiceManager.Pager<InvoiceEntity>(wherePredicate, sortby, filter.Order.Equals("desc"), filter.Offset, filter.Limit, include);
            }

            var list = tuple.Item1;
            var count = tuple.Item2;

            if(count == 0)
                return new Pager<InvoiceDto>(new List<InvoiceDto>(), 0, filter.Offset, filter.Limit);

            var page = (filter.Offset + filter.Limit) / filter.Limit;

            var result = _mapper.Map<List<InvoiceDto>>(list);
            return new Pager<InvoiceDto>(result, count, page, filter.Limit, new string[] { totalAmount.ToString(), totalAmount.ToCurrency() });
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

        public async Task<bool> DeleteInvoice(long[] ids) {
            var invoices = await _invoiceManager.FindByIds(ids);
            var hasPayments = invoices.Any(x => x.Payments != null && x.Payments.Count > 0);

            if(!hasPayments) {
                int result = await _invoiceManager.Delete(invoices);
                return result != 0;
            }
            
            return false;
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
            var orderPredicate = "Id";
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

        public async Task<bool> DeleteSavedReport(long id) {
            var entity = await _savedReportManager.FindInclude(id);
            if(entity == null) {
                return false;
            }
            int result = await _savedReportManager.Delete(entity);
            return result != 0;
        }

        public async Task<SavedReportFileDto> GetSavedFile(long id) {
            var entity = await _savedReportFileManager.Find(id);
            return _mapper.Map<SavedReportFileDto>(entity);
        }
        #endregion
    }
}
