﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

using AutoMapper;

using Core.Context;
using Core.Data.Dto;
using Core.Extension;
using Core.Services.Business;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IActionResult Index() {
            return View();
        }

        public async Task<IActionResult> DetailsFact(long id) {
            var company = await _companyBusinessManager.GetCompany(id);
            if(company == null) {
                return BadRequest();
            }
            return View(_mapper.Map<CompanyViewModel>(company));
        }

        public async Task<IActionResult> DetailsPlan(long id) {
            var company = await _companyBusinessManager.GetCompany(id);
            if(company == null) {
                return BadRequest();
            }
            ViewBag.CompanyId = company.Id;
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

        public async Task<IActionResult> CreatePlan(long id) {
            var companies = await _companyBusinessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var savedReportItem = await _reportBusinessManager.GetSavedPlanReport(id);
            var model = new ReportFilterViewModel() {
                Date = DateTime.Now.LastDayOfMonth()
            };

            if(savedReportItem != null) {
                model.Date = savedReportItem.Date;
                model.CompanyId = savedReportItem.CompanyId ?? 0;
                model.NumberOfPeriods = savedReportItem.NumberOfPeriods;
            }

            return View(model);
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

        [HttpGet("Download", Name = "Download")]
        public async Task<IActionResult> Download(long id) {
            var item = await _reportBusinessManager.GetSavedFactFile(id);
            if(item == null) {
                return NotFound();
            }

            MemoryStream ms = new MemoryStream();
            ms.Write(item.File, 0, item.File.Length);
            ms.Position = 0;

            FileStreamResult fileStreamResult = new FileStreamResult(ms, "application/octet-stream");
            fileStreamResult.FileDownloadName = item.Name;

            return fileStreamResult;
        }
    }
}

namespace Web.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class SavedReportController: ControllerBase {
        private readonly IMapper _mapper;
        private readonly IViewRenderService _viewRenderService;
        private readonly ICompanyBusinessManager _companyBusinessManager;
        private readonly ICrudBusinessManager _businessManager;
        private readonly ICustomerBusinessManager _customerBusinessManager;
        private readonly IReportBusinessManager _reportBusinessManager;

        public SavedReportController(IMapper mapper, IViewRenderService viewRenderService,
             ICompanyBusinessManager companyBusinessManager,
             ICrudBusinessManager businessManager,
             ICustomerBusinessManager customerBusinessManager,
             IReportBusinessManager reportBusinessManager) {
            _mapper = mapper;
            _viewRenderService = viewRenderService;
            _companyBusinessManager = companyBusinessManager;
            _businessManager = businessManager;
            _customerBusinessManager = customerBusinessManager;
            _reportBusinessManager = reportBusinessManager;
        }

        [HttpGet("GetSavedReportFactList", Name = "GetSavedReportFactList")]
        public async Task<List<SavedReportListViewModel>> GetSavedReportFactList([FromQuery] ReportFilterViewModel model) {
            var companies = await _companyBusinessManager.GetCompanies();
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var list = await _reportBusinessManager.GetSavedFactReport(userId);
            var result = list.GroupBy(x => x.CompanyId, x => x.Name, (companyId, names) => new {
                Name = names.First(),
                Key = companyId ?? 0,
                Count = names.Count()
            }).Select(x => new SavedReportListViewModel() {
                Name = x.Name,
                CompanyId = x.Key,
                Count = x.Count
            }).ToList();

            var factCompanies = companies.Where(x => !result.Any(y => y.CompanyId == x.Id)).ToList();
            if(factCompanies.Count > 0)
                result.AddRange(factCompanies.Select(x => new SavedReportListViewModel() {
                    Name = x.Name,
                    CompanyId = x.Id,
                    Count = 0
                }));

            return result;
        }

