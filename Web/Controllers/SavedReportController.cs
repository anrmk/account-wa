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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

using Web.Extension;
using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class SavedReportController: BaseController<SavedReportController> {
        private readonly ICompanyBusinessManager _companyBusinessManager;

        public SavedReportController(ILogger<SavedReportController> logger, IMapper mapper, ApplicationContext context,
            ICompanyBusinessManager companyBusinessManager) : base(logger, mapper, context) {
            _companyBusinessManager = companyBusinessManager;
        }


        public IActionResult Index() {
            return View();
        }

        public async Task<IActionResult> Plan() {
            var companies = await _companyBusinessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();


            return View(new ReportFilterViewModel() {
                Date = DateTime.Now.LastDayOfMonth()
            });
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

        public SavedReportController(IMapper mapper, IViewRenderService viewRenderService,
             ICrudBusinessManager businessManager) {
            _mapper = mapper;
            _viewRenderService = viewRenderService;
            _businessManager = businessManager;
        }

        [HttpPost("GenerateSavedReportPlan", Name = "GenerateSavedReportPlan")]
        public async Task<IActionResult> GenerateSavedReportPlan(ReportFilterViewModel model) {
            try {
                if(!ModelState.IsValid) {
                    throw new Exception("");
                }

                var customerTypes = await _businessManager.GetCustomerTypes();

                var fields = new List<SavedReportPlanFieldViewModel>();
                fields.Add(new SavedReportPlanFieldViewModel() {
                    Code = "TotalCustomers",
                    Name = "Total Customers",
                    Value = "0"
                });

                //Add customer types
                foreach(var ctype in customerTypes) {
                    fields.Add(new SavedReportPlanFieldViewModel() {
                        Code = ctype.Name, //not CODE!!!
                        Name = ctype.Name,
                        Value = "0"
                    });
                }

                fields.Add(new SavedReportPlanFieldViewModel() {
                    Code = "BalanceCustomers",
                    Name = "Balance",
                    Value = "0"
                });

                fields.Add(new SavedReportPlanFieldViewModel() {
                    Code = "NoBalanceCustomers",
                    Name = "No balance",
                    Value = "0"
                });

                //Add Balance
                #region CREATE HEADERS
                var daysPerPeriod = 30;

                for(int i = -1; i < model.NumberOfPeriods; i++) {
                    var from = (i < 0 ? -1 : 1) + i * daysPerPeriod;
                    var to = (i + 1) * daysPerPeriod;

                    fields.Add(new SavedReportPlanFieldViewModel() {
                        Name = $"{from}-{to}",
                        Code = $"{from}-{to}",
                        Value = "0"
                    });
                }

                fields.Add(new SavedReportPlanFieldViewModel() {
                    Name = $"{1 + model.NumberOfPeriods * daysPerPeriod}+",
                    Code = $"{1 + model.NumberOfPeriods * daysPerPeriod}+"
                });

                fields.Add(new SavedReportPlanFieldViewModel() {
                    Name = "Total Late"
                });

                fields.Add(new SavedReportPlanFieldViewModel() {
                    Name = "Total"
                });
                #endregion

                var result = new SavedReportPlanViewModel() {
                    CompanyId = model.CompanyId,
                    Date = model.Date,
                    Fields = fields,
                    NumberOfPeriods = model.NumberOfPeriods,
                };

                var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) {
                    //  { "Company", _mapper.Map<CompanyViewModel>(company)},
                    //         { "TotalCustomers", result.Rows.Count }
                };

                string html = await _viewRenderService.RenderToStringAsync("_SavedReportPlanPartial", result, viewDataDictionary);
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

                return Ok();


            } catch(Exception er) {
                return BadRequest(er.Message ?? er.StackTrace);
            }
        }
    }
}