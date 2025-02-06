using Microsoft.AspNetCore.Mvc;

namespace ProductClassification.Controllers
{
    public class TrialController : Controller
    {


        public IActionResult Index()
        {
            return View();
        }
    }
}
