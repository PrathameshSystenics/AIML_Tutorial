using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using ProcessFrameworkPOC.Models;
using ProcessFrameworkPOC.Plugins;
using ProcessFrameworkPOC.Services;
using ProcessFrameworkPOC.Utilities;
using System.ComponentModel;

namespace ProcessFrameworkPOC.Steps
{
    public class GatherInformationStep : KernelProcessStep
    {
        [KernelFunction]
        public async Task<UserInputs> GetInformationRegardingQuery(UserInputs inputs, Kernel _kernel)
        {
            string yamlfilepath = Path.Combine(Directory.GetCurrentDirectory(), Path.Join("PromptTemplates", "GatherInformation.yaml"));

            string yamltext = File.ReadAllText(yamlfilepath);

            PromptTemplateConfig promptTemplateConfig = KernelFunctionYaml.ToPromptTemplateConfig(yamltext);
            HandlebarsPromptTemplateFactory handleprompttemplatefactory = new HandlebarsPromptTemplateFactory();

            AzureOpenAIPromptExecutionSettings promptExecutionSettings = new AzureOpenAIPromptExecutionSettings()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            };

/*            GeminiPromptExecutionSettings promptExecutionSettings = new GeminiPromptExecutionSettings()
            {
                ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                MaxTokens = 9000
            };*/

            KernelArguments kernelArguments = new KernelArguments(promptExecutionSettings)
            {
                {"inputs",inputs }
            };

            string prompttemplatestring = await handleprompttemplatefactory.Create(promptTemplateConfig).RenderAsync(kernel: _kernel, kernelArguments);

            Kernel kernelclone = _kernel.Clone();

            kernelclone.Plugins.AddFromType<GoogleSearchPlugin>();

            ChatCompletionAgent informationagent = new ChatCompletionAgent(promptTemplateConfig, handleprompttemplatefactory)
            {
                Arguments = kernelArguments,
                Kernel = kernelclone,
            };

            string output = "";

            var response = informationagent.InvokeAsync("Gather the Information");
            await foreach (var item in response)
            {
                PrettyPrint.Print("GatherInformationStep", ConsoleColor.Green, ConsoleColor.White);
                output = item.Message.Content!;
                Console.WriteLine(item.Message.Content);
            }
            inputs.References = output;
            return inputs;
        }
    }
}
