using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
namespace SemanticKernelTut
{
    public class _07_AgentFramework
    {
        private Kernel kernel;
        private readonly IConfiguration configuration;
        public _07_AgentFramework(IConfiguration config)
        {
            configuration = config;

            IKernelBuilder kernelbuilder = Kernel.CreateBuilder();
            kernelbuilder.AddGoogleAIGeminiChatCompletion(
                modelId: config["GeminiModel:ModelName"]!,
                apiKey: config["GeminiModel:ApiKey"]!
            );

            kernel = kernelbuilder.Build();
        }

        public async Task RunGeminiAgent()
        {
            // Creating the ChatCompletion Agent
            ChatCompletionAgent agent = new ChatCompletionAgent()
            {
                Name = "JokeBot",
                Instructions = "You are the joke Provider Agent. Provide the joke based on user input or type.",
                Kernel = kernel
            };

            ChatHistory history = [];
            Console.WriteLine("JOKE AGENT");

            // Conversing With the Agent.
            while (true)
            {
                Console.Write("You >> ");
                string userinput = Console.ReadLine()!;

                history.AddUserMessage(userinput);

                Console.Write("\nAgent >> ");
                
                await foreach (var content in agent.InvokeAsync(history))
                {
                    Console.Write(content);
                    
                }
            }
        }
    }
}
