using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

namespace Core.Services.Managers {
    public interface ISavedReportFieldManager: IEntityManager<SavedReportFactFieldEntity> {
    }

    public class SavedReportFieldManager: AsyncEntityManager<SavedReportFactFieldEntity>, ISavedReportFieldManager {
        public SavedReportFieldManager(IApplicationContext context) : base(context) { }
    }
}
