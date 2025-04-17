using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using ProcessFrameworkPOC.Models;
using ProcessFrameworkPOC.Utilities;
using System.Text.Json.Serialization;

namespace ProcessFrameworkPOC.Steps
{
    public class ReviewStep : KernelProcessStep
    {
        [KernelFunction]
        public async Task ReviewContent(Kernel _kernel, KernelProcessStepContext context, UserInputs inputs)
        {
            string yamlfilepath = Path.Combine(Directory.GetCurrentDirectory(), Path.Join("PromptTemplates", "ContentReviewer.yaml"));

            string yamltext = File.ReadAllText(yamlfilepath);

            PromptTemplateConfig promptTemplateConfig = KernelFunctionYaml.ToPromptTemplateConfig(yamltext);
            HandlebarsPromptTemplateFactory handleprompttemplatefactory = new HandlebarsPromptTemplateFactory();

/*            GeminiPromptExecutionSettings promptExecutionSettings = new GeminiPromptExecutionSettings()
            {
                MaxTokens = 9000
            };*/

            AzureOpenAIPromptExecutionSettings promptExecutionSettings = new AzureOpenAIPromptExecutionSettings()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            };

            KernelArguments kernelArguments = new KernelArguments(promptExecutionSettings)
            {
                {"inputs",inputs }
            };

            ChatCompletionAgent revieweragent = new ChatCompletionAgent(promptTemplateConfig, handleprompttemplatefactory)
            {
                Arguments = kernelArguments,
                Kernel = _kernel,

            };

            string output = "";
            var response = revieweragent.InvokeAsync("Review the Blog", options: new() { KernelArguments = kernelArguments });
            await foreach (var item in response)
            {
                PrettyPrint.Print("ReviewerStep", ConsoleColor.DarkGray, ConsoleColor.White);
                output = item.Message.Content!;
                Console.WriteLine(item.Message.Content);
            }

            if (output.Contains("ContentReviewed", StringComparison.OrdinalIgnoreCase))
            {
                inputs.ModificationsNeeded = string.Empty;
                await context.EmitEventAsync("ProofRead", inputs);
            }
            else if (output.Contains("Modification", StringComparison.OrdinalIgnoreCase))
            {
                inputs.ModificationsNeeded = output;
                await context.EmitEventAsync("ReWriteContent", inputs);
            }

        }
    }
}
