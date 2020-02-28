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
        private readonly IReportBusinessManager _businessManager;

        public ReportController(IMemoryCache memoryCache, ILogger<ReportController> logger, IMapper mapper, ApplicationContext context,
            INsiBusinessManager nsiBusinessManager, ICrudBusinessManager crudBusinessManager, IReportBusinessManager businessManager) : base(logger, mapper, context) {
            _memoryCache = memoryCache;
            _crudBusinessManager = crudBusinessManager;
            _nsiBusinessManager = nsiBusinessManager;
            _businessManager = businessManager;
        }

        public async Task<IActionResult> Index() {
            var companies = await _crudBusinessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(new ReportFilterViewModel() {
                Date = DateTime.Now.LastDayOfMonth()
            });
        }


        public async Task<IActionResult> GetExportSettings(long id) {
            var item = await _crudBusinessManager.GetCompany(id);
            if(item == null) {
                return NotFound();
            }

            var periods = await _nsiBusinessManager.GetReportPeriods();

            //var fields = new List<ExportSettingsFieldValueViewModel>();
            //fields.Add(new ExportSettingsFieldValueViewModel() { Name = "Account Number", Value = "New Name" });
            //fields.Add(new ExportSettingsFieldValueViewModel() { Name = "Business Name", Value = "Business Name" });
            //periods.ForEach(x =>
            //    fields.Add(new ExportSettingsFieldValueViewModel() { Name = x.Name, Value = x.Name })
            //);
            //fields.Add(new ExportSettingsFieldValueViewModel() { Name = "Total", Value = "Total" });

            var settings = new CompanyExportSettingsViewModel() {
                CompanyId = id,
                ShowEmptyRows = true,
                Title = "Some Title",
               // Fields = fields
            };

            return View("_ExportSettingsPartial", settings);
        }

        [HttpPost]
        public async Task<IActionResult> ExportToCsv(ReportFilterViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var result = await _businessManager.GetAgingReport(model.CompanyId, model.Date, _daysPerPeriod, model.NumberOfPeriods);

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
