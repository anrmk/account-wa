using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface IPaymentManager: IEntityManager<PaymentEntity> {
        Task<PaymentEntity> FindInclude(long Id);
        Task<ICollection<PaymentEntity>> FindByInvoiceId(long Id);
        Task<ICollection<PaymentEntity>> FindAllInclude();
        //  Task<PaymentEntity> PaydInvoices();
    }

    public class PaymentManager: AsyncEntityManager<PaymentEntity>, IPaymentManager {
        public PaymentManager(IApplicationContext context) : base(context) { }

        public async Task<PaymentEntity> FindInclude(long Id) {
            return await DbSet
                .Include(x => x.Invoice)
                .Where(x => x.Id == Id).SingleOrDefaultAsync();
        }

        public async Task<ICollection<PaymentEntity>> FindAllInclude() {
            return await DbSet.Include(x => x.Invoice)
                .ToListAsync();
        }

        public async Task<ICollection<PaymentEntity>> FindByInvoiceId(long Id) {
            return await DbSet
                .Where(x => x.InvoiceId == Id)
                .ToListAsync();
        }
    }
}
