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
    public class CompanyController: BaseController<CompanyController> {
        private readonly ICompanyBusinessManager _companyBusinessManager;
        private readonly ICrudBusinessManager _businessManager;

        public CompanyController(ILogger<CompanyController> logger, IMapper mapper, ApplicationContext context,
            ICompanyBusinessManager companyBusinessManager, ICrudBusinessManager businessManager) : base(logger, mapper, context) {
            _companyBusinessManager = companyBusinessManager;
            _businessManager = businessManager;
        }

        // GET: Company
        public IActionResult Index() {
            return View();
        }

        // GET: Company/Details/5
        public async Task<IActionResult> Details(long id) {
            var item = await _companyBusinessManager.GetCompany(id);

            if(item == null) {
                return NotFound();
            }
            var customers = await _businessManager.GetCustomers(item.Customers.Select(x => x.Id).ToArray());
            ViewBag.Customers = customers;

            return View(_mapper.Map<CompanyViewModel>(item));
        }

        // GET: Company/Create
        public async Task<IActionResult> Create() {
            var customers = await _businessManager.GetUntiedCustomers(null);
            ViewBag.Customers = customers.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(new CompanyViewModel());
        }

        // POST: Company/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _companyBusinessManager.CreateCompany(_mapper.Map<CompanyDto>(model));
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

        // GET: Company/Edit/5
        public async Task<IActionResult> Edit(long id) {
            var item = await _companyBusinessManager.GetCompany(id);
            if(item == null) {
                return NotFound();
            }

            var summary = await _companyBusinessManager.GetSummaryRanges(item.Id);
            ViewBag.Summary = summary;

            var exportSetting = await _companyBusinessManager.GetAllExportSettings(item.Id);
            ViewBag.ExportSettings = exportSetting;

            var creditUtilizedSettings = await _businessManager.GetCustomerCreditUtilizedSettingsList(item.Id);
            ViewBag.CreditUtilizedSettings = _mapper.Map<List<CustomerCreditUtilizedSettingsViewModel>>(creditUtilizedSettings);

            var model = _mapper.Map<CompanyViewModel>(item);

            return View(model);
        }

        // POST: Company/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, CompanyViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var dto = _mapper.Map<CompanyDto>(model);
                    var item = await _companyBusinessManager.UpdateCompany(id, dto);
                    if(item == null) {
                        return NotFound();
                    }
                    return RedirectToAction(nameof(Edit), new { id = item.Id });
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            //TODO: Change user interface on editing 
            var address = await _companyBusinessManager.GetAddress(id);
            ViewBag.Address = address;

            var settings = await _companyBusinessManager.GetSettings(id);
            ViewBag.Settings = settings;

            var summary = await _companyBusinessManager.GetSummaryRanges(id);
            ViewBag.Summary = summary;

            var exportSetting = await _companyBusinessManager.GetAllExportSettings(id);
            ViewBag.ExportSettings = exportSetting;

            return View(model);
        }

        // POST: Company/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id) {
            try {
                var item = await _companyBusinessManager.DeleteCompany(id);
                if(item == false) {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));

            } catch(Exception er) {
                _logger.LogError(er, er.Message);
                return BadRequest(er);
            }
        }

        #region ADDRESS
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAddress(long companyId, CompanyAddressViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var dto = _mapper.Map<CompanyAddressDto>(model);
                    var item = await _companyBusinessManager.UpdateAddress(companyId, dto);
                    if(item == null) {
                        return BadRequest();
                    }
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            return RedirectToAction(nameof(Edit), new { Id = companyId });
        }
        #endregion

        #region SETTINGS
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditSettings(long companyId, CompanySettingsViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var dto = _mapper.Map<CompanySettingsDto>(model);
                    var item = await _companyBusinessManager.UpdateSettings(companyId, dto);
                    if(item == null) {
                        return BadRequest();
                    }
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            return RedirectToAction(nameof(Edit), new { Id = companyId });
        }
        #endregion

        #region SUMMARY RANGE
        [HttpGet]
        [Route("{companyId}/summary")]
        public async Task<IActionResult> CreateSummary(long companyId) {
            var item = await _companyBusinessManager.GetCompany(companyId);

            if(item == null) {
                return NotFound();
            }

            ViewBag.CompanyName = item.Name;

            var model = new CompanySummaryRangeViewModel() {
                CompanyId = companyId
            };
            return View(model);
        }

        [HttpPost]
        [Route("{companyId}/summary")]
        public async Task<IActionResult> CreateSummary(CompanySummaryRangeViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _companyBusinessManager.CreateSummaryRange(_mapper.Map<CompanySummaryRangeDto>(model));
                    if(item == null) {
                        return BadRequest();
                    }

                    return RedirectToAction(nameof(Edit), new { Id = model.CompanyId });
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            return View(model);
        }

        public async Task<IActionResult> EditSummary(long id) {
            var item = await _companyBusinessManager.GetSummaryRange(id);
            if(item == null) {
                return NotFound();
            }
            var company = await _companyBusinessManager.GetCompany(item.CompanyId);
            if(company == null) {
                return NotFound();
            }

            ViewBag.CompanyName = company.Name;

            return View(_mapper.Map<CompanySummaryRangeViewModel>(item));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSummary(long id, CompanySummaryRangeViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var dto = _mapper.Map<CompanySummaryRangeDto>(model);
                    var item = await _companyBusinessManager.UpdateSummaryRange(id, dto);
                    if(item == null) {
                        return NotFound();
                    }
                    return RedirectToAction(nameof(EditSummary), new { Id = item.Id });
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
                return BadRequest(er);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSummary(long id) {
            try {
                var item = await _companyBusinessManager.GetSummaryRange(id);
                if(item == null) {
                    return NotFound();
                }

                var result = await _companyBusinessManager.DeleteSummaryRange(item.Id);
                return RedirectToAction(nameof(Edit), new { Id = item.CompanyId });

            } catch(Exception er) {
                _logger.LogError(er, er.Message);
                return BadRequest(er);
            }
        }
        #endregion

        #region EXPORT SETTINGS
        [Route("{id}/ExportSettings/Create")]
        public async Task<IActionResult> CreateExportSettings(long id) {
            var item = await _companyBusinessManager.GetCompany(id);

            if(item == null) {
                return NotFound();
            }

            var dto = new CompanyExportSettingsDto() {
                CompanyId = id,
                Name = "Default Name",
                Title = string.Format("Report-{0}.csv", item.Name),
                Sort = 0,
                IncludeAllCustomers = false
            };

            dto = await _companyBusinessManager.CreateExportSettings(dto);

            return RedirectToAction(nameof(EditExportSettings), new { Id = dto.Id });
        }

        [Route("ExportSettings/{id}")]
        public async Task<IActionResult> EditExportSettings(long id) {
            var dto = await _companyBusinessManager.GetExportSettings(id);
            if(dto == null) {
                return NotFound();
            }

            var company = await _companyBusinessManager.GetCompany(dto.CompanyId ?? 0);
            ViewBag.CompanyName = company.Name;

            return View(_mapper.Map<CompanyExportSettingsViewModel>(dto));
        }

        [HttpPost]
        [Route("ExportSettings/{id}")]
        public async Task<IActionResult> EditExportSettings(long id, CompanyExportSettingsViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var item = await _companyBusinessManager.UpdateExportSettings(id, _mapper.Map<CompanyExportSettingsDto>(model));
                    if(item == null) {
                        return BadRequest();
                    }
                    return RedirectToAction(nameof(EditExportSettings), new { Id = model.Id });
                } else {
                    var message = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    _logger.LogError(message);
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
            }

            var company = await _companyBusinessManager.GetCompany(model.CompanyId);
            ViewBag.CompanyName = company.Name;

            return View(model);
        }

        [HttpPost]
        [Route("ExportSettings/{id}/Delete")]
        public async Task<IActionResult> DeleteExportSettings(long id) {
            var item = await _companyBusinessManager.GetExportSettings(id);
            if(item == null) {
                return BadRequest();
            }

            await _companyBusinessManager.DeleteExportSettings(id);

            return RedirectToAction(nameof(Edit), new { Id = item.CompanyId });
        }

        [Route("ExportSettingsField/{id}/Create")]
        public async Task<IActionResult> CreateExportSettingsField(long id) {
            var item = await _companyBusinessManager.GetExportSettings(id);

            if(item == null) {
                return NotFound();
            }

            var dto = new CompanyExportSettingsFieldDto() {
                Name = "Name",
                Value = "Value",
                ExportSettingsId = item.Id,
                IsActive = true,
                IsEditable = true,
                Sort = item.Fields?.Count ?? 0
            };

            dto = await _companyBusinessManager.CreateExportSettingsField(dto);

            return View("_CompanyExportSettingsFieldPartial", _mapper.Map<CompanyExportSettingsFieldViewModel>(dto));
        }

        [HttpGet]
        [Route("ExportSettingsField/{id}/Delete")]
        public async Task<IActionResult> DeleteExportSettingsField(long id) {
            var item = await _companyBusinessManager.GetExportSettingsField(id);
            if(item == null) {
                return BadRequest();
            }

            await _companyBusinessManager.DeleteExportSettingsField(id);

            return RedirectToAction(nameof(EditExportSettings), new { Id = item.ExportSettingsId });
        }
        #endregion
    }
}

