using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using ProductClassification.Models;
using ProductClassification.SemanticKernel;
using ProductClassification.Services;
using System.Text.Json;

namespace ProductClassification.Controllers
{
    public class ChatController : Controller
    {
        private ILogger<ChatController> _logger;
        private RagChatService _ragchatservice;

        public ChatController(ILogger<ChatController> logger, RagChatService ragchatservice)
        {
            _logger = logger;
            _ragchatservice = ragchatservice;
        }

        public IActionResult Index()
        {
            return View();
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

                IAsyncEnumerable<StreamingChatMessageContent> messagecontents = _ragchatservice.ChatMessageContents(chatrequest.ToChatHistory, selectedmodel);

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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
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
