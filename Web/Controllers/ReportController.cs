using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Core.Context;
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
        private readonly IMemoryCache _memoryCache;
        public ICrudBusinessManager _crudBusinessManager;
        public IReportBusinessManager _businessManager;
        public ReportController(IMemoryCache memoryCache, ILogger<ReportController> logger, IMapper mapper, ApplicationContext context,
            ICrudBusinessManager crudBusinessManager, IReportBusinessManager businessManager) : base(logger, mapper, context) {
            _memoryCache = memoryCache;
            _crudBusinessManager = crudBusinessManager;
            _businessManager = businessManager;
        }

        public async Task<IActionResult> Index() {
            var companies = await _crudBusinessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(new ReportViewModel() {

            });
        }

        [HttpPost]
        public async Task<IActionResult> ExportToCsv(ReportViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var result = await _businessManager.GetAgingReport(model.CompanyId ?? 0, model.Date, model.DaysPerPeriod, model.NumberOfPeriod);

                    var mem = new MemoryStream();
                    var writer = new StreamWriter(mem);
                    var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

                    //csvWriter.Configuration.Delimiter = ";";
                    //csvWriter.Configuration.HasHeaderRecord = true;
                    //csvWriter.Configuration.AutoMap<ExpandoObject>();

                    csvWriter.WriteField("AccountNumber");
                    csvWriter.WriteField("CustomerName");
                    foreach(var header in result.Columns) {
                        csvWriter.WriteField(header);
                    }
                    csvWriter.NextRecord();

                    foreach(var summary in result.Data) {
                        csvWriter.WriteField(summary.AccountNumber);
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

        //[HttpGet]
        //public async Task<string> GetString(long id) {
        //    return await Task.Run(() => { return $"HelLLO {id}"; });
        //}

        [HttpPost("aging", Name = "Aging")]
        public async Task<IActionResult> PostRunAgingReport(ReportViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
                    _memoryCache.Set("_ReportViewModel", model, cacheEntryOptions);

                    var result = await _businessManager.GetAgingReport(model.CompanyId ?? 0, model.Date, model.DaysPerPeriod, model.NumberOfPeriod);
                    string html = _viewRenderService.RenderToStringAsync("_AgingReport", result).Result;

                    return Ok(html);
                }
            } catch(Exception er) {
                Console.Write(er.Message);
            }
            return null;
        }
    }
}
