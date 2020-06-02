using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface IInvoiceDraftManager: IEntityManager<InvoiceDraftEntity> {
        Task<List<InvoiceDraftEntity>> FindByConstructorId(long constructorId);
        Task<List<InvoiceDraftEntity>> FindByConstructorId(long[] constructorIds);
    }

    public class InvoiceDraftManager: AsyncEntityManager<InvoiceDraftEntity>, IInvoiceDraftManager {
        public InvoiceDraftManager(IApplicationContext context) : base(context) { }

        public async Task<List<InvoiceDraftEntity>> FindByConstructorId(long constructorId) {
            return await DbSet.Where(x => x.ConstructorId == constructorId).ToListAsync();
        }

        public async Task<List<InvoiceDraftEntity>> FindByConstructorId(long[] constructorIds) {
            return await DbSet.Where(x => constructorIds.Contains(x.ConstructorId ?? 0)).ToListAsync();
        }
    }
}
