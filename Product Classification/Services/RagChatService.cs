using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using ProductClassification.Data;
using ProductClassification.SemanticKernel;
using ProductClassification.SemanticKernel.Plugins;

namespace ProductClassification.Services
{
    public class RagChatService
    {

        private Kernel _kernel;


        public RagChatService(AIConnectorService aiconnectorservice)
        {
            _kernel = aiconnectorservice.BuildModels();
        }

        public async IAsyncEnumerable<StreamingChatMessageContent> StreamChatMessagesAsync(ChatHistory userchathistory, ModelEnum modelselected)
        {
            string modelselectedservicekey = Enum.GetName<ModelEnum>(modelselected)!;

            


            IChatCompletionService chatcompletionservice = _kernel.GetRequiredService<IChatCompletionService>(modelselectedservicekey);

            ChatHistory chathistory = new ChatHistory();
            chathistory.AddSystemMessage(Prompt.ChatSystemPrompt);
            chathistory.AddRange(userchathistory);


            /* PromptExecutionSettings promptexecsettings = new PromptExecutionSettings()
             {
                 FunctionChoiceBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,
                 ServiceId = modelselectedservicekey
             };*/

            GeminiPromptExecutionSettings promptexecsettings = new GeminiPromptExecutionSettings()
            {
                ServiceId = modelselectedservicekey,
                ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions
            };

            IAsyncEnumerable<StreamingChatMessageContent> message = chatcompletionservice.GetStreamingChatMessageContentsAsync(chathistory, promptexecsettings);

            await foreach (StreamingChatMessageContent chatcontent in message)
            {
                yield return chatcontent;
            }
        }
    }
}
