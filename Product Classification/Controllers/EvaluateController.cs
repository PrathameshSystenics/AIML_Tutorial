using Microsoft.AspNetCore.Mvc;
using ProductClassification.Data;
using ProductClassification.Models;
using ProductClassification.SemanticKernel;
using ProductClassification.Services;
using System.Text.Json;
using ProductClassification.Extensions;

namespace ProductClassification.Controllers
{
    public class EvaluateController : Controller
    {
        private readonly ILogger<EvaluateController> _logger;
        private readonly IConfiguration _config;
        private readonly EvaluationService _evaluationService;
        private readonly EvaluationDataRepository _evaldatarepo;

        public EvaluateController(ILogger<EvaluateController> logger, IConfiguration config, EvaluationService evalservice, EvaluationDataRepository evaldatarepo)
        {
            _logger = logger;
            _config = config;
            _evaluationService = evalservice;
            _evaldatarepo = evaldatarepo;
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

                IAsyncEnumerable<EvaluatedResult> evaluationTask = _evaluationService.EvaluateProductCategoryBatch(modelselected);

                await foreach (var results in evaluationTask)
                {
                    if (results.EvaluationMetrics != null)
                    {
                        await SentSSEEventAsync(results.EvaluationMetrics, "completed");
                    }
                    else
                    {
                        await SentSSEEventAsync(results);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await SentSSEEventAsync(new EvaluatedResult() { Result = "Failed to Evaluate the Model" }, "error");
            }
        }

        /// <summary>
        /// Sends the Data using Server Sent Events
        /// </summary>
        /// <param name="data"> want to send continuously to the client</param>
        /// <param name="customevent">Event to apply to it.</param>
        private async Task SentSSEEventAsync(object data, string customevent = "result")
        {
            try
            {
                string json = JsonSerializer.Serialize(data);
                await Response.WriteAsync($"event: {customevent}\n");
                await Response.WriteAsync($"data: {json}\n\n");
                await Response.Body.FlushAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async Task<IActionResult> PreviousEvalResult()
        {
            try
            {

                List<EvaluationBatch> batches = await _evaldatarepo.GetEvaluationBatchesWithMetrics();
                return View(batches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("Index");
            }
        }

        public IActionResult EvalResultByBatch(int id)
        {
            EvaluationBatch batch = _evaldatarepo.GetEvaluationResultsByBatch(id);
            if (batch == null)
            {
                return RedirectToAction("PreviousEvalResult");
            }

            // Getting the total time Taken to Complete the Evaluation
            var timeDifference = batch.EvaluatedResults.LastOrDefault()?.CreatedAt
                        .Subtract(batch.EvaluatedResults.FirstOrDefault()?.CreatedAt ?? DateTime.MinValue);

            string formattedTime = timeDifference.HasValue
                ? $"{timeDifference.Value.Hours}h {timeDifference.Value.Minutes}m {timeDifference.Value.Seconds}s {timeDifference.Value.Milliseconds}ms"
                : "N/A";

            ViewData["totalEvalTime"] = formattedTime;

            return View(batch);
        }

    }
}

