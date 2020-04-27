using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using AutoMapper;

using Core.Context;
using Core.Data.Dto;
using Core.Extension;
using Core.Services.Business;
using Core.Services.Managers;

using CsvHelper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using Web.Extension;
using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class ReportController: BaseController<ReportController> {
        private readonly int _daysPerPeriod = 30;

        private readonly INsiBusinessManager _nsiBusinessManager;
        private readonly IMemoryCache _memoryCache;
        private readonly ICrudBusinessManager _crudBusinessManager;
        private readonly IReportBusinessManager _reportBusinessManager;
        private readonly IReportManager _reportManager;

        public ReportController(IMemoryCache memoryCache, ILogger<ReportController> logger, IMapper mapper, ApplicationContext context,
            INsiBusinessManager nsiBusinessManager, ICrudBusinessManager crudBusinessManager, IReportBusinessManager businessManager, IReportManager reportManager) : base(logger, mapper, context) {
            _memoryCache = memoryCache;
            _crudBusinessManager = crudBusinessManager;
            _nsiBusinessManager = nsiBusinessManager;
            _reportBusinessManager = businessManager;
            _reportManager = reportManager;
        }

        public async Task<IActionResult> Index() {
            var companies = await _crudBusinessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(new ReportFilterViewModel() {
                Date = DateTime.Now.LastDayOfMonth()
            });
        }

        public async Task<IActionResult> Saved() {
            var result = await _crudBusinessManager.GetSavedReport(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var savedReportList = result.GroupBy(x => x.CompanyId, x => x.Name, (companyId, names) => new {
                Key = companyId ?? 0,
                Count = names.Count(),
                Name = names.First()
            }).Select(x => new SavedReportListViewModel() {
                Name = x.Name,
                CompanyId = x.Key,
                Count = x.Count
            }).ToList();

            var comanies = await _crudBusinessManager.GetCompanies();
            comanies = comanies.Where(x => !savedReportList.Any(y => y.CompanyId == x.Id)).ToList();

            if(comanies.Count > 0)
                savedReportList.AddRange(comanies.Select(x => new SavedReportListViewModel() {
                    Name = x.Name,
                    CompanyId = x.Id,
                    Count = 0
                }));

            return View(savedReportList);
        }

        public async Task<IActionResult> View(long companyId) {
            var company = await _crudBusinessManager.GetCompany(companyId);
            if(company == null) {
                return BadRequest();
            }
            ViewData["Title"] = company.Name;

            var result = await _crudBusinessManager.GetSavedReport(User.FindFirstValue(ClaimTypes.NameIdentifier), companyId);

            Dictionary<string, List<string>> rows = new Dictionary<string, List<string>>();
            rows.Add("Name", new List<string>());
            rows.Add("Id", new List<string>());
            foreach(var report in result) {
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

            var columns = result.Select(x => x.Date.ToString("MMM/dd/yyyy"));
            return View(rows);
        }

        [HttpPost]
        [Route("exportSettings")]
        public async Task<IActionResult> ExportSettings([FromBody]ReportFilterViewModel model) {
            var companyId = model.CompanyId;

            var company = await _crudBusinessManager.GetCompany(companyId);
            if(company == null) {
                return NotFound();
            }

            //var periods = await _nsiBusinessManager.GetReportPeriods();
            var settings = await _crudBusinessManager.GetCompanyAllExportSettings(company.Id);
            ViewBag.Settings = _mapper.Map<List<CompanyExportSettingsViewModel>>(settings);

            return View("_ExportSettingsPartial", model);
        }

        [HttpPost]
        [Route("savedReport")]
        public async Task<IActionResult> SavedReport([FromBody]ReportFilterViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var company = await _crudBusinessManager.GetCompany(model.CompanyId);
                    if(company == null) {
                        return NotFound();
                    }

                    var savedItem = await _crudBusinessManager.GetSavedReport(User.FindFirstValue(ClaimTypes.NameIdentifier), model.CompanyId, model.Date);
                    if(savedItem != null && savedItem.IsPublished) {
                        return View("_SavedReportPartial", _mapper.Map<SavedReportViewModel>(savedItem));
                    }

                    var result = new SavedReportViewModel() {
                        CompanyId = model.CompanyId,
                        Name = company.Name,
                        Date = model.Date,
                        NumberOfPeriods = model.NumberOfPeriods
                    };

                    var settings = await _crudBusinessManager.GetCompanyAllExportSettings(company.Id);
                    ViewBag.Settings = _mapper.Map<List<CompanyExportSettingsViewModel>>(settings);

                    return View("_SavedReportPartial", result);
                }
            } catch(Exception er) {
                Console.WriteLine(er.Message);
            }
            return BadRequest();
        }

        //TODO: Сделать сохранение новых значение CreditLimit & CreditUtilized
        [HttpPost]
        [Route("createCustomerCredits")]
        public async Task<IActionResult> CreateCredits([FromBody]ReportFilterViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var settings = await _crudBusinessManager.GetCompanyExportSettings(model.CompanyId);
                    if(settings == null) {
                        return NotFound();
                    }
                }
            } catch(Exception er) {
                Console.WriteLine(er.Message);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("export/{id}")]
        public async Task<IActionResult> Export(long id, ReportFilterViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var settings = await _crudBusinessManager.GetCompanyExportSettings(id);
                    if(settings == null) {
                        return NotFound();
                    }

                    var result = await _reportBusinessManager.GetAgingReport(model.CompanyId, model.Date, _daysPerPeriod, model.NumberOfPeriods, settings.IncludeAllCustomers);

                    var mem = new MemoryStream();
                    var writer = new StreamWriter(mem);
                    var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

                    //csvWriter.Configuration.Delimiter = ";";
                    //csvWriter.Configuration.HasHeaderRecord = true;
                    //csvWriter.Configuration.AutoMap<ExpandoObject>();

                    var fields = settings.Fields.OrderBy(x => x.Sort);//.Concat(columns);

                    foreach(var field in fields) {
                        if(field.IsActive) {
                            csvWriter.WriteField<string>(field.Value);
                        }
                    }
                    csvWriter.NextRecord();

                    #region SORT BY ACCOUNT NUMBER/CUSTOMER BUSINESS NAME
                    var sortedInvoices = result.Data;
                    if(settings.Sort == 0) {
                        sortedInvoices = sortedInvoices.OrderBy(x => x.AccountNo).ToList();
                    } else {
                        sortedInvoices = sortedInvoices.OrderBy(x => x.Customer.Name).ToList();
                    }
                    #endregion

                    foreach(var summary in sortedInvoices) {
                        foreach(var field in fields) {
                            if(field.IsActive) {
                                var value = ObjectExtension.GetPropValue(summary, field.Name);
                                //TODO: Здесь нужно сделать проверку на массив и взять его значение

                                //if(value.GetType() == typeof(Array)) {
                                //    Console.WriteLine("Array");
                                //}
                                var data = summary.Data.ContainsKey(field.Name) ? summary.Data[field.Name].ToString() : value;

                                csvWriter.WriteField(data == null || data.Equals("0") ? "" : data);
                            }
                        }

                        csvWriter.NextRecord();
                    }

                    writer.Flush();
                    mem.Position = 0;

                    var fileName = settings.Title;
                    var match = Regex.Match(fileName, @"(?:\$)?\{.*?\}", RegexOptions.IgnoreCase);

                    if(match.Success) {
                        string template = match.Value.Trim(new char[] { '{', '}', '$' });
                        var date = model.Date.ToString(template, DateTimeFormatInfo.InvariantInfo);

                        fileName = Regex.Replace(fileName, @"(?:\$)?\{.*?\}", match.Value.Contains('$') ? date.ToUpper() : date, RegexOptions.IgnoreCase);
                    }

                    FileStreamResult fileStreamResult = new FileStreamResult(mem, "application/octet-stream");
                    fileStreamResult.FileDownloadName = fileName;

                    return fileStreamResult;
                }
            } catch(Exception er) {
                Console.Write(er.Message);
            }
            return BadRequest();
        }

        [Route("download/{id}")]
        public async Task<IActionResult> Download(long id) {
            var item = await _crudBusinessManager.GetSavedFile(id);
            if(item == null) {
                return NotFound();
            }

            var report = await _crudBusinessManager.GetSavedReport(item.ReportId ?? 0);

            MemoryStream ms = new MemoryStream();
            ms.Write(item.File, 0, item.File.Length);
            ms.Position = 0;

            FileStreamResult fileStreamResult = new FileStreamResult(ms, "application/octet-stream");
            fileStreamResult.FileDownloadName = item.Name;

            return fileStreamResult;

        }

        /// <summary>
        /// Delete Saved Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(long id) {
            try {
                var item = await _crudBusinessManager.GetSavedReport(id);
                if(item == null)
                    return NotFound();

                var companyId = item.CompanyId;

                var result = await _crudBusinessManager.DeleteSavedReport(id);
                if(result == false) {
                    return NotFound();
                }
                return RedirectToAction(nameof(View), new { companyId = companyId });

            } catch(Exception er) {
                _logger.LogError(er, er.Message);
                return BadRequest(er);
            }
        }

        #region OLD EXPORT
        /*
        [HttpPost]
        [Route("Export/{id}")]
        public async Task<IActionResult> Export(long id, ReportFilterViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var settings = _crudBusinessManager.GetCompanyExportSettings(id);
                    if(settings == null) {
                        return NotFound();
                    }

                    var result = await _reportBusinessManager.GetAgingReport(model.CompanyId, model.Date, _daysPerPeriod, model.NumberOfPeriods);

                    var mem = new MemoryStream();
                    var writer = new StreamWriter(mem);
                    var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

                    //csvWriter.Configuration.Delimiter = ";";
                    //csvWriter.Configuration.HasHeaderRecord = true;
                    //csvWriter.Configuration.AutoMap<ExpandoObject>();

                    csvWriter.WriteField("No");
                    csvWriter.WriteField("CustomerName");
                    foreach(var header in result.Columns) {
                        csvWriter.WriteField(header);
                    }
                    csvWriter.NextRecord();

                    foreach(var summary in result.Data) {
                        csvWriter.WriteField(summary.AccountNo);
                        csvWriter.WriteField(summary.CustomerName);
                        foreach(var column in result.Columns) {
                            csvWriter.WriteField(summary.Data.ContainsKey(column) ? summary.Data[column].ToString() : "");
                        }

                        csvWriter.NextRecord();
                    }

                    writer.Flush();
                    //var res = Encoding.UTF8.GetString(mem.ToArray());
                    mem.Position = 0;

                    FileStreamResult fileStreamResult = new FileStreamResult(mem, "application/octet-stream");
                    fileStreamResult.FileDownloadName = $"{result.CompanyName}_Report_{DateTime.Now.ToShortTimeString()}.csv";
                    return fileStreamResult;
                }
            } catch(Exception er) {
                Console.Write(er.Message);
            }
            return BadRequest();
        }*/
        #endregion
    }
}

