using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Dto;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface IReportManager {
        Task<ReportDto<AgingReportDataDto>> GetAgingReport(long companyId, DateTime period, int daysPerPeriod, int numberOfPeriod);
    }

    public class ReportManager: IReportManager {
        private readonly ApplicationContext _context;
        public ReportManager(ApplicationContext context) {
            _context = context;
        }

        public async Task<ReportDto<AgingReportDataDto>> GetAgingReport(long companyId, DateTime period, int daysPerPeriod, int numberOfPeriod) {
            var query = "SELECT CUS.[AccountNumber], CUS.[Name] AS CustomerName, INV.[Customer_Id] AS CustomerId, INV.[Id], INV.[No], CAST(INV.[Subtotal] * (1+INV.[TaxRate]/100) as decimal(10,2)) AS Amount, INV.[Date], INV.[DueDate], PAY.[Amount] AS PayAmount, PAY.[Date] AS PayDate, DATEDIFF(DAY, INV.[DueDate], CURRENT_TIMESTAMP ) AS DiffDate " +
                        "FROM[accountWa].[dbo].[Invoices] AS INV " +
                        "LEFT JOIN[accountWa].[dbo].[Payments] AS PAY " +
                        "ON PAY.[Invoice_Id] = INV.[Id] " +
                        "LEFT JOIN [accountWa].[dbo].Customers as CUS " +
                        "ON CUS.[Id] = INV.[Customer_Id] " +
                        "WHERE INV.[Company_Id] = @COMPANY_ID AND INV.[DueDate] >= @PERIOD " +
                        "ORDER BY [CustomerId], [Date]";

            try {
                using(var connection = _context.Database.GetDbConnection()) {
                    using(var command = connection.CreateCommand()) {
                        command.CommandText = query;
                        command.Parameters.Add(new SqlParameter("@COMPANY_ID", System.Data.SqlDbType.BigInt));
                        command.Parameters.Add(new SqlParameter("@PERIOD", System.Data.SqlDbType.Date));
                        command.Parameters["@COMPANY_ID"].Value = companyId;
                        command.Parameters["@PERIOD"].Value = period.AddDays(daysPerPeriod * numberOfPeriod * -1);

                        if(connection.State == System.Data.ConnectionState.Closed) {
                            await connection.OpenAsync();
                        }
                        var report = new ReportDto<AgingReportDataDto>() {
                            CompanyId = companyId,
                            Date = period,
                            DaysPerAgingPeriod = daysPerPeriod,
                            NumberOfPeriod = numberOfPeriod,
                            Datas = new List<AgingReportDataDto>()
                        };

                        using(var reader = await command.ExecuteReaderAsync()) {
                            while(reader.Read()) {
                                report.AddData(new AgingReportDataDto() {
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
                            }
                        }
                        return report;
                    }
                }
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }
            return null;
        }
    }
}
