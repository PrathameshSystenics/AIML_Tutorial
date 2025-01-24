using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using System.ComponentModel;

namespace SemanticKernelTut
{
    public class _04_Ollama_Plugin
    {
        private Kernel _kernel;
        public _04_Ollama_Plugin()
        {
            IKernelBuilder kernelbuilder = Kernel.CreateBuilder();
#pragma warning disable SKEXP0070
            kernelbuilder.AddOllamaChatCompletion(modelId: "llama3.2:1b", endpoint: new Uri("http://localhost:11434"));

            // Registering the Plugin
            kernelbuilder.Plugins.AddFromType<MathPlugin>("maths");

            _kernel = kernelbuilder.Build();
        }

        public async Task RunModel()
        {
            try
            {

                OllamaPromptExecutionSettings promptExec = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };

                IChatCompletionService chatservice = _kernel.GetRequiredService<IChatCompletionService>();

                string input = Console.ReadLine().ToString();

                ChatHistory history = new ChatHistory();
                history.AddUserMessage(input);

                // getting the response from the bot. 
                var results = await chatservice.GetChatMessageContentsAsync(history, promptExec, _kernel);

                Console.Write("Bot >>> ");

                foreach (var res in results)
                {
                    Console.Write(res);
                }

                // checking if the model has called the function
                //IEnumerable<FunctionCallContent> functions = FunctionCallContent.GetFunctionCalls(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

    // Create the Plugin
    public class MathPlugin
    {
        [KernelFunction("add_two_number")]
        [Description("Adds the two number provided a and b and returns their sum")]
        public int AddTwoNumber([Description("Number one to Add")] int a, [Description("Number two to add")] int b)
        {
            Console.WriteLine("Adding the Two Number from the Plugin");
            return (a + b) * 2;
        }

        [KernelFunction("log_of_number")]
        [Description("Gets the Log of the Provided Number.")]
        public double LogOfNumber([Description("Number for which want the log")] int number)
        {
            return Math.Log(number);
        }

        [KernelFunction("get_your_name")]
        [Description("Gets the current name of the Model or bot")]
        public string GetYourName()
        {
            Console.WriteLine("Execting the Name method");
            return "I m llama bot integrated by prathamesh dhande";
        }
    }
}
