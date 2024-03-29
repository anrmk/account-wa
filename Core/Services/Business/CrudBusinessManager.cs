﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using AutoMapper;

using Core.Data.Dto;
using Core.Data.Entities;
using Core.Data.Enum;
using Core.Extension;
using Core.Services.Managers;

using Microsoft.EntityFrameworkCore.Internal;

namespace Core.Services.Business {
    public interface ICrudBusinessManager {
        #region CUSTOMER
        Task<CustomerDto> GetCustomer(long id);
        Task<List<CustomerDto>> GetCustomers();
        Task<Pager<CustomerDto>> GetCustomersPage(CustomerFilterDto filter);
        Task<List<CustomerDto>> GetUntiedCustomers(long? companyId);
        Task<List<CustomerDto>> GetCustomers(long[] ids);
        Task<List<CustomerDto>> GetCustomers(long companyId);
        Task<List<CustomerDto>> GetCustomers(InvoiceConstructorDto dto);

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
        //Task<CustomerCreditUtilizedDto> GetCustomerCreditUtilized(long companyId, DateTime date);
        Task<List<CustomerCreditUtilizedDto>> GetCustomerCreditUtilizeds(long customerId);
        Task<CustomerCreditUtilizedDto> CreateCustomerCreditUtilized(CustomerCreditUtilizedDto dto);
        Task<CustomerCreditUtilizedDto> UpdateCustomerCreditUtilized(long id, CustomerCreditUtilizedDto dto);
        Task<bool> DeleteCustomerCreditUtilized(long id);
        Task<bool> DeleteCustomerCreditUtilized(long[] id);

        //Credit Utilized Settings
        Task<CustomerCreditUtilizedSettingsDto> GetCustomerCreditUtilizedSettings(long id);
        Task<CustomerCreditUtilizedSettingsDto> GetCustomerCreditUtilizedSettings(long companyId, DateTime date);
        Task<List<CustomerCreditUtilizedSettingsDto>> GetCustomerCreditUtilizedSettingsList(long companyId);
        Task<CustomerCreditUtilizedSettingsDto> CreateCustomerCreditUtilizedSettings(CustomerCreditUtilizedSettingsDto dto);
        Task<CustomerCreditUtilizedSettingsDto> UpdateCustomerCreditUtilizedSettings(long id, CustomerCreditUtilizedSettingsDto dto);
        Task<bool> DeleteCustomerCreditUtilizedSettings(long id);

        //Tags
        Task<List<CustomerTagDto>> GetCustomerTags();
        Task<Pager<CustomerTagDto>> GetCustomerTags(PagerFilterDto filter);
        Task<CustomerTagDto> GetCustomerTag(long id);
        Task<CustomerTagDto> CreateCustomerTag(CustomerTagDto dto);
        Task<CustomerTagDto> UpdateCustomerTag(long id, CustomerTagDto dto);
        Task<bool> DeleteCustomerTag(long id);

        Task<List<CustomerTagDto>> GetCustomerTags(long customerId);
        Task<List<CustomerTypeDto>> GetCustomerTypes();

        //Recheck
        Task<List<CustomerRecheckDto>> GetCustomerRechecks();
        Task<List<CustomerRecheckDto>> GetCustomerRechecks(long customerId);
        Task<CustomerRecheckDto> GetCustomerRecheck(long id);
        Task<CustomerRecheckDto> CreateCustomerRecheck(CustomerRecheckDto dto);
        Task<CustomerRecheckDto> UpdateCustomerRecheck(long id, CustomerRecheckDto dto);
        Task<bool> DeleteCustomerRecheck(long id);
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

        #region INVOICE GENERATE CONSTRUCTOR
        Task<List<InvoiceDraftDto>> GetInvoiceDraft(long constructorId);
        Task<List<InvoiceDraftDto>> GetInvoiceDraft(long[] constructorIds);
        Task<Pager<InvoiceDraftDto>> GetInvoiceDraftPage(InvoiceDraftFilterDto filter);
        Task<List<InvoiceDraftDto>> UpdateInvoiceDraft(long constructorId);
        Task<InvoiceConstructorDto> CreateInvoiceDraft(InvoiceConstructorDto constructor);
        Task<List<InvoiceDto>> CopyInvoiceFromDraft(long[] constructorIds);
        Task<bool> DeleteInvoiceDraft(long[] ids);

