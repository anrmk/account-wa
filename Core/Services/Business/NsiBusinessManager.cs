using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using AutoMapper;

using Core.Data.Dto.Nsi;
using Core.Data.Entities.Nsi;
using Core.Extension;
using Core.Services.Managers.Nsi;

namespace Core.Services.Business {
    public interface INsiBusinessManager {
        //Task<ReportPeriodDto> GetReportPeriodById(long id);
        //Task<List<ReportPeriodDto>> GetReportPeriods();
        //Task<List<NsiDto>> GetReportFields();
        Task<Pager<NsiDto>> GetReportFields(string search, string sort, string order, int offset = 0, int limit = 10);
        Task<List<NsiDto>> GetCustomerTypes();
    }
    public class NsiBusinessManager: BaseBusinessManager, INsiBusinessManager {
        private readonly IMapper _mapper;
        //private readonly IReportPeriodManager _reportPeriodManager;
        private readonly IReportFieldManager _reportFieldManager;
        private readonly ICustomerTypeManager _customerTypeManager;

        public NsiBusinessManager(IMapper mapper, IReportFieldManager reportFieldManager, ICustomerTypeManager customerTypeManager) {
            _mapper = mapper;
            //_reportPeriodManager = reportPeriodManager;
            _reportFieldManager = reportFieldManager;
            _customerTypeManager = customerTypeManager;
        }

        public async Task<List<NsiDto>> GetCustomerTypes() {
            var result = await _customerTypeManager.All();
            return _mapper.Map<List<NsiDto>>(result);
        }

        //public async Task<ReportPeriodDto> GetReportPeriodById(long id) {
        //    var result = await _reportPeriodManager.Find(id);
        //    return _mapper.Map<ReportPeriodDto>(result);
        //}

        //public async Task<List<ReportPeriodDto>> GetReportPeriods() {
        //    var result = await _reportPeriodManager.All();
        //    return _mapper.Map<List<ReportPeriodDto>>(result);
        //}

        public async Task<Pager<NsiDto>> GetReportFields(string search, string sort, string order, int offset, int limit) {
            Expression<Func<ReportFieldEntity, bool>> wherePredicate = x =>
                   (true)
                   && (x.Name.Contains(search) || x.Code.Contains(search));

            #region Sort
            //var sortby = GetExpression<ReportFieldEntity>(sort ?? "Name");
            var sortby = sort ?? "Name";
            #endregion

            Tuple<List<ReportFieldEntity>, int> tuple = await _reportFieldManager.Pager<ReportFieldEntity>(wherePredicate, sortby, offset, limit);
            var list = tuple.Item1;
            var count = tuple.Item2;

            if(count == 0)
                return new Pager<NsiDto>(new List<NsiDto>(), 0, offset, limit);

            var page = (offset + limit) / limit;

            var result = _mapper.Map<List<NsiDto>>(list);
            return new Pager<NsiDto>(result, count, page, limit);
        }
    }
}
