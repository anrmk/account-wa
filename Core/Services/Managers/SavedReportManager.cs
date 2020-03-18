using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface ISavedReportManager: IEntityManager<SavedReportEntity> {
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
                .Where(x => x.ApplicationUserId == new System.Guid(userId) && x.CompanyId == companyId).ToListAsync();
        }
    }
}
