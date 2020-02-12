using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface ICompanySummaryRangeManager: IEntityManager<CompanySummaryRangeEntity> {
        Task<CompanySummaryRangeEntity> FindInclude(long id);
        Task<List<CompanySummaryRangeEntity>> AllInclude(long companyId);
    }

    public class CompanySummaryRangeManager: AsyncEntityManager<CompanySummaryRangeEntity>, ICompanySummaryRangeManager {
        public CompanySummaryRangeManager(IApplicationContext context) : base(context) { }

        public async Task<List<CompanySummaryRangeEntity>> AllInclude(long companyId) {
            return await DbSet.Include(x => x.Company)
                .Where(x => x.CompanyId == companyId).ToListAsync();
        }

        public async Task<CompanySummaryRangeEntity> FindInclude(long id) {
            return await DbSet.Include(x => x.Company)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
        }
    }
}
