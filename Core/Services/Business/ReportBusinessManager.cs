using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using AutoMapper;

using Core.Data.Dto;
using Core.Data.Entities;
using Core.Data.Enum;
using Core.Extension;
using Core.Services.Managers;

namespace Core.Services.Business {
    public interface IReportBusinessManager {
        Task<AgingReportResultDto> GetAgingReport(long companyId, DateTime period, int daysPerPeriod, int numberOfPeriod, bool includeAllCustomers);
        Task<ReportStatusDto> CheckingCustomerAccountNumber(long companyId, DateTime dateTo, int numberOfPeriods);
        Task<Pager<CustomerCreditUtilizedDto>> GetCustomerCreditUtilizedReport(ReportFilterDto filter);
        Task<Pager<CustomerCreditUtilizedDto>> GetCustomerCreditUtilizedComparedReport(ReportFilterDto filter);

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
    public class ReportBusinessManager: IReportBusinessManager {
        private readonly IMapper _mapper;
        private readonly IReportManager _reportManager;
        private readonly ICompanyManager _companyManager;
        private readonly ICustomerManager _customerManager;
        private readonly ICustomerActivityManager _customerActivityManager;
        private readonly ICrudBusinessManager _businessManager;
        private readonly ICustomerCreditUtilizedManager _customerCreditUtilizedManager;

        private readonly ISavedReportManager _savedReportManager;
        private readonly ISavedReportFieldManager _savedReportFieldManager;
        private readonly ISavedReportFileManager _savedReportFileManager;

        private readonly ISavedReportPlanManager _savedReportPlanManager;
        private readonly ISavedReportPlanFieldManager _savedReportPlanFieldManager;

        public ReportBusinessManager(IMapper mapper, ICompanyManager companyManager,
            ICustomerManager customerManager,
            ICustomerActivityManager customerActivityManager,
            ICustomerCreditUtilizedManager customerCreditUtilizedManager,

            ISavedReportManager savedReportManager,
            ISavedReportFieldManager savedReportFieldManager,
            ISavedReportFileManager savedReportFileManager,

            ISavedReportPlanManager savedReportPlanManager,
            ISavedReportPlanFieldManager savedReportPlanFieldManager,

            ICrudBusinessManager businessManager,
            IReportManager reportManager
            ) {
            _mapper = mapper;
            _companyManager = companyManager;
            _customerManager = customerManager;
            _customerActivityManager = customerActivityManager;
            _customerCreditUtilizedManager = customerCreditUtilizedManager;

            _savedReportManager = savedReportManager;
            _savedReportFieldManager = savedReportFieldManager;
            _savedReportFileManager = savedReportFileManager;

            _savedReportPlanManager = savedReportPlanManager;
            _savedReportPlanFieldManager = savedReportPlanFieldManager;

            _businessManager = businessManager;
            _reportManager = reportManager;
        }