        Task<InvoiceConstructorDto> GetConstructorInvoice(long id);
        Task<InvoiceConstructorDto> CreateConstructorInvoice(InvoiceConstructorDto dto);
        //Task<InvoiceConstructorDto> UpdateConstructorInvoice(long id, InvoiceConstructorDto dto);

        Task<List<InvoiceConstructorDto>> GetConstructorInvoices(long companyId, DateTime date);

        #endregion

        #region PAYMENT
        Task<PaymentDto> GetPayment(long id);
        Task<Pager<PaymentDto>> GetPaymentPages(PaymentFilterDto filter);
        Task<List<PaymentDto>> GetPaymentByInvoiceId(long id);
        Task<List<InvoiceDto>> GetInvoices(long[] ids);
        Task<PaymentDto> CreatePayment(PaymentDto dto);
        Task<List<PaymentDto>> CreatePayment(List<PaymentDto> list);
        Task<PaymentDto> UpdatePayment(long id, PaymentDto dto);
        Task<bool> DeletePayment(long id);
        Task<bool> DeletePayment(long[] ids);
        #endregion



        #region SEARCH CRITERIA
        Task<InvoiceConstructorSearchDto> GetInvoiceConstructorSearchCriteria(long id);
        Task<List<InvoiceConstructorSearchDto>> GetInvoiceConstructorSearchCriterias(long[] ids);
        Task<List<InvoiceConstructorSearchDto>> GetInvoiceConstructorSearchCriterias();
        Task<Pager<InvoiceConstructorSearchDto>> GetInvoiceConstructorSearchCriterias(PagerFilterDto filter);

        Task<InvoiceConstructorSearchDto> CreateInvoiceConstructorSearchCriterias(InvoiceConstructorSearchDto dto);
        Task<InvoiceConstructorSearchDto> UpdateInvoiceConstructorSearchCriterias(long id, InvoiceConstructorSearchDto dto);
        Task<bool> DeleteInvoiceConstructorSearchCriterias(long id);
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
        private readonly ICustomerCreditUtilizedSettingsManager _customerCreditUtilizedSettingsManager;
        private readonly ICustomerTagManager _customerTagManager;
        private readonly ICustomerTagLinkManager _customerTagLinkManager;
        private readonly ICustomerTypeManager _customerTypeManager;
        private readonly ICustomerRecheckManager _customerRecheckManager;

        private readonly IInvoiceManager _invoiceManager;
        private readonly IInvoiceConstructorManager _invoiceConstructorManager;
        private readonly IInvoiceDraftManager _invoiceDraftManager;
        private readonly IPaymentManager _paymentManager;
        private readonly IReportManager _reportManager;
        private readonly IInvoiceConstructorSearchManager _invoiceConstructorSearchManager;


        public CrudBusinessManager(IMapper mapper, ICompanyManager companyManager,
            ICompanyAddressMananger companyAddressManager,
            ICompanySettingsManager companySettingsManager,
            ICompanySummaryRangeManager companySummaryManager,
            ICompanyExportSettingsManager companyExportSettingsManager,
            ICompanyExportSettingsFieldManager companyExportSettingsFieldManager,
            ICustomerManager customerManager, ICustomerActivityManager customerActivityManager, ICustomerCreditLimitManager customerCreditLimitManager, ICustomerCreditUtilizedManager customerCreditUtilizedManager, ICustomerCreditUtilizedSettingsManager customerCreditUtilizedSettingsManager, ICustomerTagManager customerTagManager, ICustomerTagLinkManager customerTagLinkManager, ICustomerTypeManager customerTypeManager, ICustomerRecheckManager customerRecheckManager,
            IInvoiceManager invoiceManager, IInvoiceConstructorManager invoiceConstructorManager, IInvoiceDraftManager invoiceDraftManager, IPaymentManager paymentManager,
            IReportManager reportManager,
            IInvoiceConstructorSearchManager invoiceConstructorSearchManager) {
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
            _customerCreditUtilizedSettingsManager = customerCreditUtilizedSettingsManager;
            _customerTagManager = customerTagManager;
            _customerTagLinkManager = customerTagLinkManager;
            _customerTypeManager = customerTypeManager;
            _customerRecheckManager = customerRecheckManager;

            _invoiceManager = invoiceManager;
            _invoiceConstructorManager = invoiceConstructorManager;
            _invoiceDraftManager = invoiceDraftManager;
            _paymentManager = paymentManager;

            _reportManager = reportManager;

            _invoiceConstructorSearchManager = invoiceConstructorSearchManager;
        }

