using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Data.Dto;
using Core.Data.Entities;
using Core.Services.Managers;

namespace Core.Services.Business {
    public interface IReportBusinessManager {
        Task<AgingSummaryReport> GetAgingReport(long companyId, DateTime period, int daysPerPeriod, int numberOfPeriod);
    }
    public class ReportBusinessManager: IReportBusinessManager {
        private readonly IReportManager _reportManager;
        private readonly ICompanyManager _companyManager;
        private readonly ICustomerManager _customerManager;
        private readonly ICustomerActivityManager _customerActivityManager;

        public ReportBusinessManager(IReportManager reportManager,
            ICompanyManager companyManager,
            ICustomerManager customerManager,
            ICustomerActivityManager customerActivityManager
            ) {
            _reportManager = reportManager;
            _companyManager = companyManager;
            _customerManager = customerManager;
            _customerActivityManager = customerActivityManager;
        }

        public async Task<AgingSummaryReport> GetAgingReport(long companyId, DateTime dateTo, int daysPerPeriod, int numberOfPeriod) {
            var company = await _companyManager.FindInclude(companyId);
            if(company == null)
                return null;

            var customers = await _customerManager.FindByCompanyId(companyId, dateTo);

            var result = await _reportManager.GetAgingInvoices(companyId, dateTo, daysPerPeriod, numberOfPeriod);

            #region CREATE HEADERS
            var columns = new List<string>() { "Current" };
            for(int i = 0; i < numberOfPeriod; i++) {
                columns.Add($"{1 + i * daysPerPeriod}-{(i + 1) * daysPerPeriod}");
            }
            columns.Add("Total");
            #endregion

            var report = new Dictionary<long, AgingSummaryData>();

            foreach(var invoice in result) {
                var recordKey = invoice.CustomerId ?? 0;

                if(!report.ContainsKey(recordKey)) {
                    report.Add(recordKey, new AgingSummaryData() {
                        CustomerName = invoice.Customer.Name,
                        AccountNumber = invoice.Customer.AccountNumber,
                        Data = new Dictionary<string, decimal>()
                    });
                }

                var summary = report[recordKey].Data;
                //init all column names
                foreach(var c in columns) {
                    if(!summary.ContainsKey(c))
                        summary.Add(c, 0);
                }

                var invoiceAmount = invoice.Subtotal * (1 + invoice.TaxRate / 100); //get total amount
                var diffPay = invoiceAmount - (invoice.Payments?.Sum(x => x.Amount) ?? 0);

                if(diffPay > 0) {
                    var diffDate = (dateTo - invoice.DueDate).Days;

                    //Add amount to Current fiedl if DiffDate negativ 
                    if(diffDate <= 0) {
                        summary["Current"] += diffPay;
                    } else {
                        for(var i = 0; i < numberOfPeriod; i++) {
                            int from = i * daysPerPeriod;
                            int to = (i + 1) * daysPerPeriod;
                            string key = $"{from + 1}-{to}";

                            if(diffDate > from && diffDate <= to) {
                                summary[key] += diffPay;
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

            var balance = new Dictionary<string, AgingSummaryBalance>();
            foreach(var c in columns) {
                balance.Add(c, new AgingSummaryBalance {
                    Sum = report.Values.Sum(x => x.Data[c]),
                    Count = report.Values.Count(x => x.Data[c] != 0)
                });
            }

            return new AgingSummaryReport() {
                CompanyId = companyId,
                CompanyName = company.Name,
                Date = dateTo,
                Columns = columns.ToArray(),
                Data = report.Select(x => x.Value).ToList(),
                Balance = balance,
                TotalCustomers = customers.Count,
                BalanceCustomers = report.Count(x => x.Value.Data["Total"] != 0)
            };
        }
    }
}
