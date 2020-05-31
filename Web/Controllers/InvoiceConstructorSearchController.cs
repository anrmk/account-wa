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
    public class InvoiceConstructorSearchController: BaseController<InvoiceConstructorSearchController> {
        private readonly ICrudBusinessManager _businessManager;

        public InvoiceConstructorSearchController(ILogger<InvoiceConstructorSearchController> logger, IMapper mapper, ApplicationContext context,
            ICrudBusinessManager businessManager) : base(logger, mapper, context) {
            _businessManager = businessManager;
        }

        public IActionResult Index() {
            return View();
        }

        public async Task<IActionResult> Create([FromQuery] InvoiceConstructorSearchViewModel model) {
            // ReportSearchCriteriaViewModel model = new ReportSearchCriteriaViewModel();
            var customerTags = await _businessManager.GetCustomerTags();
            ViewBag.CustomerTags = customerTags.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var customerTypes = await _businessManager.GetCustomerTypes();
            ViewBag.CustomerTypes = customerTypes.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            ViewBag.CustomerRechecks = model.Recheck?.Select(x => new SelectListItem() { Text = x.ToString(), Value = x.ToString() }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateConstructorSearch([FromBody] InvoiceConstructorSearchViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _businessManager.CreateReportSearchCriteria(_mapper.Map<InvoiceConstructorSearchDto>(model));
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
            return View(_mapper.Map<InvoiceConstructorSearchViewModel>(item));
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
        public async Task<Pager<InvoiceConstructorSearchViewModel>> GetReportSearchCriteria([FromQuery] PagerFilterViewModel model) {
            var result = await _businessManager.GetReportSearchCriterias(_mapper.Map<PagerFilter>(model));
            var pager = new Pager<InvoiceConstructorSearchViewModel>(_mapper.Map<List<InvoiceConstructorSearchViewModel>>(result.Items), result.TotalItems, result.CurrentPage, result.PageSize);
            pager.Filter = result.Filter;
            return pager;
        }

    }
}