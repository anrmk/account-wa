using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Core.Context;
using Core.Data.Dto;
using Core.Extension;
using Core.Services.Business;
using Core.Services.Managers;

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
        private readonly IReportBusinessManager _reportBusinessManager;

        public SavedReportController(IMapper mapper, IViewRenderService viewRenderService,
             ICrudBusinessManager businessManager, IReportBusinessManager reportBusinessManager) {
            _mapper = mapper;
            _viewRenderService = viewRenderService;
            _businessManager = businessManager;
            _reportBusinessManager = reportBusinessManager;
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
                        Name = $"{from}-{to}",
                        CountDisplay = true
                    });
                }

                fields.Add(new SavedReportPlanFieldViewModel() {
                    Name = $"{1 + model.NumberOfPeriods * daysPerPeriod}+",
                    CountDisplay = true
                });

                fields.Add(new SavedReportPlanFieldViewModel() {
                    Name = "Total Late",
                    CountDisplay = true,
                    AmountReadOnly = true,
                    CountReadOnly = true
                }) ;

                fields.Add(new SavedReportPlanFieldViewModel() {
                    Name = "Total",
                    CountDisplay = true,
                    AmountReadOnly = true,
                    CountReadOnly = true
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

                var result = await _reportBusinessManager.CreateSavedPlanReport(_mapper.Map<SavedReportDto>(model));
                if(result == null)
                    return NotFound();

                return Ok(_mapper.Map<SavedReportPlanViewModel>(result));
            } catch(Exception er) {
                return BadRequest(er.Message ?? er.StackTrace);
            }
        }
    }
}