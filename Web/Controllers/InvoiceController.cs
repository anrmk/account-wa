using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Core.Context;
using Core.Data.Dto;
using Core.Extension;
using Core.Services.Business;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;

using Web.Extension;
using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class InvoiceController: BaseController<InvoiceController> {
        private readonly ICrudBusinessManager _businessManager;
        private readonly ICompanyBusinessManager _companyBusinessManager;

        public InvoiceController(ILogger<InvoiceController> logger, IMapper mapper, ApplicationContext context,
            ICrudBusinessManager businessManager,
            ICompanyBusinessManager companyBusinessManager) : base(logger, mapper, context) {
            _businessManager = businessManager;
            _companyBusinessManager = companyBusinessManager;
        }

        // GET: Invoice
        public async Task<ActionResult> Index() {
            var companies = await _companyBusinessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(new InvoiceFilterViewModel());
        }

        // GET: Invoice using Aging filter
        public async Task<ActionResult> IndexFilter([FromQuery] InvoiceFilterViewModel model) {
            var companies = await _companyBusinessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View("Index", model);
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
            var companies = await _companyBusinessManager.GetCompanies();
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

            var companies = await _companyBusinessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            if(model.CompanyId != null) {
                var customers = await _businessManager.GetCustomers(model.CompanyId ?? 0);
                ViewBag.Customers = customers.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            }

            return View(model);
        }

        // GET: Invoice/Bulk
        public async Task<ActionResult> Bulk() {
            var companies = await _companyBusinessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var selectedCompany = companies.FirstOrDefault();

            var model = new CustomerFilterViewModel() {
                CompanyId = selectedCompany?.Id ?? 0,
                DateFrom = DateTime.Now.FirstDayOfMonth(),
                DateTo = DateTime.Now.LastDayOfMonth()
            };

            var summary = await _companyBusinessManager.GetSummaryRanges(selectedCompany?.Id ?? 0);
            ViewBag.SummaryRange = summary.Select(x => new SelectListItem() { Text = $"{x.From} - {x.To}", Value = x.Id.ToString() });

            var customerTags = await _businessManager.GetCustomerTags();
            ViewBag.Tags = customerTags.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });

            var customerTypes = await _businessManager.GetCustomerTypes();
            ViewBag.CustomerTypes = customerTypes.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });


            //var customers = await _businessManager.GetBulkCustomers(selectedCompany?.Id ?? 0, model.DateFrom, model.DateTo);
            //ViewBag.Customers = _mapper.Map<List<CustomerListViewModel>>(customers);

            return View(model);
        }

        // GET: Invoice/Edit/5
        public async Task<ActionResult> Edit(long id) {
            var item = await _businessManager.GetInvoice(id);
            if(item == null) {
                return NotFound();
            }

            var companies = await _companyBusinessManager.GetCompanies();
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
                    model = _mapper.Map<InvoiceViewModel>(item);
                    //return RedirectToAction(nameof(Index));
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            var companies = await _companyBusinessManager.GetCompanies();
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

        public async Task<ActionResult> BulkDelete(long[] ids) {
            if(ids.Length > 0) {
                var result = await _businessManager.DeleteInvoice(ids);
                return Ok(result);
            }

            return Ok(false);
        }
    }
}

namespace Web.Controllers.Api {
    [Route("api/[controller]")]
    public class InvoiceController: BaseApiController<InvoiceController> {
        private readonly IViewRenderService _viewRenderService;
        private readonly ICrudBusinessManager _businessManager;
        private readonly ICompanyBusinessManager _companyBusinessManager;
        private readonly ICustomerBusinessManager _customerBusinessManager;
        private readonly IMemoryCache _memoryCache;

