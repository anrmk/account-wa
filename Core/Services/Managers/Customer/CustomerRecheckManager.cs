using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface ICustomerRecheckManager: IEntityManager<CustomerRecheckEntity> {
        Task<List<CustomerRecheckEntity>> FindAllByCustomerId(long customerId);
    }

    public class CustomerRecheckManager: AsyncEntityManager<CustomerRecheckEntity>, ICustomerRecheckManager {
        public CustomerRecheckManager(IApplicationContext context) : base(context) { }

        public async Task<List<CustomerRecheckEntity>> FindAllByCustomerId(long customerId) {
            return await DbSet.Where(x => x.CustomerId == customerId)
                .OrderBy(x => x.CreatedDate)
                .ToListAsync();
        }
    }
}
