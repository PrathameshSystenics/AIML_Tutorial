using Microsoft.SemanticKernel;

namespace Classification.SemanticKernel
{
    /// <summary>
    /// Various Connectors to Connected with the System.
    /// </summary>
    public class Connections
    {
        private IKernelBuilder _kernelbuilder;
        private IConfiguration _config;
        private ILogger _logger;

        public Connections(IConfiguration config, ILogger logger)
        {
            // Kernel builder instance
            _kernelbuilder = Kernel.CreateBuilder();

            _config = config;
            _logger = logger;

        }

        public Kernel UseOllama_DeepSeekModel()
        {
            // Ollama Chat Completion
            Kernel kernel = this._kernelbuilder.AddOllamaChatCompletion(
                modelId: _config["DeepSeekLocal:ModelName"] ?? "",
                endpoint: new Uri(_config["DeepSeekLocal:Url"] ?? "")
                ).Build();

            _logger.LogInformation("Kernel Connected to DeepSeekModel");
            return kernel;
        }

        public Kernel UseAzure_OpenAI()
        {
            // Azure OpenAI Chat Completion
            Kernel kernel = this._kernelbuilder.AddAzureOpenAIChatCompletion(
                deploymentName: _config["AzureOpenAI:ModelName"].ToString(),
                endpoint: _config["AzureOpenAI:Url"].ToString(),
                apiKey: _config["AzureOpenAI:ApiKey"].ToString(),
                modelId: _config["AzureOpenAI:ModelVersion"].ToString()).Build();

            _logger.LogInformation("Kernel Connected to AzureOpenAI");
            return kernel;
        }

        public Kernel UseAzure_DeepSeek()
        {
            // Azure AIInference
            Kernel kernel = this._kernelbuilder.AddAzureAIInferenceChatCompletion(
                modelId: _config["DeepSeekCloud:ModelName"],
                apiKey: _config["DeepSeekCloud:ApiKey"],
               endpoint: new Uri(_config["DeepSeekCloud:Url"])).Build();

            _logger.LogInformation("Kernel Connected to AzureDeepSeek");
            return kernel;
        }

    }
}
