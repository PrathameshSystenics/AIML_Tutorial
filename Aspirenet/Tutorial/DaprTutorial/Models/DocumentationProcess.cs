using Microsoft.SemanticKernel;
using ProcessFramework.Steps;

namespace ProcessFramework
{
    // Defining the process flow
    public class DocumentationProcess
    {
        private readonly IConfiguration _config;

        public DocumentationProcess(IConfiguration config)
        {
            _config = config;
        }

        public KernelProcess BuildSimpleProcess()
        {
            // Creating the Process Builder
            ProcessBuilder processbuilder = new ProcessBuilder("processTut");

            // Adding the Steps into the Process Builder
            ProcessStepBuilder gatherinformationstep = processbuilder.AddStepFromType<GatherInformationStep>();
            ProcessStepBuilder generateDocumentationStep = processbuilder.AddStepFromType<GenerateDocumentationStep>();
            ProcessStepBuilder statefulstep = processbuilder.AddStepFromType<StatefulStep>();
            ProcessStepBuilder publishdocumentationstep = processbuilder.AddStepFromType<PublishDocumentationStep>();

            // Defining the process Steps flow or orchestrating the steps
            processbuilder.OnInputEvent("start").SendEventTo(new(gatherinformationstep, parameterName: "inputs"));

            gatherinformationstep.OnFunctionResult().SendEventTo(new(generateDocumentationStep));

            generateDocumentationStep.OnEvent("DocumentationGenerated").SendEventTo(new(statefulstep, functionName: "LoggingData"));

            statefulstep.OnEvent("GenerateMore").SendEventTo(new(generateDocumentationStep));

            statefulstep.OnEvent("PublishDocs").SendEventTo(new(publishdocumentationstep));

            publishdocumentationstep.OnFunctionResult().StopProcess();

            // Building the process
            return processbuilder.Build();
        }
    }
}
