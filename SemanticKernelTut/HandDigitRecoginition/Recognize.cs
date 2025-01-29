using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Configuration;

namespace HandDigitRecoginition
{
    public class Recognize
    {
        private IKernelBuilder _kernelbuilder;

        private IConfiguration _configuration;

        public Recognize(IConfiguration config)
        {
            // creating the kernel
            this._kernelbuilder = Kernel.CreateBuilder();

            // Storing all the configuration data
            _configuration = config;
        }

        /// <summary>
        /// Uses the Ollama LLava Model 
        /// </summary>
        /// <returns><see cref="Kernel"/> </returns>
        public Kernel UseOllama_LlavaModel()
        {
#pragma warning disable 
            this._kernelbuilder.AddOllamaChatCompletion(modelId: "llava:latest", endpoint: new Uri("http://localhost:11434"));

            return _kernelbuilder.Build();

            /* // ChatCompletion Service from the kernel
             IChatCompletionService chatcompletionservice = kernel.GetRequiredService<IChatCompletionService>();

             #region Testing the Model for Text to Image
             // Code For Testing the Text to image
             ChatHistory chathistory = new ChatHistory();

             string path = "C:\\Users\\PrathameshDhande\\Downloads\\archive\\mnist_png\\training\\7\\9716.png";
             byte[] imagebytes = File.ReadAllBytes(path);

             chathistory.AddUserMessage([
                 new TextContent("What number is shown in the image ?"),
                 new ImageContent(imagebytes,"image/png"),
                 ]);



             Console.WriteLine(await chatcompletionservice.GetChatMessageContentAsync(chathistory));
             return kernel;*/
            //#endregion
        }

        // Not Working -- Due the Inference API for these Model is not Working
        public async Task<Kernel> UseHuggingFace_MoonDreamModel()
        {
            this._kernelbuilder.AddHuggingFaceChatCompletion(model: "liuhaotian/llava-v1.5-7b", apiKey: _configuration["HuggingFace:ApiKey"]);

            Kernel kernel = _kernelbuilder.Build();

            // ChatCompletion Service from the kernel
            IChatCompletionService chatcompletionservice = kernel.GetRequiredService<IChatCompletionService>();

            #region Testing the Model for Text to Image
            // Code For Testing the Text to image
            ChatHistory chathistory = new ChatHistory();

            string path = "C:\\Users\\PrathameshDhande\\Downloads\\archive\\mnist_png\\training\\7\\9716.png";
            byte[] imagebytes = File.ReadAllBytes(path);

            chathistory.AddUserMessage([
                new ImageContent(imagebytes,"image/png"),
                 new TextContent("What number is shown in the image ?")
                ]);

            Console.WriteLine(await chatcompletionservice.GetChatMessageContentAsync(chathistory));
            return kernel;
            #endregion

        }

        /// <summary>
        /// Uses the Hugging Face Meta - LLama-3.2-LLB Vision Instruct model
        /// </summary>
        /// <returns>Kernel</returns>
        public Kernel UseHuggingFace_MetaLlamaModel()
        {
            this._kernelbuilder.AddHuggingFaceChatCompletion(model: "meta-llama/Llama-3.2-11B-Vision-Instruct", apiKey: _configuration["HuggingFace:ApiKey"]);

            return _kernelbuilder.Build();
        }

        // Not Working - Image Description model Not suitable for work.
        public Kernel UseHuggingFace_SalesForceBlipModel()
        {
            this._kernelbuilder.AddHuggingFaceChatCompletion(model: "Salesforce/blip-image-captioning-base", apiKey: _configuration["HuggingFace:ApiKey"]);

            return _kernelbuilder.Build();
        }

        /// <summary>
        /// Uses the Azure OpenAI Gpt mini model 
        /// </summary>
        /// <returns>Builded Kernel</returns>
        public Kernel UseAzureOpenAI_GptMiniModel()
        {
            this._kernelbuilder.AddAzureOpenAIChatCompletion(
               deploymentName: _configuration["AzureOpenAI:ModelNames"].ToString(),
               endpoint: _configuration["AzureOpenAI:Endpoint"].ToString(),
               apiKey: _configuration["AzureOpenAI:ApiKey"].ToString(),
               modelId: _configuration["AzureOpenAI:ModelVersion"].ToString());

            return _kernelbuilder.Build();
        }

        /// <summary>
        /// Uses the Ollama MoonDream Model
        /// </summary>
        /// <returns><see cref="Kernel"/></returns>
        public Kernel UseOllama_MoonDreamModel()
        {
            this._kernelbuilder.AddOllamaChatCompletion(modelId: "moondream:latest", endpoint: new Uri("http://localhost:11434"));
            
            return _kernelbuilder.Build();
        }
    }
}
