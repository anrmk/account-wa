using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

namespace Core.Services.Managers {
    public interface ISavedReportFileManager: IEntityManager<SavedReportFileEntity> {
    }

    public class SavedReportFileManager: AsyncEntityManager<SavedReportFileEntity>, ISavedReportFileManager {
        public SavedReportFileManager(IApplicationContext context) : base(context) { }
    }
}