        #region COMPANY
        //public async Task<CompanyDto> GetCompany(long id) {
        //    var result = await _companyManager.FindInclude(id);
        //    return _mapper.Map<CompanyDto>(result);
        //}



        //public async Task<List<CompanyDto>> GetCompanies() {
        //    var result = await _companyManager.AllInclude();
        //    var map = _mapper.Map<List<CompanyDto>>(result.OrderBy(x => x.Name));
        //    return map;
        //}











        #endregion

        #region CUSTOMER
        public async Task<CustomerDto> GetCustomer(long id) {
            var result = await _customerManager.FindInclude(id);
            return _mapper.Map<CustomerDto>(result);
        }

        //public async Task<CustomerDto> GetCustomer(string no, long companyId) {
        //    var result = await _customerManager.FindInclude(no, companyId);
        //    return _mapper.Map<CustomerDto>(result);
        //}

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

                //var createdMonth = filter.CreatedDate?.Month;
                //var createdYear = filter.CreatedDate?.Year;

                customers = customers.Where(x => (true)
                    && (string.IsNullOrEmpty(filter.Search)
                       || x.Name.ToLower().Contains(filter.Search.ToLower())
                       || x.No.ToLower().Contains(filter.Search.ToLower())
                    )
                    && ((filter.TagsIds == null || filter.TagsIds.Count == 0)
                        || x.TagLinks.Where(y => filter.TagsIds.Contains(y.TagId)).Count() > 0 || (filter.TagsIds.Contains(0) && x.TagLinks.Count == 0))
                    && ((filter.TypeIds == null || filter.TypeIds.Count == 0) || filter.TypeIds.Contains(x.TypeId))
                    && ((filter.Recheck == null || filter.Recheck.Count == 0) || filter.Recheck.Contains(x.Recheck))

                    && ((filter.CreatedDateFrom == null) || filter.CreatedDateFrom <= x.CreatedDate)
                    && ((filter.CreatedDateTo == null) || filter.CreatedDateTo >= x.CreatedDate)

                    && ((!filter.CurrentInvoices.HasValue) || x.TotalInvoices == filter.CurrentInvoices)
                    && ((!filter.LateInvoices.HasValue) || x.UnpaidInvoices == filter.LateInvoices)
                   ).ToList();

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

                string[] include = new string[] { "Company", "Address", "Activities", "TagLinks", "TagLinks.Tag", "Type", "CreditLimits", "CreditUtilizeds" };

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

