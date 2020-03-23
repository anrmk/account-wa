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
            foreach(var report in result) {
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
            var company = await _crudBusinessManager.GetCompany(model.CompanyId);
            if(company == null) {
                return NotFound();
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

                    //var invoices = await _reportManager.GetAgingInvoices(model.CompanyId, model.Date, _daysPerPeriod, model.NumberOfPeriods);
                    //if(invoices == null || invoices.Count == 0) {
                    //    return NotFound();
                    //}

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
                                var data = summary.Data.ContainsKey(field.Name) ? summary.Data[field.Name].ToString() : value;

                                csvWriter.WriteField(data == null || data.Equals("0") ? "" : data);
                            }
                        }

                        csvWriter.NextRecord();
                    }

                    writer.Flush();
                    mem.Position = 0;

                    var fileDate = Regex.Replace(model.Date.ToString("d", DateTimeFormatInfo.InvariantInfo), @"\b(?<month>\d{1,2})/(?<day>\d{1,2})/(?<year>\d{2,4})\b", settings.Title, RegexOptions.IgnoreCase);

                    FileStreamResult fileStreamResult = new FileStreamResult(mem, "application/octet-stream");
                    fileStreamResult.FileDownloadName = fileDate;

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

        [HttpPost("aging", Name = "Aging")]
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

        [HttpPost("savedReport", Name = "CreateSavedReport")]
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
                    foreach(var settingId in model.ExportSettings) {
                        var settings = await _crudBusinessManager.GetCompanyExportSettings(settingId);
                        if(settings != null) {
                            var file = await GetExportData(model.CompanyId, model.Date, model.NumberOfPeriods, settings);
                            if(file != null) {
                                var fileDate = Regex.Replace(model.Date.ToString("d", DateTimeFormatInfo.InvariantInfo), @"\b(?<month>\d{1,2})/(?<day>\d{1,2})/(?<year>\d{2,4})\b", settings.Title, RegexOptions.IgnoreCase);
                                files.Add(new SavedReportFileDto() {
                                    Name = fileDate,
                                    File = file
                                });
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
