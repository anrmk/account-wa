using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Core.Context;
using Core.Data.Dto;
using Core.Extension;
using Core.Extensions;
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

        // GET: Invoice/Bulk
        public async Task<ActionResult> Bulk() {
            var companies = await _businessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var selectedCompany = companies.FirstOrDefault();

            var model = new BulkInvoiceViewModel() {
                CompanyId = selectedCompany?.Id ?? 0
            };

            var summaryRange = await _businessManager.GetCompanySummary(selectedCompany?.Id ?? 0);
            ViewBag.SummaryRange = summaryRange.Select(x => new SelectListItem() { Text = $"{x.SummaryFrom} - {x.SummaryTo}", Value = x.Id.ToString() });

            var customers = await _businessManager.GetBulkCustomers(selectedCompany?.Id ?? 0, model.DateFrom, model.DateTo);
            ViewBag.Customers = customers;

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
        private readonly IViewRenderService _viewRenderService;
        private readonly ICrudBusinessManager _businessManager;

        public InvoiceController(IMapper mapper, IViewRenderService viewRenderService,
            ICrudBusinessManager businessManager) {
            _mapper = mapper;
            _viewRenderService = viewRenderService;
            _businessManager = businessManager;
        }

        [HttpGet]
        public async Task<Pager<InvoiceDto>> GetInvoices(string search, string order, int offset = 0, int limit = 10) {
            return await _businessManager.GetInvoicePage(search ?? "", order, offset, limit);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<InvoiceViewModel> GetInvoice(long id) {
            var result = await _businessManager.GetInvoice(id);
            return _mapper.Map<InvoiceViewModel>(result);
        }

        [HttpGet]
        [Route("unpaid/{id}")]
        public async Task<List<InvoiceListViewModel>> GetUnpaid(long id) {
            var result = await _businessManager.GetUnpaidInvoices(id);
            return _mapper.Map<List<InvoiceListViewModel>>(result);
        }

        [HttpGet]
        [Route("{id}/customers")]
        public async Task<List<CustomerListViewModel>> GetCustomersByCompanyId(long id) {
            var result = await _businessManager.GetCustomers(id);
            return _mapper.Map<List<CustomerListViewModel>>(result);
        }

        [HttpPost]
        [Route("bulk")]
        public async Task<IActionResult> GenerateBulkInvoice(BulkInvoiceViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var customers = await _businessManager.GetCustomers(model.Customers.ToArray());
                    var subtotalRange = await _businessManager.GetCompanySummery(model.SummaryRangeId ?? 0);

                    if(subtotalRange != null) {
                        model.SummaryRange = $"{subtotalRange.SummaryFrom}-{subtotalRange.SummaryTo}";
                        List<InvoiceViewModel> invoices = new List<InvoiceViewModel>();
                        Random random = new Random();

                        foreach(var customer in customers) {
                            var date = random.NextDate(model.DateFrom, model.DateTo);
                            var invoice = new InvoiceViewModel() {
                                CompanyId = model.CompanyId,
                                CustomerId = customer.Id,
                                Customer = _mapper.Map<CustomerViewModel>(customer),
                                Date = date,
                                DueDate = date.AddDays(30),
                                No = $"{date.ToString("mmyy")}_{random.Next(100000, 999999)}",
                                Subtotal = random.NextDecimal(subtotalRange.SummaryFrom, subtotalRange.SummaryTo)
                            };
                            invoices.Add(invoice);
                        }
                        model.Invoices = invoices;

                        string html = _viewRenderService.RenderToStringAsync("_BulkInvoicePartial", model).Result;
                        return Ok(html);
                    } else {
                        return BadRequest();
                    }
                }
            } catch(Exception er) {
                Console.WriteLine(er.Message);
            }
            return Ok(model);
        }

        [HttpPost]
        [Route("bulkcreate")]
        public async Task<IActionResult> CreateBulkInvoices(BulkInvoiceViewModel model) {
            if(!ModelState.IsValid) {
                return BadRequest(model);
            }

            var invoiceList = _mapper.Map<List<InvoiceDto>>(model.Invoices);
            var result = await _businessManager.CreateInvoice(invoiceList);
            if(result == null || result.Count == 0) {
                return BadRequest(model);
            }
            return Ok(result);
        }
    }
}