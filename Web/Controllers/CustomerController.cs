using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using Core.Context;
using Core.Data.Dto;
using Core.Extension;
using Core.Services.Business;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class CustomerController: BaseController<ReportController> {
        public ICrudBusinessManager _businessManager;
        public CustomerController(ILogger<ReportController> logger, IMapper mapper, ApplicationContext context,
             ICrudBusinessManager businessManager) : base(logger, mapper, context) {
            _businessManager = businessManager;
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
        public ActionResult Create() {
            var item = new CustomerViewModel();

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
                    return RedirectToAction(nameof(Index));
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

                    return RedirectToAction(nameof(Index));
                }

            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }
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
    }
}

namespace Web.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController: ControllerBase {
        private readonly IMapper _mapper;
        private readonly ICrudBusinessManager _businessManager;

        public CustomerController(IMapper mapper, ICrudBusinessManager businessManager) {
            _mapper = mapper;
            _businessManager = businessManager;
        }

        /// <summary>
        /// Get list of Customers
        /// </summary>
        /// <param name="search"></param>
        /// <param name="order"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Pager<CustomerListViewModel>> GetCustomers(string search, string sort, string order, int offset = 0, int limit = 10) {
            var result = await _businessManager.GetCustomersPage(search ?? "", sort, order, offset, limit);
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
    }
}