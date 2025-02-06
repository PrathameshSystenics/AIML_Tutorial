using System.Diagnostics;
using System.Text.Json;
using ProductClassification.Models;
using ProductClassification.SemanticKernel;
using Microsoft.AspNetCore.Mvc;
using ProductClassification.Services;

namespace ProductClassification.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ClassificationService _classificationService;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, ClassificationService classificationservice,IConfiguration config)
        {
            _logger = logger;
            _classificationService = classificationservice;
            _config = config;
        }

        public IActionResult Index()
        {
            // Retrieving the Models names from the configuration
            Dictionary<string, string> modelselector = _config.MapConfigurationToClass<Dictionary<string, string>>("Models");
            ViewBag.ModelNameWithValue = modelselector;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> ClassifyProductCategory([FromForm] ModelCallParameters modelcallparameters)
        {
            try
            {
                // Validating the Input/Prompt got from the user.
                if (String.IsNullOrWhiteSpace(modelcallparameters.UserInput))
                {
                    return BadRequest(new ClassificationResult() { ResultStatus = StatusEnum.Warning, Content = "Input or Prompt is Required" });
                }

                // parsing the model name to enum
                ModelEnum modelselected = ModelEnum.None;
                Enum.TryParse<ModelEnum>(modelcallparameters.ModelName, true, out modelselected);

                if (modelselected == ModelEnum.None)
                {
                    return BadRequest(new ClassificationResult() { ResultStatus = StatusEnum.Error, Content = "Select the Correct Model Name" });
                }

                // gets the result from the model
                ClassificationResult result = await _classificationService.ClassifyCategoryFromDescription(modelcallparameters.UserInput, modelselected);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return BadRequest(new ClassificationResult() { Content = "Failed to Get the Result", ResultStatus = StatusEnum.Error });
            }
        }

    }
}

