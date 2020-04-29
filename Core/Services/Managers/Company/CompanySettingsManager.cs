using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

namespace Core.Services.Managers {
    public interface ICompanySettingsManager: IEntityManager<CompanySettingsEntity> {
    }
    public class CompanySettingsManager: AsyncEntityManager<CompanySettingsEntity>, ICompanySettingsManager {
        public CompanySettingsManager(IApplicationContext context) : base(context) { }
    }
}
