using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Core.Context;
using Core.Data.Dto;
using Core.Data.Enum;
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
        private readonly ICrudBusinessManager _businessManager;
        private readonly ICompanyBusinessManager _companyBusinessManager;

        public InvoiceConstructorController(ILogger<InvoiceConstructorController> logger, IMapper mapper, ApplicationContext context,
            ICrudBusinessManager businessManager,
            ICompanyBusinessManager companyBusinessManager) : base(logger, mapper, context) {
            _businessManager = businessManager;
            _companyBusinessManager = companyBusinessManager;
        }

        public async Task<IActionResult> Index() {
            var companies = await _companyBusinessManager.GetCompanies();
            ViewBag.Companies = companies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });

            var filters = await _businessManager.GetInvoiceConstructorSearchCriterias();
            ViewBag.SearchCriterias = filters.Select(x => new SelectListItem() { Text = x.Name ?? $"Search criteria {x.Id}", Value = x.Id.ToString() });

            var model = new InvoiceConstructorFilterViewModel() {
                Date = DateTime.Now.LastDayOfMonth()
            };
            return View(model);
        }

        public async Task<IActionResult> View(long id) {
            var constructor = await _businessManager.GetConstructorInvoice(id);

            var summaryRange = await _companyBusinessManager.GetSummaryRange(constructor.SummaryRangeId);
            ViewBag.SummaryRange = $"{summaryRange.From} - {summaryRange.To}";

            var searchCriteria = await _businessManager.GetInvoiceConstructorSearchCriteria(constructor.SearchCriteriaId);
            ViewBag.SearchCriteria = _mapper.Map<InvoiceConstructorSearchViewModel>(searchCriteria);

            if(searchCriteria.Group == CustomerGroupType.OnlyNew) {
                ViewBag.CreatedDate = $"{constructor.Date.FirstDayOfMonth().ToString("MM/dd/yyyy")} - {constructor.Date.LastDayOfMonth().ToString("MM/dd/yyyy")}";
            } else if(searchCriteria.Group == CustomerGroupType.ExcludeNew) {
                ViewBag.CreatedDate = $"None - {constructor.Date.AddMonths(-1).LastDayOfMonth().ToString("MM/dd/yyyy")}";
            } else if(searchCriteria.Group == CustomerGroupType.All) {
                ViewBag.CreatedDate = $"None - {constructor.Date.LastDayOfMonth().ToString("MM/dd/yyyy")}";
            }

            var tags = await _businessManager.GetCustomerTags();
            ViewBag.Tags = string.Join(',', tags.Where(x => searchCriteria.TagsIds.Contains(x.Id)).Select(x => x.Name));

            var types = await _businessManager.GetCustomerTypes();
            ViewBag.Types = string.Join(',', types.Where(x => searchCriteria.TypeIds.Contains(x.Id)).Select(x => x.Name));

            var constructors = await _businessManager.GetConstructorInvoices(constructor.CompanyId, constructor.Date);
            constructors = constructors.Where(x => x.SearchCriteriaId == constructor.SearchCriteriaId).ToList();

            var invoices = await _businessManager.GetInvoiceDraft(constructors.Select(x => x.Id).ToArray());
            ViewBag.Invoices = invoices.Count();

            var customers = await _businessManager.GetCustomers(constructor);
            ViewBag.Customers = customers.Count();

            var model = _mapper.Map<InvoiceConstructorViewModel>(constructor);
            if(IsAjaxRequest)
                return PartialView(model);
            else
                return View(model);
        }

        [HttpPost("CopyInvoiceFromDraft", Name = "CopyInvoiceFromDraft")]
        public async Task<IActionResult> CopyInvoiceFromDraft(long[] ids) {
            if(ids.Length == 0) {
                return Ok("No items to save");
            }

            var item = await _businessManager.CopyInvoiceFromDraft(ids);
            return Ok($"You have saved {item?.Count ?? 0} invoices in the Data Base.");

        }

        [HttpPost("DeleteDraftInvoices", Name = "DeleteDraftInvoices")]
        public async Task<IActionResult> DeleteDraftInvoices(long[] ids) {
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
        private readonly ICompanyBusinessManager _companyBusinessManager;


        public InvoiceConstructorController(ILogger<InvoiceConstructorController> logger, IMapper mapper, ICompanyBusinessManager companyBusinessManager, ICrudBusinessManager businessManager, IViewRenderService viewRenderService) : base(logger, mapper) {
            _businessManager = businessManager;
            _companyBusinessManager = companyBusinessManager;
            _viewRenderService = viewRenderService;
        }

        [HttpGet("GetDraftCustomers", Name = "GetDraftCustomers")]
        public async Task<Pager<CustomerListViewModel>> GetDraftCustomers([FromQuery] InvoiceDraftFilterViewModel model) {
            var constructor = await _businessManager.GetConstructorInvoice(model.ConstructorId ?? 0);

            var constructors = await _businessManager.GetConstructorInvoices(constructor.CompanyId, constructor.Date);
            constructors = constructors.Where(x => x.SearchCriteriaId == constructor.SearchCriteriaId).ToList();

            var invoices = await _businessManager.GetInvoiceDraft(constructors.Select(x => x.Id).ToArray());

            var customers = await _businessManager.GetCustomers(constructor);

            customers = customers.Where(x => !invoices.Any(y => y.CustomerId == x.Id)).ToList();
            var count = customers.Count();
            customers = customers.Skip(model.Offset).Take(model.Limit).ToList();

            if(customers.Count == 0)
                return new Pager<CustomerListViewModel>(new List<CustomerListViewModel>(), 0, model.Offset, model.Limit);

            var page = (model.Offset + model.Limit) / model.Limit;

            var result = _mapper.Map<List<CustomerListViewModel>>(customers);
            return new Pager<CustomerListViewModel>(result, count, page, model.Limit);
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
                if(!ModelState.IsValid) {
                    throw new Exception("Form is not valid!");
                }
                var company = await _companyBusinessManager.GetCompany(model.CompanyId);
                var summary = await _companyBusinessManager.GetSummaryRanges(model.CompanyId);
                var constructorSearches = await _businessManager.GetInvoiceConstructorSearchCriterias(model.SearchCriterias.ToArray());
                var constructors = await _businessManager.GetConstructorInvoices(model.CompanyId, model.Date ?? DateTime.Now);

                foreach(var constructorSearch in constructorSearches) {
                    foreach(var summaryRange in summary) {
                        var constructor = constructors.Where(x => x.SearchCriteriaId == constructorSearch.Id && x.SummaryRangeId == summaryRange.Id).FirstOrDefault();
                        if(constructor == null) {
                            var entity = await _businessManager.CreateConstructorInvoice(new InvoiceConstructorDto() {
                                CompanyId = model.CompanyId,
                                Date = model.Date.Value,
                                SearchCriteriaId = constructorSearch.Id,
                                SummaryRangeId = summaryRange.Id
                            });

                            constructors.Add(entity);
                        } else {
                            //update
                        }
                    }
                }

                var customerCounts = new Dictionary<long, int>();
                foreach(var searchCriteria in constructorSearches) {
                    var constructor = new InvoiceConstructorDto() {
                        CompanyId = model.CompanyId,
                        Date = model.Date ?? DateTime.Now,
                        SearchCriteriaId = searchCriteria.Id,
                    };

                    var customers = await _businessManager.GetCustomers(constructor);
                    if(!customerCounts.ContainsKey(searchCriteria.Id))
                        customerCounts.Add(searchCriteria.Id, customers.Count);
                }

                var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) {
                        { "SummaryRanges", _mapper.Map<List<CompanySummaryRangeViewModel>>(summary) },
                        { "SearchCriterias", _mapper.Map<List<InvoiceConstructorSearchViewModel>>(constructorSearches)},
                        { "CompanyName", company.Name },
                        { "Constructors", _mapper.Map<List<InvoiceConstructorViewModel>>(constructors) },
                        { "CustomerCounts", customerCounts }
                    };

                string html = _viewRenderService.RenderToStringAsync("_ConstructorPartial", model, viewDataDictionary).Result;
                return Ok(html);
            } catch(Exception er) {
                return BadRequest(er.Message ?? er.StackTrace);
            }
        }

        [HttpPost("CreateConstructorInvoices", Name = "CreateConstructorInvoices")]
        public async Task<IActionResult> CreateConstructorInvoices([FromBody] InvoiceConstructorViewModel model) {
            try {
                if(!ModelState.IsValid) {
                    throw new Exception("Form is not valid!");
                }
                var result = await _businessManager.CreateInvoiceDraft(_mapper.Map<InvoiceConstructorDto>(model));
                if(result == null)
                    return NotFound();

                return Ok(_mapper.Map<InvoiceConstructorViewModel>(result));
            } catch(Exception er) {
                return BadRequest(er.Message ?? er.StackTrace);
            }
        }

        [HttpPost("UpdateDraftInvoices", Name = "UpdateDraftInvoices")]
        public async Task<IActionResult> UpdateDraftInvoices(long id) {
            var result = await _businessManager.UpdateInvoiceDraft(id);
            int count = 0;
            decimal totalAmount = 0;
            if(result != null) {
                count = result.Count();
                totalAmount = result.Sum(x => x.Subtotal);
            }
            return Ok(new { Count = count, TotalAmount = totalAmount });
        }
    }
}