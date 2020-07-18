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

using Web.Extension;
using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class PaymentController: BaseController<PaymentController> {
        public ICrudBusinessManager _businessManager;
        private readonly ICompanyBusinessManager _companyBusinessManager;

        public PaymentController(ILogger<PaymentController> logger, IMapper mapper, ApplicationContext context,
            ICrudBusinessManager businessManager, ICompanyBusinessManager companyBusinessManager) : base(logger, mapper, context) {
            _businessManager = businessManager;
            _companyBusinessManager = companyBusinessManager;
        }

        public IActionResult Index() {
            return View(new PaymentFilterViewModel());
        }

        public async Task<IActionResult> Details(long id) {
            var item = await _businessManager.GetPayment(id);
            if(item == null) {
                return NotFound();
            }

            return View(_mapper.Map<PaymentViewModel>(item));
        }

        public async Task<IActionResult> Create() {
            Random rd = new Random();
            byte[] bytes = new byte[4];
            rd.NextBytes(bytes);

            var customers = await _businessManager.GetCustomers();
            ViewBag.Customers = customers.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var paymentModel = new PaymentViewModel() {
                No = BitConverter.ToString(bytes).Replace("-", ""),
                Date = DateTime.Now
            };

            return View(paymentModel);
        }

        // GET: Filter Partial
        public async Task<ActionResult> Filter([FromQuery] PaymentFilterViewModel model) {
            var companies = await _companyBusinessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View("_FilterPaymentPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PaymentViewModel model) {
            try {
                if(ModelState.IsValid) {

                    var item = await _businessManager.CreatePayment(_mapper.Map<PaymentDto>(model));
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

        // GET: Payment/Bulk
        public async Task<ActionResult> Bulk() {
            var companies = await _companyBusinessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var selectedCompany = companies.FirstOrDefault();

            var model = new BulkPaymentViewModel() {
                CompanyId = selectedCompany?.Id ?? 0
            };

            var invoices = await _businessManager.GetUnpaidInvoicesByCompanyId(selectedCompany?.Id ?? 0, model.DateFrom, model.DateTo);
            ViewBag.Invoices = _mapper.Map<List<InvoiceListViewModel>>(invoices);

            return View(model);
        }

        //[Obsolete]
        //// GET: Payment/BulkPayment
        //public async Task<ActionResult> BulkPayment(List<long> ids) {
        //    var invoices = await _businessManager.GetInvoices(ids.ToArray());
        //    Random rd = new Random();

        //    var payments = invoices.Select(x => new PaymentViewModel() {
        //        No = string.Format("PMNT_{0}", rd.NextLong(11111, 99999).ToString()),
        //        Amount = x.Subtotal - x.Payments.TotalAmount(),
        //        CustomerId = x.CustomerId,
        //        Date = DateTime.Now,
        //        InvoiceId = x.Id,
        //        InvoiceNo = x.No,
        //        InvoiceAmount = x.Subtotal * (1 + x.TaxRate / 100),
        //    }).ToList();

        //    var model = new BulkPaymentViewModel() {
        //        DateFrom = DateTime.Now.FirstDayOfMonth(),
        //        DateTo = DateTime.Now.LastDayOfMonth(),
        //        CompanyId = 0,
        //        Payments = payments,
        //        Invoices = ids
        //    };

        //    return View("_PaymentsPartial", model);
        //}

        // GET: Payment/Edit/5
        public async Task<ActionResult> Edit(long id) {
            var item = await _businessManager.GetPayment(id);
            if(item == null) {
                return NotFound();
            }

            var customers = await _businessManager.GetCustomers();
            ViewBag.Customers = customers.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var paidInvoice = await _businessManager.GetInvoice(item.InvoiceId ?? 0);
            var invoices = await _businessManager.GetUnpaidInvoices(item.CustomerId ?? 0);
            if(paidInvoice != null) {
                invoices.Add(paidInvoice);
            }
            ViewBag.Invoices = invoices.Select(x => new SelectListItem() { Text = $"{x.No} - {x.Subtotal}", Value = x.Id.ToString() }).ToList();

            return View(_mapper.Map<PaymentViewModel>(item));
        }

        // POST: Payment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, PaymentViewModel model) {
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

            var customers = await _businessManager.GetCustomers();
            ViewBag.Customers = customers.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(model);
        }

        // GET: Invoice/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(long id) {
            try {
                var item = await _businessManager.DeletePayment(id);
                if(item == false) {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));

            } catch(Exception er) {
                _logger.LogError(er, er.Message);
                return BadRequest(er);
            }
        }

        public async Task<ActionResult> BulkDelete(long[] ids) {
            if(ids.Length > 0) {
                var result = await _businessManager.DeletePayment(ids);
                return Ok(result);
            }

            return Ok(false);
        }
    }
}

