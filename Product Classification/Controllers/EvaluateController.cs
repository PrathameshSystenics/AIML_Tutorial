using ProductClassification.Models;
using ProductClassification.SemanticKernel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;
using ProductClassification.Services;

namespace ProductClassification.Controllers
{
    public class EvaluateController : Controller
    {
        private readonly ILogger<EvaluateController> _logger;
        private readonly IConfiguration _config;
        private readonly EvaluationService _evaluationService;

        public EvaluateController(ILogger<EvaluateController> logger, IConfiguration iconfig, EvaluationService evalservice)
        {
            _logger = logger;
            _config = iconfig;
            _evaluationService = evalservice;
        }

        public IActionResult Index()
        {
            // Retrieving the Models names from the configuration
            Dictionary<string, string> modelselector = _config.MapConfigurationToClass<Dictionary<string, string>>("Models");
            ViewBag.ModelNameWithValue = modelselector;
            return View();
        }

        [HttpPost]
        public ObjectResult SetModelForEvaluation([FromForm] string ModelName)
        {
            if (String.IsNullOrWhiteSpace(ModelName))
            {
                return BadRequest(new Dictionary<string, object>() { { "Content", "Select the Model" } });
            }
            // storing the modelname in the session
            HttpContext.Session.SetString("ModelName", ModelName);
            return Ok(new Dictionary<string, object>() { { "Content", "Successfully Set the Model" } });
        }

        [HttpGet]
        public async Task EvaluateAndStreamResults()
        {
            Response.ContentType = "text/event-stream";
            try
            {
                // Retrieve model name from session
                string modelName = HttpContext.Session.GetString("ModelName") ?? "";

                // parsing the model name to enum
                ModelEnum modelselected = ModelEnum.None;
                Enum.TryParse<ModelEnum>(modelName, true, out modelselected);

                // if model name gets the value as none then give the error message for selecting the correct model
                if (modelselected == ModelEnum.None)
                {
                    await SentSSEEventAsync(
                        new EvaluatedResult()
                        {
                            Result = "Select the Model First",
                        }, "error"
                    );
                    return;
                }

                IAsyncEnumerable<EvaluatedResult> evaluationTask = _evaluationService.RunCategoryEvaluationBatch(modelselected);

                await foreach (var results in evaluationTask)
                {
                    await SentSSEEventAsync(results);

                    if (results.EvaluationMetrics != null)
                    {
                        await SentSSEEventAsync(results.EvaluationMetrics, "complete");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }


        private async Task SentSSEEventAsync(object data, string customevent = "message")
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                await Response.WriteAsync($"event: {customevent}\n\n");
                await Response.WriteAsync($"data: {json}\n\n");
                await Response.Body.FlushAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

    }
}

