using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Context;
using Core.Extension;
using Core.Services.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class SettingsController: BaseController<SettingsController> {

        public SettingsController(ILogger<SettingsController> logger, IMapper mapper, ApplicationContext context) : base(logger, mapper, context) {
        }

        public IActionResult ExportField() {
            return View();
        }
    }
}

namespace Web.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController: ControllerBase {
        private readonly IMapper _mapper;
        private readonly INsiBusinessManager _nsiBusinessManager;

        public SettingsController(IMapper mapper, INsiBusinessManager nsiBusinessManager) {
            _mapper = mapper;
            _nsiBusinessManager = nsiBusinessManager;
        }

        [HttpGet]
        [Route("exportfield")]
        public async Task<Pager<NsiViewModel>> GenerateBulkInvoice([FromQuery] PagerFilterViewModel model) {
            var result = await _nsiBusinessManager.GetReportFields(model.Search, model.Sort, model.Order, model.Offset, model.Limit);
            return new Pager<NsiViewModel>(_mapper.Map<List<NsiViewModel>>(result.Items), result.TotalItems, result.CurrentPage, result.PageSize);
        }


        //public async Task<Pager<InvoiceListViewModel>> GetInvoices([FromQuery] InvoiceFilterViewModel model) {
        //    var result = await _businessManager.GetInvoicePage(_mapper.Map<InvoiceFilterDto>(model));
        //    return new Pager<InvoiceListViewModel>(_mapper.Map<List<InvoiceListViewModel>>(result.Items), result.TotalItems, result.CurrentPage, result.PageSize);
        //}
    }
}