using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SemanticKernelTut
{
    public class _05_AzureOpenAI_Connector
    {
        private Kernel _kernel;

        public _05_AzureOpenAI_Connector(IConfiguration config)
        {
            // Add the required services and connectors to the Kernel
            IKernelBuilder builder = Kernel.CreateBuilder();

            builder.AddAzureOpenAIChatCompletion(
                deploymentName: config["AzureOpenAI:ModelNames"].ToString(),
                endpoint: config["AzureOpenAI:Endpoint"].ToString(),
                apiKey: config["AzureOpenAI:ApiKey"].ToString(),
                modelId: config["AzureOpenAI:ModelVersion"].ToString());

            _kernel = builder.Build();
        }

        public async Task RunModel()
        {
            // creating the Prompt
            string promptTemplate = """
                <message role="system"> Give the Answer to the Above Query in 2-3 tokens only. Do not Exceed the Token usage. Your name is HelpBot</message>
                <message role="user"> {{$input}}
                </message>
                
                """;

            // Reading the user input
            Console.WriteLine("Enter the Prompt > ");
            string input = Console.ReadLine() ?? "";

            // Prompt Execution Settings for the OpenAI
            OpenAIPromptExecutionSettings promptexec = new OpenAIPromptExecutionSettings()
            {
                Logprobs = true, // log the probability of the tokens.
                MaxTokens = 20, // Max completion tokens
                Temperature = 0.2,// ranges from 0 to 2
                FrequencyPenalty = 1,
                ChatSystemPrompt = "You are a basic ChatBot. Who Gives Answer to the User Queries. give the answers in Short and avoid Long descriptions",
                TopLogprobs = 2, // To return the Probability of the predicted token along with number of token provided. 
                PresencePenalty = 1,// ranges from -100 to 100.
                //TokenSelectionBiases = new Dictionary<int, int>() { { 1011, 50 } },// also known as logit_bias
                //ResponseFormat = "json",
                //TopP=0.3 //Not to be used both temperature and topp at a same time
                
            };

            // Passing the Kernel Arguments into the Prompt
            KernelArguments arg = new KernelArguments(promptexec) { { "input", input } };

            // Get the Streaming Results
            var results = _kernel.InvokePromptStreamingAsync(promptTemplate, arg);

            // Streaming the content
            await foreach (var content in results)
            {
                Console.Write(content);
            }

        }
    }
}
