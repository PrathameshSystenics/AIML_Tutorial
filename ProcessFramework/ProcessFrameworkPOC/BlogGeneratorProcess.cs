using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using ProcessFrameworkPOC.Models;
using ProcessFrameworkPOC.Steps;

namespace ProcessFrameworkPOC
{
    public class BlogGeneratorProcess
    {
        private readonly IConfiguration _config;

        public BlogGeneratorProcess(IConfiguration config)
        {
            _config = config;
        }

        private KernelProcess CreateProcess()
        {
            ProcessBuilder processbuilder = new ProcessBuilder("blog writer assistant");

            var gatherinfostep = processbuilder.AddStepFromType<GatherInformationStep>();
            var outlinesectionstep = processbuilder.AddStepFromType<OutlineSectionStep>();
            var contentwriterstep = processbuilder.AddStepFromType<WriterStep>();
            var reviewstep = processbuilder.AddStepFromType<ReviewStep>();
            var proofreadstep = processbuilder.AddStepFromType<ProofReadStep>();

            processbuilder.OnInputEvent("start").SendEventTo(new(gatherinfostep, parameterName: "inputs"));

            gatherinfostep.OnFunctionResult().SendEventTo(new(outlinesectionstep));

            outlinesectionstep.OnEvent("OutlineGenerated").SendEventTo(new(contentwriterstep));

            contentwriterstep.OnEvent("BlogGenerated").SendEventTo(new(reviewstep));

            reviewstep.OnEvent("ReWriteContent").SendEventTo(new(contentwriterstep));

            reviewstep.OnEvent("ProofRead").SendEventTo(new(proofreadstep));

            proofreadstep.OnEvent("ImprovementByUser").SendEventTo(new(contentwriterstep));

            return processbuilder.Build();
        }


        public async Task RunProcessAsync(UserInputs inputs)
        {
/*            Kernel kernel = Kernel.CreateBuilder().AddGoogleAIGeminiChatCompletion(
                  apiKey: _config["GeminiModel:ApiKey"]!,
                  modelId: _config["GeminiModel:ModelName"]!
              ).Build();*/

            IKernelBuilder kernelbuilder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(
                deploymentName: _config["AzureOpenAI:ModelName"]!.ToString(),
                endpoint: _config["AzureOpenAI:Url"]!.ToString(),
                apiKey: _config["AzureOpenAI:ApiKey"]!.ToString(),
                modelId: _config["AzureOpenAI:ModelVersion"]!.ToString()
              );

            Kernel kernel = kernelbuilder.Build();
            ILogger logger = kernel.LoggerFactory.CreateLogger("logger");




            KernelProcess process = CreateProcess();
            await process.StartAsync(kernel, new() { Id = "start", Data = inputs });
        }
    }
}
