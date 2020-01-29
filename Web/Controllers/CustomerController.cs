using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using Core.Context;
using Core.Data.Dto;
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

        [HttpGet]
        public async Task<List<CustomerViewModelList>> GetCustomers(string search, string order, int offset = 0, int limit = 10) {
            Dictionary<string, string> _filter = new Dictionary<string, string>();
            //var filter = "";
            //if(filter != null) {
            //    var _pairs = filter.Split('|');
            //    foreach(var _pair in _pairs) {
            //        var _splittedPair = _pair.Split(':');
            //        _filter.Add(_splittedPair[0], _splittedPair[1]);
            //    }
            //}

            var result = await _businessManager.GetCustomers();
            //var result = await _businessManager.GetCustomersPage(search, order, offset, limit);
            return _mapper.Map<List<CustomerViewModelList>>(result);
        }
    }
}