namespace Web.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController: ControllerBase {
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;
        private readonly IViewRenderService _viewRenderService;
        private readonly IReportBusinessManager _reportBusinessManager;
        public ICrudBusinessManager _crudBusinessManager;

        public ReportController(IMemoryCache memoryCache,
            IMapper mapper, IViewRenderService viewRenderService,
            ICrudBusinessManager crudBusinessManager,
            IReportBusinessManager businessManager) {
            _memoryCache = memoryCache;
            _mapper = mapper;
            _viewRenderService = viewRenderService;
            _reportBusinessManager = businessManager;
            _crudBusinessManager = crudBusinessManager;
        }

        [HttpPost("RunAgingReport", Name = "RunAgingReport")]
        public async Task<IActionResult> PostRunAgingReport(ReportFilterViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var result = await _reportBusinessManager.GetAgingReport(model.CompanyId, model.Date, 30, model.NumberOfPeriods, false);
                    string html = _viewRenderService.RenderToStringAsync("_AgingReportPartial", result).Result;

                    return Ok(html);
                }
            } catch(Exception er) {
                Console.Write(er.Message);
            }
            return null;
        }

        [HttpGet]
        [Route("savedReport")]
        public async Task<IActionResult> GetSavedReport(long companyId, DateTime date) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _crudBusinessManager.GetSavedReport(userId, companyId, date);
            return Ok(result);
        }

        [HttpPost("CreateSavedReport", Name = "CreateSavedReport")]
        public async Task<IActionResult> CreateSavedReport([FromBody] SavedReportViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var report = await _reportBusinessManager.GetAgingReport(model.CompanyId, model.Date, 30, model.NumberOfPeriods, false);

                    #region Fields
                    var fields = new List<SavedReportFieldDto>() {
                        new SavedReportFieldDto() {
                            Name = "Total Customers",
                            Value = report.TotalCustomers.ToString()
                        },
                        new SavedReportFieldDto() {
                            Name = "Balance",
                            Value = report.BalanceCustomers.ToString()
                        },

                        new SavedReportFieldDto() {
                            Name = "No balance",
                            Value = (report.TotalCustomers - report.BalanceCustomers).ToString()
                        },
                    };
                    foreach(var column in report.Columns) {
                        fields.Add(new SavedReportFieldDto() {
                            Name = column.Name,
                            Value = $"{report.Balance[column.Name].Count}|{report.Balance[column.Name].Sum}"
                        });
                    }
                    #endregion

                    #region Files
                    var files = new List<SavedReportFileDto>();

                    if(model.ExportSettings != null) {
                        foreach(var settingId in model.ExportSettings) {
                            var settings = await _crudBusinessManager.GetCompanyExportSettings(settingId);
                            if(settings != null) {
                                var file = await GetExportData(model.CompanyId, model.Date, model.NumberOfPeriods, settings);
                                if(file != null) {
                                    var fileName = settings.Title;
                                    var match = Regex.Match(fileName, @"(?:\$)?\{.*?\}", RegexOptions.IgnoreCase);

                                    if(match.Success) {
                                        string template = match.Value.Trim(new char[] { '{', '}', '$' });
                                        var date = model.Date.ToString(template, DateTimeFormatInfo.InvariantInfo);

                                        fileName = Regex.Replace(fileName, @"(?:\$)?\{.*?\}", match.Value.Contains('$') ? date.ToUpper() : date, RegexOptions.IgnoreCase);
                                    }

                                    //  var fileDate = Regex.Replace(model.Date.ToString("d", DateTimeFormatInfo.InvariantInfo), @"\b(?<month>\d{1,2})/(?<day>\d{1,2})/(?<year>\d{2,4})\b", settings.Title, RegexOptions.IgnoreCase);
                                    files.Add(new SavedReportFileDto() {
                                        Name = fileName,
                                        File = file
                                    });
                                }
                            }
                        }
                    }
                    #endregion

                    var dto = _mapper.Map<SavedReportDto>(model);
                    dto.ApplicationUserId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

                    dto.Fields = fields;
                    dto.Files = files;

                    var result = await _crudBusinessManager.CreateSavedReport(dto);
                    return Ok(result);
                }
            } catch(Exception er) {
                Console.WriteLine(er.Message);
            }
            return null;
        }

        [HttpPost("PublishSavedReport", Name = "PublishSavedReport")]
        public async Task<IActionResult> PublishSavedReport(long id) {
            var result = await _crudBusinessManager.UpdateSavedReport(id, new SavedReportDto() { IsPublished = true });
            return Ok(_mapper.Map<SavedReportDto>(result));
        }

        private async Task<byte[]> GetExportData(long companyId, DateTime date, int numberOfPeriods, CompanyExportSettingsDto settings) {
            var result = await _reportBusinessManager.GetAgingReport(companyId, date, 30, numberOfPeriods, settings.IncludeAllCustomers);

            var mem = new MemoryStream();
            var writer = new StreamWriter(mem);
            var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

            var fields = settings.Fields.OrderBy(x => x.Sort);//.Concat(columns);

            foreach(var field in fields) {
                if(field.IsActive) {
                    csvWriter.WriteField<string>(field.Value);
                }
            }
            csvWriter.NextRecord();

            #region SORT BY ACCOUNT NUMBER/CUSTOMER BUSINESS NAME
            var sortedInvoices = result.Data;
            if(settings.Sort == 0) {
                sortedInvoices = sortedInvoices.OrderBy(x => x.AccountNo).ToList();
            } else {
                sortedInvoices = sortedInvoices.OrderBy(x => x.Customer.Name).ToList();
            }
            #endregion

            foreach(var summary in sortedInvoices) {
                foreach(var field in fields) {
                    if(field.IsActive) {
                        var value = ObjectExtension.GetPropValue(summary, field.Name);
                        var data = summary.Data.ContainsKey(field.Name) ? summary.Data[field.Name].ToString() : value;

                        csvWriter.WriteField(data == null || data.Equals("0") ? "" : data);
                    }
                }

                csvWriter.NextRecord();
            }

            writer.Flush();
            mem.Position = 0;

            return mem.ToArray();
        }
    }
}
