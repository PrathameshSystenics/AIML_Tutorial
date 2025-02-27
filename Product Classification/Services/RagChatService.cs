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
        private ProductPlugin _productplugin;


        public RagChatService(AIConnectorService aiconnectorservice, ProductPlugin productplugin)
        {
            _kernel = aiconnectorservice.BuildModels();
            _productplugin = productplugin;
        }

        public async IAsyncEnumerable<StreamingChatMessageContent> StreamChatMessagesAsync(ChatHistory userchathistory, ModelEnum modelselected)
        {
            _kernel.Plugins.AddFromObject(_productplugin);
            string modelselectedservicekey = Enum.GetName<ModelEnum>(modelselected)!;

            IChatCompletionService chatcompletionservice = _kernel.GetRequiredService<IChatCompletionService>(modelselectedservicekey);

            ChatHistory chathistory = new ChatHistory();
            chathistory.AddSystemMessage(Prompt.ChatSystemPrompt);
            chathistory.AddRange(userchathistory);

            PromptExecutionSettings promptexecsettings = PromptExecutionSettingsProvider.CreateExecutionSettingsForModel(modelselected);

            IAsyncEnumerable<StreamingChatMessageContent> message = chatcompletionservice.GetStreamingChatMessageContentsAsync(chathistory, kernel: _kernel, executionSettings: promptexecsettings);

            await foreach (StreamingChatMessageContent chatcontent in message)
            {
                yield return chatcontent;
            }
        }
    }
}