                    //CREATE/UPDATE ACTIVITY
                    if(columns.Contains("ActivityDate")) {
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
                    if(columns.Contains("ActivityDate")) {
                        createActivityEntityList.Add(new CustomerActivityEntity() {
                            CustomerId = customerEntity.Id,
                            IsActive = true,
                            CreatedDate = dto.ActivityDate,
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

        //public async Task<CustomerCreditUtilizedDto> GetCustomerCreditUtilized(long companyId, DateTime date) {
        //    var result = await _customerCreditUtilizedManager.FindByCustomerIdAndDate(companyId, date);
        //    return _mapper.Map<CustomerCreditUtilizedDto>(result);
        //}

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
            return await DeleteCustomerCreditUtilized(new long[] { id });
        }

        public async Task<bool> DeleteCustomerCreditUtilized(long[] ids) {
            var entities = await _customerCreditUtilizedManager.Filter(x => ids.Contains(x.Id));
            if(entities == null && entities.Count() == 0) {
                return false;
            }
            int result = await _customerCreditUtilizedManager.Delete(entities);
            return result != 0;
        }
        #endregion

        #region CREDIT UTILIZED SETTINGS
        public async Task<CustomerCreditUtilizedSettingsDto> GetCustomerCreditUtilizedSettings(long id) {
            var result = await _customerCreditUtilizedSettingsManager.Find(id);
            return _mapper.Map<CustomerCreditUtilizedSettingsDto>(result);
        }

        public async Task<CustomerCreditUtilizedSettingsDto> GetCustomerCreditUtilizedSettings(long companyId, DateTime date) {
            var result = await _customerCreditUtilizedSettingsManager.FindInclude(companyId, date);
            return _mapper.Map<CustomerCreditUtilizedSettingsDto>(result);
        }

        public async Task<List<CustomerCreditUtilizedSettingsDto>> GetCustomerCreditUtilizedSettingsList(long companyId) {
            var result = await _customerCreditUtilizedSettingsManager.FindByCompany(companyId);
            return _mapper.Map<List<CustomerCreditUtilizedSettingsDto>>(result);
        }

        public async Task<CustomerCreditUtilizedSettingsDto> CreateCustomerCreditUtilizedSettings(CustomerCreditUtilizedSettingsDto dto) {
            var entity = _mapper.Map<CustomerCreditUtilizedSettingsEntity>(dto);
            entity = await _customerCreditUtilizedSettingsManager.Create(entity);

            return _mapper.Map<CustomerCreditUtilizedSettingsDto>(entity);
        }

        public async Task<CustomerCreditUtilizedSettingsDto> UpdateCustomerCreditUtilizedSettings(long id, CustomerCreditUtilizedSettingsDto dto) {
            if(id != dto.Id) {
                return null;
            }
            var entity = await _customerCreditUtilizedSettingsManager.Find(id);
            if(entity == null) {
                return null;
            }

            var newEntity = _mapper.Map(dto, entity);
            entity = await _customerCreditUtilizedSettingsManager.Update(newEntity);

            return _mapper.Map<CustomerCreditUtilizedSettingsDto>(entity);
        }

        public async Task<bool> DeleteCustomerCreditUtilizedSettings(long id) {
            var entity = await _customerCreditUtilizedSettingsManager.Find(id);
            if(entity == null) {
                return false;
            }
            int result = await _customerCreditUtilizedSettingsManager.Delete(entity);
            return result != 0;
        }
        #endregion

        #region TAGS
        public async Task<List<CustomerTagDto>> GetCustomerTags() {
            var result = await _customerTagManager.All();
            return _mapper.Map<List<CustomerTagDto>>(result);
        }

        public async Task<Pager<CustomerTagDto>> GetCustomerTags(PagerFilterDto filter) {
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

        #region TYPES
        public async Task<List<CustomerTypeDto>> GetCustomerTypes() {
            var result = await _customerTypeManager.All();
            return _mapper.Map<List<CustomerTypeDto>>(result);
        }
        #endregion

        #region RECHECK
        public async Task<List<CustomerRecheckDto>> GetCustomerRechecks() {
            var result = await _customerRecheckManager.All();
            return _mapper.Map<List<CustomerRecheckDto>>(result);
        }

        public async Task<List<CustomerRecheckDto>> GetCustomerRechecks(long customerId) {
            var result = await _customerRecheckManager.FindAllByCustomerId(customerId);
            return _mapper.Map<List<CustomerRecheckDto>>(result);
        }

        public async Task<CustomerRecheckDto> GetCustomerRecheck(long id) {
            var result = await _customerRecheckManager.Find(id);
            return _mapper.Map<CustomerRecheckDto>(result);
        }

        public async Task<CustomerRecheckDto> CreateCustomerRecheck(CustomerRecheckDto dto) {
            var customer = await _customerManager.Find(dto.CustomerId);
            if(customer == null) {
                return null;
            }
            var newEntity = _mapper.Map<CustomerRecheckEntity>(dto);

            var entity = await _customerRecheckManager.Create(newEntity);
            return _mapper.Map<CustomerRecheckDto>(entity);
        }

        public async Task<CustomerRecheckDto> UpdateCustomerRecheck(long id, CustomerRecheckDto dto) {
            var entity = await _customerRecheckManager.Find(id);
            if(entity == null) {
                return null;
            }

            var newEntity = _mapper.Map(dto, entity);
            entity = await _customerRecheckManager.Update(newEntity);

            return _mapper.Map<CustomerRecheckDto>(entity);
        }

        public async Task<bool> DeleteCustomerRecheck(long id) {
            var entity = await _customerRecheckManager.Find(id);
            if(entity == null) {
                return false;
            }
            int result = await _customerRecheckManager.Delete(entity);
            return result != 0;
        }
        #endregion

        public async Task<List<CustomerDto>> GetCustomers(InvoiceConstructorDto dto) {
            var searchCriteria = await _invoiceConstructorSearchManager.Find(dto.SearchCriteriaId);
            var searchCriteriaDto = _mapper.Map<InvoiceConstructorSearchDto>(searchCriteria);

            var dateFrom = dto.Date.FirstDayOfMonth();
            var dateTo = dto.Date.LastDayOfMonth();
            var random = new Random();

            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;

            if(searchCriteria.Group == CustomerGroupType.OnlyNew) {
                createdDateFrom = dto.Date.FirstDayOfMonth();
                createdDateTo = dto.Date.LastDayOfMonth();
            } else if(searchCriteria.Group == CustomerGroupType.ExcludeNew) {
                createdDateTo = dto.Date.AddMonths(-1).LastDayOfMonth();
            } else if(searchCriteria.Group == CustomerGroupType.All) {
                createdDateTo = dto.Date.LastDayOfMonth();
            }


            var customers = await _customerManager.FindBulks(dto.CompanyId, dateFrom, dateTo);
            var recheckFilter = customers.GroupBy(x => x.Recheck).Select(x => x.Key.ToString()).ToList();

            customers = customers.Where(x => (true)
                && ((searchCriteriaDto.TagsIds == null || searchCriteriaDto.TagsIds.Count == 0)
                    || x.TagLinks.Where(y => searchCriteriaDto.TagsIds.Contains(y.TagId ?? 0)).Count() > 0 || (searchCriteriaDto.TagsIds.Contains(0) && x.TagLinks.Count == 0))
                && ((searchCriteriaDto.TypeIds == null || searchCriteriaDto.TypeIds.Count == 0) || searchCriteriaDto.TypeIds.Contains(x.TypeId ?? 0))
                && ((searchCriteriaDto.Recheck == null || searchCriteriaDto.Recheck.Count == 0) || searchCriteriaDto.Recheck.Contains(x.Recheck))

                && ((createdDateFrom == null) || createdDateFrom <= x.CreatedDate)
                && ((createdDateTo == null) || createdDateTo >= x.CreatedDate)

                && ((!searchCriteriaDto.CurrentInvoices.HasValue) || x.TotalInvoices == searchCriteriaDto.CurrentInvoices)
                && ((!searchCriteriaDto.LateInvoices.HasValue) || x.UnpaidInvoices == searchCriteriaDto.LateInvoices)
               ).ToList();

            if(searchCriteriaDto.RandomSort)
                customers = customers.OrderBy(x => Guid.NewGuid().ToString()).ToList();
            else /*if(dto.SortBy)*/ {
                customers = SortExtension.OrderByDynamic(customers.AsQueryable(), "Id", false).ToList();
            }

            return _mapper.Map<List<CustomerDto>>(customers);
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
                        } else if(p.Equals("Total Late")) {
                            filterFrom = 1;
                            filterTo = 30 * filter.NumberOfPeriods;
                            newList.AddRange(invoices.Where(x =>
                                true && ((filter.Date.Value - x.DueDate).Days >= filterFrom)
                                     //  && ((filter.Date.Value - x.DueDate).Days <= filterTo)
                                     && (x.Subtotal * (1 + x.TaxRate / 100)) - (x.Payments?.Sum(x => x.Amount) ?? 0) > 0
                                ));
                        } else if(p.Equals("Total")) {
                            filterFrom = -31;
                            filterTo = 30 * filter.NumberOfPeriods;
                            newList.AddRange(invoices.Where(x =>
                                true && ((filter.Date.Value - x.DueDate).Days >= filterFrom)
                                     && (x.Subtotal * (1 + x.TaxRate / 100)) - (x.Payments?.Sum(x => x.Amount) ?? 0) > 0
                                ));
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
                    && ((filter.CreatedDateFrom == null) || filter.CreatedDateFrom <= x.CreatedDate)
                    && ((filter.CreatedDateTo == null) || filter.CreatedDateTo >= x.CreatedDate)
                ).ToList();

                //TODO: BUG FIX
                invoices = string.IsNullOrEmpty(sortby) ?
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
                        || x.No.ToLower().Contains(filter.Search.ToLower())
                        || x.Customer.Name.ToLower().Contains(filter.Search.ToLower())
                        || x.Subtotal.ToString().Equals(filter.Search.ToLower())
                        )
                   && ((filter.CompanyId == null) || filter.CompanyId == x.CompanyId)
                   && ((filter.CustomerId == null) || filter.CustomerId == x.CustomerId)
                   && ((filter.TypeId == null) || filter.TypeId == x.Customer.TypeId)
                   && ((filter.DateFrom == null) || filter.DateFrom <= x.Date)
                   && ((filter.DateTo == null) || filter.DateTo >= x.Date)
                   && ((filter.CreatedDateFrom == null) || filter.CreatedDateFrom <= x.CreatedDate)
                   && ((filter.CreatedDateTo == null) || filter.CreatedDateTo >= x.CreatedDate);
                #endregion

                string[] include = new string[] { "Company", "Customer", "Customer.Type", "Customer.Activities", "Payments" };

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

        #region INVOICE DRAFT
        public async Task<List<InvoiceDto>> CopyInvoiceFromDraft(long[] constructorIds) {
            var draftInvoices = await _invoiceDraftManager.FindByConstructorId(constructorIds);
            if(draftInvoices == null || draftInvoices.Count == 0) {
                return null;
            }
            var invoices = await _invoiceManager.Create(_mapper.Map<List<InvoiceEntity>>(draftInvoices));
            await _invoiceDraftManager.Delete(draftInvoices);

            return _mapper.Map<List<InvoiceDto>>(invoices);
        }

        public async Task<List<InvoiceDraftDto>> GetInvoiceDraft(long constructorId) {
            var result = await _invoiceDraftManager.FindByConstructorId(constructorId);
            return _mapper.Map<List<InvoiceDraftDto>>(result);
        }

        public async Task<List<InvoiceDraftDto>> GetInvoiceDraft(long[] constructorIds) {
            var result = await _invoiceDraftManager.FindByConstructorId(constructorIds);
            return _mapper.Map<List<InvoiceDraftDto>>(result);
        }

        public async Task<Pager<InvoiceDraftDto>> GetInvoiceDraftPage(InvoiceDraftFilterDto filter) {
            Expression<Func<InvoiceDraftEntity, bool>> wherePredicate = x =>
                     (true)
                  && (string.IsNullOrEmpty(filter.Search)
                       || x.No.ToLower().Contains(filter.Search.ToLower())
                       || x.Subtotal.ToString().Equals(filter.Search.ToLower())
                       )
                  && ((filter.ConstructorId == null) || filter.ConstructorId == x.ConstructorId)
                  //&& ((filter.CustomerId == null) || filter.CustomerId == x.CustomerId)
                  // && ((filter.TypeId == null) || filter.TypeId == x.Customer.TypeId)
                  //&& ((filter.DateFrom == null) || filter.DateFrom <= x.Date)
                  //&& ((filter.DateTo == null) || filter.DateTo >= x.Date)
                  //&& ((filter.CreatedDateFrom == null) || filter.CreatedDateFrom <= x.CreatedDate)
                  //&& ((filter.CreatedDateTo == null) || filter.CreatedDateTo >= x.CreatedDate)
                  ;

            #region Sort
            var sortBy = "Id";
            #endregion

            string[] include = new string[] { };

            Tuple<List<InvoiceDraftEntity>, int> tuple = await _invoiceDraftManager.Pager<InvoiceDraftEntity>(wherePredicate, sortBy, filter.Offset, filter.Limit, include);
            var list = tuple.Item1;
            var count = tuple.Item2;

            if(count == 0)
                return new Pager<InvoiceDraftDto>(new List<InvoiceDraftDto>(), 0, filter.Offset, filter.Limit);

            var page = (filter.Offset + filter.Limit) / filter.Limit;

            var result = _mapper.Map<List<InvoiceDraftDto>>(list);
            return new Pager<InvoiceDraftDto>(result, count, page, filter.Limit);
        }

        public async Task<InvoiceConstructorDto> CreateInvoiceDraft(InvoiceConstructorDto dto) {
            var constructor = await _invoiceConstructorManager.Find(dto.Id);

            var company = await _companyManager.FindInclude(constructor.CompanyId);
            var summaryRange = await _companySummaryManager.Find(constructor.SummaryRangeId);

            var dateFrom = constructor.Date.FirstDayOfMonth().AddDays(2); //Start from 3rd day of the month
            var dateTo = constructor.Date.LastDayOfMonth();
            var random = new Random();

            var customers = await _customerManager.FindByIds(dto.Customers.ToArray());

            var newInvoices = new List<InvoiceDraftEntity>();
            foreach(var customer in customers) {
                var date = random.NextDate(dateFrom, dateTo);

                byte[] bytes = new byte[4];
                random.NextBytes(bytes);

                var invoice = new InvoiceDraftEntity() {
                    ConstructorId = constructor.Id,
                    CompanyId = company.Id,
                    CustomerId = customer.Id,
                    Date = date,
                    DueDate = date.AddDays(30),
                    No = BitConverter.ToString(bytes).Replace("-", ""),
                    Subtotal = random.NextDecimal(summaryRange.From, summaryRange.To)
                };
                newInvoices.Add(invoice);
            }
            if(newInvoices.Count > 0)
                await _invoiceDraftManager.Create(newInvoices);

            var invoices = await _invoiceDraftManager.FindByConstructorId(constructor.Id);

            constructor.Invoices = invoices;
            constructor.Count = invoices.Count;

            return _mapper.Map<InvoiceConstructorDto>(constructor);
        }

        public async Task<List<InvoiceDraftDto>> UpdateInvoiceDraft(long constructorId) {
            var constructor = await _invoiceConstructorManager.Find(constructorId);
            var invoices = await _invoiceDraftManager.FindByConstructorId(constructor.Id);
            var summaryRange = await _companySummaryManager.Find(constructor.SummaryRangeId);

            var dateFrom = constructor.Date.FirstDayOfMonth();
            var dateTo = constructor.Date.LastDayOfMonth();
            var random = new Random();

            foreach(var invoice in invoices) {
                var date = random.NextDate(dateFrom, dateTo);
                invoice.Date = date;
                invoice.DueDate = date.AddDays(30);
                invoice.Subtotal = random.NextDecimal(summaryRange.From, summaryRange.To);
            }

            await _invoiceDraftManager.Update(invoices.AsEnumerable());

            return _mapper.Map<List<InvoiceDraftDto>>(invoices);
        }

        public async Task<bool> DeleteInvoiceDraft(long[] ids) {
            var invoices = await _invoiceDraftManager.Filter(x => ids.Contains(x.Id));
            int result = await _invoiceDraftManager.Delete(invoices);
            return result != 0;
        }

        #endregion

        #region CONSTRUCTOR INVOICE
        public async Task<InvoiceConstructorDto> GetConstructorInvoice(long id) {
            var entity = await _invoiceConstructorManager.FindInclude(id);
            var dto = _mapper.Map<InvoiceConstructorDto>(entity);
            return dto;
        }

        public async Task<InvoiceConstructorDto> CreateConstructorInvoice(InvoiceConstructorDto dto) {
            var entity = _mapper.Map<InvoiceConstructorEntity>(dto);
            entity = await _invoiceConstructorManager.Create(entity);
            return _mapper.Map<InvoiceConstructorDto>(entity);
        }

        //public async Task<InvoiceConstructorDto> UpdateConstructorInvoice(long id, InvoiceConstructorDto dto) {
        //    return null;
        //}

        public async Task<List<InvoiceConstructorDto>> GetConstructorInvoices(long companyId, DateTime date) {
            var entities = await _invoiceConstructorManager.FindAll(companyId, date);
            return _mapper.Map<List<InvoiceConstructorDto>>(entities);
        }


        #endregion

        #region PAYMENT
        public async Task<PaymentDto> GetPayment(long id) {
            var result = await _paymentManager.FindInclude(id);
            return _mapper.Map<PaymentDto>(result);
        }

        public async Task<Pager<PaymentDto>> GetPaymentPages(PaymentFilterDto filter) {
            Expression<Func<PaymentEntity, bool>> wherePredicate = x =>
                   (true)
                && (string.IsNullOrEmpty(filter.Search)
                    || x.No.ToLower().Contains(filter.Search.ToLower())
                    || x.Amount.ToString().Contains(filter.Search.ToLower())
                    || x.Invoice.No.Contains(filter.Search.ToLower())
                    )
                && ((filter.CompanyId == null) || filter.CompanyId == x.Invoice.CompanyId)
                && ((filter.DateFrom == null) || filter.DateFrom <= x.Date)
                && ((filter.DateTo == null) || filter.DateTo >= x.Date)
                && ((filter.CreatedDateFrom == null) || filter.CreatedDateFrom <= x.CreatedDate)
                && ((filter.CreatedDateTo == null) || filter.CreatedDateTo >= x.CreatedDate);

            #region Sort
            var sortby = filter.RandomSort ? "" : filter.Sort ?? "No";
            #endregion

            string[] include = new string[] { "Invoice", "Invoice.Company" };

            Tuple<List<PaymentEntity>, int> tuple = await _paymentManager.Pager<PaymentEntity>(wherePredicate, sortby, filter.Order.Equals("desc"), filter.Offset, filter.Limit, include);
            var list = tuple.Item1;
            var count = tuple.Item2;

            if(count == 0)
                return new Pager<PaymentDto>(new List<PaymentDto>(), 0, filter.Offset, filter.Limit);

            var page = (filter.Offset + filter.Limit) / filter.Limit;

            var result = _mapper.Map<List<PaymentDto>>(list);
            return new Pager<PaymentDto>(result, count, page, filter.Limit);
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

        public async Task<bool> DeletePayment(long[] ids) {
            var payments = await _paymentManager.FindByIds(ids);
            int result = await _paymentManager.Delete(payments);
            return result != 0;
        }
        #endregion



        #region CONSTRUCTOR SEARCH CRITERIA
        public async Task<InvoiceConstructorSearchDto> GetInvoiceConstructorSearchCriteria(long id) {
            var entity = await _invoiceConstructorSearchManager.Find(id);
            return _mapper.Map<InvoiceConstructorSearchDto>(entity);
        }

        public async Task<List<InvoiceConstructorSearchDto>> GetInvoiceConstructorSearchCriterias(long[] ids) {
            var result = await _invoiceConstructorSearchManager.Filter(x => ids.Contains(x.Id));
            return _mapper.Map<List<InvoiceConstructorSearchDto>>(result);
        }

        public async Task<List<InvoiceConstructorSearchDto>> GetInvoiceConstructorSearchCriterias() {
            var result = await _invoiceConstructorSearchManager.All();
            return _mapper.Map<List<InvoiceConstructorSearchDto>>(result);
        }

        public async Task<Pager<InvoiceConstructorSearchDto>> GetInvoiceConstructorSearchCriterias(PagerFilterDto filter) {
            Expression<Func<InvoiceConstructorSearchEntity, bool>> wherePredicate = x =>
              (true)
                && (string.IsNullOrEmpty(filter.Search)
                || x.CustomerTags.ToLower().Contains(filter.Search.ToLower()));

            #region Sort
            var sortBy = "Id";
            #endregion

            string[] include = new string[] { };

            Tuple<List<InvoiceConstructorSearchEntity>, int> tuple = await _invoiceConstructorSearchManager.Pager<InvoiceConstructorSearchEntity>(wherePredicate, sortBy, filter.Offset, filter.Limit, include);
            var list = tuple.Item1;
            var count = tuple.Item2;

            if(count == 0)
                return new Pager<InvoiceConstructorSearchDto>(new List<InvoiceConstructorSearchDto>(), 0, filter.Offset, filter.Limit);

            var page = (filter.Offset + filter.Limit) / filter.Limit;

            var result = _mapper.Map<List<InvoiceConstructorSearchDto>>(list);
            return new Pager<InvoiceConstructorSearchDto>(result, count, page, filter.Limit);
        }

        public async Task<InvoiceConstructorSearchDto> CreateInvoiceConstructorSearchCriterias(InvoiceConstructorSearchDto dto) {
            dto.Name = string.IsNullOrEmpty(dto.Name) ? $"Search criteria {DateTime.Now}" : dto.Name;

            var entity = _mapper.Map<InvoiceConstructorSearchEntity>(dto);
            entity = await _invoiceConstructorSearchManager.Create(entity);
            return _mapper.Map<InvoiceConstructorSearchDto>(entity);
        }

        public async Task<InvoiceConstructorSearchDto> UpdateInvoiceConstructorSearchCriterias(long id, InvoiceConstructorSearchDto dto) {
            var entity = await _invoiceConstructorSearchManager.Find(id);
            if(entity == null) {
                return null;
            }

            var newEntity = _mapper.Map(dto, entity);
            entity = await _invoiceConstructorSearchManager.Update(newEntity);
            return _mapper.Map<InvoiceConstructorSearchDto>(entity);
        }

        public async Task<bool> DeleteInvoiceConstructorSearchCriterias(long id) {
            var entity = await _invoiceConstructorSearchManager.Find(id);
            if(entity == null) {
                return false;
            }
            int result = await _invoiceConstructorSearchManager.Delete(entity);
            return result != 0;
        }
        #endregion
    }
}