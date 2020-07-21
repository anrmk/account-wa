using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

namespace Core.Services.Managers {
    public interface ISavedReportPlanManager: IEntityManager<SavedReportPlanEntity> {
    }

    public class SavedReportPlanManager: AsyncEntityManager<SavedReportPlanEntity>, ISavedReportPlanManager {
        public SavedReportPlanManager(IApplicationContext context) : base(context) { }
    }
}
