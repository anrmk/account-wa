using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Core.Context;
using Core.Services.Business;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class ReportController: BaseController<ReportController> {
        public ICrudBusinessManager _crudBusinessManager;
        public IReportBusinessManager _businessManager;
        public ReportController(ILogger<ReportController> logger, IMapper mapper, ApplicationContext context,
            ICrudBusinessManager crudBusinessManager, IReportBusinessManager businessManager) : base(logger, mapper, context) {
            _crudBusinessManager = crudBusinessManager;
            _businessManager = businessManager;
        }

        public IActionResult Index() {
            return View();
        }

        public async Task<IActionResult> AgingReport() {
            var companies = await _crudBusinessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return View(new ReportViewModel());
        }

        [HttpPost(Name = "RunAgingReport")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunAgingReport(ReportViewModel model) {
            if(!model.CompanyId.HasValue)
                return BadRequest();

            try {
                if(ModelState.IsValid) {
                    var company = await _crudBusinessManager.GetCompany(model.CompanyId ?? 0);
                    if(company == null) {
                        return BadRequest();
                    }
                    var result = await _businessManager.GetAgingReport(company.Id, model.Date, model.DaysPerPeriod, model.NumberOfPeriod);

                    var report = new AgingReportViewModel<AgingReportDataViewModel>() {
                        CompanyId = result.CompanyId,
                        CompanyName = company.Name,
                        DaysPerPeriod = result.DaysPerAgingPeriod,
                        NumberOfPeriod = result.NumberOfPeriod,
                        DataForm = new Dictionary<long, AgingReportDataViewModel>(),
                        NamesOfPeriod = new List<string>() { },
                        Date = model.Date
                    };

                    for(int i = 0; i < model.NumberOfPeriod; i++) {
                        int from = 1 + i * model.DaysPerPeriod;
                        int to = (i + 1) * model.DaysPerPeriod;
                        string key = $"{from}-{to}";
                        report.NamesOfPeriod.Add(key);
                    }

                    foreach(var d in result.Datas) {
                        AgingReportDataViewModel record;
                        if(report.DataForm.ContainsKey(d.CustomerId)) {
                            record = report.DataForm[d.CustomerId];
                        } else {
                            record = new AgingReportDataViewModel() {
                                CustomerId = d.CustomerId,
                                CustomerName = d.CustomerName,
                                AccountNumber = d.AccountNumber,
                                Aging = new Dictionary<string, decimal>()
                            };
                            report.DataForm.Add(d.CustomerId, record);
                        }

                        #region Headers
                        if(!record.Aging.ContainsKey("Current"))
                            record.Aging.Add("Current", 0);

                        for(int i = 0; i < model.NumberOfPeriod; i++) {
                            int from = 1 + i * model.DaysPerPeriod;
                            int to = (i + 1) * model.DaysPerPeriod;
                            string key = $"{from}-{to}";
                            if(!record.Aging.ContainsKey(key))
                                record.Aging.Add(key, 0);
                        }

                        #endregion

                        var DiffPay = d.Amount - (d.PayAmount ?? 0);
                        var DateDiff = d.DiffDate;
                        if(DiffPay > 0) {
                            if(DateDiff <= 0) {
                                record.Aging["Current"] += DiffPay;
                            } else {
                                for(int i = 0; i < model.NumberOfPeriod; i++) {
                                    int from = 1 + i * model.DaysPerPeriod;
                                    int to = (i + 1) * model.DaysPerPeriod;
                                    string key = $"{from}-{to}";

                                    if(DateDiff >= from && DateDiff < to) {
                                        record.Aging[key] += DiffPay;
                                    }
                                }
                            }
                        }
                    }
                    foreach(var d in report.DataForm) {
                        d.Value.Aging.Add("Total", d.Value.Aging.Sum(x => x.Value));
                    }

                    return View(report);
                }
            } catch(Exception er) {
                Console.Write(er.Message);
            }
            return null;
        }
    }
}

namespace Web.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController: ControllerBase {
        private readonly IMapper _mapper;
        private readonly IReportBusinessManager _businessManager;
        public ICrudBusinessManager _crudBusinessManager;

        public ReportController(IMapper mapper, ICrudBusinessManager crudBusinessManager, IReportBusinessManager businessManager) {
            _mapper = mapper;
            _businessManager = businessManager;
            _crudBusinessManager = crudBusinessManager;
        }

        [HttpGet]
        public async Task<string> GetString(long id) {
            return await Task.Run(() => { return $"HelLLO {id}"; });
        }

        [HttpPost("aging", Name="Aging")]
        public async Task<IActionResult> PostRunAgingReport(ReportViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var company = await _crudBusinessManager.GetCompany(model.CompanyId ?? 0);
                    if(company == null) {
                        return BadRequest();
                    }
                    var result = await _businessManager.GetAgingReport(company.Id, model.Date, model.DaysPerPeriod, model.NumberOfPeriod);

                    var report = new AgingReportViewModel<AgingReportDataViewModel>() {
                        CompanyId = result.CompanyId,
                        CompanyName = company.Name,
                        DaysPerPeriod = result.DaysPerAgingPeriod,
                        NumberOfPeriod = result.NumberOfPeriod,
                        DataForm = new Dictionary<long, AgingReportDataViewModel>(),
                        NamesOfPeriod = new List<string>() { },
                        Date = model.Date
                    };

                    for(int i = 0; i < model.NumberOfPeriod; i++) {
                        int from = 1 + i * model.DaysPerPeriod;
                        int to = (i + 1) * model.DaysPerPeriod;
                        string key = $"{from}-{to}";
                        report.NamesOfPeriod.Add(key);
                    }

                    var summaryTable = new Dictionary<string, string[]>();

                    foreach(var d in result.Datas) {
                        AgingReportDataViewModel record;
                        if(report.DataForm.ContainsKey(d.CustomerId)) {
                            record = report.DataForm[d.CustomerId];
                        } else {
                            record = new AgingReportDataViewModel() {
                                CustomerId = d.CustomerId,
                                CustomerName = d.CustomerName,
                                AccountNumber = d.AccountNumber,
                                Aging = new Dictionary<string, decimal>()
                            };
                            report.DataForm.Add(d.CustomerId, record);
                        }

                        #region Headers
                        if(!record.Aging.ContainsKey("Current"))
                            record.Aging.Add("Current", 0);

                        for(int i = 0; i < model.NumberOfPeriod; i++) {
                            int from = 1 + i * model.DaysPerPeriod;
                            int to = (i + 1) * model.DaysPerPeriod;
                            string key = $"{from}-{to}";
                            if(!record.Aging.ContainsKey(key))
                                record.Aging.Add(key, 0);
                        }

                        #endregion

                        var DiffPay = d.Amount - (d.PayAmount ?? 0);
                        var DateDiff = d.DiffDate;
                        if(DiffPay > 0) {
                            if(DateDiff <= 0) {
                                record.Aging["Current"] += DiffPay;
                            } else {
                                for(int i = 0; i < model.NumberOfPeriod; i++) {
                                    int from = 1 + i * model.DaysPerPeriod;
                                    int to = (i + 1) * model.DaysPerPeriod;
                                    string key = $"{from}-{to}";

                                    if(DateDiff >= from && DateDiff < to) {
                                        record.Aging[key] += DiffPay;
                                    }
                                }
                            }
                        }
                    }
                    foreach(var d in report.DataForm) {
                        d.Value.Aging.Add("Total", d.Value.Aging.Sum(x => x.Value));
                    }
                    return Ok(report);
                }
            } catch(Exception er) {
                Console.Write(er.Message);
            }
            return null;
        }
    }
}
