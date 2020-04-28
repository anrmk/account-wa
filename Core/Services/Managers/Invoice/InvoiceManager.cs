using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface IInvoiceManager: IEntityManager<InvoiceEntity> {
        Task<InvoiceEntity> FindInclude(long id);
        Task<List<InvoiceEntity>> FindByIds(long[] ids);
        Task<List<InvoiceEntity>> AllInclude();
        Task<List<InvoiceEntity>> FindUnpaidByCustomerId(long id);
        Task<List<InvoiceEntity>> FindUnpaidByCompanyId(long companyId, DateTime from, DateTime to);
        Task<decimal> GetTotalAmount(Expression<Func<InvoiceEntity, bool>> where);
    }

    public class InvoiceManager: AsyncEntityManager<InvoiceEntity>, IInvoiceManager {
        public InvoiceManager(IApplicationContext context) : base(context) { }

        public async Task<List<InvoiceEntity>> AllInclude() {
            return await DbSet.Include(x => x.Company)
                .Include(x => x.Customer)
                .Include(x => x.Payments)
                .ToListAsync();
        }

        public async Task<InvoiceEntity> FindInclude(long id) {
            return await DbSet.Include(x => x.Company)
                .Include(x => x.Customer)
                .Include(x => x.Payments)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<InvoiceEntity>> FindByIds(long[] ids) {
            return await DbSet.Include(x => x.Company)
                .Include(x => x.Customer)
                .Include(x => x.Payments)
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();
        }

        //Get unpaid invoices by customer
        public async Task<List<InvoiceEntity>> FindUnpaidByCustomerId(long id) {
            return await DbSet
                .Include(x => x.Customer)
                .Include(x => x.Payments)
                .Where(x => x.CustomerId == id && x.Subtotal * (1 + x.TaxRate / 100) > x.Payments.Select(b => b.Amount).Sum() || x.Payments.Count == 0)
                .ToListAsync();
        }

        public async Task<List<InvoiceEntity>> FindUnpaidByCompanyId(long companyId, DateTime from, DateTime to) {
            var result = await DbSet
                .Include(x => x.Company)
                .Include(x => x.Customer)
                .Include(x => x.Payments)
                .Where(x => x.CompanyId == companyId && (x.Subtotal * (1 + x.TaxRate / 100) > x.Payments.Select(b => b.Amount).Sum() || x.Payments.Count == 0))
                .ToListAsync();

            var oplacheniy = result.Where(x => x.Id == 14474).FirstOrDefault();

            var serachPayment = result.Where(x => x.Payments.Count() > 0).ToList();
            var invoice = serachPayment.Where(x => x.Payments.Where(y => y.Id == 14474).Any()).FirstOrDefault();

            return result;
        }

        public async Task<decimal> GetTotalAmount(Expression<Func<InvoiceEntity, bool>> where) {
            return await DbSet
                .Where(where)
                .SumAsync(x => x.Subtotal);
        }
    }
}
