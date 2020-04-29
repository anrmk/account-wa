using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface ICustomerCreditLimitManager: IEntityManager<CustomerCreditLimitEntity> {
        Task<CustomerCreditLimitEntity> FindInclude(long id);
        Task<List<CustomerCreditLimitEntity>> FindAllByCustomerId(long customerId);
        Task<CustomerCreditLimitEntity> FindByCustomerIdAndDate(long customerId, DateTime date);
    }

    public class CustomerCreditLimitManager: AsyncEntityManager<CustomerCreditLimitEntity>, ICustomerCreditLimitManager {
        public CustomerCreditLimitManager(IApplicationContext context) : base(context) { }

        public async Task<List<CustomerCreditLimitEntity>> FindAllByCustomerId(long customerId) {
            return await DbSet.Where(x => x.CustomerId == customerId)
                .OrderBy(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<CustomerCreditLimitEntity> FindByCustomerIdAndDate(long customerId, DateTime date) {
            return await DbSet.Where(x => x.CustomerId == customerId && x.CreatedDate == date).FirstOrDefaultAsync();
        }

        public async Task<CustomerCreditLimitEntity> FindInclude(long id) {
            return await DbSet
                .Include(x => x.Customer)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
        }
    }
}
