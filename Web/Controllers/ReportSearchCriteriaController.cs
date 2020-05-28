using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Core.Context;
using Core.Data.Dto;
using Core.Extension;
using Core.Services.Business;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class ReportSearchCriteriaController: BaseController<ReportSearchCriteriaController> {
        private readonly ICrudBusinessManager _businessManager;

        public ReportSearchCriteriaController(ILogger<ReportSearchCriteriaController> logger, IMapper mapper, ApplicationContext context,
            ICrudBusinessManager businessManager) : base(logger, mapper, context) {
            _businessManager = businessManager;
        }

        public IActionResult Index() {
            return View();
        }

        public async Task<IActionResult> Create([FromQuery] ReportSearchCriteriaViewModel model) {
            // ReportSearchCriteriaViewModel model = new ReportSearchCriteriaViewModel();
            var customerTags = await _businessManager.GetCustomerTags();
            ViewBag.CustomerTags = customerTags.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var customerTypes = await _businessManager.GetCustomerTypes();
            ViewBag.CustomerTypes = customerTypes.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            ViewBag.CustomerRechecks = model.Recheck?.Select(x => new SelectListItem() { Text = x.ToString(), Value = x.ToString() }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSearchCriteria([FromBody] ReportSearchCriteriaViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _businessManager.CreateReportSearchCriteria(_mapper.Map<ReportSearchCriteriaDto>(model));
                    if(item == null) {
                        return NotFound();
                    }
                    //model = _mapper.Map<CustomerViewModel>(item);
                    return RedirectToAction(nameof(Edit), new { id = item.Id });
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            // ReportSearchCriteriaViewModel model = new ReportSearchCriteriaViewModel();
            var customerTags = await _businessManager.GetCustomerTags();
            ViewBag.CustomerTags = customerTags.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var customerTypes = await _businessManager.GetCustomerTypes();
            ViewBag.CustomerTypes = customerTypes.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            ViewBag.CustomerRechecks = model.Recheck?.Select(x => new SelectListItem() { Text = x.ToString(), Value = x.ToString() }).ToList();

            return View("Create", model);
        }

        public async Task<IActionResult> Edit(long id) {
            var item = await _businessManager.GetReportSearchCriteria(id);
            if(item == null) {
                return NotFound();
            }
            return View(_mapper.Map<ReportSearchCriteriaViewModel>(item));
        }
    }
}

namespace Web.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class ReportSearchCriteriaController: ControllerBase {
        private readonly IMapper _mapper;
        private readonly ICrudBusinessManager _businessManager;

        public ReportSearchCriteriaController(IMapper mapper, ICrudBusinessManager businessManager) {
            _mapper = mapper;
            _businessManager = businessManager;
        }

        [HttpGet]
        public async Task<Pager<ReportSearchCriteriaViewModel>> GetReportSearchCriteria([FromQuery] PagerFilterViewModel model) {
            var result = await _businessManager.GetReportSearchCriterias(_mapper.Map<PagerFilter>(model));
            var pager = new Pager<ReportSearchCriteriaViewModel>(_mapper.Map<List<ReportSearchCriteriaViewModel>>(result.Items), result.TotalItems, result.CurrentPage, result.PageSize);
            pager.Filter = result.Filter;
            return pager;
        }

    }
}