using System;
using System.Collections.Generic;
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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class CustomerController: BaseController<ReportController> {
        private readonly ICrudBusinessManager _businessManager;
        private readonly IViewRenderService _viewRenderService;

        public CustomerController(ILogger<ReportController> logger, IMapper mapper, ApplicationContext context,
             ICrudBusinessManager businessManager, IViewRenderService viewRenderService) : base(logger, mapper, context) {
            _businessManager = businessManager;
            _viewRenderService = viewRenderService;
        }

        // GET: Customer
        public ActionResult Index() {
            return View();
        }

        // GET: Customer/Details/5
        public ActionResult Details(long id) {
            return View();
        }

        // GET: Customer/Create
        public async Task<ActionResult> Create() {
            var item = new CustomerViewModel();

            var companies = await _businessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(item);
        }

        // POST: Customer/Create
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
                    //return RedirectToAction(nameof(Index));
                }

            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }
            return View(model);
        }

        // GET: Customer/Edit/5
        public async Task<ActionResult> Edit(long id) {
            var item = await _businessManager.GetCustomer(id);
            if(item == null) {
                return NotFound();
            }

            var companies = await _businessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(_mapper.Map<CustomerViewModel>(item));
        }

        // POST: Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, CustomerViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _businessManager.UpdateCustomer(id, _mapper.Map<CustomerDto>(model));
                    if(item == null) {
                        return NotFound();
                    }
                    model = _mapper.Map<CustomerViewModel>(item);
                    return RedirectToAction(nameof(Edit), new { id = item.Id });
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            var companies = await _businessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(model);
        }

        // POST: Customer/Delete/5
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
        public async Task<ActionResult> Upload([FromForm] IFormCollection files) {
            var customerList = new List<CustomerViewModel>();
            //using(var memoryStream = new MemoryStream()) {
            // await FileUpload.FormFile.CopyToAsync(memoryStream);
            var html = "";
            using(var reader = new System.IO.StreamReader(files.Files[0].OpenReadStream())) {
                try {
                    var result = await reader.ReadToEndAsync();

                    var dtoList = JsonConvert.DeserializeObject<List<CustomerDto>>(result);
                    customerList = _mapper.Map<List<CustomerViewModel>>(dtoList);
                    
                    html = _viewRenderService.RenderToStringAsync("_BulkCreatePartial", customerList).Result;

                } catch (Exception e) {
                    _logger.LogError(e, e.Message);
                }
            }
            //return View("_BulkCreatePartial", customerList);
            return Json(html);
        }
    }
}

namespace Web.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController: ControllerBase {
        private readonly IMapper _mapper;
        private readonly IViewRenderService _viewRenderService;
        private readonly ICrudBusinessManager _businessManager;

        public CustomerController(IMapper mapper, IViewRenderService viewRenderService, ICrudBusinessManager businessManager) {
            _mapper = mapper;
            _viewRenderService = viewRenderService;
            _businessManager = businessManager;
        }

        [HttpGet]
        public async Task<Pager<CustomerListViewModel>> GetCustomers([FromQuery] PagerFilterViewModel model) {
            var result = await _businessManager.GetCustomersPage(_mapper.Map<PagerFilter>(model));
            var pager = new Pager<CustomerListViewModel>(_mapper.Map<List<CustomerListViewModel>>(result.Items), result.TotalItems, result.CurrentPage, result.PageSize);
            return pager;
        }

        [HttpGet]
        [Route("company/{id}")]
        public async Task<List<CustomerListViewModel>> GetCustomersByCompanyId(long id) {
            var result = await _businessManager.GetCustomers(id);
            return _mapper.Map<List<CustomerListViewModel>>(result);
        }

        [HttpGet]
        [Route("bulk")]
        public async Task<List<CustomerDto>> GetBulkCustomers(long Id, DateTime from, DateTime to) {
            var reuslt = await _businessManager.GetBulkCustomers(Id, from, to);
            return reuslt;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<ActionResult> Upload([FromForm] IFormCollection files) {
            var customerList = new List<CustomerViewModel>();
            using(var reader = new System.IO.StreamReader(files.Files[0].OpenReadStream())) {
                try {
                    var result = await reader.ReadToEndAsync();

                    var dtoList = JsonConvert.DeserializeObject<List<CustomerDto>>(result);
                    customerList = _mapper.Map<List<CustomerViewModel>>(dtoList);
                } catch(Exception e) {
                    //_logger.LogError(e, e.Message);
                }
            }

            string html = _viewRenderService.RenderToStringAsync("_BulkInvoicePartial", customerList).Result;
            return Ok(html);
        }
    }
}