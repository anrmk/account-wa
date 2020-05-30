using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Core.Context;
using Core.Data.Dto;
using Core.Services.Business;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

using Web.Extension;
using Web.ViewModels;

namespace Web.Controllers.Mvc {
    public class InvoiceConstructorController: BaseController<InvoiceConstructorController> {
        public ICrudBusinessManager _businessManager;

        public InvoiceConstructorController(ILogger<InvoiceConstructorController> logger, IMapper mapper, ApplicationContext context,
            ICrudBusinessManager businessManager) : base(logger, mapper, context) {
            _businessManager = businessManager;
        }

        public async Task<IActionResult> Index() {
            var companies = await _businessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });

            var filters = await _businessManager.GetReportSearchCriterias();
            ViewBag.SearchCriterias = filters.Select(x => new SelectListItem() { Text = x.Name ?? $"Search criteria {x.Id}", Value = x.Id.ToString() });

            var model = new InvoiceConstructorFilterViewModel() {
                Date = DateTime.Now
            };
            return View(model);
        }
    }
}

namespace Web.Controllers.Api {
    [Route("api/[controller]")]
    public class InvoiceConstructorController: BaseApiController<InvoiceConstructorController> {
        private readonly IViewRenderService _viewRenderService;
        private readonly ICrudBusinessManager _businessManager;

        public InvoiceConstructorController(ILogger<InvoiceConstructorController> logger, IMapper mapper, ICrudBusinessManager businessManager, IViewRenderService viewRenderService) : base(logger, mapper) {
            _businessManager = businessManager;
            _viewRenderService = viewRenderService;
        }

        [HttpPost("GenerateConstructor", Name = "GenerateConstructor")]
        public async Task<IActionResult> GenerateConstructor(InvoiceConstructorFilterViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var company = await _businessManager.GetCompany(model.CompanyId);
                    var summaryRanges = await _businessManager.GetCompanyAllSummaryRange(model.CompanyId);
                    var searchCriterias = await _businessManager.GetReportSearchCriterias(model.SearchCriterias.ToArray());

                    var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) {
                        { "SummaryRanges", _mapper.Map<List<CompanySummaryRangeViewModel>>(summaryRanges) },
                        { "SearchCriterias", _mapper.Map<List<InvoiceConstructorSearchViewModel>>(searchCriterias)},
                        { "CompanyName", company.Name }
                    };

                    string html = _viewRenderService.RenderToStringAsync("_ConstructorPartial", model, viewDataDictionary).Result;

                    return Ok(html);
                }
            } catch(Exception er) {
                Console.Write(er.Message);
            }
            return null;
        }

        [HttpPost("GenerateConstructorInvoices", Name = "GenerateConstructorInvoices")]
        public async Task<IActionResult> CreateConstructorInvoices(InvoiceConstructorViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var result = await _businessManager.CreateConstructorInvoices(_mapper.Map<InvoiceConstructorDto>(model));
                    if(result == null)
                        NotFound();

                    return Ok(_mapper.Map<InvoiceConstructorViewModel>(result));
                }
            } catch(Exception er) {
                _logger.LogError(er, er.Message);
                BadRequest();
            }
            return null;
        }
    }
}