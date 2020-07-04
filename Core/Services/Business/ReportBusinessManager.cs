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

using Microsoft.VisualBasic;

namespace Core.Services.Business {
    public interface IReportBusinessManager {
        Task<AgingReportResultDto> GetAgingReport(long companyId, DateTime period, int daysPerPeriod, int numberOfPeriod, bool includeAllCustomers);
        Task<ReportStatusDto> CheckingCustomerAccountNumber(long companyId, DateTime dateTo, int numberOfPeriods);
        Task<ReportResultDto> GetCustomerCreditUtilizedReport(long companyId, DateTime dateTo);
    }
    public class ReportBusinessManager: IReportBusinessManager {
        private readonly IMapper _mapper;
        private readonly IReportManager _reportManager;
        private readonly ICompanyManager _companyManager;
        private readonly ICustomerManager _customerManager;
        private readonly ICustomerActivityManager _customerActivityManager;
        private readonly ICrudBusinessManager _businessManager;

        public ReportBusinessManager(IMapper mapper, ICompanyManager companyManager,
            ICustomerManager customerManager,
            ICustomerActivityManager customerActivityManager,
            ICrudBusinessManager businessManager,
            IReportManager reportManager
            ) {
            _mapper = mapper;
            _companyManager = companyManager;
            _customerManager = customerManager;
            _customerActivityManager = customerActivityManager;
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

        public async Task<ReportResultDto> GetCustomerCreditUtilizedReport(long companyId, DateTime dateTo) {
            var company = await _companyManager.FindInclude(companyId);
            if(company == null || company.Settings == null || string.IsNullOrEmpty(company.Settings.AccountNumberTemplate)) {
                throw new Exception("Please, check company settings! \"Account Number Template\" is not defined. ");
            }

            var dateFrom = dateTo.FirstDayOfMonth();

            var customers = await _customerManager.FindByCompanyId(companyId);
            foreach(var customer in customers) {
                
            }
            var columns = customers.Select(x => x.CreditUtilizeds.Where(y => y.CreatedDate >= dateFrom && y.CreatedDate <= dateTo).Select(x => x.CreatedDate));

            var result = new ReportResultDto() {
                Date = dateTo,
                CompanyId = companyId
            };

            #region COLS
            var cols = new List<ReportColsDto>();
            cols.Add(new ReportColsDto() { Name = "Account Number" });
            cols.Add(new ReportColsDto() { Name = "Business Name" });

            #endregion



            return new ReportResultDto() {
                
            };
        }
    }
}
