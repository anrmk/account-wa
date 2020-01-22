using System;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Core.Context;
using Core.Services.Business;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

using Web.Extension;
using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class ReportController: BaseController<ReportController> {
        public ICrudBusinessManager _crudBusinessManager;
        public IReportBusinessManager _businessManager;
        public ReportController(ILogger<ReportController> logger, IMapper mapper, ApplicationContext context,
            ICrudBusinessManager crudBusinessManager, IReportBusinessManager businessManager) : base(logger, mapper, context) {
            _crudBusinessManager = crudBusinessManager;
            _businessManager = businessManager;
        }

        public async Task<IActionResult> Index() {
            var companies = await _crudBusinessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(new ReportViewModel() {

            });
        }
    }
}

namespace Web.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController: ControllerBase {
        private readonly IMapper _mapper;
        private readonly IViewRenderService _viewRenderService;
        private readonly IReportBusinessManager _businessManager;
        public ICrudBusinessManager _crudBusinessManager;

        public ReportController(IMapper mapper, IViewRenderService viewRenderService,
            ICrudBusinessManager crudBusinessManager,
            IReportBusinessManager businessManager) {
            _mapper = mapper;
            _viewRenderService = viewRenderService;
            _businessManager = businessManager;
            _crudBusinessManager = crudBusinessManager;
        }

        //[HttpGet]
        //public async Task<string> GetString(long id) {
        //    return await Task.Run(() => { return $"HelLLO {id}"; });
        //}

        [HttpPost("aging", Name = "Aging")]
        public async Task<IActionResult> PostRunAgingReport(ReportViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var result = await _businessManager.GetAgingReport(model.CompanyId ?? 0, model.Date, model.DaysPerPeriod, model.NumberOfPeriod);
                    string html = _viewRenderService.RenderToStringAsync("_AgingReport", result).Result;

                    return Ok(html);
                }
            } catch(Exception er) {
                Console.Write(er.Message);
            }
            return null;
        }
    }
}
