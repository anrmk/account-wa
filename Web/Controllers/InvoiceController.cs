using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using Core.Context;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class InvoiceController: BaseController<InvoiceController> {
        public InvoiceController(ILogger<InvoiceController> logger, IMapper mapper, ApplicationContext context) : base(logger, mapper, context) {
        }

        // GET: Invoice
        public ActionResult Index() {
            return View();
        }

        // GET: Invoice/Details/5
        public ActionResult Details(int id) {
            return View();
        }

        // GET: Invoice/Create
        public ActionResult Create() {
            return View();
        }

        // POST: Invoice/Create
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

        // GET: Invoice/Edit/5
        public ActionResult Edit(int id) {
            return View();
        }

        // POST: Invoice/Edit/5
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

        // GET: Invoice/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: Invoice/Delete/5
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
    public class InvoiceController: ControllerBase {
        private readonly IMapper _mapper;
        private readonly ApplicationContext _context;

        public InvoiceController(IMapper mapper, ApplicationContext context) {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<List<InvoiceViewModelList>> GetCompanies() {
            var result = await _context.Invoices.ToListAsync();
            return _mapper.Map<List<InvoiceViewModelList>>(result);
        }
    }
}