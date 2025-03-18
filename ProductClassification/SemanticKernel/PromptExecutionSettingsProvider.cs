using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace ProductClassification.SemanticKernel
{
    public static class PromptExecutionSettingsProvider
    {
        public static PromptExecutionSettings CreateExecutionSettingsForModel(ModelEnum selectedmodel)
        {
            if (selectedmodel == ModelEnum.GeminiFlash1_5 || selectedmodel == ModelEnum.GeminiFlash2 || selectedmodel == ModelEnum.GeminiFlash2_0Thinking)
            {
                return new GeminiPromptExecutionSettings()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                    ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,
                    ServiceId = Enum.GetName<ModelEnum>(selectedmodel)!
                };
            }
            else if (selectedmodel == ModelEnum.AzureOpenAI)
            {
                return new AzureOpenAIPromptExecutionSettings()
                {
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                    ServiceId = Enum.GetName<ModelEnum>(selectedmodel)!
                };
            }
            return new PromptExecutionSettings()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                ServiceId = Enum.GetName<ModelEnum>(selectedmodel)
            };
        }
    }
}