        public InvoiceController(ILogger<InvoiceController> logger, IMapper mapper, IMemoryCache memoryCache,
            ICrudBusinessManager businessManager,
            ICompanyBusinessManager companyBusinessManager,
            ICustomerBusinessManager customerBusinessManager,
            IViewRenderService viewRenderService) : base(logger, mapper) {
            _businessManager = businessManager;
            _companyBusinessManager = companyBusinessManager;
            _customerBusinessManager = customerBusinessManager;
            _viewRenderService = viewRenderService;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<Pager<InvoiceListViewModel>> GetInvoices([FromQuery] InvoiceFilterViewModel model) {
            var result = await _businessManager.GetInvoicePage(_mapper.Map<InvoiceFilterDto>(model));
            var list = _mapper.Map<List<InvoiceListViewModel>>(result.Items);

            foreach(var invoice in list) {
                var tags = await _businessManager.GetCustomerTags(invoice.CustomerId);
                if(tags != null)
                    invoice.CustomerTags = tags.Select(x => x.Name).ToArray();
            }

            return new Pager<InvoiceListViewModel>(list, result.TotalItems, result.CurrentPage, result.PageSize, result.Params);
        }

        [HttpGet("FilterView", Name = "FilterView")]
        public async Task<IActionResult> FilterView([FromQuery] InvoiceFilterViewModel model) {
            var companies = await _companyBusinessManager.GetCompanies();
            var customerTypes = await _businessManager.GetCustomerTypes();
            
            var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) {
                            { "Companies", companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList()},
                            { "CustomerTypes", customerTypes.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList() }
                        };

            string html = await _viewRenderService.RenderToStringAsync("_FilterInvoicePartial", model, viewDataDictionary);
            return Ok(html);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<InvoiceViewModel> GetInvoice(long id) {
            var result = await _businessManager.GetInvoice(id);
            return _mapper.Map<InvoiceViewModel>(result);
        }

        [HttpGet]
        [Route("unpaid")]
        public async Task<List<InvoiceListViewModel>> GetUnpaid(long id, DateTime from, DateTime to) {
            var result = await _businessManager.GetUnpaidInvoicesByCompanyId(id, from, to);
            return _mapper.Map<List<InvoiceListViewModel>>(result);
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

        [HttpPost("GenerateBulkInvoices", Name = "GenerateBulkInvoices")]
        public async Task<IActionResult> GenerateBulkInvoices(InvoiceBulkViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var customers = await _businessManager.GetCustomers(model.Customers.ToArray());
                    var subtotalRange = await _companyBusinessManager.GetSummaryRange(model.SummaryRangeId ?? 0);

                    if(subtotalRange != null) {
                        model.Header = $"{subtotalRange.From}-{subtotalRange.To}";
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
                                Subtotal = random.NextDecimal(subtotalRange.From, subtotalRange.To)
                            };
                            invoices.Add(invoice);
                        }
                        model.Invoices = invoices;

                        string html = await _viewRenderService.RenderToStringAsync("_BulkInvoicePartial", model);
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
        public async Task<IActionResult> CreateBulkInvoices(InvoiceBulkViewModel model) {
            if(!ModelState.IsValid) {
                return BadRequest(model);
            }

            if(model.Invoices != null && model.Invoices?.Count != 0) {
                var invoiceList = _mapper.Map<List<InvoiceDto>>(model.Invoices);

                var result = await _businessManager.CreateInvoice(invoiceList);
                if(result == null || result.Count == 0) {
                    return BadRequest(model);
                }
                return Ok(result);
            }

            return Ok("");
        }

        [HttpPost("UploadInvoices", Name = "UploadInvoices")]
        public async Task<IActionResult> UploadInvoices([FromForm] IFormCollection forms) {
            try {
                if(forms.Files.Count == 0) {
                    throw new Exception("No file uploaded!");
                }

                var file = forms.Files[0];
                var extension = Path.GetExtension(file.FileName);

                if(!extension.Equals(".csv")) {
                    throw new Exception($"Unsupported file type: {extension}!");
                }

                using(var reader = new StreamReader(file.OpenReadStream())) {
                    using(TextFieldParser csvParser = new TextFieldParser(reader)) {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        var model = new InvoiceImportCsvViewModel() {
                            HeadRow = csvParser.ReadFields().ToList(),
                            Rows = new List<ImportCsvRowViewModel[]>()
                        };

                        while(!csvParser.EndOfData) {
                            string[] fields = csvParser.ReadFields();
                            var rows = new List<ImportCsvRowViewModel>();
                            for(int i = 0; i < fields.Count(); i++) {
                                rows.Add(new ImportCsvRowViewModel() {
                                    Index = i,
                                    Name = model.HeadRow[i],
                                    Value = fields[i]
                                }); ;
                            }
                            model.Rows.Add(rows.ToArray());
                        }

                        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
                        _memoryCache.Set("_InvoiceUploadCache", model, cacheEntryOptions);

                        var companies = await _companyBusinessManager.GetCompanies();
                        var companyList = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

                        var customerFields = typeof(InvoiceImportViewModel).GetProperties().Where(x => !x.IsCollectible && x.IsSpecialName)
                           .Select(x => new SelectListItem() {
                               Text = Attribute.IsDefined(x, typeof(RequiredAttribute)) ?
                                "* " + x.Name
                                : x.Name,
                               Value = x.Name
                           });


                        var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) {
                                    { "Companies", companyList},
                                    { "Fields", customerFields }
                                };

                        string html = await _viewRenderService.RenderToStringAsync("_InvoiceUploadPartial", model, viewDataDictionary);
                        return Ok(html);
                    }
                }
            } catch(Exception er) {
                return BadRequest(er.Message);
            }
        }

        [HttpPost("CreateUploadInvoices", Name = "CreateUploadInvoices")]
        public async Task<IActionResult> CreateUploadInvoices(InvoiceImportCsvViewModel model) {
            try {
                if(!ModelState.IsValid) {
                    throw new Exception("Form is not valid!");
                }

                var cacheModel = _memoryCache.Get<InvoiceImportCsvViewModel>("_InvoiceUploadCache");
                model.Rows = cacheModel?.Rows;

                var invoiceList = new List<InvoiceViewModel>();
                for(var i = 0; i < model.Rows?.Count(); i++) {
                    var row = model.Rows[i];
                    var rnd = new Random();

                    var invoiceModel = new InvoiceViewModel() {
                        No = $"{DateTime.Now.ToString("mmyy")}_{rnd.Next(100000, 999999)}"
                    };

                    for(var j = 0; j < row.Count(); j++) {
                        var column = model.Columns[j];
                        if(column != null && !string.IsNullOrEmpty(column.Name) && row[j].Index == column.Index) {
                            if(column.Name == "CustomerNo") {
                                var customer = await _customerBusinessManager.GetCustomer(row[j].Value, model.CompanyId);
                                if(customer != null) {
                                    var propertyCustomerId = invoiceModel.GetType().GetProperty("CustomerId");
                                    propertyCustomerId.SetValue(invoiceModel, customer.Id);
                                }
                                continue;
                            }

                            var property = invoiceModel.GetType().GetProperty(column.Name);

                            if(property != null && property.CanWrite) {
                                if(property.PropertyType == typeof(double)) {
                                    if(double.TryParse(row[j].Value, out double doubleVal)) {
                                        property.SetValue(invoiceModel, doubleVal);
                                    }
                                } else if(property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?)) {
                                    if(decimal.TryParse(row[j].Value, out decimal decimalVal)) {
                                        property.SetValue(invoiceModel, decimalVal);
                                    }
                                } else if(property.PropertyType == typeof(int)) {
                                    if(int.TryParse(row[j].Value, out int intVal)) {
                                        property.SetValue(invoiceModel, intVal);
                                    }
                                } else if(property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?)) {
                                    if(DateTime.TryParse(row[j].Value, out DateTime dateVal)) {
                                        property.SetValue(invoiceModel, dateVal);
                                    }
                                } else {
                                    property.SetValue(invoiceModel, row[j].Value);
                                }
                            }
                        }
                    }

                    if(TryValidateModel(invoiceModel)) {
                        var invoice = await _businessManager.CreateInvoice(_mapper.Map<InvoiceDto>(invoiceModel));
                        if(invoice != null) {
                            var paColumn = model.Columns.Where(x => x.Name == "PaymentAmount").FirstOrDefault();
                            var pdColumn = model.Columns.Where(x => x.Name == "PaymentDate").FirstOrDefault();
                            var paValue = row[paColumn.Index].Value;
                            var pdValue = row[pdColumn.Index].Value;

                            if(decimal.TryParse(paValue, out decimal paymentValue) && DateTime.TryParse(pdValue, out DateTime paymentDate)) {
                                var paymentModel = new PaymentViewModel() {
                                    No = $"{DateTime.Now.ToString("mmyy")}_{rnd.Next(100000, 999999)}",
                                    Amount = paymentValue,
                                    Date = paymentDate,
                                    InvoiceId = invoice.Id
                                };
                                if(TryValidateModel(paymentModel)) {
                                    var payment = await _businessManager.CreatePayment(_mapper.Map<PaymentDto>(paymentModel));
                                }
                            }

                            invoiceList.Add(_mapper.Map<InvoiceViewModel>(invoice));
                        }
                    }
                }

                if(invoiceList.Count == 0) {
                    throw new Exception("No records have been created! Please, fill the required fields!");
                }

                return Ok(new { Message = $"{invoiceList.Count}/{model.Rows?.Count} invoices are created!" });

            } catch(Exception er) {
                return BadRequest(er.Message ?? er.StackTrace);
            }
        }
    }
}