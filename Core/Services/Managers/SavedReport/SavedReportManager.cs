using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface ISavedReportManager: IEntityManager<SavedReportEntity> {
        Task<SavedReportEntity> FindInclude(long id);
        Task<SavedReportEntity> FindInclude(Guid userId, long companyId, DateTime date);
        Task<List<SavedReportEntity>> FindAllByUserId(string userId);
        Task<List<SavedReportEntity>> FindAllByUserAndCompanyId(string userId, long companyId);
    }

    public class SavedReportManager: AsyncEntityManager<SavedReportEntity>, ISavedReportManager {
        public SavedReportManager(IApplicationContext context) : base(context) { }

        public async Task<List<SavedReportEntity>> FindAllByUserId(string userId) {
            return await DbSet
                .Where(x => x.ApplicationUserId == new System.Guid(userId)).ToListAsync();
        }

        public async Task<List<SavedReportEntity>> FindAllByUserAndCompanyId(string userId, long companyId) {
            return await DbSet
                .Include(x => x.Fields)
                .Include(x => x.Files)
                .Where(x => x.ApplicationUserId == new System.Guid(userId) && x.CompanyId == companyId).ToListAsync();
        }

        public async Task<SavedReportEntity> FindInclude(long id) {
            return await DbSet
               .Include(x => x.Fields)
               .Include(x => x.Files)
               .Where(x => x.Id == id)
               .FirstOrDefaultAsync();
        }

        public async Task<SavedReportEntity> FindInclude(Guid userId, long companyId, DateTime date) {
            return await DbSet
               .Include(x => x.Fields)
               .Include(x => x.Files)
               .Where(x => x.ApplicationUserId == userId && x.CompanyId == companyId && x.Date == date)
               .FirstOrDefaultAsync();
        }
    }
}