        [HttpGet("GetSavedReportPlanList", Name = "GetSavedReportPlanList")]
        public async Task<List<SavedReportListViewModel>> GetSavedReportPlanList([FromQuery] ReportFilterViewModel model) {
            var companies = await _companyBusinessManager.GetCompanies();
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var list = await _reportBusinessManager.GetSavedPlanReport(userId);
            var result = list.GroupBy(x => x.CompanyId, x => x.Name, (companyId, names) => new {
                Key = companyId ?? 0,
                Count = names.Count(),
                Name = names.First()
            }).Select(x => new SavedReportListViewModel() {
                Name = x.Name,
                CompanyId = x.Key,
                Count = x.Count
            }).ToList();

            var planCompanies = companies.Where(x => !result.Any(y => y.CompanyId == x.Id)).ToList();
            if(planCompanies.Count > 0)
                result.AddRange(planCompanies.Select(x => new SavedReportListViewModel() {
                    Name = x.Name,
                    CompanyId = x.Id,
                    Count = 0
                }));

            return result;
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
                model.Fields.ForEach(x => {
                    if(x.Name.Equals("Total Customers") && x.Count < 1) {
                        ModelState.AddModelError("Count", "Total Customers field must be more than 0");
                    } else if(x.Name.Equals("Total") && x.Count < 1) {
                        ModelState.AddModelError("Count", "Total field must be more than 0");
                    }
                });

                if(!ModelState.IsValid) {
                    var allErrors = ModelState.Values.SelectMany(v => v.Errors);
                    var errors = allErrors.Select(x => x.ErrorMessage).ToList();
                    throw new Exception($"Form is not valid!\n{string.Join('\n', errors)}");
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

        #region FACT
        [HttpGet("GetSavedReportFact", Name = "GetSavedReportFact")]
        public async Task<IActionResult> GetSavedReportFact(long id) {
            try {
                var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var result = await _reportBusinessManager.GetSavedFactReport(userId, id);
                var customerTypes = await _customerBusinessManager.GetCustomerTypes();

                var columns = result.Select(x => x.Date.ToString("MMM.dd.yyyy")).ToList();

                var dictRows = new Dictionary<string, List<object>>();
                foreach(var report in result) {
                    //extend fields by customerTypes
                    foreach(var ctypes in customerTypes) {
                        if(!report.Fields.Any(x => x.Name.Equals(ctypes.Name))) {
                            report.Fields.Add(new SavedReportFieldDto() {
                                Name = ctypes.Name,
                                ReportId = report.Id
                            });
                        }
                    }

                    foreach(var field in report.Fields) {
                        if(!dictRows.ContainsKey(field.Name)) {
                            dictRows.Add(field.Name, new List<object>());
                        }
                        dictRows[field.Name].Add(new { Count = field.Count, Amount = field.Amount?.ToCurrency() ?? null });
                    }

                    #region FILES
                    if(!dictRows.ContainsKey("Files")) {
                        dictRows.Add("Files", new List<object>());
                    }
                    dictRows["Files"].Add(report.Files.Select(x => new { Id = x.Id, Name = x.Name }).ToArray());
                    #endregion

                    #region ID/ACTION
                    if(!dictRows.ContainsKey("Id")) {
                        dictRows.Add("Id", new List<object>());
                    }
                    dictRows["Id"].Add(new { Id = report.Id, IsPublished = report.IsPublished });
                    #endregion
                }

                #region EXPAND TABLE
                var rows = new List<object>();
                foreach(var key in dictRows.Keys) {
                    var obj = new Dictionary<string, object>();
                    obj.Add("Name", key);

                    for(var i = 0; i < columns.Count; i++) {
                        var columnName = columns[i];
                        obj.Add(columnName, dictRows[key][i]);
                    }
                    rows.Add(obj);
                }
                #endregion

                return Ok(new { items = rows, columns = columns.Prepend("Name").Select(x => new { field = x, title = x }) });
            } catch(Exception er) {
                return BadRequest(er.Message ?? er.StackTrace);
            }
        }

        [HttpPost("PublishSavedReportFact", Name = "PublishSavedReportFact")]
        public async Task<IActionResult> PublishSavedReportFact(long id) {
            var result = await _reportBusinessManager.UpdateSavedFactReport(id, new SavedReportDto() { IsPublished = true });
            return Ok(_mapper.Map<SavedReportDto>(result));
        }

        [HttpDelete("DeleteSavedReportFact", Name = "DeleteSavedReportFact")]
        public async Task<IActionResult> DeleteSavedReportFact([FromQuery] long id) {
            var result = await _reportBusinessManager.DeleteSavedFactReport(id);
            if(result)
                return Ok(id);

            return BadRequest("No items selected");
        }

        #endregion
    }
}