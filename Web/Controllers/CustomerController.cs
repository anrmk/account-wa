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
using Core.Extensions;
using Core.Services.Business;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;

using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class CustomerController: BaseController<ReportController> {
        private readonly ICrudBusinessManager _businessManager;
        private readonly INsiBusinessManager _nsiManager;
        private readonly IViewRenderService _viewRenderService;
        private readonly IMemoryCache _memoryCache;

        public CustomerController(ILogger<ReportController> logger, IMapper mapper, IMemoryCache memoryCache, ApplicationContext context,
             ICrudBusinessManager businessManager, INsiBusinessManager nsiManager, IViewRenderService viewRenderService) : base(logger, mapper, context) {
            _businessManager = businessManager;
            _nsiManager = nsiManager;
            _viewRenderService = viewRenderService;
            _memoryCache = memoryCache;
        }

        public ActionResult Index() {
            return View();
        }

        public ActionResult Details(long id) {
            return View();
        }

        public async Task<ActionResult> Create() {
            var item = new CustomerViewModel();

            var companies = await _businessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var customerTypes = await _nsiManager.GetCustomerTypes();
            ViewBag.CustomerTypes = customerTypes.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CustomerViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _businessManager.CreateCustomer(_mapper.Map<CustomerDto>(model));
                    if(item == null) {
                        return BadRequest();
                    }
                    return RedirectToAction(nameof(Edit), new { id = item.Id });
                }

            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }
            return View(model);
        }

        public async Task<ActionResult> Edit(long id) {
            var item = await _businessManager.GetCustomer(id);
            if(item == null) {
                return NotFound();
            }

            var companies = await _businessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var activities = await _businessManager.GetCustomerAllActivity(id);
            ViewBag.Activities = _mapper.Map<List<CustomerActivityViewModel>>(activities);

            var customerTypes = await _nsiManager.GetCustomerTypes();
            ViewBag.CustomerTypes = customerTypes.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var customerCreditLimit = await _businessManager.GetCustomerCreditLimits(id);
            ViewBag.CreditLimit = _mapper.Map<List<CustomerCreditLimitViewModel>>(customerCreditLimit);

            var customerCreditUtilized = await _businessManager.GetCustomerCreditUtilizeds(id);
            ViewBag.CreditUtilized = _mapper.Map<List<CustomerCreditUtilizedViewModel>>(customerCreditUtilized);

            var custoremTags = await _businessManager.GetCustomerTags();
            ViewBag.Tags = custoremTags.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(_mapper.Map<CustomerViewModel>(item));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, CustomerViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _businessManager.UpdateCustomer(id, _mapper.Map<CustomerDto>(model));
                    if(item == null) {
                        return NotFound();
                    }
                    //model = _mapper.Map<CustomerViewModel>(item);
                    return RedirectToAction(nameof(Edit), new { id = item.Id });
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            var companies = await _businessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var activities = await _businessManager.GetCustomerAllActivity(id);
            ViewBag.Activities = _mapper.Map<List<CustomerActivityViewModel>>(activities);

            var customerTypes = await _nsiManager.GetCustomerTypes();
            ViewBag.CustomerTypes = customerTypes.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var customerCreditLimit = await _businessManager.GetCustomerCreditLimits(id);
            ViewBag.CreditLimit = _mapper.Map<List<CustomerCreditLimitViewModel>>(customerCreditLimit);

            var customerCreditUtilized = await _businessManager.GetCustomerCreditUtilizeds(id);
            ViewBag.CreditUtilized = _mapper.Map<List<CustomerCreditUtilizedViewModel>>(customerCreditUtilized);

            var custoremTags = await _businessManager.GetCustomerTags();
            ViewBag.Tags = custoremTags.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(long id) {
            try {
                var item = await _businessManager.DeleteCustomer(id);
                if(item == false) {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));

            } catch(Exception er) {
                _logger.LogError(er, er.Message);
                return BadRequest(er);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Upload([FromForm] IFormCollection forms) {
            var companies = await _businessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            //TODO: Сделать отображение атрибута Display 
            ViewBag.CustomerFields = typeof(CustomerViewModel).GetProperties().Where(x => !x.IsCollectible && x.IsSpecialName)
                .Select(x => new SelectListItem() { Text = Attribute.IsDefined(x, typeof(RequiredAttribute)) ? "* " + x.Name : x.Name, Value = x.Name });

            if(forms.Files.Count > 0) {
                var file = forms.Files[0];
                var extension = Path.GetExtension(file.FileName);

                try {
                    //if(extension.Equals(".json")) {
                    //    using(var reader = new StreamReader(file.OpenReadStream())) {
                    //        try {
                    //            var result = await reader.ReadToEndAsync();

                    //            var dtoList = JsonConvert.DeserializeObject<List<CustomerDto>>(result);
                    //            var customers = _mapper.Map<List<CustomerViewModel>>(dtoList);

                    //            var resultHtml = _viewRenderService.RenderToStringAsync("_CustomerBulkCreatePartial", customers).Result;

                    //            return Json(new { html = resultHtml });

                    //        } catch(Exception e) {
                    //            _logger.LogError(e, e.Message);
                    //        }
                    //    }
                    //} else 
                    if(extension.Equals(".csv")) {
                        using(var reader = new StreamReader(file.OpenReadStream())) {
                            using(TextFieldParser csvParser = new TextFieldParser(reader)) {
                                csvParser.CommentTokens = new string[] { "#" };
                                csvParser.SetDelimiters(new string[] { "," });
                                csvParser.HasFieldsEnclosedInQuotes = true;

                                var model = new CustomerBulkViewModel() {
                                    HeadRow = csvParser.ReadFields().ToList(),
                                    Rows = new List<CustomerRowViewModel[]>()
                                };

                                while(!csvParser.EndOfData) {
                                    string[] fields = csvParser.ReadFields();
                                    var rows = new List<CustomerRowViewModel>();
                                    for(int i = 0; i < fields.Count(); i++) {
                                        rows.Add(new CustomerRowViewModel() {
                                            Index = i,
                                            Name = model.HeadRow[i],
                                            Value = fields[i]
                                        });
                                    }
                                    model.Rows.Add(rows.ToArray());
                                }

                                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
                                _memoryCache.Set("_CustomerUpload", model, cacheEntryOptions);

                                var resultHtml = _viewRenderService.RenderToStringAsync("_CustomerBulkCreatePartial", model, ViewData).Result;
                                return Json(new { html = resultHtml });
                            }
                        }
                    } else {
                        throw new Exception("Unsupported file type: " + extension);
                    }
                } catch(Exception e) {
                    _logger.LogError(e, e.Message);
                    return BadRequest(e);
                }
            }
            return Json(new { html = "" });
        }

        [HttpPost]
        public async Task<ActionResult> UploadCredits([FromForm] IFormCollection forms) {
            var companies = await _businessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            //TODO: Сделать отображение атрибута Display 
            ViewBag.CustomerFields = typeof(CustomerImportCreditsViewModel).GetProperties().Where(x => !x.IsCollectible && x.IsSpecialName)
                .Select(x => new SelectListItem() { Text = Attribute.IsDefined(x, typeof(RequiredAttribute)) ? "* " + x.Name : x.Name, Value = x.Name });

            if(forms.Files.Count > 0) {
                var file = forms.Files[0];
                var extension = Path.GetExtension(file.FileName);

                try {
                    if(extension.Equals(".csv")) {
                        using(var reader = new StreamReader(file.OpenReadStream())) {
                            using(TextFieldParser csvParser = new TextFieldParser(reader)) {
                                csvParser.CommentTokens = new string[] { "#" };
                                csvParser.SetDelimiters(new string[] { "," });
                                csvParser.HasFieldsEnclosedInQuotes = true;

                                var model = new CustomerCreditsBulkViewModel() {
                                    HeadRow = csvParser.ReadFields().ToList(),
                                    Rows = new List<CustomerRowViewModel[]>(),
                                    CreatedDate = DateTime.Now.LastDayOfMonth()
                                };

                                while(!csvParser.EndOfData) {
                                    string[] fields = csvParser.ReadFields();
                                    var rows = new List<CustomerRowViewModel>();
                                    for(int i = 0; i < fields.Count(); i++) {
                                        rows.Add(new CustomerRowViewModel() {
                                            Index = i,
                                            Name = model.HeadRow[i],
                                            Value = fields[i]
                                        });
                                    }
                                    model.Rows.Add(rows.ToArray());
                                }

                                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
                                _memoryCache.Set("_CustomerCreditsUpload", model, cacheEntryOptions);

                                var resultHtml = _viewRenderService.RenderToStringAsync("_CustomerCreditsBulkCreatePartial", model, ViewData).Result;
                                return Json(new { html = resultHtml });
                            }
                        }
                    } else {
                        throw new Exception("Unsupported file type: " + extension);
                    }
                } catch(Exception e) {
                    _logger.LogError(e, e.Message);
                    return BadRequest(e);
                }
            }
            return BadRequest("Something went wrong....");
        }

        #region ACTIVITY
        [Route("{customerId}/activity")]
        public async Task<ActionResult> CreateActivity(long customerId) {
            var item = await _businessManager.GetCustomer(customerId);

            if(item == null) {
                return NotFound();
            }
            ViewBag.CustomerName = item.Name;

            var model = new CustomerActivityViewModel() {
                CustomerId = customerId,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            return View(model);
        }

        [HttpPost]
        [Route("{customerId}/activity")]
        public async Task<ActionResult> CreateActivity(CustomerActivityViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _businessManager.CreateCustomerActivity(_mapper.Map<CustomerActivityDto>(model));
                    if(item == null) {
                        return BadRequest();
                    }

                    return RedirectToAction(nameof(Edit), new { Id = model.CustomerId });
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            return View(model);
        }
        #endregion

        #region CreditLimit
        [Route("{customerId}/creditlimit")]
        public async Task<ActionResult> CreateCreditLimit(long customerId) {
            var item = await _businessManager.GetCustomer(customerId);

            if(item == null) {
                return NotFound();
            }

            ViewBag.CustomerName = item.Name;

            var model = new CustomerCreditLimitViewModel() {
                CustomerId = customerId,
                CreatedDate = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        [Route("{customerId}/creditlimit")]
        public async Task<ActionResult> CreateCreditLimit(CustomerCreditLimitViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _businessManager.CreateCustomerCreditLimit(_mapper.Map<CustomerCreditLimitDto>(model));
                    if(item == null) {
                        return BadRequest();
                    }

                    return RedirectToAction(nameof(Edit), new { Id = model.CustomerId });
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            return View(model);
        }

        public async Task<ActionResult> EditCreditLimit(long id) {
            var item = await _businessManager.GetCustomerCreditLimit(id);
            if(item == null) {
                return NotFound();
            }

            var customer = await _businessManager.GetCustomer(item.CustomerId ?? 0);
            ViewBag.CustomerName = customer.Name;

            return View(_mapper.Map<CustomerCreditLimitViewModel>(item));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCreditLimit(long id, CustomerCreditLimitViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _businessManager.UpdateCustomerCreditLimit(id, _mapper.Map<CustomerCreditLimitDto>(model));
                    if(item == null) {
                        return NotFound();
                    }
                    return RedirectToAction(nameof(EditCreditLimit), new { id = item.Id });
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            var customer = await _businessManager.GetCustomer(model.CustomerId ?? 0);
            ViewBag.CustomerName = customer.Name;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCreditLimit(long id) {
            try {
                var item = await _businessManager.GetCustomerCreditLimit(id);
                if(item == null) {
                    return NotFound();
                }

                var result = await _businessManager.DeleteCustomerCreditLimit(id);
                if(result == false) {
                    return NotFound();
                }
                return RedirectToAction(nameof(Edit), new { Id = item.CustomerId });

            } catch(Exception er) {
                _logger.LogError(er, er.Message);
                return BadRequest(er);
            }
        }
        #endregion

        #region CreditUtilized
        [Route("{customerId}/creditutilized")]
        public async Task<ActionResult> CreateCreditUtilized(long customerId) {
            var item = await _businessManager.GetCustomer(customerId);

            if(item == null) {
                return NotFound();
            }

            ViewBag.CustomerName = item.Name;

            var model = new CustomerCreditUtilizedViewModel() {
                CustomerId = customerId,
                CreatedDate = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        [Route("{customerId}/creditutilized")]
        public async Task<ActionResult> CreateCreditUtilized(CustomerCreditUtilizedViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _businessManager.CreateCustomerCreditUtilized(_mapper.Map<CustomerCreditUtilizedDto>(model));
                    if(item == null) {
                        return BadRequest();
                    }

                    return RedirectToAction(nameof(Edit), new { Id = model.CustomerId });
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            return View(model);
        }

        public async Task<ActionResult> EditCreditUtilized(long id) {
            var item = await _businessManager.GetCustomerCreditUtilized(id);
            if(item == null) {
                return NotFound();
            }

            var customer = await _businessManager.GetCustomer(item.CustomerId ?? 0);
            ViewBag.CustomerName = customer.Name;

            return View(_mapper.Map<CustomerCreditUtilizedViewModel>(item));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCreditUtilized(long id, CustomerCreditUtilizedViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _businessManager.UpdateCustomerCreditUtilized(id, _mapper.Map<CustomerCreditUtilizedDto>(model));
                    if(item == null) {
                        return NotFound();
                    }
                    return RedirectToAction(nameof(EditCreditUtilized), new { id = item.Id });
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            var customer = await _businessManager.GetCustomer(model.CustomerId ?? 0);
            ViewBag.CustomerName = customer.Name;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCreditUtilized(long id) {
            try {
                var item = await _businessManager.GetCustomerCreditUtilized(id);
                if(item == null) {
                    return NotFound();
                }

                var result = await _businessManager.DeleteCustomerCreditUtilized(id);
                if(result == false) {
                    return NotFound();
                }
                return RedirectToAction(nameof(Edit), new { Id = item.CustomerId });

            } catch(Exception er) {
                _logger.LogError(er, er.Message);
                return BadRequest(er);
            }
        }
        #endregion
    }
}

namespace Web.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController: ControllerBase {
        private readonly IMapper _mapper;
        private readonly IViewRenderService _viewRenderService;
        private readonly ICrudBusinessManager _businessManager;
        private readonly INsiBusinessManager _nsiManager;
        private readonly IMemoryCache _memoryCache;

        public CustomerController(IMapper mapper, IViewRenderService viewRenderService, IMemoryCache memoryCache, ICrudBusinessManager businessManager, INsiBusinessManager nsiManager) {
            _mapper = mapper;
            _viewRenderService = viewRenderService;
            _memoryCache = memoryCache;
            _businessManager = businessManager;
            _nsiManager = nsiManager;
        }

        [HttpGet]
        public async Task<Pager<CustomerListViewModel>> GetCustomers([FromQuery] CustomerFilterViewModel model) {
            var result = await _businessManager.GetCustomersPage(_mapper.Map<CustomerFilterDto>(model));
            var pager = new Pager<CustomerListViewModel>(_mapper.Map<List<CustomerListViewModel>>(result.Items), result.TotalItems, result.CurrentPage, result.PageSize);
            pager.Filter = result.Filter;
            return pager;
        }

        //[Obsolete]
        //[HttpGet]
        //[Route("company/{id}")]
        //public async Task<List<CustomerListViewModel>> GetCustomersByCompanyId(long id) {
        //    var result = await _businessManager.GetCustomers(id);
        //    return _mapper.Map<List<CustomerListViewModel>>(result);
        //}

        //[Obsolete]
        //[HttpGet]
        //[Route("bulk")]
        //public async Task<List<CustomerListViewModel>> GetBulkCustomers(long Id, DateTime from, DateTime to) {
        //    var result = await _businessManager.GetBulkCustomers(Id, from, to);
        //    return _mapper.Map<List<CustomerListViewModel>>(result);
        //}

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> CreateCustomers(CustomerBulkViewModel model) {
            var customerList = new List<CustomerViewModel>();

            try {
                if(ModelState.IsValid) {
                    //Получить данные из кэш
                    var cacheModel = _memoryCache.Get<CustomerBulkViewModel>("_CustomerUpload");
                    model.Rows = cacheModel?.Rows;

                    var customerTypes = await _nsiManager.GetCustomerTypes();
                    var customerTags = await _businessManager.GetCustomerTags();

                    for(var i = 0; i < model.Rows?.Count(); i++) {
                        var row = model.Rows[i];
                        var customer = new CustomerViewModel() {
                            CompanyId = model.CompanyId,
                        };

                        for(var j = 0; j < row.Count(); j++) {
                            var column = model.Columns[j];
                            if(column != null && !string.IsNullOrEmpty(column.Name) && row[j].Index == column.Index) {
                                var property = customer.GetType().GetProperty(column.Name);

                                if(property != null && property.CanWrite) {
                                    /*if(property.PropertyType == typeof(long)) {
                                        if(long.TryParse(row[j].Value, out long longValue)) {
                                            property.SetValue(customer, longValue);
                                        }
                                    } else*/
                                    if(property.PropertyType == typeof(double)) {
                                        if(double.TryParse(row[j].Value, out double doubleVal)) {
                                            property.SetValue(customer, doubleVal);
                                        }
                                    } else if(property.PropertyType == typeof(decimal)) {
                                        if(decimal.TryParse(row[j].Value, out decimal decimalVal)) {
                                            property.SetValue(customer, decimalVal);
                                        }
                                    } else if(property.PropertyType == typeof(int)) {
                                        if(int.TryParse(row[j].Value, out int intVal)) {
                                            property.SetValue(customer, intVal);
                                        }
                                    } else if(property.PropertyType == typeof(bool)) {
                                        if(bool.TryParse(row[j].Value, out bool boolVal)) {
                                            property.SetValue(customer, boolVal);
                                        }
                                    } else if(property.PropertyType == typeof(DateTime)) {
                                        if(DateTime.TryParse(row[j].Value, out DateTime dateVal)) {
                                            property.SetValue(customer, dateVal);
                                        }
                                    } else if(property.PropertyType == typeof(ICollection<long?>)) {
                                        if(property.Name.Equals("TagsIds")) {
                                            var tagsValues = row[j].Value.Split(',').Select(x => x.Trim()).ToList();
                                            var tagsIds = customerTags.Where(x => tagsValues.Contains(x.Name)).Select(x => x?.Id).ToList();
                                            if(tagsIds.Count() > 0)
                                                property.SetValue(customer, tagsIds);
                                        }
                                    } else {
                                        if(property.Name.Equals("TypeId")) {
                                            var ctype = customerTypes.Where(x => x.Name.ToLower().Equals(row[j].Value.ToLower()) || x.Code.ToLower().Equals(row[j].Value.ToLower())).FirstOrDefault();
                                            if(ctype != null) {
                                                var propertyTypeId = customer.GetType().GetProperty("TypeId");
                                                propertyTypeId.SetValue(customer, ctype.Id);
                                            }
                                        } else {
                                            property.SetValue(customer, row[j].Value);
                                        }
                                    }
                                }
                            }
                        }

                        if(TryValidateModel(customer)) {
                            customerList.Add(customer);
                        }
                    }
                }

                if(customerList.Count == 0) {
                    throw new Exception("No records have been created! Please, fill the required fields!");
                }

                var customerDtoList = _mapper.Map<List<CustomerDto>>(customerList);
                var result = await _businessManager.CreateOrUpdateCustomer(customerDtoList, model.Columns.Where(x => !string.IsNullOrEmpty(x.Name)).Select(x => x.Name).ToList());
                return Ok(_mapper.Map<List<CustomerViewModel>>(result));

            } catch(Exception e) {
                return BadRequest(e.Message ?? e.StackTrace);
            }
        }

        [HttpPost]
        [Route("createcredits")]
        public async Task<ActionResult> CreateCustomerCredits(CustomerCreditsBulkViewModel model) {
            var creditsList = new List<CustomerImportCreditsViewModel>();

            try {
                if(ModelState.IsValid) {
                    //Получить данные из кэш
                    var cacheModel = _memoryCache.Get<CustomerCreditsBulkViewModel>("_CustomerCreditsUpload");
                    model.Rows = cacheModel?.Rows;

                    for(var i = 0; i < model.Rows?.Count(); i++) {
                        var row = model.Rows[i];
                        var creditsModel = new CustomerImportCreditsViewModel() {
                            CompanyId = model.CompanyId,
                            CreatedDate = model.CreatedDate
                        };

                        for(var j = 0; j < row.Count(); j++) {
                            var column = model.Columns[j];
                            if(column != null && !string.IsNullOrEmpty(column.Name) && row[j].Index == column.Index) {
                                var property = creditsModel.GetType().GetProperty(column.Name);

                                if(property != null && property.CanWrite) {
                                    if(property.PropertyType == typeof(double)) {
                                        if(double.TryParse(row[j].Value, out double doubleVal)) {
                                            property.SetValue(creditsModel, doubleVal);
                                        }
                                    } else if(property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?)) {
                                        if(decimal.TryParse(row[j].Value, out decimal decimalVal)) {
                                            property.SetValue(creditsModel, decimalVal);
                                        }
                                    } else if(property.PropertyType == typeof(int)) {
                                        if(int.TryParse(row[j].Value, out int intVal)) {
                                            property.SetValue(creditsModel, intVal);
                                        }
                                    } else if(property.PropertyType == typeof(bool)) {
                                        if(bool.TryParse(row[j].Value, out bool boolVal)) {
                                            property.SetValue(creditsModel, boolVal);
                                        }
                                    } else if(property.PropertyType == typeof(DateTime)) {
                                        if(DateTime.TryParse(row[j].Value, out DateTime dateVal)) {
                                            property.SetValue(creditsModel, dateVal);
                                        }
                                    } else if(property.PropertyType == typeof(ICollection<long?>)) {

                                    } else {
                                        property.SetValue(creditsModel, row[j].Value);
                                    }
                                }
                            }
                        }

                        if(TryValidateModel(creditsModel)) {
                            creditsList.Add(creditsModel);
                        }
                    }
                }

                if(creditsList.Count == 0) {
                    throw new Exception("No records have been created! Please, fill the required fields!");
                }

                var customerDtoList = _mapper.Map<List<CustomerImportCreditsDto>>(creditsList);
                var result = await _businessManager.CreateOrUpdateCustomerCredits(customerDtoList, model.Columns.Where(x => !string.IsNullOrEmpty(x.Name)).Select(x => x.Name).ToList());
                return Ok(_mapper.Map<List<CustomerImportCreditsViewModel>>(result));

            } catch(Exception e) {
                return BadRequest(e.Message ?? e.StackTrace);
            }
        }
    }
}