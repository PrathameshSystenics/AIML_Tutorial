using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using ProductClassification.Data;
using ProductClassification.SemanticKernel;

namespace ProductClassification.Services
{
    public class RagChatService
    {
        private ProductDataRepository _productdatarepo;
        private Kernel _kernel;

        public RagChatService(AIConnectorService aiconnectorservice, ProductDataRepository productdatarepo)
        {
            _kernel = aiconnectorservice.BuildModels();
            _productdatarepo = productdatarepo;
        }

        public async IAsyncEnumerable<StreamingChatMessageContent> ChatMessageContents(ChatHistory userchathistory, ModelEnum modelselected)
        {
            string modelselectedservicekey = Enum.GetName<ModelEnum>(modelselected)!;

            // TODO: Add the Plugin Later on

            IChatCompletionService chatcompletionservice = _kernel.GetRequiredService<IChatCompletionService>(modelselectedservicekey);

            ChatHistory chathistory = new ChatHistory();
            chathistory.AddSystemMessage(Prompt.ChatSystemPrompt);
            chathistory.AddRange(userchathistory);

            PromptExecutionSettings promptexecsettings = new PromptExecutionSettings()
            {
                ServiceId = modelselectedservicekey
            };

            IAsyncEnumerable<StreamingChatMessageContent> message = chatcompletionservice.GetStreamingChatMessageContentsAsync(chathistory, promptexecsettings);

            await foreach (StreamingChatMessageContent chatcontent in message)
            {
                yield return chatcontent;
            }
        }
    }
}
