using Microsoft.SemanticKernel;
using ProductClassification.SemanticKernel;

namespace ProductClassification.Services
{
    /// <summary>
    /// Various Connectors to Connect with the Kernel.
    /// </summary>
    public class AIConnectorService
    {
        private IConfiguration _config;
        private ILogger<AIConnectorService> _logger;

        public AIConnectorService(IConfiguration config, ILogger<AIConnectorService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public Kernel BuildModels()
        {
            IKernelBuilder _kernelbuilder = Kernel.CreateBuilder();
            try
            {
                #region Configs
                LLMConnectionConfig OllamaDeekSeekConfigs = _config.MapConfigurationToClass<LLMConnectionConfig>(Enum.GetName(ModelEnum.OllamaDeepSeek) ?? "");

                AzureOpenAIConnectionConfig AzureOpenAIConfigs = _config.MapConfigurationToClass<AzureOpenAIConnectionConfig>(Enum.GetName(ModelEnum.AzureOpenAI) ?? "");

                LLMConnectionConfig AzureDeepSeekConfigs = _config.MapConfigurationToClass<LLMConnectionConfig>(Enum.GetName(ModelEnum.AzureDeepSeek) ?? "");

                LLMConnectionConfig OllamaPhi3_8bConfigs = _config.MapConfigurationToClass<LLMConnectionConfig>(Enum.GetName(ModelEnum.OllamaPhi3_8b) ?? "");

                LLMConnectionConfig OllamaQwen4bConfigs = _config.MapConfigurationToClass<LLMConnectionConfig>(Enum.GetName(ModelEnum.OllamaQwen1_8b) ?? "");

                LLMConnectionConfig GeminiFlash2Configs = _config.MapConfigurationToClass<LLMConnectionConfig>(Enum.GetName(ModelEnum.GeminiFlash2) ?? "");

                #endregion

                _kernelbuilder = _kernelbuilder
                    // Ollama DeepSeek 
                    .AddOllamaChatCompletion(
                        serviceId: Enum.GetName(ModelEnum.OllamaDeepSeek),
                        modelId: OllamaDeekSeekConfigs.ModelName,
                        endpoint: new Uri(OllamaDeekSeekConfigs.Url)
                    )

                    // Azure OpenAI 
                    .AddAzureOpenAIChatCompletion(
                        serviceId: Enum.GetName(ModelEnum.AzureOpenAI),
                        deploymentName: AzureOpenAIConfigs.ModelName,
                        endpoint: AzureOpenAIConfigs.Url,
                        apiKey: AzureOpenAIConfigs.ApiKey,
                        modelId: AzureOpenAIConfigs.ModelVersion
                    )

                    // Azure DeepSeek
                    .AddAzureAIInferenceChatCompletion(
                        serviceId: Enum.GetName(ModelEnum.AzureDeepSeek),
                        modelId: AzureDeepSeekConfigs.ModelName,
                        apiKey: AzureDeepSeekConfigs.ApiKey,
                        endpoint: new Uri(AzureDeepSeekConfigs.Url)
                    )

                    // Ollama Phi3.8b Model
                    .AddOllamaChatCompletion(
                        serviceId: Enum.GetName(ModelEnum.OllamaPhi3_8b),
                        modelId: OllamaPhi3_8bConfigs.ModelName,
                        endpoint: new Uri(OllamaPhi3_8bConfigs.Url)
                    )

                    // Ollama Qwen Models
                    .AddOllamaChatCompletion(
                        serviceId: Enum.GetName(ModelEnum.OllamaQwen1_8b),
                        modelId: OllamaQwen4bConfigs.ModelName,
                        endpoint: new Uri(OllamaQwen4bConfigs.Url)
                    )

                    // Gemini flash 2
                    .AddGoogleAIGeminiChatCompletion(
                        modelId: GeminiFlash2Configs.ModelName,
                        serviceId: Enum.GetName<ModelEnum>(ModelEnum.GeminiFlash2),
                        apiKey: GeminiFlash2Configs.ApiKey
                    );

                return _kernelbuilder.Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }

    }
}
