using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

namespace Core.Services.Managers {
    public interface ISavedReportPlanFieldManager: IEntityManager<SavedReportPlanFieldEntity> {
    }

    public class SavedReportPlanFieldManager: AsyncEntityManager<SavedReportPlanFieldEntity>, ISavedReportPlanFieldManager {
        public SavedReportPlanFieldManager(IApplicationContext context) : base(context) { }
    }
}
