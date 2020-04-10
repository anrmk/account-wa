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
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class CustomerTagController: BaseController<CustomerTagController> {
        private readonly ICrudBusinessManager _businessManager;

        public CustomerTagController(ILogger<CustomerTagController> logger, IMapper mapper, IMemoryCache memoryCache, ApplicationContext context,
            ICrudBusinessManager businessManager) : base(logger, mapper, context) {
            _businessManager = businessManager;
        }

        public IActionResult Index() {
            return View();
        }

        public ActionResult Create() {
            return View(new CustomerTagViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CustomerTagViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _businessManager.CreateCustomerTag(_mapper.Map<CustomerTagDto>(model));
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

        public async Task<IActionResult> Edit(long id) {
            var item = await _businessManager.GetCustomerTag(id);
            if(item == null) {
                return NotFound();
            }

            var model = _mapper.Map<CustomerTagViewModel>(item);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, CustomerTagViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var dto = _mapper.Map<CustomerTagDto>(model);
                    var item = await _businessManager.UpdateCustomerTag(id, dto);
                    if(item == null) {
                        return NotFound();
                    }
                    
                    return RedirectToAction(nameof(Edit), new { id = item.Id });
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(long id) {
            try {
                var item = await _businessManager.DeleteCustomerTag(id);
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
    public class CustomerTagController: ControllerBase {
        private readonly IMapper _mapper;
        private readonly ICrudBusinessManager _businessManager;

        public CustomerTagController(IMapper mapper, ICrudBusinessManager businessManager) {
            _mapper = mapper;
            _businessManager = businessManager;
        }

        [HttpGet]
        public async Task<Pager<CustomerTagViewModel>> GetCustomerTags([FromQuery] PagerFilterViewModel model) {
            var result = await _businessManager.GetCustomerTags(_mapper.Map<PagerFilter>(model));
            var pager = new Pager<CustomerTagViewModel>(_mapper.Map<List<CustomerTagViewModel>>(result.Items), result.TotalItems, result.CurrentPage, result.PageSize);
            return pager;
        }
    }
}