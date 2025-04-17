using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using ProcessFrameworkPOC.Models;
using ProcessFrameworkPOC.Plugins;
using ProcessFrameworkPOC.Utilities;

namespace ProcessFrameworkPOC.Steps
{
    public class WriterStep : KernelProcessStep
    {
        [KernelFunction]
        public async Task WriteContent(Kernel _kernel, KernelProcessStepContext context, UserInputs inputs)
        {
            string yamlfilepath = Path.Combine(Directory.GetCurrentDirectory(), Path.Join("PromptTemplates", "ContentWriter.yaml"));

            string yamltext = File.ReadAllText(yamlfilepath);

            PromptTemplateConfig promptTemplateConfig = KernelFunctionYaml.ToPromptTemplateConfig(yamltext);
            HandlebarsPromptTemplateFactory handleprompttemplatefactory = new HandlebarsPromptTemplateFactory();

/*            GeminiPromptExecutionSettings promptexec = new GeminiPromptExecutionSettings()
            {
                MaxTokens = 9000,
                ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            };*/


            AzureOpenAIPromptExecutionSettings promptexec = new AzureOpenAIPromptExecutionSettings()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            };

            KernelArguments kernelArguments = new KernelArguments(promptexec)
            {
                {"inputs",inputs }
            };

            Kernel clonekernel = _kernel.Clone();
            clonekernel.Plugins.AddFromType<VisitSitePlugin>(); 

            ChatCompletionAgent generateoutlineagent = new ChatCompletionAgent(promptTemplateConfig, handleprompttemplatefactory)
            {
                Arguments = kernelArguments,
                Kernel = clonekernel,
            };

            string output = "";
            var response = generateoutlineagent.InvokeAsync("Generate complete blog without missing any information", options: new() { KernelArguments = kernelArguments });
            await foreach (var item in response)
            {
                PrettyPrint.Print("WriterStep", ConsoleColor.Yellow, ConsoleColor.Red);
                output = item.Message.Content!;
                Console.WriteLine(item.Message.Content);
            }
            inputs.Content = output;

            await context.EmitEventAsync("BlogGenerated", inputs);
        }
    }
}
