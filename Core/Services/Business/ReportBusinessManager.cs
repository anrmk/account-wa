using System;
using System.Threading.Tasks;

using Core.Data.Dto;
using Core.Services.Managers;

namespace Core.Services.Business {
    public interface IReportBusinessManager {
        Task<ReportDto<AgingReportDataDto>> GetAgingReport(long companyId, DateTime period, int daysPerPeriod, int numberOfPeriod);
    }
    public class ReportBusinessManager: IReportBusinessManager {
        private readonly IReportManager _reportManager;
        public ReportBusinessManager(IReportManager reportManager, ICompanyManager customerManager) {
            _reportManager = reportManager;
        }

        public async Task<ReportDto<AgingReportDataDto>> GetAgingReport(long companyId, DateTime period, int daysPerPeriod, int numberOfPeriod) {
            var result = await _reportManager.GetAgingReport(companyId, period, daysPerPeriod, numberOfPeriod);



            return result;
        }

       // private class string[] PeriodName ()
    }
}