namespace Web.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController: ControllerBase {
        private readonly IMapper _mapper;
        private readonly IViewRenderService _viewRenderService;
        private readonly ICrudBusinessManager _businessManager;

        public PaymentController(IMapper mapper, IViewRenderService viewRenderService, ICrudBusinessManager businessManager) {
            _mapper = mapper;
            _viewRenderService = viewRenderService;
            _businessManager = businessManager;
        }

        [HttpGet]
        public async Task<Pager<PaymentListViewModel>> GetPayments([FromQuery] PaymentFilterViewModel model) {
            var result = await _businessManager.GetPaymentPages(_mapper.Map<PaymentFilterDto>(model));
            var list = _mapper.Map<List<PaymentListViewModel>>(result.Items);

            return new Pager<PaymentListViewModel>(list, result.TotalItems, result.CurrentPage, result.PageSize, result.Params);
        }

        /// <summary>
        /// Формируем список оплат (PAYMENTS) 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("bulk")]
        public async Task<IActionResult> GenerateBulkInvoice(BulkPaymentViewModel model) {
            try {
                if(ModelState.IsValid) {
                    Random rd = new Random();
                    byte[] bytes = new byte[4];


                    var invoices = await _businessManager.GetInvoices(model.Invoices.ToArray());
                    var invoiceList = _mapper.Map<List<InvoiceListViewModel>>(invoices);

                    //if(subtotalRange != null) {
                    //    model.Header = $"{subtotalRange.From}-{subtotalRange.To}";
                    List<PaymentViewModel> payments = new List<PaymentViewModel>();
                    Random random = new Random();

                    foreach(var invoice in invoices) {
                        rd.NextBytes(bytes);

                        var date = random.NextDate(model.PaymentDateFrom, model.PaymentDateTo);
                        var invoiceAmount = invoice.Subtotal * (1 + invoice.TaxRate / 100);

                        var payment = new PaymentViewModel() {
                            No = BitConverter.ToString(bytes).Replace("-", ""),
                            CustomerId = invoice.Customer?.Id ?? 0,
                            InvoiceId = invoice.Id,
                            InvoiceNo = invoice.No,
                            InvoiceAmount = invoiceAmount,
                            Date = date,
                            Amount = invoiceAmount - invoice.Payments.TotalAmount()
                        };
                        payments.Add(payment);
                    }
                    model.Payments = payments;

                    string html = _viewRenderService.RenderToStringAsync("_BulkPaymentPartial", model).Result;
                    return Ok(html);
                    //} else {
                    //    return BadRequest();
                    //}
                }
            } catch(Exception er) {
                Console.WriteLine(er.Message);
            }
            return Ok(model);
        }


        /// <summary>
        /// Сохраняем список
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("bulkcreate")]
        public async Task<IActionResult> CreateBulkPayments(BulkPaymentViewModel model) {
            if(!ModelState.IsValid) {
                return BadRequest(model);
            }

            var invoiceList = _mapper.Map<List<PaymentDto>>(model.Payments);
            var result = await _businessManager.CreatePayment(invoiceList);
            if(result == null || result.Count == 0) {
                return BadRequest(model);
            }
            return Ok(result);
        }

        [HttpGet("CreatePaymentsView", Name = "CreatePaymentsView")]
        public async Task<IActionResult> CreatePaymentsView([FromQuery] long[] id) {
            if(id.Length > 0) {
                var invoices = await _businessManager.GetInvoices(id);
                Random rd = new Random();

                var payments = invoices.Select(x => new PaymentViewModel() {
                    No = string.Format("PMNT_{0}", rd.NextLong(11111, 99999).ToString()),
                    Amount = x.Subtotal - x.Payments.TotalAmount(),
                    CustomerId = x.CustomerId,
                    Date = DateTime.Now,
                    InvoiceId = x.Id,
                    InvoiceNo = x.No,
                    InvoiceAmount = x.Subtotal * (1 + x.TaxRate / 100),
                }).ToList();

                var model = new BulkPaymentViewModel() {
                    DateFrom = DateTime.Now.FirstDayOfMonth(),
                    DateTo = DateTime.Now.LastDayOfMonth(),
                    CompanyId = 0,
                    Payments = payments,
                    Invoices = id.ToList()
                };

                string html = _viewRenderService.RenderToStringAsync("_PaymentsPartial", model).Result;
                return Ok(html);
            }
            return BadRequest("No items selected");
        }

        [HttpPost("CreatePayments", Name="CreatePayments")]
        public async Task<IActionResult> CreatePayments([FromBody] BulkPaymentViewModel model) {
            if(!ModelState.IsValid) {
                return BadRequest(model);
            }
            var invoiceList = _mapper.Map<List<PaymentDto>>(model.Payments);
            var result = await _businessManager.CreatePayment(invoiceList);
            if(result == null || result.Count == 0) {
                return BadRequest(model);
            }
            return Ok(result);
        }
    }
}