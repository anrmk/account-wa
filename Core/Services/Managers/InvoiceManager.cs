using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface IInvoiceManager: IEntityManager<InvoiceEntity> {
        Task<InvoiceEntity> FindInclude(long id);
        Task<List<InvoiceEntity>> AllInclude();
        Task<List<InvoiceEntity>> FindUnpaidByCustomerId(long id);
    }

    public class InvoiceManager: AsyncEntityManager<InvoiceEntity>, IInvoiceManager {
        public InvoiceManager(IApplicationContext context) : base(context) { }

        public async Task<List<InvoiceEntity>> AllInclude() {
            return await DbSet.Include(x => x.Company)
                .Include(x => x.Customer)
                .Include(x => x.Payment)
                .ToListAsync();
        }

        public async Task<InvoiceEntity> FindInclude(long id) {
            return await DbSet.Include(x => x.Company)
                .Include(x => x.Customer)
                .Include(x => x.Payment)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<InvoiceEntity>> FindUnpaidByCustomerId(long id) {
            return await DbSet.Include(x => x.Customer)
                .Include(x => x.Payment)
                .Where(x => x.CustomerId == id &&
                       x.Subtotal * (1 + x.TaxRate / 100) > x.Payment.Select(b => b.Amount).Sum() || x.Payment.Count == 0)
                .ToListAsync();
        }
    }
}
