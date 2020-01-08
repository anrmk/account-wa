using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using Core.Context;
using Core.Services.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class CompanyController: BaseController<CompanyController> {
        public CompanyController(ILogger<CompanyController> logger, IMapper mapper, ApplicationContext context) : base(logger, mapper, context) {
        }


        // GET: Company
        public ActionResult Index() {
            return View();
        }

        // GET: Company/Details/5
        public ActionResult Details(int id) {
            return View();
        }

        // GET: Company/Create
        public ActionResult Create() {
            return View();
        }

        // POST: Company/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection) {
            try {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            } catch {
                return View();
            }
        }

        // GET: Company/Edit/5
        public ActionResult Edit(int id) {
            return View();
        }

        // POST: Company/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection) {
            try {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            } catch {
                return View();
            }
        }

        // GET: Company/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: Company/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection) {
            try {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            } catch {
                return View();
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