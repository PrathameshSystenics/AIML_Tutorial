using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using ProcessFrameworkPOC.Models;
using ProcessFrameworkPOC.Utilities;

namespace ProcessFrameworkPOC.Steps
{
    public class OutlineSectionStep : KernelProcessStep
    {
        [KernelFunction]
        public async Task GenerateOutlineAndSections(Kernel _kernel, KernelProcessStepContext context, UserInputs inputs)
        {
            string yamlfilepath = Path.Combine(Directory.GetCurrentDirectory(), Path.Join("PromptTemplates", "GenerateOutlineSections.yaml"));

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

            ChatCompletionAgent generateoutlineagent = new ChatCompletionAgent(promptTemplateConfig, handleprompttemplatefactory)
            {
                Arguments = kernelArguments,
                Kernel = _kernel,
            };

            string output = "";
            var response = generateoutlineagent.InvokeAsync("Generate Outline and Sections", options: new() { KernelArguments = kernelArguments });
            await foreach (var item in response)
            {
                PrettyPrint.Print("OUtlineSectionStep", ConsoleColor.Red, ConsoleColor.White);
                output = item.Message.Content!;
                Console.WriteLine(item.Message.Content);
            }
            inputs.OutlineAndSections = output;

            await context.EmitEventAsync("OutlineGenerated", inputs);

        }
    }
}
