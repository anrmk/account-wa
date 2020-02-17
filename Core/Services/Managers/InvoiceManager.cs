using System;
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
        Task<List<InvoiceEntity>> FindUnpaidByCompanyId(long companyId, DateTime from, DateTime to);
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

        //Get unpaid invoices by customer
        public async Task<List<InvoiceEntity>> FindUnpaidByCustomerId(long id) {
            return await DbSet
                .Include(x => x.Customer)
                .Include(x => x.Payment)
                .Where(x => x.CustomerId == id && x.Subtotal * (1 + x.TaxRate / 100) > x.Payment.Select(b => b.Amount).Sum() || x.Payment.Count == 0)
                .ToListAsync();
        }

        public async Task<List<InvoiceEntity>> FindUnpaidByCompanyId(long companyId, DateTime from, DateTime to) {
            var result1 = await DbSet
                .Include(x => x.Company)
                .Include(x => x.Customer)
                .Include(x => x.Payment)
                .Where(x => x.CompanyId == companyId)
                .ToListAsync();
            

            var result = await DbSet
                .Include(x => x.Company)
                .Include(x => x.Customer)
                .Include(x => x.Payment)
                .Where(x => x.CompanyId == companyId && (x.Subtotal * (1 + x.TaxRate / 100) > x.Payment.Select(b => b.Amount).Sum() || x.Payment.Count == 0))
                .ToListAsync();

            var serachPayment = result.Where(x => x.Payment.Count() > 0).ToList();
            var invoice = serachPayment.Where(x => x.Payment.Where(y => y.Id == 33810).Any()).FirstOrDefault();

            return result;
        }
    }
}
