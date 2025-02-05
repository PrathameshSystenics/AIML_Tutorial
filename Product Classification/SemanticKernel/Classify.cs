using Classification.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace Classification.SemanticKernel
{
    /// <summary>
    /// Model Selection Enum
    /// </summary>
    public enum Model
    {
        None,
        OllamaDeepSeek,
        AzureOpenAI,
        AzureDeepSeek

    }

    public class Classify
    {
        private Connections _connections;
        private ILogger _logger;

        public Classify(IConfiguration config, ILogger logger)
        {
            _connections = new Connections(config, logger);
            _logger = logger;
        }

        public async Task<ClassificationResult> GetResult(Model modelselection, string userprompt)
        {
            try
            {
                Kernel kernel = new Kernel();
                PromptExecutionSettings executionSettings = new PromptExecutionSettings();

                // if the ollama deep seek model is selected then acquire its kernel
                if (modelselection == Model.OllamaDeepSeek)
                {
                    kernel = _connections.UseOllama_DeepSeekModel();
                    executionSettings = new OllamaPromptExecutionSettings();
                }
                // Azure Open AI model Selection
                else if (modelselection == Model.AzureOpenAI)
                {
                    kernel = _connections.UseAzure_OpenAI();
                }
                // Azure DeepSeek Model Selection
                else if (modelselection == Model.AzureDeepSeek)
                {
                    kernel = _connections.UseAzure_DeepSeek();
                }

                // Prompting and invoking the Model
                userprompt = $"""<message role="user">**Description**: {userprompt} </message>""";
                var result = await kernel.InvokePromptAsync(Prompt.BasePrompt + userprompt, new(executionSettings) { });

                string output = result.ToString();

                // removing the thinking steps from the result for the following models
                if (modelselection == Model.OllamaDeepSeek || modelselection == Model.AzureDeepSeek)
                {
                    //output = RemoveThinkingStepsFromResult(result.ToString());
                }

                return new ClassificationResult() { Content = output, ResultStatus = Status.Success, ModelId = Enum.GetName<Model>(modelselection) ?? "", Extras = result.Metadata };
            }
            catch (Exception ex)
            {
                // Logging the Error
                _logger.LogError(ex.Message);

                return new ClassificationResult() { Content = "Error Occured while Invoking the Prompt", ResultStatus = Status.Error, ModelId = Enum.GetName<Model>(modelselection) ?? "" };
            }
        }

        /// <summary>
        /// Removes the thinking steps from the Result of the model generated. Basically removing the <pre>
        /// "<think></think>"
        /// </pre> Tag from the Output.
        /// </summary>
        /// <param name="output">Output or Response from the model</param>
        /// <returns>Actual Result</returns>
        public string RemoveThinkingStepsFromResult(string output)
        {
            var splittedtext = output.Split("</think>");
            return splittedtext[1];
        }

 /*       public async Task<ClassificationResult> GetResult_ChatHistory(Model modelselection, string userprompt, Kernel kernel)
        {
            try
            {
                PromptExecutionSettings executionSettings = new PromptExecutionSettings();

                // Chat Completion Service
                IChatCompletionService chatcompletionservice = kernel.GetRequiredService<IChatCompletionService>
                    ();

                // ChatHistory
                ChatHistory chathistory = new ChatHistory();

                chathistory.AddSystemMessage(Prompt.SystemPrompt);
                chathistory.AddUserMessage($"**Description:** {userprompt}");

                var results =await chatcompletionservice.GetChatMessageContentsAsync(chathistory,)

            }
            catch (Exception ex)
            {
                // Logging the Error
                _logger.LogError(ex.Message);

                return new ClassificationResult() { Content = "Error Occured while Invoking the Prompt", ResultStatus = Status.Error, ModelId = Enum.GetName<Model>(modelselection) ?? "" };
            }
        }*/
    }
}
