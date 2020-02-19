using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Dto;
using Core.Data.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface IReportManager {
        Task<List<ReportDataDto>> GetAgingReport(long companyId, DateTime dateTo, int daysPerPeriod, int numberOfPeriod);
        Task<List<InvoiceEntity>> GetAgingInvoices(long companyId, DateTime dateTo, int daysPerPeriod, int numberOfPeriod);
    }

    public class ReportManager: IReportManager {
        private readonly ApplicationContext _context;
        public ReportManager(ApplicationContext context) {
            _context = context;
        }

        [Obsolete]
        public async Task<List<ReportDataDto>> GetAgingReport(long companyId, DateTime dateTo, int daysPerPeriod, int numberOfPeriod) {
            var query = "SELECT CUS.[AccountNumber], CUS.[Name] AS CustomerName, INV.[Customer_Id] AS CustomerId, INV.[Id], INV.[No], CAST(INV.[Subtotal] * (1+INV.[TaxRate]/100) as decimal(10,2)) AS Amount, INV.[Date], INV.[DueDate], PAY.[Amount] AS PayAmount, PAY.[Date] AS PayDate, " + 
                        "DATEDIFF(DAY, INV.[DueDate], @PERIOD) AS DiffDate " +
                        "FROM[accountWa].[dbo].[Invoices] AS INV " +
                        "LEFT JOIN[accountWa].[dbo].[Payments] AS PAY " +
                        "ON PAY.[Invoice_Id] = INV.[Id] AND PAY.[Date] <= @PERIOD " +
                        "LEFT JOIN [accountWa].[dbo].Customers as CUS " +
                        "ON CUS.[Id] = INV.[Customer_Id] " +
                        "WHERE INV.[Company_Id] = @COMPANYID AND INV.[DueDate] >= @PERIODFROM AND INV.[Date] <= @PERIOD " +
                        "ORDER BY CUS.[AccountNumber]";
            var result = new List<ReportDataDto>();
            var invoices = new List<InvoiceDto>();

            try {
                using(var connection = _context.Database.GetDbConnection()) {
                    using(var command = connection.CreateCommand()) {
                        var from = dateTo.AddDays(daysPerPeriod * numberOfPeriod * -1);

                        command.CommandText = query;
                        command.Parameters.Add(new SqlParameter("@COMPANYID", System.Data.SqlDbType.BigInt));
                        command.Parameters.Add(new SqlParameter("@PERIOD", System.Data.SqlDbType.Date));
                        command.Parameters.Add(new SqlParameter("@PERIODFROM", System.Data.SqlDbType.Date));
                        command.Parameters["@COMPANYID"].Value = companyId;
                        command.Parameters["@PERIOD"].Value = dateTo;
                        command.Parameters["@PERIODFROM"].Value = from;

                        if(connection.State == System.Data.ConnectionState.Closed) {
                            await connection.OpenAsync();
                        }

                        using(var reader = await command.ExecuteReaderAsync()) {
                            while(reader.Read()) {
                                result.Add(new ReportDataDto() {
                                    Id = (long)reader["Id"],
                                    CustomerId = (long)reader["CustomerId"],
                                    CustomerName = reader["CustomerName"] as string,
                                    AccountNumber = reader["AccountNumber"] as string,
                                    No = reader["No"] as string,
                                    Amount = (decimal)reader["Amount"],
                                    Date = (DateTime)reader["Date"],
                                    DueDate = (DateTime)reader["DueDate"],
                                    PayAmount = reader["PayAmount"] != DBNull.Value ? (decimal)reader["PayAmount"] : (decimal?)null,
                                    PayDate = reader["PayDate"] != DBNull.Value ? (DateTime)reader["PayDate"] : (DateTime?)null,
                                    DiffDate = reader["DiffDate"] != DBNull.Value ? (int)reader["DiffDate"] : (int?)null
                                });

                                invoices.Add(new InvoiceDto() {
                                    Id = (long)reader["Id"],
                                    No = reader["No"] as string,
                                    CustomerId = (long)reader["CustomerId"],
                                    Date = (DateTime)reader["Date"],
                                    DueDate = (DateTime)reader["DueDate"],
                                });
                            }
                        }
                    }
                }
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }
            return result;
        }

        public async Task<List<InvoiceEntity>> GetAgingInvoices(long companyId, DateTime dateTo, int daysPerPeriod, int numberOfPeriod) {
            var query = "SELECT INV.[Id], INV.[No], INV.[Subtotal], INV.[TaxRate], INV.[Date], INV.[DueDate], INV.[IsDraft], " +
                        "PAY.[Id] AS PayId, PAY.[No] AS PayNo, PAY.[Amount] AS PayAmount, PAY.[Date] AS PayDate, " +
                        "CUS.[Id] AS CustomerId, CUS.[AccountNumber] AS CustomerAccountNumber, CUS.[Name] AS CustomerName, CUS.[PhoneNumber] AS CustomerPhoneNumber, CUS.[Terms] AS CustomerTerms, CUS.[CreditLimit] AS CustomerCreditLimit,  CUS.[CreditUtilized] AS CustomerCreditUtilized, " +
                        "COM.[Id] AS CompanyId, COM.[No] AS CompanyNo, COM.[Name] AS CompanyName, COM.[PhoneNumber] AS CompanyPhoneNumber, " +
                        "DATEDIFF(DAY, INV.[DueDate], @DATEFROM ) AS DiffDate  " +
                        "FROM[accountWa].[dbo].[Invoices] AS INV  " +
                        "LEFT JOIN[accountWa].[dbo].[Payments] AS PAY ON PAY.[Invoice_Id] = INV.[Id] AND PAY.[Date] <= @DATETO " +
                        "LEFT JOIN [accountWa].[dbo].[Customers] as CUS ON CUS.[Id] = INV.[Customer_Id]  " +
                        "LEFT JOIN [accountWa].[dbo].[Companies] as COM ON COM.[Id] = INV.[Company_Id]  " +
                        "WHERE INV.[Company_Id] = @COMPANYID AND INV.[DueDate] >= @DATEFROM AND INV.[Date] <= @DATETO " +
                        "ORDER BY CUS.[AccountNumber] DESC";

            var result = new List<InvoiceEntity>();

            try {
                using(var connection = _context.Database.GetDbConnection()) {
                    using(var command = connection.CreateCommand()) {
                        var dateFrom = dateTo.AddDays(daysPerPeriod * numberOfPeriod * -1);

                        command.CommandText = query;
                        command.Parameters.Add(new SqlParameter("@COMPANYID", System.Data.SqlDbType.BigInt));
                        command.Parameters.Add(new SqlParameter("@DATEFROM", System.Data.SqlDbType.Date));
                        command.Parameters.Add(new SqlParameter("@DATETO", System.Data.SqlDbType.Date));

                        command.Parameters["@COMPANYID"].Value = companyId;
                        command.Parameters["@DATEFROM"].Value = dateFrom;
                        command.Parameters["@DATETO"].Value = dateTo;

                        if(connection.State == System.Data.ConnectionState.Closed) {
                            await connection.OpenAsync();
                        }

                        using(var reader = await command.ExecuteReaderAsync()) {
                            while(reader.Read()) {
                                var invoice = new InvoiceEntity() {
                                    Id = (long)reader["Id"],
                                    No = reader["No"] as string,
                                    Subtotal = reader["Subtotal"] != DBNull.Value ? (decimal)reader["Subtotal"] : 0,
                                    TaxRate = reader["TaxRate"] != DBNull.Value ? (decimal)reader["TaxRate"] : 0,
                                    Date = (DateTime)reader["Date"],
                                    DueDate = (DateTime)reader["DueDate"],
                                    IsDraft = (bool)reader["IsDraft"]
                                };

                                if(reader["CustomerId"] != DBNull.Value) {
                                    var customer = new CustomerEntity() {
                                        Id = (long)reader["CustomerId"],
                                        AccountNumber = reader["CustomerAccountNumber"] as string,
                                        Name = reader["CustomerName"] as string,
                                        PhoneNumber = reader["CustomerPhoneNumber"] as string,
                                        Terms = reader["CustomerTerms"] as string,
                                        CreditLimit = reader["CustomerCreditLimit"] != DBNull.Value ? (decimal)reader["CustomerCreditLimit"] : (decimal?)null,
                                        CreditUtilized = reader["CustomerCreditUtilized"] != DBNull.Value ? (decimal)reader["CustomerCreditUtilized"] : (decimal?)null
                                    };

                                    invoice.CustomerId = (long)reader["CustomerId"];
                                    invoice.Customer = customer;
                                }

                                if(reader["CompanyId"] != DBNull.Value) {
                                    invoice.CompanyId = (long)reader["CompanyId"];
                                    invoice.Company = new CompanyEntity() {
                                        Id = (long)reader["CompanyId"],
                                        No = reader["CompanyNo"] as string,
                                        Name = reader["CompanyName"] as string,
                                        PhoneNumber = reader["CustomerPhoneNumber"] as string
                                    };
                                }

                                if(reader["PayId"] != DBNull.Value) {
                                    invoice.Payments = new List<PaymentEntity>();
                                    invoice.Payments.Add(new PaymentEntity() {
                                        Id = (long)reader["PayId"],
                                        No = reader["PayNo"] as string,
                                        Amount = reader["PayAmount"] != DBNull.Value ? (decimal)reader["PayAmount"] : 0,
                                        Date = reader["PayDate"] != DBNull.Value ? (DateTime)reader["PayDate"] : new DateTime(),
                                    });
                                }

                                result.Add(invoice);
                            }
                        }
                    }
                }
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }
            return result;
        }
    }
}
