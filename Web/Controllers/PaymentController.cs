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
    public class PaymentController: BaseController<PaymentController> {
        public ICrudBusinessManager _businessManager;

        public PaymentController(ILogger<PaymentController> logger, IMapper mapper, ApplicationContext context, ICrudBusinessManager businessManager) : base(logger, mapper, context) {
            _businessManager = businessManager;
        }

        public IActionResult Index() {
            return View();
        }

        public async Task<IActionResult> Details(long id) {
            var item = await _businessManager.GetPayment(id);
            if(item ==null) {
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
                Ref = BitConverter.ToString(bytes).Replace("-", ""),
                Date = DateTime.Now
            };

            return View(paymentModel);
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
    }
}

namespace Web.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController: ControllerBase {
        private readonly IMapper _mapper;
        private readonly ICrudBusinessManager _businessManager;

        public PaymentController(IMapper mapper, ICrudBusinessManager businessManager) {
            _mapper = mapper;
            _businessManager = businessManager;
        }

        [HttpGet]
        public async Task<List<PaymentViewModelList>> GetPayments() {
            var result = await _businessManager.GetPayments();
            return _mapper.Map<List<PaymentViewModelList>>(result);
        }
    }
}