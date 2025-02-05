using System.Diagnostics;
using System.Text.Json;
using Classification.Models;
using Classification.SemanticKernel;
using Microsoft.AspNetCore.Mvc;

namespace Classification.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration iconfig)
        {
            _logger = logger;
            _config = iconfig;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("api/{action}")]
        [HttpPost]
        public async Task<IActionResult> Result([FromForm] ModelCallParameters modelcallparameters)
        {
            try
            {
                // classes initialization
                Classify classify = new Classify(_config, _logger);
                ClassificationResult result = new ClassificationResult() { Content = "No Output" };

                // Validating the Input got from the user.
                if (String.IsNullOrWhiteSpace(modelcallparameters.UserInput))
                {
                    return Json(new ClassificationResult() { ResultStatus = Status.Warning, Content = "Input or Prompt is Required" });
                }

                // parsing the model name to enum
                Model modelselected = Model.None;
                Enum.TryParse<Model>(modelcallparameters.ModelName, true, out modelselected);

                // if model name gets the value as none then give the error message for selecting the correct model
                if (modelselected == Model.None)
                {
                    return Json(new ClassificationResult() { ResultStatus = Status.Error, Content = "Select the Correct Model Name" });
                }
                // gets the result from the model
                result = await classify.GetResult(modelselected, modelcallparameters.UserInput);

                return Json(result);
            }
            catch (Exception ex)
            {
                // Logging the Error
                _logger.LogError(ex.Message);

                return Json(new ClassificationResult() { Content = "Failed to Get the Result", ResultStatus = Status.Error });
            }
        }

    }
}

