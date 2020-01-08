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

namespace Web.Controllers {
    public class CustomerController: BaseController<CustomerController> {
        public CustomerController(ILogger<CustomerController> logger, IMapper mapper, ApplicationContext context) : base(logger, mapper, context) {
        }

        // GET: Customer
        public ActionResult Index() {
            return View();
        }

        // GET: Customer/Details/5
        public ActionResult Details(int id) {
            return View();
        }

        // GET: Customer/Create
        public ActionResult Create() {
            return View();
        }

        // POST: Customer/Create
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

        // GET: Customer/Edit/5
        public ActionResult Edit(int id) {
            return View();
        }

        // POST: Customer/Edit/5
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

        // GET: Customer/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: Customer/Delete/5
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
    public class CustomerController: ControllerBase {
        private readonly IMapper _mapper;
        private readonly ICompanyBusinessManager _businessManager;

        public CustomerController(IMapper mapper, ICompanyBusinessManager businessManager) {
            _mapper = mapper;
            _businessManager = businessManager;
        }

        [HttpGet]
        public async Task<List<CustomerViewModelList>> GetCompanies() {
            var result = await _businessManager.GetCompanies();
            return _mapper.Map<List<CustomerViewModelList>>(result);
        }
    }
}