using Microsoft.SemanticKernel;
using ProductClassification.Models;
using ProductClassification.SemanticKernel;

namespace ProductClassification.Services
{
    public class ClassificationService
    {
        private ILogger<ClassificationService> _logger;
        private readonly AIConnectorService _connectorService;
        private Kernel _kernel;

        public ClassificationService(ILogger<ClassificationService> logger, AIConnectorService aiconnectorservice)
        {
            _logger = logger;
            _connectorService = aiconnectorservice;
            _kernel = _connectorService.BuildModels();
        }

        public async Task<ClassificationResult> ClassifyCategoryFromDescription(string description, ModelEnum selectedmodel)
        {
            try
            {
                string serviceid = Enum.GetName(typeof(ModelEnum), selectedmodel) ?? "";

                _logger.LogInformation("Kernel Connected to => " + serviceid);

                // If ServiceID found to be empty or null then return the result as error.
                if (string.IsNullOrWhiteSpace(serviceid))
                {
                    return new ClassificationResult() { Content = "Select the Model First", ResultStatus = StatusEnum.Error };
                }

                PromptExecutionSettings executionSettings = new PromptExecutionSettings()
                {
                    ServiceId = serviceid,
                    ExtensionData = new Dictionary<string, object>() {
                        { "temperature" ,0.0}
                    }
                };

                KernelArguments arguments = new KernelArguments(executionSettings);

                // Invoking the Prompt
                description = $"""<message role="user">**Description**: {description} </message>""";
                FunctionResult result = await _kernel.InvokePromptAsync(Prompt.BasePrompt + description, arguments);

                return new ClassificationResult() { Content = result.ToString(), ResultStatus = StatusEnum.Success, ModelId = serviceid, Extras = result.Metadata };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                throw;
            }
        }
    }
}
