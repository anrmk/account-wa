using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers {
    public class FaqController: Controller {
        public IActionResult Index() {
            return View();
        }
    }
}