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
    public class CompanyController: BaseController<CompanyController> {
        public ICrudBusinessManager _businessManager;
        public CompanyController(ILogger<CompanyController> logger, IMapper mapper, ApplicationContext context,
            ICrudBusinessManager businessManager) : base(logger, mapper, context) {
            _businessManager = businessManager;
        }


        // GET: Company
        public ActionResult Index() {
            return View();
        }

        // GET: Company/Details/5
        public async Task<ActionResult> Details(long id) {
            var item = await _businessManager.GetCompany(id);

            if(item == null) {
                return NotFound();
            }
            var customers = await _businessManager.GetCustomers(item.Customers.ToArray());
            ViewBag.Customers = customers;

            return View(_mapper.Map<CompanyViewModel>(item));
        }

        // GET: Company/Create
        public async Task<ActionResult> Create() {
            var customers = await _businessManager.GetUndiedCustomers();
            ViewBag.Customers = customers.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(new CompanyViewModel());
        }

        // POST: Company/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CompanyViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _businessManager.CreateCompany(_mapper.Map<CompanyDto>(model));
                    if(item == null) {
                        return BadRequest();
                    }

                    return RedirectToAction(nameof(Index));
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }
            var customers = await _businessManager.GetCustomers();
            ViewBag.Customers = customers.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(model);
        }

        // GET: Company/Edit/5
        public async Task<ActionResult> Edit(long id) {
            var item = await _businessManager.GetCompany(id);
            if(item == null) {
                return NotFound();
            }

            var customers = await _businessManager.GetUndiedCustomers(item.Id);
            ViewBag.Customers = customers.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var summary = await _businessManager.GetCompanySummary(item.Id);
            ViewBag.Summary = summary;

            var model = _mapper.Map<CompanyViewModel>(item);
            return View(model);
        }

        // POST: Company/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, CompanyViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _businessManager.UpdateCompany(id, _mapper.Map<CompanyDto>(model));
                    if(item == null) {
                        return NotFound();
                    }
                    return RedirectToAction(nameof(Index));
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            var customers = await _businessManager.GetCustomers();
            ViewBag.Customers = customers.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return View(model);
        }

        // POST: Company/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(long id) {
            try {
                var item = await _businessManager.DeleteCompany(id);
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
    public class CompanyController: ControllerBase {
        private readonly IMapper _mapper;
        private readonly ICrudBusinessManager _businessManager;

        public CompanyController(IMapper mapper, ICrudBusinessManager businessManager) {
            _mapper = mapper;
            _businessManager = businessManager;
        }

        [HttpGet]
        public async Task<Pager<CompanyDto>> GetCompanies(string search, string order, int offset = 0, int limit = 10) {
            return await _businessManager.GetCompanyPage(search ?? "", order, offset, limit);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<CompanyViewModel> GetCompany(long id) {
            var result = await _businessManager.GetCompany(id);
            return _mapper.Map<CompanyViewModel>(result);
        }

        [HttpGet]
        [Route("{id}/summaryrange")]
        public async Task<List<CompanySummaryRangeDto>> GetRangeByCompanyId(long id) {
            var result = await _businessManager.GetCompanySummary(id);
            return _mapper.Map<List<CompanySummaryRangeDto>>(result);
        }
    }
}