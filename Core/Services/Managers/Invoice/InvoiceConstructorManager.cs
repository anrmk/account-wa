using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {

    public interface IInvoiceConstructorManager: IEntityManager<InvoiceConstructorEntity> {
        Task<InvoiceConstructorEntity> FindInclude(long id);
        Task<List<InvoiceConstructorEntity>> FindAll(long companyId, DateTime date);
    }

    public class InvoiceConstructorManager: AsyncEntityManager<InvoiceConstructorEntity>, IInvoiceConstructorManager {
        public InvoiceConstructorManager(IApplicationContext context) : base(context) { }

        public async Task<List<InvoiceConstructorEntity>> FindAll(long companyId, DateTime date) {
            return await DbSet
                .Include(x => x.Invoices)
                .Where(x => x.CompanyId == companyId && x.Date == date)
                .ToListAsync();
        }

        public async Task<InvoiceConstructorEntity> FindInclude(long id) {
            return await DbSet
                .Include(x => x.Invoices)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}
