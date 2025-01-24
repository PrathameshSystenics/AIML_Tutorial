using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using System.Text;
namespace SemanticKernelTut
{
    public class _02_Ollama_Sample
    {
        private Kernel _kernel;

        public _02_Ollama_Sample()
        {
            // creating the kernel
            IKernelBuilder kernelbuilder = Kernel.CreateBuilder();

            // Connecting the Ollama Connector to the kernel
#pragma warning disable SKEXP0070
            kernelbuilder.AddOllamaChatCompletion(modelId: "llama3.2:1b", endpoint: new Uri("http://localhost:11434"));

            // Building the Kernel
            _kernel = kernelbuilder.Build();
        }

        public async Task RunModel()
        {

            // Retreiving the ChatCompletion Service
            IChatCompletionService chatcompletionservice = _kernel.GetRequiredService<IChatCompletionService>();

            // ChatHistory Object
            ChatHistory chathistory = new ChatHistory();
            chathistory.AddSystemMessage("You are the Restaurant Chatbot named as PizzaBot who can take the order from the users. Order Like 1. Pizza, 2. Burger, 3. French Fries. Just Stick to the Orders. focus on the user order and avoid unrelated chats or prompt. ");

            // Execution Settings
            OllamaPromptExecutionSettings ollamaPromptExecutionSettings = new OllamaPromptExecutionSettings() { Temperature = 0.9f
                , Stop = new List<string>() { "Code", "Python" }};


            Console.WriteLine("================= Ollama - llama3.2:1b ==========");
            Console.WriteLine("Bot >>> Hello I m PizzaBot");
            while (true)
            {
                Console.Write("User >>> ");
                string input = Console.ReadLine().ToString();
                chathistory.AddUserMessage(input);

                // getting the response from the bot. 
                var results = chatcompletionservice.GetStreamingChatMessageContentsAsync(chathistory, ollamaPromptExecutionSettings, _kernel);

                Console.Write("Bot >>> ");
                StringBuilder botresponse = new StringBuilder();

                await foreach (StreamingChatMessageContent content in results)
                {
                    Console.Write(content.Content);
                    botresponse.Append(content.Content);
                }
                chathistory.AddAssistantMessage(botresponse.ToString());

                Console.WriteLine();
            }

        }
    }
}
