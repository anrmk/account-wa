using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Entities;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Core.Services.Managers {
    public interface IReportManager {
        //Task<List<ReportDataDto>> GetAgingReport(long companyId, DateTime dateTo, int daysPerPeriod, int numberOfPeriod);
        Task<List<InvoiceEntity>> GetAgingInvoices(long companyId, DateTime dateTo, int daysPerPeriod, int numberOfPeriod);
    }

    public class ReportManager: IReportManager {
        private readonly ApplicationContext _context;
        private readonly IConfiguration _configuration;
        private readonly IInvoiceManager _invoiceManager;

        public ReportManager(ApplicationContext context, IConfiguration configuration, IInvoiceManager invoiceManager) {
            _context = context;
            _configuration = configuration;

            _invoiceManager = invoiceManager;
        }

        public async Task<List<InvoiceEntity>> GetAgingInvoices(long companyId, DateTime dateTo, int daysPerPeriod, int numberOfPeriod) {
            var query = "SELECT INV.[Id], INV.[No], INV.[Subtotal], INV.[TaxRate], INV.[Date], INV.[DueDate], INV.[IsDraft], " +
                        "PAY.[Id] AS PayId, PAY.[No] AS PayNo, PAY.[Amount] AS PayAmount, PAY.[Date] AS PayDate, " +
                        "CUS.[Id] AS CustomerId, CUS.[AccountNumber] AS CustomerAccountNumber, CUS.[Name] AS CustomerName, CUS.[PhoneNumber] AS CustomerPhoneNumber, CUS.[Terms] AS CustomerTerms, CUS.[CreditLimit] AS CustomerCreditLimit,  CUS.[CreditUtilized] AS CustomerCreditUtilized, " +
                        "ADDR.[Id] as CustomerAddressId, ADDR.[Address] as CustomerAddress, ADDR.[Address2] as CustomerAddress2, ADDR.[City] as CustomerCity, ADDR.[State] as CustomerState, ADDR.[ZipCode] as CustomerZipCode, ADDR.[Country] as CustomerCountry, " +
                        "COM.[Id] AS CompanyId, COM.[No] AS CompanyNo, COM.[Name] AS CompanyName, COM.[PhoneNumber] AS CompanyPhoneNumber, " +
                        "DATEDIFF(DAY, INV.[DueDate], @DATEFROM ) AS DiffDate  " +
                        "FROM [accountWa].[dbo].[Invoices] AS INV  " +
                        "LEFT JOIN [accountWa].[dbo].[Payments] AS PAY ON PAY.[Invoice_Id] = INV.[Id] AND PAY.[Date] <= @DATETO " +
                        "LEFT JOIN [accountWa].[dbo].[Customers] as CUS ON CUS.[Id] = INV.[Customer_Id]  " +
                        "LEFT JOIN [accountWa].[dbo].[CustomerAddresses] as ADDR ON ADDR.[Id] = CUS.[CustomerAddress_Id]  " +
                        "LEFT JOIN [accountWa].[dbo].[Companies] as COM ON COM.[Id] = INV.[Company_Id]  " +
                        "WHERE INV.[Company_Id] = @COMPANYID AND INV.[Date] <= @DATETO " +
                        //"WHERE INV.[Company_Id] = @COMPANYID AND INV.[DueDate] >= @DATEFROM AND INV.[Date] <= @DATETO " + //предыдущая логика
                        "ORDER BY CUS.[AccountNumber] ASC ";

            var result = new List<InvoiceEntity>();

            try {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using(var connection = new SqlConnection(connectionString)) {
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
                                        No = reader["CustomerAccountNumber"] as string,
                                        Name = reader["CustomerName"] as string,
                                        PhoneNumber = reader["CustomerPhoneNumber"] as string,
                                        Terms = reader["CustomerTerms"] as string,
                                        CreditLimit = reader["CustomerCreditLimit"] != DBNull.Value ? (decimal)reader["CustomerCreditLimit"] : (decimal?)null,
                                        CreditUtilized = reader["CustomerCreditUtilized"] != DBNull.Value ? (decimal)reader["CustomerCreditUtilized"] : (decimal?)null
                                    };

                                    if(reader["CustomerAddressId"] != DBNull.Value) {
                                        var address = new CustomerAddressEntity() {
                                            Id = (long)reader["CustomerAddressId"],
                                            Address = reader["CustomerAddress"] as string,
                                            Address2 = reader["CustomerAddress2"] as string,
                                            City = reader["CustomerCity"] as string,
                                            State = reader["CustomerState"] as string,
                                            ZipCode = reader["CustomerZipCode"] as string,
                                            Country = reader["CustomerCountry"] as string,
                                        };

                                        customer.AddressId = (long)reader["CustomerAddressId"];
                                        customer.Address = address;
                                    }

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
