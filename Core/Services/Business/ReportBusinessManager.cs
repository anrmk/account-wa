using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Core.Data.Dto;
using Core.Data.Entities;
using Core.Extension;
using Core.Services.Managers;

namespace Core.Services.Business {
    public interface IReportBusinessManager {
        Task<AgingSummaryReport> GetAgingReport(long companyId, DateTime period, int daysPerPeriod, int numberOfPeriod, bool includeAllCustomers);
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

        public async Task<AgingSummaryReport> GetAgingReport(long companyId, DateTime dateTo, int daysPerPeriod, int numberOfPeriods, bool includeAllCustomers = true) {
            var company = await _companyManager.Find(companyId);
            if(company == null)
                return null;

            var allCustomers = await _customerManager.FindByCompanyId(companyId, dateTo);
            var customers = allCustomers.Where(x => x.Activities.IsActive(dateTo)).ToList();
            //var customer = customers.Where(x => x.Activities.Count > 1).FirstOrDefault();

            var allInvoices = await _reportManager.GetAgingInvoices(companyId, dateTo, daysPerPeriod, numberOfPeriods);
            var invoices = allInvoices;//.Where(x => x.Customer?.Activities.IsActive(dateTo)).ToList(); //check invoices for Inactive customers

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
            var _col = new List<AgingSummaryPeriod>() { new AgingSummaryPeriod() {
                From = -31,
                To = 0,
                Name = "Current"
            } };

            var columns = new List<string>() { "Current" };
            for(int i = 0; i < numberOfPeriods; i++) {
                var from = 1 + i * daysPerPeriod;
                var to = (i + 1) * daysPerPeriod;
                columns.Add($"{from}-{to}");

                _col.Add(new AgingSummaryPeriod() {
                    From = from,
                    To = to,
                    Name = $"{from}-{to}"
                });
            }
            _col.Add(new AgingSummaryPeriod() {
                From = 1 + numberOfPeriods * daysPerPeriod,
                Name = $"{1 + numberOfPeriods * daysPerPeriod}+"
            });
            _col.Add(new AgingSummaryPeriod() {
                Name = "Total"
            });
            columns.Add($"{1 + numberOfPeriods * daysPerPeriod}+");
            columns.Add("Total");
            #endregion

            var report = new Dictionary<long, AgingSummaryData>();

            foreach(var invoice in invoices) {
                var recordKey = invoice.CustomerId ?? 0;

                var invoiceAmount = invoice.Subtotal * (1 + invoice.TaxRate / 100); //get total amount
                var diffPay = invoiceAmount - (invoice.Payments?.Sum(x => x.Amount) ?? 0);

                if(diffPay > 0 || includeAllCustomers) {
                    if(!report.ContainsKey(recordKey)) {
                        report.Add(recordKey, new AgingSummaryData() {
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

                    var diffDays = (dateTo - invoice.DueDate).Days;

                    //Add amount to Current field if DiffDate negative
                    if(diffDays <= 0) {
                        summary["Current"] += diffPay;
                    } else {
                        for(var i = 0; i < numberOfPeriods; i++) {
                            int from = i * daysPerPeriod;
                            int to = (i + 1) * daysPerPeriod;
                            int upTo = 1 + numberOfPeriods * daysPerPeriod;

                            string key = $"{from + 1}-{to}";

                            if(diffDays > from && diffDays <= to) {
                                summary[key] += diffPay;
                            } else if(diffDays >= upTo) {
                                summary[$"{upTo}+"] += diffPay;
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
            var balance = new Dictionary<string, AgingSummaryBalance>();
            foreach(var c in columns) {
                if(!c.Equals("Total")) {
                    balance.Add(c, new AgingSummaryBalance {
                        Sum = report.Values.Sum(x => x.Data[c]),
                        Count = report.Values.Count(x => x.Data[c] != 0)
                    });
                }
            }

            balance.Add("Total", new AgingSummaryBalance {
                Sum = report.Values.Sum(x => x.Data["Total"]),
                Count = balance.Values.Sum(x => x.Count)
            });
            #endregion

            return new AgingSummaryReport() {
                CompanyId = companyId,
                CompanyName = company.Name,
                Date = dateTo,
                DaysPerPeriod = daysPerPeriod,
                NumberOfPeriods = numberOfPeriods,
                Columns = _col,
                Data = report.Select(x => x.Value).ToList(),
                Balance = balance,
                TotalCustomers = customers.Count,
                BalanceCustomers = report.Count(x => x.Value.Data["Total"] != 0)
            };
        }
    }
}
