using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using AutoMapper;

using Core.Context;
using Core.Data.Dto;
using Core.Extension;
using Core.Services.Business;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

using Web.Extension;
using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class SavedReportController: BaseController<SavedReportController> {
        private readonly ICompanyBusinessManager _companyBusinessManager;
        private readonly IReportBusinessManager _reportBusinessManager;
        private readonly ICustomerBusinessManager _customerBusinessManager;

        public SavedReportController(ILogger<SavedReportController> logger, IMapper mapper, ApplicationContext context,
            ICompanyBusinessManager companyBusinessManager, IReportBusinessManager reportBusinessManager,
            ICustomerBusinessManager customerBusinessManager
            ) : base(logger, mapper, context) {
            _companyBusinessManager = companyBusinessManager;
            _reportBusinessManager = reportBusinessManager;
            _customerBusinessManager = customerBusinessManager;
        }

        public async Task<IActionResult> Index() {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var companies = await _companyBusinessManager.GetCompanies();

            var savedReportFact = await _reportBusinessManager.GetSavedReport(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var savedReportFactList = savedReportFact.GroupBy(x => x.CompanyId, x => x.Name, (companyId, names) => new {
                Key = companyId ?? 0,
                Count = names.Count(),
                Name = names.First()
            }).Select(x => new SavedReportListViewModel() {
                Name = x.Name,
                CompanyId = x.Key,
                Count = x.Count
            }).ToList();

            var factCompanies = companies.Where(x => !savedReportFactList.Any(y => y.CompanyId == x.Id)).ToList();
            if(factCompanies.Count > 0)
                savedReportFactList.AddRange(factCompanies.Select(x => new SavedReportListViewModel() {
                    Name = x.Name,
                    CompanyId = x.Id,
                    Count = 0
                }));

            var savedReportPlan = await _reportBusinessManager.GetSavedPlanReport(userId);
            var savedReportPlanList = savedReportPlan.GroupBy(x => x.CompanyId, x => x.Name, (companyId, names) => new {
                Key = companyId ?? 0,
                Count = names.Count(),
                Name = names.First()
            }).Select(x => new SavedReportListViewModel() {
                Name = x.Name,
                CompanyId = x.Key,
                Count = x.Count
            }).ToList();

            var planCompanies = companies.Where(x => !savedReportPlanList.Any(y => y.CompanyId == x.Id)).ToList();
            if(planCompanies.Count > 0)
                savedReportPlanList.AddRange(planCompanies.Select(x => new SavedReportListViewModel() {
                    Name = x.Name,
                    CompanyId = x.Id,
                    Count = 0
                }));

            ViewBag.SavedReportFact = savedReportFactList;
            ViewBag.SavedReportPlan = savedReportPlanList;

            return View();
        }

        public async Task<IActionResult> DetailsFact(long id) {
            var company = await _companyBusinessManager.GetCompany(id);
            if(company == null) {
                return BadRequest();
            }
            ViewBag.CompanyName = company.Name;

            var result = await _reportBusinessManager.GetSavedReport(User.FindFirstValue(ClaimTypes.NameIdentifier), id);
            var customerTypes = await _customerBusinessManager.GetCustomerTypes();

            Dictionary<string, List<string>> rows = new Dictionary<string, List<string>>();
            rows.Add("Id", new List<string>());
            rows.Add("Name", new List<string>());

            //Add Customers Type fields
            rows.Add("Total Customers", new List<string>());
            foreach(var ctype in customerTypes) {
                rows.Add(ctype.Name, new List<string>());
            }

            foreach(var report in result) {
                foreach(var ctype in customerTypes) {
                    var containField = report.Fields.Any(x => x.Name.Equals(ctype.Name));
                    if(!containField) {
                        report.Fields.Add(new SavedReportFieldDto() {
                            Name = ctype.Name,
                            Value = ""
                        });
                    }
                }

                if(rows.ContainsKey("Id")) {
                    rows["Id"].Add(report.IsPublished ? "" : report.Id.ToString());
                }

                if(rows.ContainsKey("Name")) {
                    rows["Name"].Add(report.Date.ToString("MMM/dd/yyyy"));
                }

                foreach(var field in report.Fields) {
                    if(rows.ContainsKey(field.Name)) {
                        rows[field.Name].Add(field.Value);
                    } else {
                        rows.Add(field.Name, new List<string>() {
                            field.Value
                        });
                    }
                }

                var files = report.Files.Select(x => string.Format("{0}|{1}", x.Id, x.Name));
                if(rows.ContainsKey("Files")) {
                    rows["Files"].Add(string.Join(";", files));
                } else {
                    rows.Add("Files", new List<string>() {
                         string.Join(";", files)
                     });
                }
            }

            //var columns = result.Select(x => x.Date.ToString("MMM/dd/yyyy"));
            return View(rows);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFact(long id) {
            try {
                var item = await _reportBusinessManager.GetSavedReport(id);
                if(item == null)
                    return NotFound();

                var companyId = item.CompanyId;

                var result = await _reportBusinessManager.DeleteSavedReport(id);
                if(result == false) {
                    return NotFound();
                }
                return RedirectToAction(nameof(DetailsFact), new { companyId = companyId });

            } catch(Exception er) {
                _logger.LogError(er, er.Message);
                return BadRequest(er);
            }
        }

        public async Task<IActionResult> DetailsPlan(long id) {
            var company = await _companyBusinessManager.GetCompany(id);
            if(company == null) {
                return BadRequest();
            }
            ViewBag.CompanyName = company.Name;

            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var report = await _reportBusinessManager.GetSavedPlanReport(userId, id);
            var customerTypes = await _customerBusinessManager.GetCustomerTypes();

            var rows = new Dictionary<string, List<SavedReportFieldViewModel>>();
            rows.Add("Name", new List<SavedReportFieldViewModel>());
            rows.Add("Total Customers", new List<SavedReportFieldViewModel>());
            foreach(var ctype in customerTypes) {
                rows.Add(ctype.Name, new List<SavedReportFieldViewModel>());
            }

            foreach(var item in report) {
                if(rows.ContainsKey("Name")) {
                    rows["Name"].Add(new SavedReportFieldViewModel() {
                        Id = item.Id,
                        Name = item.Date.ToString("MMM/dd/yyyy"),
                    });
                }

                foreach(var field in item.Fields) {
                    if(rows.ContainsKey(field.Name)) {
                        rows[field.Name].Add(_mapper.Map<SavedReportFieldViewModel>(field));
                    } else {
                        rows.Add(field.Name, new List<SavedReportFieldViewModel>() { _mapper.Map<SavedReportFieldViewModel>(field) });
                    }
                }
            }

            return View(rows);
        }

        public async Task<IActionResult> CreatePlan() {
            var companies = await _companyBusinessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(new ReportFilterViewModel() {
                Date = DateTime.Now.LastDayOfMonth()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePlan(long id) {
            try {
                var item = await _reportBusinessManager.GetSavedPlanReport(id);
                if(item == null)
                    return NotFound();

                var companyId = item.CompanyId;

                var result = await _reportBusinessManager.DeleteSavedPlanReport(id);
                if(result == false) {
                    return NotFound();
                }
                return RedirectToAction(nameof(DetailsPlan), new { companyId = companyId });

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
    public class SavedReportController: ControllerBase {
        private readonly IMapper _mapper;
        private readonly IViewRenderService _viewRenderService;
        private readonly ICrudBusinessManager _businessManager;
        private readonly ICustomerBusinessManager _customerBusinessManager;
        private readonly IReportBusinessManager _reportBusinessManager;

        public SavedReportController(IMapper mapper, IViewRenderService viewRenderService,
             ICrudBusinessManager businessManager,
             ICustomerBusinessManager customerBusinessManager,
             IReportBusinessManager reportBusinessManager) {
            _mapper = mapper;
            _viewRenderService = viewRenderService;
            _businessManager = businessManager;
            _customerBusinessManager = customerBusinessManager;
            _reportBusinessManager = reportBusinessManager;
        }

        [HttpPost("GenerateSavedReportPlan", Name = "GenerateSavedReportPlan")]
        public async Task<IActionResult> GenerateSavedReportPlan(ReportFilterViewModel model) {
            try {
                if(!ModelState.IsValid) {
                    throw new Exception("Form is not valid!");
                }

                var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var item = await _reportBusinessManager.GetSavedPlanReport(userId, model.CompanyId, model.Date);
                var customerTypes = await _customerBusinessManager.GetCustomerTypes();

                SavedReportPlanViewModel result;
                if(item != null) {
                    result = _mapper.Map<SavedReportPlanViewModel>(item);
                    result.Fields.ForEach(x => {
                        if(x.Name.Equals("Total Customers")) {
                            x.CountReadOnly = true;
                            x.CountIsRequired = true;
                            x.AmountDisplay = false;
                        } else if(x.Name.Equals("Balance") || x.Name.Equals("No Balance")) {
                            x.CountReadOnly = true;
                            x.AmountDisplay = false;
                        } else if(x.Name.Equals("Total Late") || x.Name.Equals("Total")) {
                            x.AmountReadOnly = true;
                            x.CountReadOnly = true;
                        } else if(customerTypes.Any(y => y.Name.Equals(x.Name))) {
                            x.AmountDisplay = false;
                        }
                    });
                } else {
                    var fields = new List<SavedReportPlanFieldViewModel>();
                    fields.Add(new SavedReportPlanFieldViewModel() {
                        Name = "Total Customers",
                        CountReadOnly = true,
                        CountIsRequired = true,
                        AmountDisplay = false,
                    });

                    //Add customer types
                    foreach(var ctype in customerTypes) {
                        fields.Add(new SavedReportPlanFieldViewModel() {
                            Name = ctype.Name,
                            AmountDisplay = false
                        });
                    }

                    fields.Add(new SavedReportPlanFieldViewModel() {
                        Name = "Balance",
                        CountReadOnly = true,
                        AmountDisplay = false
                    });

                    fields.Add(new SavedReportPlanFieldViewModel() {
                        Name = "No Balance",
                        CountReadOnly = true,
                        AmountDisplay = false
                    });

                    //Add Balance
                    #region CREATE HEADERS
                    var daysPerPeriod = 30;

                    for(int i = -1; i < model.NumberOfPeriods; i++) {
                        var from = (i < 0 ? -1 : 1) + i * daysPerPeriod;
                        var to = (i + 1) * daysPerPeriod;

                        fields.Add(new SavedReportPlanFieldViewModel() {
                            Name = $"{from}-{to}"
                        });
                    }

                    fields.Add(new SavedReportPlanFieldViewModel() {
                        Name = $"{1 + model.NumberOfPeriods * daysPerPeriod}+",
                    });

                    fields.Add(new SavedReportPlanFieldViewModel() {
                        Name = "Total Late",
                        CountDisplay = true,
                        AmountReadOnly = true,
                        CountReadOnly = true
                    });

                    fields.Add(new SavedReportPlanFieldViewModel() {
                        Name = "Total",
                        CountDisplay = true,
                        AmountReadOnly = true,
                        CountReadOnly = true
                    });
                    #endregion

                    result = new SavedReportPlanViewModel() {
                        CompanyId = model.CompanyId,
                        Date = model.Date,
                        Fields = fields,
                        NumberOfPeriods = model.NumberOfPeriods,
                    };
                }

                string html = await _viewRenderService.RenderToStringAsync("_SavedReportPlanPartial", result);
                return Ok(html);
            } catch(Exception er) {
                return BadRequest(er.Message ?? er.StackTrace);
            }
        }

        [HttpPost("CreateSavedReportPlan", Name = "CreateSavedReportPlan")]
        public async Task<IActionResult> CreateSavedReportPlan(SavedReportPlanViewModel model) {
            try {
                if(!ModelState.IsValid) {
                    throw new Exception("Form is not valid!");
                }
                var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var result = await _reportBusinessManager.CreateSavedPlanReport(userId, _mapper.Map<SavedReportDto>(model));
                if(result == null)
                    return NotFound();

                return Ok(_mapper.Map<SavedReportPlanViewModel>(result));
            } catch(Exception er) {
                return BadRequest(er.Message ?? er.StackTrace);
            }
        }

        [HttpPost("PublishFact", Name = "PublishFact")]
        public async Task<IActionResult> PublishFact(long id) {
            var result = await _reportBusinessManager.UpdateSavedReport(id, new SavedReportDto() { IsPublished = true });
            return Ok(_mapper.Map<SavedReportDto>(result));
        }
    }
}