        public async Task<AgingReportResultDto> GetAgingReport(long companyId, DateTime dateTo, int daysPerPeriod, int numberOfPeriods, bool includeAllCustomers = true) {
            var company = await _companyManager.Find(companyId);
            if(company == null)
                return null;

            var allCustomers = await _customerManager.FindByCompanyId(companyId, dateTo);
            var customers = allCustomers.Where(x => x.Activities.IsActive(dateTo)).ToList();

            var invoices = await _reportManager.GetAgingInvoices(companyId, dateTo, daysPerPeriod, numberOfPeriods);
            //invoices = invoices.Where(x => x.Customer?.Activities.IsActive(dateTo)).ToList(); //check invoices for Inactive customers

            if(includeAllCustomers) {
                //Добавить всех недостающих Customers
                var exceptedCustomers = customers.Where(x => !invoices.Any(y => y.CustomerId == x.Id)).ToList();
                var expectedInvoices = exceptedCustomers.Select(x => new InvoiceEntity() {
                    Customer = x,
                    CustomerId = x.Id,
                    Company = company,
                    CompanyId = company.Id
                });
                invoices.AddRange(expectedInvoices);
                invoices.OrderBy(x => x.CustomerAccountNumber);
            }

            #region CREATE HEADERS
            var _col = new List<AgingReportColsDto>() { };

            for(int i = -1; i < numberOfPeriods; i++) {
                var from = (i < 0 ? -1 : 1) + i * daysPerPeriod;
                var to = (i + 1) * daysPerPeriod;

                _col.Add(new AgingReportColsDto() {
                    From = from,
                    To = to,
                    Name = $"{from}-{to}"
                });
            }

            _col.Add(new AgingReportColsDto() {
                To = 10000,
                From = 1 + numberOfPeriods * daysPerPeriod,
                Name = $"{1 + numberOfPeriods * daysPerPeriod}+"
            });

            _col.Add(new AgingReportColsDto() {
                Name = "Total Late"
            });

            _col.Add(new AgingReportColsDto() {
                Name = "Total"
            });
            #endregion

            var report = new Dictionary<long, AgingReportRowDto>();

            foreach(var invoice in invoices) {
                var recordKey = invoice.CustomerId ?? 0;

                var invoiceAmount = invoice.Subtotal * (1 + invoice.TaxRate / 100); //get total amount
                var diffPay = invoiceAmount - (invoice.Payments?.Sum(x => x.Amount) ?? 0);

                if(diffPay > 0 || includeAllCustomers) {
                    if(!report.ContainsKey(recordKey)) {
                        report.Add(recordKey, new AgingReportRowDto() {
                            Customer = _mapper.Map<CustomerDto>(invoice.Customer),
                            CustomerName = invoice.Customer.Name,
                            AccountNo = invoice.Customer.No,
                            Data = new Dictionary<string, decimal>()
                        });
                    }

                    var summary = report[recordKey].Data;

                    foreach(var c in _col) {
                        if(!summary.ContainsKey(c.Name))
                            summary.Add(c.Name, 0);
                    }

                    if(diffPay > 0) {
                        var diffDays = (dateTo - invoice.DueDate).Days;

                        foreach(var col in _col) {
                            if(diffDays <= 0) {
                                summary[col.Name] += diffPay;
                                break;
                            } else if(diffDays >= col.From && diffDays <= col.To) {
                                summary[col.Name] += diffPay;
                                break;
                            }
                        }
                    }
                } else {
                    Console.WriteLine($"{invoice.CustomerId} Pay more that should {diffPay}");
                }
            }

            foreach(var d in report) {
                d.Value.Data["Total"] = d.Value.Data.Sum(x => x.Value);
            }

            #region BALANCE
            var balance = new Dictionary<string, AgingReportBalanceDto>();
            foreach(var c in _col) {
                if(c.Name.Equals("Total")) {
                    balance.Add("Total", new AgingReportBalanceDto {
                        Count = balance.Values.Sum(x => x.Count),
                        Sum = report.Values.Sum(x => x.Data["Total"])
                    });
                } else if(c.Name.Equals("Total Late")) {

                } else {
                    balance.Add(c.Name, new AgingReportBalanceDto {
                        Count = report.Values.Count(x => x.Data[c.Name] != 0),
                        Sum = report.Values.Sum(x => x.Data[c.Name])
                    });
                }
            }

            balance.Add("Total Late", new AgingReportBalanceDto {
                Count = balance["Total"].Count - balance["-31-0"].Count,
                Sum = balance["Total"].Sum - balance["-31-0"].Sum
            });
            #endregion

            #region DEBT - DOUBLE INVOICE 
            var doubleDebt = new Dictionary<string, decimal>();

            foreach(var data in report.Values) {
                var debt = data.Data.Where(x => !x.Key.Equals("Total") && x.Value > 0).ToList();
                if(debt.Count > 1) {
                    var keyName = string.Join(',', debt.Select(x => x.Key));
                    var debtCount = debt.Count(x => x.Value > 0);
                    if(!doubleDebt.ContainsKey(keyName)) {
                        doubleDebt.Add(keyName, 0);
                    }
                    doubleDebt[keyName] += 1;
                }
            }
            #endregion

            #region CUSTOMER TYPES
            var customerTypes = customers.GroupBy(x => x.Type == null ? "No types" : x.Type.Name).ToDictionary(x => x.Key, x => x.Count());
            #endregion

            return new AgingReportResultDto() {
                CompanyId = companyId,
                CompanyName = company.Name,
                Date = dateTo,
                NumberOfPeriods = numberOfPeriods,

                Cols = _col,
                //REPORT
                Rows = report.Select(x => x.Value).ToList(),
                //BALANCE
                Balance = balance,
                //CUSTOMERS
                TotalCustomers = customers.Count,
                BalanceCustomers = report.Count(x => x.Value.Data["Total"] != 0),
                //CUSTOMERS TYPE
                CustomerTypes = customerTypes,
                //DEBT
                DoubleDebt = doubleDebt
            };
        }

        public async Task<ReportStatusDto> CheckingCustomerAccountNumber(long companyId, DateTime dateTo, int numberOfPeriods) {
            try {
                var company = await _companyManager.FindInclude(companyId);
                if(company == null || company.Settings == null || string.IsNullOrEmpty(company.Settings.AccountNumberTemplate)) {
                    throw new Exception("Please, check company settings! \"Account Number Template\" is not defined. ");
                }

                var customers = new List<CustomerDto>();
                var regex = new Regex(company.Settings.AccountNumberTemplate);

                var result = await GetAgingReport(companyId, dateTo, 30, numberOfPeriods, false);
                foreach(var data in result.Rows) {
                    var customer = data.Customer;
                    var isMatch = regex.IsMatch(customer.No);

                    if(!isMatch) {
                        customers.Add(customer);
                    }
                }

                if(customers.Count == 0) {
                    return new ReportStatusDto(ReportCheckStatus.Success, $"{result.Rows.Count} {company.Name} customers has valid \"Account Number\" that match the template set in the company settings");
                } else {
                    return new ReportStatusDto(ReportCheckStatus.Warning, $"{customers.Count} out of {result.Rows.Count} customers do not match the \"Account Number\" in the company settings template");
                }
            } catch(Exception ex) {
                return new ReportStatusDto(ReportCheckStatus.Danger, ex.Message);
            }
        }

