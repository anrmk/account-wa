using AutoMapper;

using Core.Context;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using Web.Extension;

namespace Web.Controllers.Mvc {
    public class ImportController: BaseController<ImportController> {

        public ImportController(ILogger<ImportController> logger, IMapper mapper, ApplicationContext context) : base(logger, mapper, context) {
        }

        public IActionResult Index() {
            return View();
        }
    }
}

namespace Web.Controllers.Api {
    public class ImportController: BaseApiController<ImportController> {
        private readonly IMemoryCache _memoryCache;
        private readonly IViewRenderService _viewRenderService;

        public ImportController(ILogger<ImportController> logger, IMapper mapper, IMemoryCache memoryCache,
           IViewRenderService viewRenderService) : base(logger, mapper) {
            _viewRenderService = viewRenderService;
            _memoryCache = memoryCache;
        }
    }
}