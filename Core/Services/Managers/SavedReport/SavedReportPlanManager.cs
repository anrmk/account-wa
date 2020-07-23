using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface ISavedReportPlanManager: IEntityManager<SavedReportPlanEntity> {
        Task<SavedReportPlanEntity> FindInclude(long id);
        Task<SavedReportPlanEntity> FindInclude(Guid userId, long companyId, DateTime date);
        Task<List<SavedReportPlanEntity>> FindAllByUserId(Guid userId);
        Task<List<SavedReportPlanEntity>> FindAllByUserAndCompanyId(Guid userId, long companyId);
    }

    public class SavedReportPlanManager: AsyncEntityManager<SavedReportPlanEntity>, ISavedReportPlanManager {
        public SavedReportPlanManager(IApplicationContext context) : base(context) { }

        public async Task<SavedReportPlanEntity> FindInclude(long id) {
            return await DbSet
               .Include(x => x.Fields)
               .Where(x => x.Id == id)
               .FirstOrDefaultAsync();
        }

        public async Task<SavedReportPlanEntity> FindInclude(Guid userId, long companyId, DateTime date) {
            return await DbSet
               .Include(x => x.Fields)
               .Where(x => x.ApplicationUserId == userId && x.CompanyId == companyId && x.Date == date)
               .FirstOrDefaultAsync();
        }

        public async Task<List<SavedReportPlanEntity>> FindAllByUserId(Guid userId) {
            return await DbSet
                .Where(x => x.ApplicationUserId == userId).ToListAsync();
        }

        public async Task<List<SavedReportPlanEntity>> FindAllByUserAndCompanyId(Guid userId, long companyId) {
            return await DbSet
               .Include(x => x.Fields)
               .Where(x => x.ApplicationUserId == userId && x.CompanyId == companyId)
               .OrderBy(x => x.Date)
               .ToListAsync();
        }
    }
}
