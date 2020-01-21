using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface ICustomerActivityManager: IEntityManager<CustomerActivityEntity> {
        Task<List<CustomerActivityEntity>> FindByCustomerId(long customerId);
    }

    public class CustomerActivityManager: AsyncEntityManager<CustomerActivityEntity>, ICustomerActivityManager {
        public CustomerActivityManager(IApplicationContext context) : base(context) { }

        public async Task<List<CustomerActivityEntity>> FindByCustomerId(long customerId) {
            return await DbSet.Where(x => x.CustomerId == customerId).ToListAsync();
        }
    }
}
