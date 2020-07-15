using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface ICustomerCreditUtilizedManager: IEntityManager<CustomerCreditUtilizedEntity> {
        Task<List<CustomerCreditUtilizedEntity>> FindAllByCustomerId(long customerId);
        Task<CustomerCreditUtilizedEntity> FindInclude(long id);
        Task<List<CustomerCreditUtilizedEntity>> FindInclude(long[] ids);
        Task<CustomerCreditUtilizedEntity> FindByCustomerIdAndDate(long customerId, DateTime date);
        Task<List<CustomerCreditUtilizedEntity>> FindByCompanyIdAndDate(long companyId, DateTime date);
    }

    public class CustomerCreditUtilizedManager: AsyncEntityManager<CustomerCreditUtilizedEntity>, ICustomerCreditUtilizedManager {
        public CustomerCreditUtilizedManager(IApplicationContext context) : base(context) { }

        public async Task<List<CustomerCreditUtilizedEntity>> FindAllByCustomerId(long customerId) {
            return await DbSet
                .Include(x => x.Customer)
                .Where(x => x.CustomerId == customerId).ToListAsync();
        }

        public async Task<CustomerCreditUtilizedEntity> FindByCustomerIdAndDate(long customerId, DateTime date) {
            return await DbSet.Where(x => x.CustomerId == customerId && x.CreatedDate == date).FirstOrDefaultAsync();
        }



        public async Task<List<CustomerCreditUtilizedEntity>> FindByCompanyIdAndDate(long companyId, DateTime date) {
            return await DbSet
              .Include(x => x.Customer)
              .Where(x => x.Customer.CompanyId == companyId && x.CreatedDate <= date)
              .ToListAsync();
        }

        public async Task<CustomerCreditUtilizedEntity> FindInclude(long id) {
            return await DbSet
                .Include(x => x.Customer)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<CustomerCreditUtilizedEntity>> FindInclude(long[] ids) {
            return await DbSet
               .Include(x => x.Customer)
               .Where(x => ids.Contains(x.Id)).ToListAsync();
        }
    }
}

