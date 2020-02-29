using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Core.Context;
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

using Core.Extension;

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

        [HttpPost]
        [Route("GetExportSettings")]
        public async Task<IActionResult> GetExportSettings([FromBody]ReportFilterViewModel model) {
            var companyId = model.CompanyId;

            var item = await _crudBusinessManager.GetCompany(companyId);
            if(item == null) {
                return NotFound();
            }

            //var periods = await _nsiBusinessManager.GetReportPeriods();
            var settings = await _crudBusinessManager.GetCompanyAllExportSettings(item.Id);
            ViewBag.Settings = _mapper.Map<List<CompanyExportSettingsViewModel>>(settings);

            return View("_ExportSettingsPartial", model);
        }
        
        [HttpPost]
        [Route("Export/{id}")]
        public async Task<IActionResult> Export(long id, ReportFilterViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var settings = await _crudBusinessManager.GetCompanyExportSettings(id);
                    if(settings == null) {
                        return NotFound();
                    }

                    var result = await _reportBusinessManager.GetAgingReport(model.CompanyId, model.Date, _daysPerPeriod, model.NumberOfPeriods);


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


                    var fields = settings.Fields.OrderBy(x => x.Sort);
                    foreach(var field in fields) {
                        if(field.IsActive) {
                            csvWriter.WriteField(field.Value);
                        }
                    }
                    csvWriter.NextRecord();

                    foreach(var summary in result.Data) {
                        foreach(var field in fields) {
                            var CustomerName = ObjectExtension.GetPropValue<string>(summary, field.Name);
                        }
                       // csvWriter.WriteField(summary.AccountNo);
                        //csvWriter.WriteField(summary.CustomerName);
                        //foreach(var column in result.Columns) {
                        //    csvWriter.WriteField(summary.Data.ContainsKey(column) ? summary.Data[column].ToString() : "");
                        //}

                        //csvWriter.NextRecord();
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
        private readonly IReportBusinessManager _businessManager;
        public ICrudBusinessManager _crudBusinessManager;

        public ReportController(IMemoryCache memoryCache,
            IMapper mapper, IViewRenderService viewRenderService,
            ICrudBusinessManager crudBusinessManager,
            IReportBusinessManager businessManager) {
            _memoryCache = memoryCache;
            _mapper = mapper;
            _viewRenderService = viewRenderService;
            _businessManager = businessManager;
            _crudBusinessManager = crudBusinessManager;
        }

        [HttpPost("aging", Name = "Aging")]
        public async Task<IActionResult> PostRunAgingReport(ReportFilterViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
                    _memoryCache.Set("_ReportViewModel", model, cacheEntryOptions);

                    var result = await _businessManager.GetAgingReport(model.CompanyId, model.Date, 30, model.NumberOfPeriods);
                    string html = _viewRenderService.RenderToStringAsync("_AgingReportPartial", result).Result;

                    return Ok(html);
                }
            } catch(Exception er) {
                Console.Write(er.Message);
            }
            return null;
        }
    }
}
