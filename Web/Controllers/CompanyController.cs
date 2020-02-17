﻿using System;
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
            var customers = await _businessManager.GetCustomers(item.Customers.Select(x => x.Id).ToArray());
            ViewBag.Customers = customers;

            return View(_mapper.Map<CompanyViewModel>(item));
        }

        // GET: Company/Create
        public async Task<ActionResult> Create() {
            var customers = await _businessManager.GetUntiedCustomers(null);
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

            var customers = await _businessManager.GetUntiedCustomers(item.Id);
            ViewBag.Customers = customers.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var summary = await _businessManager.GetCompanyAllSummaryRange(item.Id);
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
                    var dto = _mapper.Map<CompanyDto>(model);
                    var item = await _businessManager.UpdateCompany(id, dto);
                    if(item == null) {
                        return NotFound();
                    }
                    return RedirectToAction(nameof(Index));
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            var customers = await _businessManager.GetUntiedCustomers(id);
            ViewBag.Customers = customers.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var summary = await _businessManager.GetCompanyAllSummaryRange(id);
            ViewBag.Summary = summary;

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

        [Route("{id}/createSummaryRange")]
        public async Task<ActionResult> CreateSummaryRange(long id) {
            var item = await _businessManager.GetCompany(id);

            if(item == null) {
                return NotFound();
            }
            var model = new CompanySummaryRangeViewModel() {
                CompanyId = id
            };
            return View(model);
        }

        [HttpPost]
        [Route("{id}/createSummaryRange")]
        public async Task<ActionResult> CreateSummaryRange(CompanySummaryRangeViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _businessManager.CreateCompanySummaryRange(_mapper.Map<CompanySummaryRangeDto>(model));
                    if(item == null) {
                        return BadRequest();
                    }

                    return RedirectToAction(nameof(Edit), new { Id = model.CompanyId });
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            return View(model);
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
        [Route("all")]
        public async Task<List<CompanyDto>> GetAll() {
            return await _businessManager.GetCompanies();
        }

        [HttpGet]
        public async Task<Pager<CompanyListViewModel>> GetCompanies(string search, string sort, string order, int offset = 0, int limit = 10) {
            var result = await _businessManager.GetCompanyPage(search ?? "", sort, order, offset, limit);
            var pager = new Pager<CompanyListViewModel>(_mapper.Map<List<CompanyListViewModel>>(result.Items), result.TotalItems, result.CurrentPage, result.PageSize);
            return pager;
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
            var result = await _businessManager.GetCompanyAllSummaryRange(id);
            return _mapper.Map<List<CompanySummaryRangeDto>>(result);
        }
    }
}