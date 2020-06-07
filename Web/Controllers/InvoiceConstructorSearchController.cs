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
using Microsoft.Extensions.Primitives;

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

        public async Task<IActionResult> Create() {
            return await CreateFilter(new InvoiceConstructorSearchViewModel());
        }

        public async Task<IActionResult> CreateFilter(InvoiceConstructorSearchViewModel model) {
            var customerTags = await _businessManager.GetCustomerTags();
            ViewBag.CustomerTags = customerTags.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var customerTypes = await _businessManager.GetCustomerTypes();
            ViewBag.CustomerTypes = customerTypes.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var rechecks = model.Recheck ?? new List<int>();
            if(rechecks == null || rechecks.Count() == 0)
                rechecks.Add(0);
            ViewBag.CustomerRechecks = rechecks.Select(x => new SelectListItem() { Text = x.ToString(), Value = x.ToString() }).ToList();

            if(IsAjaxRequest)
                return PartialView("_CreatePartial", model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceConstructorSearchViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _businessManager.CreateInvoiceConstructorSearchCriterias(_mapper.Map<InvoiceConstructorSearchDto>(model));
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

            var rechecks = model.Recheck;
            if(rechecks == null || rechecks.Count() == 0)
                rechecks.Add(0);
            ViewBag.CustomerRechecks = rechecks.Select(x => new SelectListItem() { Text = x.ToString(), Value = x.ToString() }).ToList();

            return View("Create", model);
        }

        public async Task<ActionResult> Edit(long id) {
            var item = await _businessManager.GetInvoiceConstructorSearchCriteria(id);
            if(item == null) {
                return NotFound();
            }

            var customerTags = await _businessManager.GetCustomerTags();
            ViewBag.CustomerTags = customerTags.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var customerTypes = await _businessManager.GetCustomerTypes();
            ViewBag.CustomerTypes = customerTypes.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var rechecks = item.Recheck;
            if(rechecks == null || rechecks.Count() == 0)
                rechecks.Add(0);
            ViewBag.CustomerRechecks = rechecks.Select(x => new SelectListItem() { Text = x.ToString(), Value = x.ToString() }).ToList();

            return View(_mapper.Map<InvoiceConstructorSearchViewModel>(item));
        }

        // POST: Invoice/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, InvoiceConstructorSearchViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _businessManager.UpdateInvoiceConstructorSearchCriterias(id, _mapper.Map<InvoiceConstructorSearchDto>(model));
                    if(item == null) {
                        return NotFound();
                    }
                    return RedirectToAction(nameof(Edit), new { id = item.Id });
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            var customerTags = await _businessManager.GetCustomerTags();
            ViewBag.CustomerTags = customerTags.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var customerTypes = await _businessManager.GetCustomerTypes();
            ViewBag.CustomerTypes = customerTypes.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(long id) {
            try {
                var item = await _businessManager.DeleteInvoiceConstructorSearchCriterias(id);
                if(item == false) {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));

            } catch(Exception er) {
                _logger.LogError(er, er.Message);
                return BadRequest(er);
            }
        }
    }
}

namespace Web.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceConstructorSearchController: ControllerBase {
        private readonly IMapper _mapper;
        private readonly ICrudBusinessManager _businessManager;

        public InvoiceConstructorSearchController(IMapper mapper, ICrudBusinessManager businessManager) {
            _mapper = mapper;
            _businessManager = businessManager;
        }

        [HttpGet("GetReportSearchCriteria", Name = "GetReportSearchCriteria")]
        public async Task<Pager<InvoiceConstructorSearchViewModel>> GetReportSearchCriteria([FromQuery] PagerFilterViewModel model) {
            var result = await _businessManager.GetInvoiceConstructorSearchCriterias(_mapper.Map<PagerFilter>(model));
            var pager = new Pager<InvoiceConstructorSearchViewModel>(_mapper.Map<List<InvoiceConstructorSearchViewModel>>(result.Items), result.TotalItems, result.CurrentPage, result.PageSize);
            pager.Filter = result.Filter;
            return pager;
        }

        [HttpPost("CreateApi", Name = "CreateApi")]
        public async Task<IActionResult> CreateApi(InvoiceConstructorSearchViewModel model) {
            if(ModelState.IsValid) {
                var item = await _businessManager.CreateInvoiceConstructorSearchCriterias(_mapper.Map<InvoiceConstructorSearchDto>(model));
                if(item == null) {
                    return NotFound();
                }
                return Ok(_mapper.Map<InvoiceConstructorSearchViewModel>(item));
            }
            return BadRequest(model);
        }

    }
}