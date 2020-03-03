using Core.Context;
using Core.Data.Entities.Nsi;
using Core.Services.Base;

namespace Core.Services.Managers.Nsi {
    public interface IReportFieldManager: IEntityManager<ReportFieldEntity> {

    }

    public class ReportFieldManager: AsyncEntityManager<ReportFieldEntity>, IReportFieldManager {
        public ReportFieldManager(IApplicationContext context) : base(context) { }
    }
}