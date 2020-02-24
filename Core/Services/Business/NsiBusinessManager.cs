using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;
using Core.Data.Dto.Nsi;
using Core.Services.Managers.Nsi;

namespace Core.Services.Business {
    public interface INsiBusinessManager {
        Task<ReportPeriodDto> GetReportPeriodById(long id);
        Task<List<ReportPeriodDto>> GetReportPeriods();
    }
    public class NsiBusinessManager: INsiBusinessManager {
        private readonly IMapper _mapper;
        private readonly IReportPeriodManager _reportPeriodManager;

        public NsiBusinessManager(IMapper mapper, IReportPeriodManager reportPeriodManager) {
            _mapper = mapper;
            _reportPeriodManager = reportPeriodManager;
        }

        public async Task<ReportPeriodDto> GetReportPeriodById(long id) {
            var result = await _reportPeriodManager.Find(id);
            return _mapper.Map<ReportPeriodDto>(result);
        }

        public async Task<List<ReportPeriodDto>> GetReportPeriods() {
            var result = await _reportPeriodManager.All();
            return _mapper.Map<List<ReportPeriodDto>>(result);
        }
    }
}
