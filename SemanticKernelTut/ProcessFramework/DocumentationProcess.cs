using Microsoft;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Process.Tools;
using ProcessFramework.Steps;

namespace ProcessFramework
{
    // Defining the process flow
    public class DocumentationProcess
    {
        private readonly IConfiguration _config;
        private KernelProcess _kernelprocess;

        public DocumentationProcess(IConfiguration config)
        {
            _config = config;
        }

        public void BuildSimpleProcess()
        {
            // Creating the Process Builder
            ProcessBuilder processbuilder = new ProcessBuilder("processTut");

            // Adding the Steps into the Process Builder
            ProcessStepBuilder gatherinformationstep = processbuilder.AddStepFromType<GatherInformationStep>();
            ProcessStepBuilder generateDocumentationStep = processbuilder.AddStepFromType<GenerateDocumentationStep>();
            ProcessStepBuilder statefulstep = processbuilder.AddStepFromType<StatefulStep>();
            ProcessStepBuilder publishdocumentationstep = processbuilder.AddStepFromType<PublishDocumentationStep>();

            // Defining the process Steps flow or orchestrating the steps
            processbuilder.OnInputEvent("start").SendEventTo(new(gatherinformationstep, parameterName: "productname"));

            gatherinformationstep.OnFunctionResult().SendEventTo(new(generateDocumentationStep));

            generateDocumentationStep.OnEvent("DocumentationGenerated").SendEventTo(new(statefulstep,functionName: "LoggingData"));

            statefulstep.OnEvent("PublishDocument").SendEventTo(new(publishdocumentationstep));

            // Building the process
            _kernelprocess = processbuilder.Build();
        }

        public async Task RunBuildProcessAsync()
        {
            if (_kernelprocess == null)
            {
                throw new NullReferenceException("Process is not built. Please build the process first.");
            }
            Kernel kernel = Kernel.CreateBuilder().AddGoogleAIGeminiChatCompletion(
                    apiKey: _config["GeminiModel:ApiKey"]!,
                    modelId: _config["GeminiModel:ModelName"]!
                ).Build();

            // Running the Process
            await _kernelprocess.StartAsync(kernel, new KernelProcessEvent() { Id = "start", Data = "GPT Model" });
        }

        public void GetProcessMermaidDiagram()
        {
            // Generating the Mermaid Diagram for the Kernel process
            string mermaiddiagram = _kernelprocess.ToMermaid();
            Console.WriteLine(mermaiddiagram);
        }
    }
}
