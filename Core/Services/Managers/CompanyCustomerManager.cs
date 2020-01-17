using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface ICompanyCustomerManager: IEntityManager<CompanyCustomerEntity> {
        Task<List<CompanyCustomerEntity>> FindAll(long companyId);
        Task<List<CustomerEntity>> FindAllCustomers(long companyId);
        Task<List<CompanyEntity>> FindAllCompanies(long customerId);
    }

    public class CompanyCustomerManager: AsyncEntityManager<CompanyCustomerEntity>, ICompanyCustomerManager {
        public CompanyCustomerManager(IApplicationContext context) : base(context) { }

        public async Task<List<CustomerEntity>> FindAllCustomers(long companyId) {
            return await DbSet
                //.Include(x => x.Company)
                // .Include(x => x.Customer)
                .Where(x => x.CompanyId == companyId)
                .Select(x => x.Customer)
                .ToListAsync();
        }

        public async Task<List<CompanyEntity>> FindAllCompanies(long customerId) {
            return await DbSet
               //.Include(x => x.Company)
               // .Include(x => x.Customer)
               .Where(x => x.CustomerId == customerId)
               .Select(x => x.Company)
               .ToListAsync();
        }

        public async Task<List<CompanyCustomerEntity>> FindAll(long companyId) {
            return await DbSet.Where(x => x.CompanyId == companyId).ToListAsync();
        }
    }
}
