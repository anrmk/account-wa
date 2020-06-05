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
    public class InvoiceConstructorController: BaseController<InvoiceConstructorController> {
        public ICrudBusinessManager _businessManager;

        public InvoiceConstructorController(ILogger<InvoiceConstructorController> logger, IMapper mapper, ApplicationContext context,
            ICrudBusinessManager businessManager) : base(logger, mapper, context) {
            _businessManager = businessManager;
        }

        public async Task<IActionResult> Index() {
            var companies = await _businessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });

            var filters = await _businessManager.GetInvoiceConstructorSearchCriterias();
            ViewBag.SearchCriterias = filters.Select(x => new SelectListItem() { Text = x.Name ?? $"Search criteria {x.Id}", Value = x.Id.ToString() });

            var model = new InvoiceConstructorFilterViewModel() {
                Date = DateTime.Now
            };
            return View(model);
        }

        public async Task<IActionResult> View(long id) {
            var item = await _businessManager.GetConstructorInvoice(id);
            var model = _mapper.Map<InvoiceConstructorViewModel>(item);
            if(IsAjaxRequest)
                return PartialView(model);
            else
                return View(model);
        }

        [HttpPost("CreateDraftInvoices", Name = "CreateDraftInvoices")]
        public async Task<IActionResult> CreateDraftInvoices(long[] ids) {
            if(ids.Length > 0) {
                var item = await _businessManager.CreateInvoiceDraft(ids);
                return Ok($"You have saved {item.Count} invoices in the Data Base.");
            }
            return Ok("No items to save");
        }

        [HttpPost("DeleteDraftInvoices", Name = "DeleteDraftInvoices")]
        public async Task<ActionResult> DeleteDraftInvoices(long[] ids) {
            if(ids.Length > 0) {
                var result = await _businessManager.DeleteInvoiceDraft(ids);
                return Ok(result);
            }

            return Ok(false);
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

        [HttpGet("GetDraftInvoices", Name = "GetDraftInvoices")]
        public async Task<Pager<InvoiceDraftViewModel>> GetDraftInvoices([FromQuery] InvoiceDraftFilterViewModel model) {
            var result = await _businessManager.GetInvoiceDraftPage(_mapper.Map<InvoiceDraftFilterDto>(model));
            var list = _mapper.Map<List<InvoiceDraftViewModel>>(result.Items);

            return new Pager<InvoiceDraftViewModel>(list, result.TotalItems, result.CurrentPage, result.PageSize, result.Params);
        }

        [HttpPost("GenerateConstructor", Name = "GenerateConstructor")]
        public async Task<IActionResult> GenerateConstructor(InvoiceConstructorFilterViewModel model) {
            try {
                if(ModelState.IsValid) {
                    var company = await _businessManager.GetCompany(model.CompanyId);
                    var summaryRanges = await _businessManager.GetCompanyAllSummaryRange(model.CompanyId);
                    var searchCriterias = await _businessManager.GetInvoiceConstructorSearchCriterias(model.SearchCriterias.ToArray());
                    var constructors = await _businessManager.GetConstructorInvoices(model.CompanyId, model.Date ?? DateTime.Now);

                    var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) {
                        { "SummaryRanges", _mapper.Map<List<CompanySummaryRangeViewModel>>(summaryRanges) },
                        { "SearchCriterias", _mapper.Map<List<InvoiceConstructorSearchViewModel>>(searchCriterias)},
                        { "CompanyName", company.Name },
                        { "Constructors", _mapper.Map<List<InvoiceConstructorViewModel>>(constructors) }
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