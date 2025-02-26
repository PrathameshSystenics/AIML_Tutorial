using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.SemanticKernel;
using ProductClassification.Extensions;
using ProductClassification.Models;
using ProductClassification.SemanticKernel;
using ProductClassification.Services;
using System.Text.Json;

namespace ProductClassification.Controllers
{
    public class ChatController : Controller
    {
        private ILogger<ChatController> _logger;
        private readonly RagChatService _ragchatservice;
        private readonly IConfiguration _configuration;

        public ChatController(ILogger<ChatController> logger, RagChatService ragchatservice, IConfiguration config)
        {
            _logger = logger;
            _ragchatservice = ragchatservice;
            _configuration = config;
        }

        public IActionResult Index(string model = "GeminiFlash2")
        {
            Dictionary<string, string> models = _configuration.MapConfigurationToClass<Dictionary<string, string>>("ChatCompletionModels");

            List<SelectListItem> modelitems = models.Keys.Select(key =>
            {
                bool isdefault = key.Equals(model);
                return new SelectListItem(models[key], key, isdefault);

            }).ToList();

            return View(modelitems);
        }

        [HttpPost]
        public async Task ChatCompletions([FromBody] ChatRequest chatrequest)
        {
            try
            {
                Response.ContentType = "text/event-stream";

                if (chatrequest == null)
                {
                    await SentSSEEventAsync(new
                    {
                        message = "The Request Body is Missing"
                    }, "error");
                    return;
                }
                ModelEnum selectedmodel = ModelEnum.None;
                Enum.TryParse<ModelEnum>(chatrequest.ModelId, out selectedmodel);

                if (selectedmodel == ModelEnum.None)
                {
                    await SentSSEEventAsync(new
                    {
                        message = "Select the Model first"
                    }, "error");
                    return;
                }

                IAsyncEnumerable<StreamingChatMessageContent> messagecontents = _ragchatservice.StreamChatMessagesAsync(chatrequest.ToChatHistory, selectedmodel);

                await foreach (StreamingChatMessageContent content in messagecontents)
                {
                    await SentSSEEventAsync(content, "chat");
                }

                await SentSSEEventAsync(new
                {
                    message = "Completed"
                }, "complete");
                return;
            }
            catch (HttpOperationException ex)
            {
                _logger.LogError("Message => {message}, Type => {type}", ex.Message, ex.GetType());
                if (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    await SentSSEEventAsync(new
                    {
                        message = "Rate Limit Exceed Please Try Again Later!",
                        exception = ex.Message
                    }, "error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Message => {message}, Type => {type}", ex.Message, ex.GetType());
                await SentSSEEventAsync(new
                {
                    message = "Error Occurred during Streaming the Responses",
                    exception = ex.Message
                }, "error");
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
    }
}
