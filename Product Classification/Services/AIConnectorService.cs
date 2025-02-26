using Microsoft.SemanticKernel;
using ProductClassification.SemanticKernel;
using ProductClassification.Extensions;
using ProductClassification.SemanticKernel.Plugins;

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

                LLMConnectionConfig GeminiFlash1_5Configs = _config.MapConfigurationToClass<LLMConnectionConfig>(Enum.GetName(ModelEnum.GeminiFlash1_5) ?? "");

                LLMConnectionConfig GeminiFlash2_ThinkingConfigs = _config.MapConfigurationToClass<LLMConnectionConfig>(Enum.GetName(ModelEnum.GeminiFlash2_0Thinking) ?? "");

                LLMConnectionConfig GoogleTextEmbeddingConfigs = _config.MapConfigurationToClass<LLMConnectionConfig>(Enum.GetName(ModelEnum.GoogleTextEmbedding_004) ?? "");

                LLMConnectionConfig OllamaNomicEmbedConfigs = _config.MapConfigurationToClass<LLMConnectionConfig>(Enum.GetName(ModelEnum.OllamaNomicEmbed_Text) ?? "");

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

                    // Gemini flash 2 model
                    .AddGoogleAIGeminiChatCompletion(
                        modelId: GeminiFlash2Configs.ModelName,
                        serviceId: Enum.GetName<ModelEnum>(ModelEnum.GeminiFlash2),
                        apiKey: GeminiFlash2Configs.ApiKey
                    )

                    // Gemini Flash 1.5 Models
                    .AddGoogleAIGeminiChatCompletion(
                        modelId: GeminiFlash1_5Configs.ModelName,
                        serviceId: Enum.GetName<ModelEnum>(ModelEnum.GeminiFlash1_5),
                        apiKey: GeminiFlash1_5Configs.ApiKey
                    )

                    // Gemini Flash 2.0 Thinkings Models
                    .AddGoogleAIGeminiChatCompletion(
                        modelId: GeminiFlash2_ThinkingConfigs.ModelName,
                        serviceId: Enum.GetName<ModelEnum>(ModelEnum.GeminiFlash2_0Thinking),
                        apiKey: GeminiFlash2_ThinkingConfigs.ApiKey
                    )

                    // Gemini Text Embedding 004 Model
                    .AddGoogleAIEmbeddingGeneration(
                        modelId: GoogleTextEmbeddingConfigs.ModelName,
                        apiKey: GoogleTextEmbeddingConfigs.ApiKey,
                        serviceId: Enum.GetName<ModelEnum>(ModelEnum.GoogleTextEmbedding_004)
                    )

                    // Ollama Nomic Embedded Text
                    .AddOllamaTextEmbeddingGeneration(
                        serviceId: Enum.GetName(ModelEnum.OllamaNomicEmbed_Text),
                        modelId: OllamaNomicEmbedConfigs.ModelName,
                        endpoint: new Uri(OllamaNomicEmbedConfigs.Url)
                );

                _kernelbuilder.Plugins.AddFromObject(new ProductPlugin());
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
