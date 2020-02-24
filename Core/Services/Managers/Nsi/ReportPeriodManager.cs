using Core.Context;
using Core.Data.Entities.Nsi;
using Core.Services.Base;

namespace Core.Services.Managers.Nsi {
    public interface IReportPeriodManager: IEntityManager<ReportPeriodEntity> {

    }

    public class ReportPeriodManager: AsyncEntityManager<ReportPeriodEntity>, IReportPeriodManager {
        public ReportPeriodManager(IApplicationContext context) : base(context) { }
    }
}
