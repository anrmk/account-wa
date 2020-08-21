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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using Web.Extension;
using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class ReportController: BaseController<ReportController> {
        private readonly int _daysPerPeriod = 30;

        private readonly IMemoryCache _memoryCache;
        private readonly ICrudBusinessManager _businessManager;
        private readonly ICompanyBusinessManager _companyBusinessManager;
        private readonly IReportBusinessManager _reportBusinessManager;
        private readonly IReportManager _reportManager;

        public ReportController(IMemoryCache memoryCache, ILogger<ReportController> logger, IMapper mapper, ApplicationContext context,
             ICrudBusinessManager crudBusinessManager, IReportBusinessManager businessManager, IReportManager reportManager,
             ICompanyBusinessManager companyBusinessManager) : base(logger, mapper, context) {
            _memoryCache = memoryCache;
            _businessManager = crudBusinessManager;
            _companyBusinessManager = companyBusinessManager;
            _reportBusinessManager = businessManager;
            _reportManager = reportManager;
        }

        public async Task<IActionResult> Index() {
            var companies = await _companyBusinessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(new ReportFilterViewModel() {
                Date = DateTime.Now.LastDayOfMonth()
            });
        }

        public async Task<IActionResult> CreditUtilized() {
            var companies = await _companyBusinessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(new ReportFilterViewModel() {
                Date = DateTime.Now.LastDayOfMonth()
            });
        }

        [HttpPost]
        [Route("export/{id}")]
        public async Task<IActionResult> Export(long id, ReportFilterViewModel model) {
            try {
                if(!ModelState.IsValid) {
                    throw new Exception("Form is not valid!");
                }

                var settings = await _companyBusinessManager.GetExportSettings(id);
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
                var sortedInvoices = result.Rows;
                if(settings.Sort == 0) {
                    sortedInvoices = sortedInvoices.OrderBy(x => x.AccountNo).ToList();
                } else {
                    sortedInvoices = sortedInvoices.OrderBy(x => x.Customer.Name).ToList();
                }
                #endregion

                foreach(var row in sortedInvoices) {
                    foreach(var field in fields) {
                        if(field.IsActive) {
                            var value = ObjectExtension.GetPropValue(row, field.Name);
                            //TODO: Здесь нужно сделать проверку на массив и взять его значение

                            //if(value.GetType() == typeof(Array)) {
                            //    Console.WriteLine("Array");
                            //}
                            var data = row.Data.ContainsKey(field.Name) ? row.Data[field.Name].ToString() : value;

                            csvWriter.WriteField(data == null || data.Equals("0") ? settings.DefaultValueIfEmpty : data);
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

            } catch(Exception er) {
                return BadRequest(er.Message ?? er.StackTrace);
            }
        }

        [Route("download/{id}")]
        public async Task<IActionResult> Download(long id) {
            var item = await _reportBusinessManager.GetSavedFile(id);
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
    public class ReportController: ControllerBase {
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;
        private readonly IViewRenderService _viewRenderService;
        private readonly IReportBusinessManager _reportBusinessManager;
        private readonly ICrudBusinessManager _businessManager;
        private readonly ICompanyBusinessManager _companyBusinessManager;
        private readonly ICustomerBusinessManager _customerBusinessManager;

        public ReportController(IMemoryCache memoryCache,
            IMapper mapper, IViewRenderService viewRenderService,
            ICrudBusinessManager businessManager,
            ICompanyBusinessManager companyBusinessManager,
            ICustomerBusinessManager customerBusinessManager,
            IReportBusinessManager reportbusinessManager) {
            _memoryCache = memoryCache;
            _mapper = mapper;
            _viewRenderService = viewRenderService;
            _reportBusinessManager = reportbusinessManager;
            _businessManager = businessManager;
            _customerBusinessManager = customerBusinessManager;
            _companyBusinessManager = companyBusinessManager;
        }

        [HttpPost("GenerateAgingReport", Name = "GenerateAgingReport")]
        public async Task<IActionResult> GenerateAgingReport(ReportFilterViewModel model) {
            try {
                if(!ModelState.IsValid) {
                    throw new Exception("Form is not valid!");
                }

                var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var result = await _reportBusinessManager.GetAgingReport(model.CompanyId, model.Date, 30, model.NumberOfPeriods, false);

                var plan = await _reportBusinessManager.GetSavedPlanReport(userId, model.CompanyId, model.Date);
                var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) {
                          { "Plan", _mapper.Map<SavedReportPlanViewModel>(plan)}
                    };

                string html = await _viewRenderService.RenderToStringAsync("_AgingReportPartial", result, viewDataDictionary);
                return Ok(html);

            } catch(Exception er) {
                return BadRequest(er.Message ?? er.StackTrace);
            }
        }

        [HttpGet("CreateSavedReportView", Name = "CreateSavedReportView")]
        public async Task<IActionResult> CreateSavedReportView([FromQuery] ReportFilterViewModel model) {
            try {
                if(!ModelState.IsValid) {
                    throw new Exception("Form is not valid!");
                }

                var company = await _companyBusinessManager.GetCompany(model.CompanyId);
                if(company == null) {
                    return NotFound();
                }

                var savedItem = await _reportBusinessManager.GetSavedReport(User.FindFirstValue(ClaimTypes.NameIdentifier), model.CompanyId, model.Date);
                if(savedItem != null && savedItem.IsPublished) {
                    string html = await _viewRenderService.RenderToStringAsync("_SaveAgingReportPartial", _mapper.Map<SavedReportViewModel>(savedItem));
                    return Ok(html);
                } else {
                    var result = _mapper.Map<SavedReportViewModel>(savedItem) ?? new SavedReportViewModel() {
                        CompanyId = model.CompanyId,
                        Name = company.Name,
                        Date = model.Date,
                        NumberOfPeriods = model.NumberOfPeriods
                    };

                    var settings = await _companyBusinessManager.GetAllExportSettings(model.CompanyId);
                    var checkingCustomerAccountNumber = await _reportBusinessManager.CheckingCustomerAccountNumber(model.CompanyId, model.Date, model.NumberOfPeriods);

                    var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) {
                            { "Settings",  _mapper.Map<List<CompanyExportSettingsViewModel>>(settings)},
                            { "CheckingCustomerAccountNumber", _mapper.Map<ReportStatusViewModel>(checkingCustomerAccountNumber)}
                        };

                    string html = await _viewRenderService.RenderToStringAsync("_SaveAgingReportPartial", result, viewDataDictionary);
                    return Ok(html);
                }

            } catch(Exception er) {
                return BadRequest(er.Message ?? er.StackTrace);
            }
        }

        [HttpPost("CreateSavedReport", Name = "CreateSavedReport")]
        public async Task<IActionResult> CreateSavedReport([FromBody] SavedReportViewModel model) {
            try {
                if(!ModelState.IsValid) {
                    throw new Exception("Form is not valid!");
                }
                var report = await _reportBusinessManager.GetAgingReport(model.CompanyId, model.Date, 30, model.NumberOfPeriods, false);
                var customerTypes = await _customerBusinessManager.GetCustomerTypes();

                var fields = new List<SavedReportFieldDto>();

                #region CUSTOMERS
                foreach (var field in report.CustomerTotal.Keys) {
                    fields.Add(new SavedReportFieldDto() {
                        Name = field,
                        Count = report.CustomerTotal[field],
                    });
                }
                #endregion

                #region CUSTOMER TYPES
                foreach(var ctype in customerTypes) {
                    fields.Add(new SavedReportFieldDto() {
                        Name = ctype.Name,
                        Count = report.CustomerTypes.ContainsKey(ctype.Name) ? report.CustomerTypes[ctype.Name] : 0
                    });
                }
                #endregion

                #region BALANCE TOTAL
                foreach(var column in report.Cols) {
                    fields.Add(new SavedReportFieldDto() {
                        Name = column.Name,
                        Count = report.BalanceTotal[column.Name].Count,
                        Amount = report.BalanceTotal[column.Name].Sum
                    });
                }
                #endregion


                #region FILES
                var files = new List<SavedReportFileDto>();

                if(model.ExportSettings != null) {
                    foreach(var settingId in model.ExportSettings) {
                        var settings = await _companyBusinessManager.GetExportSettings(settingId);
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

                var result = await _reportBusinessManager.CreateSavedReport(dto);
                return Ok(result);

            } catch(Exception er) {
                return BadRequest(er.Message ?? er.StackTrace);
            }
        }

        [HttpGet("GetSavedReport", Name = "GetSavedReport")]
        public async Task<IActionResult> GetSavedReport(long companyId, DateTime date) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reportBusinessManager.GetSavedReport(userId, companyId, date);
            return Ok(result);
        }

        [HttpPost("CheckAbilityToSaveCredits", Name = "CheckAbilityToSaveCredits")]
        public async Task<IActionResult> CheckAbilityToSaveCredits([FromBody] ReportFilterViewModel model) {
            if(!model.Date.Equals(model.Date.LastDayOfMonth())) {
                return Ok(false);
            }

            var company = await _companyBusinessManager.GetCompany(model.CompanyId);

            if(company != null && company.Settings != null && company.Settings.SaveCreditValues) {
                var savedReports = await _reportBusinessManager.GetSavedReport(User.FindFirstValue(ClaimTypes.NameIdentifier), model.CompanyId);
                if(savedReports == null || savedReports.Count == 0) {//Если у компании нет вообще сохраненных репортов, дать возможность сохранить КРЕДИТЫ
                    return Ok(true);
                }

                var settings = company.Settings;
                var date = model.Date.AddMonths(-1).LastDayOfMonth();

                var prevReport = savedReports.Where(x => x.Date == date).FirstOrDefault();
                var currentReport = savedReports.Where(x => x.Date == model.Date).FirstOrDefault();

                if(settings != null && settings.SaveCreditValues && prevReport != null && prevReport.IsPublished && currentReport == null)
                    return Ok(true);
            }

            return Ok(false);
        }

        [HttpGet("CustomerCreditUtilizedSettingsView", Name = "CustomerCreditUtilizedSettingsView")]
        public async Task<IActionResult> CustomerCreditUtilizedSettingsView([FromQuery] ReportFilterViewModel model) {
            try {
                if(!ModelState.IsValid) {
                    throw new Exception("Form is not valid!");
                }

                var company = await _companyBusinessManager.GetCompany(model.CompanyId);

                var creditUtilizedSettings = await _businessManager.GetCustomerCreditUtilizedSettings(model.CompanyId, model.Date);
                model.RoundType = creditUtilizedSettings == null ? company.Settings.RoundType : creditUtilizedSettings.RoundType;

                var report = await _reportBusinessManager.GetAgingReport(model.CompanyId, model.Date, 30, model.NumberOfPeriods, false);
                var creditUtilizedList = new List<CustomerCreditUtilizedViewModel>();

                foreach(var data in report.Rows) {
                    var customer = data.Customer;

                    var creditUtilizeds = await _businessManager.GetCustomerCreditUtilizeds(customer.Id);
                    var creditUtilized = creditUtilizeds
                                .OrderByDescending(x => x.CreatedDate)
                                .Where(x => x.CreatedDate <= model.Date).FirstOrDefault();

                    if(creditUtilized != null && creditUtilized.IsIgnored) {
                        var value = data.Data["Total"];

                        if(model.RoundType == Core.Data.Enum.RoundType.RoundUp) {
                            value = Math.Ceiling(value);
                        } else if(model.RoundType == Core.Data.Enum.RoundType.RoundDown) {
                            value = Math.Floor(value);
                        }
                        if(creditUtilized.Value < value)
                            creditUtilizedList.Add(_mapper.Map<CustomerCreditUtilizedViewModel>(creditUtilized));
                    }
                }
                creditUtilizedList = creditUtilizedList.OrderBy(x => x.CreatedDate == model.Date).ToList();
                var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) {
                        { "CreditUtilizedList", _mapper.Map<List<CustomerCreditUtilizedViewModel>>(creditUtilizedList) }
                    };

                string html = await _viewRenderService.RenderToStringAsync("_CreateCustomerCreditsPartial", model, viewDataDictionary);
                return Ok(html);
            } catch(Exception er) {
                return BadRequest(er.Message ?? er.StackTrace);
            }
        }

        [HttpPost("CreateCustomerCreditUtilized", Name = "CreateCustomerCreditUtilized")]
        public async Task<IActionResult> CreateCustomerCredits([FromBody] ReportFilterViewModel model) {
            try {
                if(!ModelState.IsValid) {
                    throw new Exception("Form is not valid!");
                }
                var company = await _companyBusinessManager.GetCompany(model.CompanyId);
                if(company == null || company.Settings == null || !company.Settings.SaveCreditValues) {//возможно ли сохранение лимитов
                    throw new Exception("Please, check company settings!");
                }

                var date = model.Date.LastDayOfMonth();
                var previousDate = model.Date.AddMonths(-1).LastDayOfMonth();

                var savedReports = await _reportBusinessManager.GetSavedReport(User.FindFirstValue(ClaimTypes.NameIdentifier), model.CompanyId);
                if(savedReports != null && savedReports.Count > 0) {
                    var prevReport = savedReports.Where(x => x.Date == previousDate).FirstOrDefault();
                    if(prevReport == null || !prevReport.IsPublished) {
                        throw new Exception($"You must save and publish a report for the previous period: {previousDate.ToShortDateString()}");
                    }
                }

                var report = await _reportBusinessManager.GetAgingReport(model.CompanyId, date, 30, model.NumberOfPeriods, false);

                #region CREDIT UTILIZED SETTINGS
                var creditUtilizedSettings = await _businessManager.GetCustomerCreditUtilizedSettings(model.CompanyId, model.Date);
                if(creditUtilizedSettings == null) {
                    creditUtilizedSettings = await _businessManager.CreateCustomerCreditUtilizedSettings(new CustomerCreditUtilizedSettingsDto() {
                        CompanyId = model.CompanyId,
                        Date = model.Date,
                        RoundType = model.RoundType
                    });
                } else {
                    if(creditUtilizedSettings.RoundType != model.RoundType) {
                        creditUtilizedSettings.RoundType = model.RoundType;
                        creditUtilizedSettings = await _businessManager.UpdateCustomerCreditUtilizedSettings(creditUtilizedSettings.Id, creditUtilizedSettings);
                    }
                }
                #endregion

                var createCreditUtilized = 0;
                var updateCreditUtilized = 0;
                var ignoreCreditUtilized = 0;

                foreach(var data in report.Rows) {
                    var customer = data.Customer;
                    var value = data.Data["Total"]; //new height credit

                    if(creditUtilizedSettings.RoundType == Core.Data.Enum.RoundType.RoundUp) {
                        value = Math.Ceiling(value);
                    } else if(creditUtilizedSettings.RoundType == Core.Data.Enum.RoundType.RoundDown) {
                        value = Math.Floor(value);
                    }

                    var creditUtilizeds = await _businessManager.GetCustomerCreditUtilizeds(customer.Id);
                    var creditUtilized = creditUtilizeds
                            .OrderByDescending(x => x.CreatedDate)
                            .Where(x => x.CreatedDate <= date).FirstOrDefault();

                    //  если в БД нет записей 
                    //  ИЛИ
                    //  запись есть и даты не совпадают, а также значение меньше значения текущего отчета
                    //  создать запись
                    if(creditUtilized == null) {
                        await _businessManager.CreateCustomerCreditUtilized(new CustomerCreditUtilizedDto() {
                            CreatedDate = date,
                            Value = value,
                            CustomerId = customer.Id
                        });
                        createCreditUtilized++;
                    } else if(creditUtilized.Value < value) { // если новое значение больше предыдущей записи
                        if(creditUtilized.CreatedDate < date) { // если даты не равны
                            if(!creditUtilized.IsIgnored || model.CreditUtilizeds == null || !model.CreditUtilizeds.Contains(creditUtilized.Id)) { // если знаение со статусом Ignore и оно не выбрано, тогда проигнорировать
                                await _businessManager.CreateCustomerCreditUtilized(new CustomerCreditUtilizedDto() { //создать новую запись
                                    CreatedDate = date,
                                    Value = value,
                                    CustomerId = customer.Id
                                });
                                createCreditUtilized++;
                            } else {
                                ignoreCreditUtilized++;
                            }
                        } else if(creditUtilized.CreatedDate == date) { // если даты равны
                            if(creditUtilized.IsIgnored) {
                                ignoreCreditUtilized++;
                            } else {
                                creditUtilized.Value = value;
                                creditUtilized = await _businessManager.UpdateCustomerCreditUtilized(creditUtilized.Id, creditUtilized); // изменить значение записи
                                updateCreditUtilized++;
                            }
                        }
                    }
                }
                return Ok(new { Updated = updateCreditUtilized, Created = createCreditUtilized, Ignored = ignoreCreditUtilized });

            } catch(Exception er) {
                return BadRequest(er.Message ?? er.StackTrace);
            }
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
            var sortedInvoices = result.Rows;
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

        [HttpPost("CompareWithSaved", Name = "CompareWithSaved")]
        public async Task<IActionResult> CompareWithSaved([FromBody] ReportFilterViewModel model) {
            try {
                if(!ModelState.IsValid) {
                    throw new Exception("Form is not valid!");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var company = await _companyBusinessManager.GetCompany(model.CompanyId);

                var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) {
                        { "CompanySettings", _mapper.Map<CompanySettingsViewModel>(company.Settings) }
                    };

                var saved = await _reportBusinessManager.GetSavedReport(userId, model.CompanyId, model.Date);
                if(saved == null) {
                    return Ok($"{company.Name} company has no saved report for {model.Date.ToString("MM/dd/yyyy")}");
                }

                var report = await _reportBusinessManager.GetAgingReport(model.CompanyId, model.Date, 30, model.NumberOfPeriods, false);

                var compareReport = new CompareReportViewModel() {
                    CompanyId = model.CompanyId,
                    Date = model.Date,
                    NumberOfPeriods = model.NumberOfPeriods,
                    Balance = new List<CompareReportFieldViewModel>(),
                    Customers = new List<CompareReportFieldViewModel>(),
                    CustomerTypes = new List<CompareReportFieldViewModel>(),
                    CreditUtilized = new List<CompareCreditsFieldViewModel>()
                    //  CreditUtilizedList = new List<CompareReportCreditUtilizedViewModel>()
                };

                #region CUSTOMERS
                foreach(var field in saved.Fields) {
                    if(report.CustomerTotal.ContainsKey(field.Name)) {
                        var reportValue = report.CustomerTotal[field.Name];
                        var citem = new CompareReportFieldViewModel() {
                            Name = field.Name,
                            SavedValue = field?.Count.ToString(),
                            ReportValue = reportValue.ToString(),
                            Status = field.Count == reportValue
                        };
                        compareReport.Customers.Add(citem);
                    }
                }
                #endregion

                #region CUSTOMER TYPES
                var customerTypesField = await _customerBusinessManager.GetCustomerTypes();
                foreach(var field in customerTypesField) {
                    var savedValue = saved.Fields.Where(x => x.Name.Equals(field.Name)).FirstOrDefault();
                    if(report.CustomerTypes.ContainsKey(field.Name)) {
                        var reportValue = report.CustomerTypes[field.Name];
                        var citem = new CompareReportFieldViewModel() {
                            Name = field.Name,
                            SavedValue = savedValue?.Count.ToString() ?? "",
                            ReportValue = reportValue.ToString(),
                            Status = reportValue == savedValue.Count
                        };
                        compareReport.CustomerTypes.Add(citem);
                    }
                }
                #endregion

                #region BALANCE TOTAL
                var savedColumns = saved.Fields.Where(x => x.Name.Contains('-') || x.Name.Contains('+') || x.Name.Equals("Total Late") || x.Name.Equals("Total")).Select(x => x.Name).ToList(); //Select only BALANCE fields
                var balanceColumns = savedColumns.Count > report.Cols.Count ? savedColumns : report.Cols.Select(x => x.Name).ToList();

                foreach(var column in balanceColumns) {
                    var savedField = saved.Fields.Where(x => x.Name.Equals(column)).FirstOrDefault();

                    var savedValue = savedField != null ? savedField.Count + "|" + savedField.Amount?.ToCurrency() : "0|0";
                    var reportValue = report.BalanceTotal.ContainsKey(column) ?
                            report.BalanceTotal[column].Count + "|" + report.BalanceTotal[column].Sum.ToCurrency() : "0|0";

                    var citem = new CompareReportFieldViewModel() {
                            Name = column,
                            SavedValue = savedValue,
                            ReportValue = reportValue,
                            Status = savedValue.Equals(reportValue)
                        };
                    compareReport.Balance.Add(citem);
                }
                #endregion

                #region CREDIT UTILIZED
                if(company != null && company.Settings != null && company.Settings.SaveCreditValues) {
                    var creditUtilizedSettings = await _businessManager.GetCustomerCreditUtilizedSettings(model.CompanyId, model.Date);
                    if(creditUtilizedSettings == null) {
                        creditUtilizedSettings = new CustomerCreditUtilizedSettingsDto() {
                            RoundType = company.Settings.RoundType,
                            CompanyId = company.Id
                        };
                    }
                    viewDataDictionary.Add("CreditUtilizedSettings", _mapper.Map<CustomerCreditUtilizedSettingsViewModel>(creditUtilizedSettings));

                    var createCreditUtilized = 0;
                    var updateCreditUtilized = 0;
                    var ignoreCreditUtilized = 0;

                    foreach(var data in report.Rows) {
                        var customer = data.Customer;
                        var value = data.Data["Total"];//new height credit

                        if(creditUtilizedSettings.RoundType == Core.Data.Enum.RoundType.RoundUp) {
                            value = Math.Ceiling(value);
                        } else if(creditUtilizedSettings.RoundType == Core.Data.Enum.RoundType.RoundDown) {
                            value = Math.Floor(value);
                        }

                        var creditUtilizeds = await _businessManager.GetCustomerCreditUtilizeds(customer.Id);
                        var creditUtilized = creditUtilizeds
                                    .OrderByDescending(x => x.CreatedDate)
                                    .Where(x => x.CreatedDate <= model.Date).FirstOrDefault();

                        if(creditUtilized == null) {
                            createCreditUtilized++;
                        } else if(creditUtilized.Value < value) {
                            if(creditUtilized.IsIgnored) {
                                ignoreCreditUtilized++;
                            } else if(creditUtilized.CreatedDate != model.Date) {
                                createCreditUtilized++;
                            } else {
                                updateCreditUtilized++;
                            }
                        }
                    }

                    compareReport.CreditUtilized.Add(new CompareCreditsFieldViewModel() {
                        Name = "Customers count",
                        CreateCount = createCreditUtilized,
                        UpdateCount = updateCreditUtilized,
                        IgnoredCount = ignoreCreditUtilized,
                        Status = createCreditUtilized == 0 && updateCreditUtilized == 0
                    });
                }
                #endregion

                string html = await _viewRenderService.RenderToStringAsync("_CompareReportPartial", compareReport, viewDataDictionary);
                return Ok(html);
            } catch(Exception er) {
                return BadRequest(er.Message ?? er.StackTrace);
            }
        }

        [HttpPost("CheckingCustomerAccountNumber", Name = "CheckingCustomerAccountNumber")]
        public async Task<IActionResult> CheckingCustomerAccountNumber([FromBody] ReportFilterViewModel model) {
            try {
                if(!ModelState.IsValid) {
                    throw new Exception("Form is not valid!");
                }

                var company = await _companyBusinessManager.GetCompany(model.CompanyId);
                if(company == null || company.Settings == null || string.IsNullOrEmpty(company.Settings.AccountNumberTemplate)) {
                    throw new Exception("Please, check company settings! \"Account Number Template\" is not defined. ");
                }

                var customers = new List<CustomerListViewModel>();
                var regex = new Regex(company.Settings.AccountNumberTemplate);

                var result = await _reportBusinessManager.GetAgingReport(model.CompanyId, model.Date, 30, model.NumberOfPeriods, false);
                foreach(var data in result.Rows) {
                    var customer = data.Customer;
                    var isMatch = regex.IsMatch(customer.No);

                    if(!isMatch) {
                        customers.Add(_mapper.Map<CustomerListViewModel>(customer));
                    }
                }

                if(customers.Count == 0) {
                    return Ok($"{result.Rows.Count} {company.Name} customers has valid \"Account Number\" that match the template set in the company settings");
                }
                var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) {
                            { "Company", _mapper.Map<CompanyViewModel>(company)},
                            { "TotalCustomers", result.Rows.Count }
                        };

                string html = await _viewRenderService.RenderToStringAsync("_CheckingCustomerAccountNumberPartial", customers, viewDataDictionary);
                return Ok(html);

            } catch(Exception er) {
                return BadRequest(er.Message ?? er.StackTrace);
            }
        }

        [HttpGet("GetCustomerCreditUtilizedComparedReport", Name = "GetCustomerCreditUtilizedComparedReport")]
        public async Task<Pager<CustomerCreditUtilizedViewModel>> GetCustomerCreditUtilizedComparedReport([FromQuery] ReportFilterViewModel model) {
            var result = await _reportBusinessManager.GetCustomerCreditUtilizedComparedReport(_mapper.Map<ReportFilterDto>(model));
            var pager = new Pager<CustomerCreditUtilizedViewModel>(_mapper.Map<List<CustomerCreditUtilizedViewModel>>(result.Items), result.TotalItems, result.CurrentPage, result.PageSize);
            pager.Filter = result.Filter;
            return pager;
        }

        [HttpGet("GetCustomerCreditUtilizedReport", Name = "GetCustomerCreditUtilizedReport")]
        public async Task<Pager<CustomerCreditUtilizedViewModel>> GetCustomerCreditUtilizedReport([FromQuery] ReportFilterViewModel model) {
            var result = await _reportBusinessManager.GetCustomerCreditUtilizedReport(_mapper.Map<ReportFilterDto>(model));
            var pager = new Pager<CustomerCreditUtilizedViewModel>(_mapper.Map<List<CustomerCreditUtilizedViewModel>>(result.Items), result.TotalItems, result.CurrentPage, result.PageSize);
            pager.Filter = result.Filter;
            return pager;
        }

        [HttpGet("ExportSettingsView", Name = "ExportSettingsView")]
        public async Task<IActionResult> ExportSettingsView([FromQuery] ReportFilterViewModel model) {
            try {
                if(!ModelState.IsValid) {
                    throw new Exception("Form is not valid!");
                }

                var company = await _companyBusinessManager.GetCompany(model.CompanyId);
                if(company == null) {
                    return NotFound();
                }

                //var periods = await _nsiBusinessManager.GetReportPeriods();
                var settings = await _companyBusinessManager.GetAllExportSettings(company.Id);
                var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) {
                        { "Settings", _mapper.Map<List<CompanyExportSettingsViewModel>>(settings) }
                    };

                string html = await _viewRenderService.RenderToStringAsync("_ExportSettingsPartial", model, viewDataDictionary);
                return Ok(html);
            } catch(Exception er) {
                return BadRequest(er.Message ?? er.StackTrace);
            }
        }
    }
}