        /// <summary>
        /// Display Credit Utilized Report by Company and Month with the options to delete record
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<Pager<CustomerCreditUtilizedDto>> GetCustomerCreditUtilizedReport(ReportFilterDto filter) {
            var customers = await _customerManager.FindByCompanyId(filter.CompanyId);
            customers = customers.Where(x => (true)
                        && (string.IsNullOrEmpty(filter.Search) || x.Name.ToLower().Contains(filter.Search.ToLower()) || x.No.ToLower().Contains(filter.Search.ToLower()))
                        ).ToList();

            var credits = await _customerCreditUtilizedManager.FindByCompanyIdAndDate(filter.CompanyId, filter.Date);
            credits = credits.GroupBy(x => x.Customer, x => x, (customer, credits) => new { Customer = customer, Credit = credits.OrderByDescending(x => x.CreatedDate).FirstOrDefault() })
                .Where(x => x.Credit != null)
                .Select(x => x.Credit).ToList();

            var dates = credits.GroupBy(x => x.CreatedDate).Select(x => x.Key)
                 .OrderBy(x => x)
                 .Select(x => $"{x.Month}/{x.Year}")
                 .ToList();

            if(filter.FilterDate.HasValue)
                credits = credits.Where(x => x.CreatedDate.ToString("MM/yyyy") == filter.FilterDate.Value.ToString("MM/yyyy")).ToList();

            var result = _mapper.Map<List<CustomerCreditUtilizedDto>>(credits);

            var count = result.Count;
            if(count == 0)
                return new Pager<CustomerCreditUtilizedDto>(new List<CustomerCreditUtilizedDto>(), 0, filter.Offset, filter.Limit);

            var page = (filter.Offset + filter.Limit) / filter.Limit;
            var pager = new Pager<CustomerCreditUtilizedDto>(result.Skip(filter.Offset).Take(filter.Limit), count, page, filter.Limit);
            pager.Filter.Add("CreatedDate", dates);
            return pager;
        }

        public async Task<Pager<CustomerCreditUtilizedDto>> GetCustomerCreditUtilizedComparedReport(ReportFilterDto filter) {
            var company = await _companyManager.FindInclude(filter.CompanyId);
            if(company.Settings == null || !company.Settings.SaveCreditValues)
                return new Pager<CustomerCreditUtilizedDto>(new List<CustomerCreditUtilizedDto>(), 0, filter.Offset, filter.Limit);

            var creditUtilizedSettings = await _businessManager.GetCustomerCreditUtilizedSettings(filter.CompanyId, filter.Date);
            if(creditUtilizedSettings == null) {
                creditUtilizedSettings = new CustomerCreditUtilizedSettingsDto() {
                    RoundType = company.Settings.RoundType,
                    CompanyId = company.Id
                };
            }

            var creditUtilizedList = new List<CustomerCreditUtilizedDto>();
            var report = await GetAgingReport(filter.CompanyId, filter.Date, 30, 4, false);
            foreach(var data in report.Rows) {
                var customer = data.Customer;
                var value = data.Data["Total"];//new height credit

                if(creditUtilizedSettings.RoundType == RoundType.RoundUp) {
                    value = Math.Ceiling(value);
                } else if(creditUtilizedSettings.RoundType == RoundType.RoundDown) {
                    value = Math.Floor(value);
                }

                var creditUtilized = customer.CreditUtilizeds.FirstOrDefault();

                if(creditUtilized == null || (creditUtilized.CreatedDate != filter.Date && creditUtilized.Value < value)) {
                    if(creditUtilized == null) {
                        creditUtilized = new CustomerCreditUtilizedDto() {
                            CreatedDate = DateTime.Now
                        };
                    }

                    creditUtilized.CustomerId = customer.Id;
                    creditUtilized.Customer = customer;
                    creditUtilized.IsNew = true;
                    creditUtilized.NewValue = value;
                    creditUtilized.NewCreatedDate = filter.Date;

                    creditUtilizedList.Add(creditUtilized);
                } else if(creditUtilized.Value < value) {

                    creditUtilized.CustomerId = customer.Id;
                    creditUtilized.Customer = customer;
                    creditUtilized.NewValue = value;
                    creditUtilized.NewCreatedDate = filter.Date;

                    creditUtilizedList.Add(creditUtilized);
                }
            }
            var count = creditUtilizedList.Count();

            if(count == 0)
                return new Pager<CustomerCreditUtilizedDto>(new List<CustomerCreditUtilizedDto>(), 0, filter.Offset, filter.Limit);

            var page = (filter.Offset + filter.Limit) / filter.Limit;
            var pager = new Pager<CustomerCreditUtilizedDto>(creditUtilizedList.Skip(filter.Offset).Take(filter.Limit), count, page, filter.Limit);

            return pager;
        }

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
