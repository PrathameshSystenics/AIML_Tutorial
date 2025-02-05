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

        public HomeController(ILogger<HomeController> logger, ClassificationService classificationservice)
        {
            _logger = logger;
            _classificationService = classificationservice;
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

        [HttpPost]
        public async Task<IActionResult> Result([FromForm] ModelCallParameters modelcallparameters)
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

                return Json(new ClassificationResult() { Content = "Failed to Get the Result", ResultStatus = StatusEnum.Error });
            }
        }

    }
}

