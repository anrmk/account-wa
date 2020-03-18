using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

namespace Core.Services.Managers {
    public interface ISavedReportFieldManager: IEntityManager<SavedReportFieldEntity> {
    }

    public class SavedReportFieldManager: AsyncEntityManager<SavedReportFieldEntity>, ISavedReportFieldManager {
        public SavedReportFieldManager(IApplicationContext context) : base(context) { }
    }
}
