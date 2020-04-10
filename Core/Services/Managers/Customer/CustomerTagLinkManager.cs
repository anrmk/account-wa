using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface ICustomerTagLinkManager: IEntityManager<CustomerTagLinkEntity> {
        Task<List<CustomerTagLinkEntity>> FindByCustomerId(long id);
    }

    public class CustomerTagLinkManager: AsyncEntityManager<CustomerTagLinkEntity>, ICustomerTagLinkManager {
        public CustomerTagLinkManager(IApplicationContext context) : base(context) { }

        public async Task<List<CustomerTagLinkEntity>> FindByCustomerId(long id) {
            return await DbSet
                .Include(x => x.Tag)
                .Include(x => x.Customer)
                .Where(x => x.CustomerId == id).ToListAsync();
        }
    }
}