namespace Web.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController: ControllerBase {
        private readonly IMapper _mapper;
        private readonly ICompanyBusinessManager _companyBusinessManager;
        private readonly ICrudBusinessManager _businessManager;

        public CompanyController(IMapper mapper, ICompanyBusinessManager companyBusinessManager,
            ICrudBusinessManager businessManager) {
            _mapper = mapper;
            _companyBusinessManager = companyBusinessManager;
            _businessManager = businessManager;
        }

        [HttpGet]
        //public async Task<Pager<CompanyListViewModel>> GetCompanies([FromQuery] PagerFilterViewModel model) {
        public async Task<Pager<CompanyListViewModel>> GetCompanies(string search, string sort, string order, int offset = 0, int limit = 10) {
            var result = await _companyBusinessManager.GetCompanyPage(search ?? "", sort, order, offset, limit);
            var pager = new Pager<CompanyListViewModel>(_mapper.Map<List<CompanyListViewModel>>(result.Items), result.TotalItems, result.CurrentPage, result.PageSize);
            return pager;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<CompanyViewModel> GetCompany(long id) {
            var result = await _companyBusinessManager.GetCompany(id);
            return _mapper.Map<CompanyViewModel>(result);
        }

        [HttpGet]
        [Route("{id}/summaryrange")]
        public async Task<List<CompanySummaryRangeDto>> GetRangeByCompanyId(long id) {
            var result = await _companyBusinessManager.GetSummaryRanges(id);
            return _mapper.Map<List<CompanySummaryRangeDto>>(result);
        }
    }
}