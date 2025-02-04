using Classification.Models;
using Classification.SemanticKernel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace Classification.Controllers
{
    public class EvaluateController : Controller
    {
        private readonly ILogger<EvaluateController> _logger;
        private readonly IConfiguration _config;
        private string _modelname;

        public EvaluateController(ILogger<EvaluateController> logger, IConfiguration iconfig)
        {
            _logger = logger;
            _config = iconfig;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("api/{action}")]
        public JsonResult SetModel([FromForm] string ModelName)
        {
            if (String.IsNullOrWhiteSpace(ModelName))
            {
                return Json(new Dictionary<string, object>() { { "Content", "Select the Model" } });
            }
            // storing the modelname in the session
            HttpContext.Session.SetString("ModelName", ModelName);
            return Json(new Dictionary<string, object>() { { "Content", "Successfully Set the Model" } });
        }

        [HttpGet]
        public async Task EvalResult()
        {
            try
            {

            // Implementing the Server Sent Events
            Response.ContentType = "text/event-stream";

            // Reading the Evaluation.json File
            string evaljsonfile = "D:\\Training\\AIML\\Classification\\SemanticKernel\\Evaluation.json";
            string jsondata = new StreamReader(evaljsonfile).ReadToEnd();

            // Deserializing it using newtonsoft.json lib.
            EvaluationList evallist = JsonSerializer.Deserialize<EvaluationList>(jsondata);

            Classify classify = new Classify(_config, _logger);

            // Retrieve model name from session
            string modelName = HttpContext.Session.GetString("ModelName") ?? "";

            // parsing the model name to enum
            Model modelselected = Model.None;
            Enum.TryParse<Model>(modelName, true, out modelselected);

            // if model name gets the value as none then give the error message for selecting the correct model
            if (modelselected == Model.None)
            {
                 await WriteEventAsync(new EvalResult()
                {
                    Result = "Select the Model First",
                    Status = "Error"
                }, true);
            }

            int index = 0;
            bool isEvalCompleted = false;
            int totalquestion = evallist.Evaluate.Length;
            int countforcorrect = 0;
            bool isAnswerEquals = false;

            // Looping thorough each Description
            while (!isEvalCompleted)
            {
                isAnswerEquals = false;
                // Get the current evaluation Answer and Description
                Evaluation current = evallist.Evaluate[index];
                ClassificationResult result = new ClassificationResult();
                try
                {
                    // classify the result
                    result = await classify.GetResult(modelselected, current.Description);

                    // if the result contains the answer then increment the correct question and answer equals
                    if (result.Content.Contains(current.Answer))
                    {
                        countforcorrect++;
                        isAnswerEquals = true;
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + "\nType => " + ex.GetType().ToString());
                }
                finally
                {
                    index++;

                    // if all eval questions are completed then stop the loop
                    if (totalquestion == index)
                    {
                        isEvalCompleted = true;

                    }
                }

                // If the answer is correct then yield the answer.
                if (isAnswerEquals)
                {

                    EvalResult evaluationresult = new EvalResult()
                    {
                        Status = "Correct",
                        Description = current.Description,
                        Expected = current.Answer,
                        Result = result.Content,
                        Correct = countforcorrect,
                        Total = totalquestion
                    };

                    // Sending the Server Events
                    await WriteEventAsync(evaluationresult);
                }
                else
                {
                    EvalResult evaluationresult = new EvalResult
                    {
                        Status = "Wrong",
                        Description = current.Description,
                        Expected = current.Answer,
                        Result = result.Content,
                        Correct = countforcorrect,
                        Total = totalquestion
                    };

                    // Sending the Server Events
                    await WriteEventAsync(evaluationresult);
                }
            }

            // return the final answer with Accuracy
            EvalResult completedevalresult = new EvalResult
            {
                Status = "Complete",
                Correct = countforcorrect,
                Total = totalquestion,
                Accuracy = (((double)countforcorrect / totalquestion) * 100)
            };

            // Sending the Server Events as complete
            await WriteEventAsync(completedevalresult, true);

            }
            catch (Exception ex)
            {

            }
        }


        private async Task WriteEventAsync(object data, bool iscompleted = false)
        {
            // sents the events asyncrohonusly
            var json = JsonSerializer.Serialize(data);
            await Response.WriteAsync($"data: {json}\n\n");


            // if the data is completed then complete the response.
            if (iscompleted)
            {
                //await Response.WriteAsync("retry: 0\n\n");
                await Response.Body.FlushAsync();
                // Complete the response to prevent further data from being sent
                await HttpContext.Response.CompleteAsync();
                return;
            }
            else
            {
                await Response.Body.FlushAsync();
            }
        }
    }
}
