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
        Task<List<CustomerEntity>> FindAllInclude(long[] ids);
        //Task<List<CustomerEntity>> FindByCompanyId(long id);
        Task<List<CustomerEntity>> AllInclude();
    }

    public class CustomerManager: AsyncEntityManager<CustomerEntity>, ICustomerManager {
        public CustomerManager(IApplicationContext context) : base(context) { }

        public async Task<List<CustomerEntity>> AllInclude() {
            return await DbSet.Include(x => x.Address).ToListAsync();
        }

        public async Task<List<CustomerEntity>> FindAllInclude(long[] ids) {
            return await DbSet.Include(x => x.Address)
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();
        }

        //public async Task<List<CustomerEntity>> FindByCompanyId(long id) {
        //    return await DbSet.Where(x => x.Company.Contains(id)).ToListAsync();
        //}

        public async Task<CustomerEntity> FindInclude(long id) {
            return await DbSet.Include(x => x.Address)
               .Where(x => x.Id == id)
               .FirstOrDefaultAsync();
        }


    }
}
