using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

namespace Core.Services.Managers {
    public interface IReportSearchCriteriaManager: IEntityManager<ReportSearchCriteriaEntity> {
    }

    public class ReportSearchCriteriaManager: AsyncEntityManager<ReportSearchCriteriaEntity>, IReportSearchCriteriaManager {
        public ReportSearchCriteriaManager(IApplicationContext context) : base(context) { }

    }
}
