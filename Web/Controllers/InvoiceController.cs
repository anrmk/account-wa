using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Core.Context;
using Core.Data.Dto;
using Core.Services.Business;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class InvoiceController: BaseController<InvoiceController> {
        public ICrudBusinessManager _businessManager;
        public InvoiceController(ILogger<InvoiceController> logger, IMapper mapper, ApplicationContext context, ICrudBusinessManager businessManager) : base(logger, mapper, context) {
            _businessManager = businessManager;
        }

        // GET: Invoice
        public ActionResult Index() {
            return View();
        }

        // GET: Invoice/Details/5
        public async Task<ActionResult> Details(long id) {
            var item = await _businessManager.GetInvoice(id);
            if(item == null) {
                return NotFound();
            }

            var payment = await _businessManager.GetPaymentByInvoiceId(item.Id);

            ViewBag.Company = _mapper.Map<CompanyViewModel>(item.Company);
            ViewBag.Customer = _mapper.Map<CustomerViewModel>(item.Customer);
            ViewBag.Payment = _mapper.Map<List<PaymentViewModel>>(payment);

            var model = _mapper.Map<InvoiceViewModel>(item);

            return View(model);
        }

        // GET: Invoice/Create
        public async Task<ActionResult> Create() {
            var companies = await _businessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            Random rd = new Random();
            byte[] bytes = new byte[4];
            rd.NextBytes(bytes);

            var invoiceModel = new InvoiceViewModel() {
                No = BitConverter.ToString(bytes).Replace("-", ""),
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(30),
            };

            return View(invoiceModel);
        }

        // POST: Invoice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InvoiceViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _businessManager.CreateInvoice(_mapper.Map<InvoiceDto>(model));
                    if(item == null) {
                        return BadRequest();
                    }

                    return RedirectToAction(nameof(Index));
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            var companies = await _businessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            if(model.CompanyId != null) {
                var customers = await _businessManager.GetCustomers(model.CompanyId ?? 0);
                ViewBag.Customers = customers.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            }

            return View(model);
        }

        // GET: Invoice/Edit/5
        public async Task<ActionResult> Edit(long id) {
            var item = await _businessManager.GetInvoice(id);
            if(item == null) {
                return NotFound();
            }

            var companies = await _businessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var customers = await _businessManager.GetCustomers(item.CompanyId ?? 0);
            ViewBag.Customers = customers.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(_mapper.Map<InvoiceViewModel>(item));
        }

        // POST: Invoice/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, InvoiceViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _businessManager.UpdateInvoice(id, _mapper.Map<InvoiceDto>(model));
                    if(item == null) {
                        return NotFound();
                    }
                    return RedirectToAction(nameof(Index));
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            var companies = await _businessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var customers = await _businessManager.GetCustomers(model.CompanyId ?? 0);
            ViewBag.Customers = customers.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(model);
        }

        // GET: Invoice/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(long id) {
            try {
                var item = await _businessManager.DeleteInvoice(id);
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
    public class InvoiceController: ControllerBase {
        private readonly IMapper _mapper;
        private readonly ICrudBusinessManager _businessManager;

        public InvoiceController(IMapper mapper, ICrudBusinessManager businessManager) {
            _mapper = mapper;
            _businessManager = businessManager;
        }

        [HttpGet]
        public async Task<List<InvoiceViewModelList>> GetInvoices() {
            var result = await _businessManager.GetInvoices();
            return _mapper.Map<List<InvoiceViewModelList>>(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<InvoiceViewModel> GetInvoice(long id) {
            var result = await _businessManager.GetInvoice(id);
            return _mapper.Map<InvoiceViewModel>(result);
        }

        [HttpGet]
        [Route("unpaid/{id}")]
        public async Task<List<InvoiceViewModelList>> GetUnpaid(long id) {
            var result = await _businessManager.GetUnpaidInvoices(id);
            return _mapper.Map<List<InvoiceViewModelList>>(result);
        }

        [HttpGet]
        [Route("{id}/customers")]
        public async Task<List<CustomerViewModelList>> GetCustomersByCompanyId(long id) {
            var result = await _businessManager.GetCustomers(id);
            return _mapper.Map<List<CustomerViewModelList>>(result);
        }
    }
}