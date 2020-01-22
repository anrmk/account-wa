using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface ICustomerManager: IEntityManager<CustomerEntity> {
        Task<CustomerEntity> FindInclude(long id);
        Task<List<CustomerEntity>> FindByIds(long[] ids);
        Task<List<CustomerEntity>> FindByCompanyId(long id);
        Task<List<CustomerEntity>> FindByCompanyId(long id, DateTime till);


        Task<List<CustomerEntity>> AllInclude();
        Task<List<CustomerEntity>> AllUntied(long? companyId);
    }

    public class CustomerManager: AsyncEntityManager<CustomerEntity>, ICustomerManager {
        public CustomerManager(IApplicationContext context) : base(context) { }

        public async Task<List<CustomerEntity>> AllInclude() {
            return await DbSet
                .Include(x => x.Address)
                .Include(x => x.Activities)
                .ToListAsync();
        }

        public async Task<List<CustomerEntity>> AllUntied(long? companyId) {
            return await DbSet.Where(x => x.CompanyId == null || x.CompanyId == companyId).ToListAsync();
        }

        public async Task<List<CustomerEntity>> FindByIds(long[] ids) {
            return await DbSet.Include(x => x.Address)
                .Include(x => x.Activities)
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();
        }

        public async Task<List<CustomerEntity>> FindByCompanyId(long id) {
            return await DbSet.Where(x => x.CompanyId == id).ToListAsync();
        }

        public async Task<List<CustomerEntity>> FindByCompanyId(long id, DateTime till) {
            var result = await DbSet
                .Where(x => x.CompanyId == id)
                .SelectMany(x => x.Activities.Where(b => b.IsActive == true && b.CreatedDate <= till).DefaultIfEmpty(),
                (customer, activity) => new { Customer = customer })
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
    }
}
