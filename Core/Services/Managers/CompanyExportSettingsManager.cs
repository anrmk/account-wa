using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface ICompanyExportSettingsManager: IEntityManager<CompanyExportSettingsEntity> {
        Task<CompanyExportSettingsEntity> FindInclude(long id);
        Task<List<CompanyExportSettingsEntity>> FindAllByCompanyId(long companyId);
    }
    public class CompanyExportSettingsManager: AsyncEntityManager<CompanyExportSettingsEntity>, ICompanyExportSettingsManager {
        public CompanyExportSettingsManager(IApplicationContext context) : base(context) { }

        public async Task<CompanyExportSettingsEntity> FindInclude(long id) {
            return await DbSet
                .Include(x => x.Fields)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<CompanyExportSettingsEntity>> FindAllByCompanyId(long companyId) {
            return await DbSet
                .Where(x => x.CompanyId == companyId).ToListAsync();
        }
    }
}