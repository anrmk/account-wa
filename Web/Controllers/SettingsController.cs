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

using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class SettingsController: BaseController<SettingsController> {
        private readonly ICrudBusinessManager _businessManager;
        private readonly ICompanyBusinessManager _companyBusinessManager;
        private readonly ISettingsBusinessService _settingsBusinessService;

        public SettingsController(ILogger<SettingsController> logger, IMapper mapper, ApplicationContext context,
          ICrudBusinessManager crudBusinessManager,
          ICompanyBusinessManager companyBusinessManager,
          ISettingsBusinessService settingsBusinessManaer) : base(logger, mapper, context) {
            _companyBusinessManager = companyBusinessManager;
            _settingsBusinessService = settingsBusinessManaer;
            _businessManager = crudBusinessManager;
        }

        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateSettingsRestrictedWord() {
            var companies = await _companyBusinessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSettingsRestrictedWord(SettingsRestrictedWordViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _settingsBusinessService.CreateRestrictedWord(_mapper.Map<SettingsRestrictedWordDto>(model));
                    if(item == null) {
                        return BadRequest();
                    }

                    if(model.CompanyIds != null && model.CompanyIds.Count > 0) {
                        var restrictedWords = await _companyBusinessManager.UpdateRestrictedWords(item.Id, model.CompanyIds.ToArray());
                    }

                    return RedirectToAction(nameof(Index));
                }

            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }
            return View(model);
        }

        public async Task<IActionResult> EditSettingsRestrictedWord(long id) {
            var item = await _settingsBusinessService.GetRestrictedWord(id);
            if(item == null) {
                return NotFound();
            }

            var model = _mapper.Map<SettingsRestrictedWordViewModel>(item);

            var companies = await _companyBusinessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var companyRestrictedWords = await _companyBusinessManager.GetRestrictedWord(id);
            model.CompanyIds = companyRestrictedWords.Select(x => x.CompanyId).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSettingsRestrictedWord(long id, SettingsRestrictedWordViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _settingsBusinessService.UpdateRestrictedWord(id, _mapper.Map<SettingsRestrictedWordDto>(model));

                    if(item == null) {
                        return BadRequest();
                    }
                    var restrictedWords = await _companyBusinessManager.UpdateRestrictedWords(item.Id, model.CompanyIds?.ToArray() ?? new long?[] { });
                }
            } catch(Exception er) {
                BadRequest(er.Message);
            }

            return RedirectToAction(nameof(Index), new { Id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteSettingsRestrictedWord(long id) {
            try {
                var item = await _settingsBusinessService.DeleteRestrictedWord(new long[] { id });
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
    public class SettingsController: ControllerBase {
        private readonly IMapper _mapper;
        private readonly ISettingsBusinessService _settingsBusinessService;

        public SettingsController(IMapper mapper,
           ISettingsBusinessService customerBusinessService) {
            _mapper = mapper;
            _settingsBusinessService = customerBusinessService;
        }

        [HttpGet("GetSettingsRestrictedWords", Name = "GetSettingsRestrictedWords")]
        public async Task<Pager<SettingsRestrictedWordViewModel>> GetSettingsRestrictedWords([FromQuery] PagerFilterViewModel model) {
            var result = await _settingsBusinessService.GetRestrictedWordPage(_mapper.Map<PagerFilterDto>(model));
            var pager = new Pager<SettingsRestrictedWordViewModel>(_mapper.Map<List<SettingsRestrictedWordViewModel>>(result.Items), result.TotalItems, result.CurrentPage, result.PageSize);
            return pager;
        }

        [HttpGet("DeleteSettingsRestrictedWord", Name = "DeleteSettingsRestrictedWord")]
        public async Task<ActionResult> DeleteSettingsRestrictedWord([FromQuery] long[] id) {
            if(id.Length > 0) {
                var result = await _settingsBusinessService.DeleteRestrictedWord(id);
                if(result)
                    return Ok(id);
            }
            return BadRequest("No items selected");
        }
    }
}