using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using Core.Context;
using Core.Data.Dto;
using Core.Services.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class CompanyController: BaseController<CompanyController> {
        public ICompanyBusinessManager _businessManager;
        public CompanyController(ILogger<CompanyController> logger, IMapper mapper, ApplicationContext context,
            ICompanyBusinessManager businessManager) : base(logger, mapper, context) {
            _businessManager = businessManager;
        }


        // GET: Company
        public ActionResult Index() {
            return View();
        }

        // GET: Company/Details/5
        public ActionResult Details(long id) {
            return View();
        }

        // GET: Company/Create
        public ActionResult Create() {
            var item = new CompanyViewModel();

            return View(item);
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

                return RedirectToAction(nameof(Index));
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }
            return View(model);
        }

        // GET: Company/Edit/5
        public async Task<ActionResult> Edit(long id) {
            var item = await _businessManager.GetCompany(id);
            if(item == null) {
                return NotFound();
            }

            return View(_mapper.Map<CompanyViewModel>(item));
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
            return View(model);
        }

        // GET: Company/Delete/5
        public ActionResult Delete(int id) {
            return View();
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
        private readonly ICompanyBusinessManager _businessManager;

        public CompanyController(IMapper mapper, ICompanyBusinessManager businessManager) {
            _mapper = mapper;
            _businessManager = businessManager;
        }

        [HttpGet]
        public async Task<List<CompanyViewModelList>> GetCompanies() {
            var result = await _businessManager.GetCompanies();
            return _mapper.Map<List<CompanyViewModelList>>(result);
        }
    }
}