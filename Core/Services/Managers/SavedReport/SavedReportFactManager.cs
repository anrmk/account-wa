using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface ISavedReportFactManager: IEntityManager<SavedReportFactEntity> {
        Task<SavedReportFactEntity> FindInclude(long id);
        Task<SavedReportFactEntity> FindInclude(Guid userId, long companyId, DateTime date);
        Task<List<SavedReportFactEntity>> FindAllByUserId(Guid userId);
        Task<List<SavedReportFactEntity>> FindAllByUserAndCompanyId(Guid userId, long companyId);
    }

    public class SavedReportFactManager: AsyncEntityManager<SavedReportFactEntity>, ISavedReportFactManager {
        public SavedReportFactManager(IApplicationContext context) : base(context) { }

        public async Task<List<SavedReportFactEntity>> FindAllByUserId(Guid userId) {
            return await DbSet
                .Include(x => x.Company)
                .Where(x => x.ApplicationUserId.Equals(userId)).ToListAsync();
        }

        public async Task<List<SavedReportFactEntity>> FindAllByUserAndCompanyId(Guid userId, long companyId) {
            return await DbSet
                .Include(x => x.Fields)
                .Include(x => x.Files)
                .Where(x => x.ApplicationUserId.Equals(userId) && x.CompanyId == companyId)
                .OrderBy(x => x.Date)
                .ToListAsync();
        }

        public async Task<SavedReportFactEntity> FindInclude(long id) {
            return await DbSet
               .Include(x => x.Fields)
               .Include(x => x.Files)
               .Where(x => x.Id == id)
               .FirstOrDefaultAsync();
        }

        public async Task<SavedReportFactEntity> FindInclude(Guid userId, long companyId, DateTime date) {
            return await DbSet
               .Include(x => x.Fields)
               .Include(x => x.Files)
               .Where(x => x.ApplicationUserId.Equals(userId) && x.CompanyId == companyId && x.Date == date)
               .FirstOrDefaultAsync();
        }
    }
}
