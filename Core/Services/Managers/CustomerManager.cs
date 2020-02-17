using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface ICustomerManager: IEntityManager<CustomerEntity> {
        Task<CustomerEntity> FindInclude(long id);
        Task<List<CustomerEntity>> FindByIds(long[] ids);
        Task<List<CustomerEntity>> FindByCompanyId(long id);
        Task<List<CustomerEntity>> FindByCompanyId(long id, DateTime till);

        Task<List<CustomerEntity>> AllInclude();
        Task<List<CustomerEntity>> FindUntied(long? companyId);

        Task<List<CustomerBulkEntity>> FindBulks(long companyId, DateTime from, DateTime to);
    }

    public class CustomerManager: AsyncEntityManager<CustomerEntity>, ICustomerManager {
        public CustomerManager(IApplicationContext context) : base(context) { }

        public async Task<List<CustomerEntity>> AllInclude() {
            return await DbSet
                .Include(x => x.Address)
                .Include(x => x.Activities)
                .ToListAsync();
        }

        public async Task<List<CustomerEntity>> FindUntied(long? companyId) {
            return await DbSet.Where(x => x.CompanyId == null || x.CompanyId == companyId).ToListAsync();
        }

        public async Task<List<CustomerEntity>> FindByIds(long[] ids) {
            return await DbSet.Include(x => x.Address)
                .Include(x => x.Activities)
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();
        }

        public async Task<List<CustomerEntity>> FindByCompanyId(long id) {
            return await DbSet
                .Include(x => x.Address)
                .Where(x => x.CompanyId == id).ToListAsync();
        }

        public async Task<List<CustomerEntity>> FindByCompanyId(long id, DateTime till) {
            var result = await DbSet
                .Where(x => x.CompanyId == id)
                .SelectMany(x => x.Activities.Where(b => b.IsActive == true && b.CreatedDate <= till),
                (customer, activity) => new { Customer = customer }).Distinct()

                .Select(p => p.Customer).ToListAsync();

            return result;
        }

        public async Task<CustomerEntity> FindInclude(long id) {
            return await DbSet
                .Include(x => x.Address)
                .Include(x => x.Activities)
               .Where(x => x.Id == id)
               .FirstOrDefaultAsync();
        }

        public async Task<List<CustomerBulkEntity>> FindBulks(long companyId, DateTime from, DateTime to) {
            var context = (ApplicationContext)_context;
            var result = new List<CustomerBulkEntity>();
            var query = "SELECT CUS.[Id], INV.[Total], CUS.[AccountNumber], CUS.[Name], CUS.[Description], CUS.[Terms], CUS.[CreditLimit], CUS.[CreditUtilized], CUS.[Company_Id] " +
                                                    "FROM[dbo].[Customers] AS CUS " +
                                                    "LEFT JOIN(SELECT Customer_Id, COUNT(*) AS[Total] FROM [dbo].[Invoices] " +
                                                    "WHERE [Date] > @DATE_FROM AND [DATE] <= @DATE_TO " +
                                                    "group by[Customer_Id]) AS INV " +
                                                    "ON CUS.[Id] = INV.[Customer_Id] " +
                                                    "WHERE CUS.[Company_Id] = @COMPANY_ID";

            try {
                using(var connection = context.Database.GetDbConnection()) {
                    using(var command = connection.CreateCommand()) {
                        command.CommandText = query;
                        command.Parameters.Add(new SqlParameter("@COMPANY_ID", System.Data.SqlDbType.BigInt));
                        command.Parameters.Add(new SqlParameter("@DATE_FROM", System.Data.SqlDbType.Date));
                        command.Parameters.Add(new SqlParameter("@DATE_TO", System.Data.SqlDbType.Date));
                        command.Parameters["@COMPANY_ID"].Value = companyId;
                        command.Parameters["@DATE_FROM"].Value = from;
                        command.Parameters["@DATE_TO"].Value = to;

                        if(connection.State == System.Data.ConnectionState.Closed) {
                            await connection.OpenAsync();
                        }

                        using(var reader = await command.ExecuteReaderAsync()) {
                            while(reader.Read()) {
                                result.Add(new CustomerBulkEntity() {
                                    Id = (long)reader["Id"],
                                    Total = reader["Total"] != DBNull.Value ? (int)reader["Total"] : 0,
                                    AccountNumber = reader["AccountNumber"] as string,
                                    Name = reader["Name"] as string,
                                    Description = reader["Description"] as string,
                                    Terms = reader["Terms"] as string,
                                    //CreditLimit = (double)reader["CreditLimit"],
                                    //CreditUtilized = (double)reader["CreditUtilized"],
                                    CompanyId = (long)reader["Company_Id"]
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
    }
}
