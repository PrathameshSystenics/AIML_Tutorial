using Microsoft.SemanticKernel;
using ProductClassification.Models;
using ProductClassification.SemanticKernel;

namespace ProductClassification.Services
{

    // TODO: Inject these Service
    public class ClassificationService
    {
        private ILogger<ClassificationService> _logger;

        public ClassificationService(ILogger<ClassificationService> logger)
        {
            _logger = logger;
        }

        public async Task<ClassificationResult> ClassifyCategoryFromDescription(string description, ModelEnum selectedmodel)
        {
            try
            {
                Kernel kernel = new Kernel();

                string serviceid = Enum.GetName(typeof(ModelEnum), selectedmodel) ?? "";

                // If ServiceID found to be empty or null then return the result as error.
                if (string.IsNullOrWhiteSpace(serviceid))
                {
                    return new ClassificationResult() { Content = "Select the Model First", ResultStatus = StatusEnum.Error };
                }

                PromptExecutionSettings executionSettings = new PromptExecutionSettings()
                {
                    ServiceId = serviceid
                };

                KernelArguments arguments = new KernelArguments(executionSettings);

                // Invoking the Prompt
                description = $"""<message role="user">**Description**: {description} </message>""";
                var result = await kernel.InvokePromptAsync(Prompt.BasePrompt + description, arguments);

                return new ClassificationResult() { Content = result.ToString(), ResultStatus = StatusEnum.Success, ModelId = Enum.GetName<ModelEnum>(selectedmodel) ?? "", Extras = result.Metadata };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return new ClassificationResult() { Content = "Error Occured while Classifying the Category", ResultStatus = StatusEnum.Error, ModelId = Enum.GetName<ModelEnum>(selectedmodel) ?? "" };
            }
        }
    }
}
