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
        Task<CustomerEntity> FindInclude(string no, long companyId);
        Task<List<CustomerEntity>> FindByIds(long[] ids);
        Task<List<CustomerEntity>> FindByCompanyId(long id);
        Task<List<CustomerEntity>> FindByCompanyId(long id, DateTime till);

        Task<List<CustomerEntity>> AllInclude();
        Task<List<CustomerEntity>> FindUntied(long? companyId);

        Task<List<CustomerEntity>> FindBulks(long companyId, DateTime from, DateTime to);
    }

    public class CustomerManager: AsyncEntityManager<CustomerEntity>, ICustomerManager {
        public CustomerManager(IApplicationContext context) : base(context) { }

        public async Task<CustomerEntity> FindInclude(long id) {
            return await DbSet
                .Include(x => x.Company)
                .Include(x => x.Address)
                .Include(x => x.Activities)
                .Include(x => x.TagLinks)
                    .ThenInclude(x => x.Tag)
                .Include(x => x.Type)
               .Where(x => x.Id == id)
               .FirstOrDefaultAsync();
        }

        public async Task<CustomerEntity> FindInclude(string no, long companyId) {
            return await DbSet
               .Include(x => x.Company)
               .Include(x => x.Address)
               .Include(x => x.Activities)
               .Include(x => x.Type)
              .Where(x => x.No.Equals(no) && x.CompanyId == companyId)
              .FirstOrDefaultAsync();
        }
        public async Task<List<CustomerEntity>> AllInclude() {
            return await DbSet
                .Include(x => x.Address)
                .Include(x => x.Activities)
                .Include(x => x.Type)
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

        [Obsolete]
        public async Task<List<CustomerEntity>> FindByCompanyId(long id, DateTime till) {
            var result = await DbSet
                .Include(x => x.Address)
                .Include(x => x.Activities)
                .Include(x => x.Type)
                .Include(x => x.CreditLimits)
                .Include(x => x.CreditUtilizeds)
                .Where(x => x.CompanyId == id)
                    .SelectMany(x => x.Activities.Where(b => b.IsActive == true && b.CreatedDate <= till), (customer, activity) => new { Customer = customer }).Distinct()
                    .Select(p => p.Customer).ToListAsync();

            //Select only one limit in list matching condition
            result.ForEach(x => x.CreditLimits = x.CreditLimits.Where(y => y.CreatedDate <= till).OrderByDescending(z => z.CreatedDate).Take(1).ToList());
            result.ForEach(x => x.CreditUtilizeds = x.CreditUtilizeds.Where(y => y.CreatedDate <= till).OrderByDescending(z => z.CreatedDate).Take(1).ToList());


            return result;
        }

        public async Task<List<CustomerEntity>> FindBulks(long companyId, DateTime from, DateTime to) {
            var context = (ApplicationContext)_context;
            var result = new List<CustomerEntity>();
            var query = "SELECT CUS.[Id], CUS.[AccountNumber] AS No, CUS.[Name], CUS.[Description], CUS.[Terms], CUS.[Company_Id], CUS.[CustomerType_Id], CUS.[CreatedDate], CUS.[CreatedBy], " +
                            "CUST.[Name] AS CustomerTypeName, CUST.[Code] AS CustomerTypeCode, " +
                            "CACT.[ActiveCreatedDate], CACT.[TotalActive], " +
                            "RCH.[Recheck], INV.[TotalInvoices], INV2.[TotalUnpaidInvoices], " +
                            "TAGS.[TagLinkIds], TAGS.[TagIds], TAGS.[TagNames] " +
                        "FROM [accountWa].[dbo].[Customers] AS CUS " +
                        "LEFT JOIN (SELECT Customer_Id, Count(IsActive) AS TotalActive, MIN([CreatedDate]) AS ActiveCreatedDate " +
                                "FROM [accountWa].[dbo].[CustomerActivities] " +
                                "GROUP BY [Customer_Id]) AS CACT " +
                            "ON CACT.[Customer_Id] = CUS.[Id] " +

                        //GET TOTAL INVOICES COUNT
                        "LEFT JOIN (SELECT Customer_Id, COUNT(*) AS [TotalInvoices] FROM [accountWa].[dbo].[Invoices] AS I " +
                                "WHERE I.[Date] >= @DATEFROM AND I.[DATE] <= @DATETO GROUP BY I.[Customer_Id]) AS INV " +
                            "ON CUS.[Id] = INV.[Customer_Id] " +

                        //GET UNPAID INVOICES COUNT
                        "LEFT JOIN (SELECT Customer_Id, COUNT(*) AS [TotalUnpaidInvoices] FROM [accountWa].[dbo].[Invoices] AS I " +
                                "LEFT JOIN (SELECT [Invoice_Id], SUM(Amount) AS [PaymentAmount], COUNT(*) AS [PaymentCount] FROM [accountWa].[dbo].[Payments] WHERE [Date] <= @DATETO GROUP BY [Invoice_Id]) AS P " +
                                "ON I.[Id] = P.[Invoice_Id] AND I.[Subtotal] = P.[PaymentAmount] " +
                                "WHERE I.[Date] < @DATEFROM AND P.[PaymentCount] IS NULL GROUP BY I.[Customer_Id]) AS INV2 " +
                            "ON CUS.[Id] = INV2.[Customer_Id] " +
                        //GET CUSTOMER TYPES
                        "LEFT JOIN (SELECT * FROM [accountWa].[dbo].[nsi.CustomerType]) AS CUST " +
                            "ON CUS.[CustomerType_Id] = CUST.[Id] " +
                        //GET RECHECK
                        "LEFT JOIN (SELECT COUNT(*) AS [Recheck], [Customer_Id] FROM [accountWa].[dbo].[CustomerRechecks] GROUP BY [Customer_Id]) AS RCH " +
                            "ON CUS.[Id] = RCH.[Customer_Id] " +
                        "OUTER APPLY (SELECT STRING_AGG(CTL.[Id], ',') AS TagLinkIds, STRING_AGG(CT.[Id], ',') AS TagIds, STRING_AGG(CT.[Name], ',') AS TagNames FROM [accountWa].[dbo].[CustomerTags] AS CT " +
                            "LEFT JOIN [accountWa].[dbo].[CustomerTagLinks] AS CTL " +
                            "ON CT.[Id] = CTL.[CustomerTag_Id] WHERE CTL.[Customer_Id] = CUS.[Id]) AS TAGS " +
                        "WHERE CUS.[Company_Id] = @COMPANYID ";
            try {
                using(var connection = context.Database.GetDbConnection()) {
                    using(var command = connection.CreateCommand()) {
                        command.CommandText = query;
                        command.Parameters.Add(new SqlParameter("@COMPANYID", System.Data.SqlDbType.BigInt));
                        command.Parameters.Add(new SqlParameter("@DATEFROM", System.Data.SqlDbType.Date));
                        command.Parameters.Add(new SqlParameter("@DATETO", System.Data.SqlDbType.Date));
                        command.Parameters["@COMPANYID"].Value = companyId;
                        command.Parameters["@DATEFROM"].Value = from;
                        command.Parameters["@DATETO"].Value = to;

                        if(connection.State == System.Data.ConnectionState.Closed) {
                            await connection.OpenAsync();
                        }

                        using(var reader = await command.ExecuteReaderAsync()) {
                            while(reader.Read()) {

                                var taglinks = new List<CustomerTagLinkEntity>();
                                if(reader["TagLinkIds"] != DBNull.Value && reader["TagIds"] != DBNull.Value && reader["TagNames"] != DBNull.Value) {
                                    var tagLinksIds = reader["TagLinkIds"] as string;
                                    var tagIds = reader["TagIds"] as string;
                                    var tagNames = reader["TagNames"] as string;

                                    var tags = tagIds.Split(',').Zip(tagNames.Split(','), (id, name) => new CustomerTagEntity() {
                                        Id = long.Parse(id),
                                        Name = name
                                    });

                                    taglinks = tagLinksIds.Split(',').Zip(tags, (id, tag) => new CustomerTagLinkEntity() {
                                        Id = long.Parse(id),
                                        CustomerId = (long)reader["Id"],
                                        Tag = tag,
                                        TagId = tag.Id
                                    }).ToList();
                                }

                                var customerType = reader["CustomerType_Id"] != DBNull.Value ? new Data.Entities.Nsi.CustomerTypeEntity() {
                                    Id = (long)reader["CustomerType_Id"],
                                    Code = reader["CustomerTypeCode"] as string,
                                    Name = reader["CustomerTypeName"] as string
                                } : null;

                                result.Add(new CustomerEntity() {
                                    Id = (long)reader["Id"],
                                    TotalInvoices = reader["TotalInvoices"] != DBNull.Value ? (int)reader["TotalInvoices"] : 0,
                                    UnpaidInvoices = reader["TotalUnpaidInvoices"] != DBNull.Value ? (int)reader["TotalUnpaidInvoices"] : 0,
                                    No = reader["No"] as string,
                                    Name = reader["Name"] as string,
                                    Description = reader["Description"] as string,
                                    Terms = reader["Terms"] as string,
                                    CompanyId = (long)reader["Company_Id"],
                                    TypeId = customerType != null ? customerType.Id : (long?)null,
                                    Type = customerType,
                                    Recheck = reader["Recheck"] != DBNull.Value ? (int)reader["Recheck"] : 0,
                                    TagLinks = taglinks,
                                    CreatedDate = reader["ActiveCreatedDate"] != DBNull.Value ? (DateTime)reader["ActiveCreatedDate"] : (DateTime)reader["ActCreatedDate"]
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
