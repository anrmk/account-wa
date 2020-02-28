using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

namespace Core.Services.Managers {
    public interface ICompanyExportSettingsFieldManager: IEntityManager<CompanyExportSettingsFieldEntity> {
    }
    public class CompanyExportSettingsFieldManager: AsyncEntityManager<CompanyExportSettingsFieldEntity>, ICompanyExportSettingsFieldManager {
        public CompanyExportSettingsFieldManager(IApplicationContext context) : base(context) { }
    }
}
