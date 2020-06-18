using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Core.Data.Entities;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Core.Services.Managers {
    public interface IReportManager {
        //Task<List<ReportDataDto>> GetAgingReport(long companyId, DateTime dateTo, int daysPerPeriod, int numberOfPeriod);
        Task<List<InvoiceEntity>> GetAgingInvoices(long companyId, DateTime dateTo, int daysPerPeriod, int numberOfPeriod);
    }

    public class ReportManager: IReportManager {
        private readonly IConfiguration _configuration;
        private readonly IInvoiceManager _invoiceManager;

        public ReportManager(IConfiguration configuration, IInvoiceManager invoiceManager) {
            _configuration = configuration;

            _invoiceManager = invoiceManager;
        }

        public async Task<List<InvoiceEntity>> GetAgingInvoices(long companyId, DateTime dateTo, int daysPerPeriod, int numberOfPeriod) {
            var query = "SELECT INV.[Id], INV.[No], INV.[Subtotal], INV.[TaxRate], INV.[Date], INV.[DueDate], INV.[IsDraft], INV.[CreatedDate]," +
                        "PAY.[Id] AS PayId, PAY.[No] AS PayNo, PAY.[Amount] AS PayAmount, PAY.[Date] AS PayDate, " +
                        "CUS.[Id] AS CustomerId, CUS.[AccountNumber] AS CustomerAccountNumber, CUS.[Name] AS CustomerName, CUS.[PhoneNumber] AS CustomerPhoneNumber, CUS.[Terms] AS CustomerTerms, CUS.[CustomerType_Id] AS CustomerTypeId, CUST.[Name] AS CustomerTypeName, CUS.[CreatedDate] AS CustomerCreatedDate, " +
                        "CACT.[Id] AS ActivityId, CACT.[CreatedDate] AS ActivityDate, CACT.[IsActive] AS ActivityStatus, " +
                        "CLIM.[Id] AS CreditLimitId, CLIM.[Value] AS CreditLimit, CLIM.[CreatedDate] AS CreditLimitDate, " +
                        "CUTIL.[Id] AS CreditUtilizedId, CUTIL.[Value] AS CreditUtilized, CUTIL.[CreatedDate] AS CreditUtilizedDate, " +
                        "ADDR.[Id] as CustomerAddressId, ADDR.[Address] as CustomerAddress, ADDR.[Address2] as CustomerAddress2, ADDR.[City] as CustomerCity, ADDR.[State] as CustomerState, ADDR.[ZipCode] as CustomerZipCode, ADDR.[Country] as CustomerCountry, " +
                        "COM.[Id] AS CompanyId, COM.[No] AS CompanyNo, COM.[Name] AS CompanyName, COM.[PhoneNumber] AS CompanyPhoneNumber, " +
                        "DATEDIFF(DAY, INV.[DueDate], @DATEFROM ) AS DiffDate " +
                        "FROM [accountWa].[dbo].[Invoices] AS INV " +
                        "LEFT JOIN [accountWa].[dbo].[Payments] AS PAY ON PAY.[Invoice_Id] = INV.[Id] AND PAY.[Date] <= @DATETO " +
                        "LEFT JOIN [accountWa].[dbo].[Customers] as CUS ON CUS.[Id] = INV.[Customer_Id]  " +
                        "LEFT JOIN [accountWa].[dbo].[CustomerTypes] as CUST ON CUS.[CustomerType_Id] = CUST.[Id] " +
                        "OUTER APPLY (SELECT TOP 1 * FROM [accountWa].[dbo].[CustomerActivities] " + //проверяем на активность пользователя
                         //"WHERE [Customer_Id] = CUS.[Id] AND [IsActive] = 'TRUE' AND [CreatedDate] <= @DATETO " +
                            "WHERE [Customer_Id] = CUS.[Id] AND [CreatedDate] <= @DATETO " +
                            "ORDER BY [CreatedDate] DESC) AS CACT " +
                        "OUTER APPLY (SELECT TOP 1 * FROM [accountWa].[dbo].[CustomerCreditLimit] " +
                            "WHERE [Customer_Id] = CUS.[Id] AND [CreatedDate] <= @DATETO " +
                            "ORDER BY [CreatedDate] DESC) AS CLIM " +
                        "OUTER APPLY (SELECT TOP 1 * FROM [accountWa].[dbo].[CustomerCreditUtilized] " +
                            "WHERE [Customer_Id] = CUS.[Id] AND [CreatedDate] <= @DATETO " +
                            "ORDER BY [CreatedDate] DESC) AS CUTIL " +
                        "LEFT JOIN [accountWa].[dbo].[CustomerAddresses] as ADDR ON ADDR.[Id] = CUS.[CustomerAddress_Id]  " +
                        "LEFT JOIN [accountWa].[dbo].[Companies] as COM ON COM.[Id] = INV.[Company_Id]  " +
                        "WHERE INV.[Company_Id] = @COMPANYID AND CACT.[Id] IS NOT NULL AND INV.[Date] <= @DATETO " +
                        "AND CACT.[IsActive] = 'TRUE' " +
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
                                    IsDraft = (bool)reader["IsDraft"],
                                    CreatedDate = (DateTime)reader["CreatedDate"]
                                };

                                if(reader["CustomerId"] != DBNull.Value) {
                                    var customer = new CustomerEntity() {
                                        Id = (long)reader["CustomerId"],
                                        No = reader["CustomerAccountNumber"] as string,
                                        Name = reader["CustomerName"] as string,
                                        PhoneNumber = reader["CustomerPhoneNumber"] as string,
                                        Terms = reader["CustomerTerms"] as string,
                                        CreatedDate = (DateTime)reader["CustomerCreatedDate"]
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

                                    if(reader["CustomerTypeId"] != DBNull.Value) {
                                        var customerType = new CustomerTypeEntity() {
                                            Id = (long)reader["CustomerTypeId"],
                                            Name = reader["CustomerTypeName"] as string
                                        };

                                        customer.TypeId = (long)reader["CustomerTypeId"];
                                        customer.Type = customerType;
                                    }

                                    if(reader["ActivityId"] != DBNull.Value) {
                                        var activity = new CustomerActivityEntity() {
                                            Id = (long)reader["ActivityId"],
                                            CustomerId = customer.Id,
                                            CreatedDate = (DateTime)reader["ActivityDate"],
                                            IsActive = (bool)reader["ActivityStatus"]
                                        };

                                        customer.Activities = new Collection<CustomerActivityEntity>();
                                        customer.Activities.Add(activity);
                                    }

                                    if(reader["CreditLimitId"] != DBNull.Value) {
                                        var creditLimit = new CustomerCreditLimitEntity() {
                                            Id = (long)reader["CreditLimitId"],
                                            CustomerId = customer.Id,
                                            CreatedDate = (DateTime)reader["CreditLimitDate"],
                                            Value = reader["CreditLimit"] != DBNull.Value ? (decimal)reader["CreditLimit"] : 0
                                        };

                                        customer.CreditLimits = new Collection<CustomerCreditLimitEntity>();
                                        customer.CreditLimits.Add(creditLimit);
                                    }

                                    if(reader["CreditUtilizedId"] != DBNull.Value) {
                                        var creditUtilized = new CustomerCreditUtilizedEntity() {
                                            Id = (long)reader["CreditUtilizedId"],
                                            CustomerId = customer.Id,
                                            CreatedDate = (DateTime)reader["CreditUtilizedDate"],
                                            Value = reader["CreditUtilized"] != DBNull.Value ? (decimal)reader["CreditUtilized"] : 0
                                        };

                                        customer.CreditUtilizeds = new Collection<CustomerCreditUtilizedEntity>();
                                        customer.CreditUtilizeds.Add(creditUtilized);
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
