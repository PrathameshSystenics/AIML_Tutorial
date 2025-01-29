using HandDigitRecoginition;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SemanticKernelTut
{
    public class _06_ImagetoText_Sample(IConfiguration _configuration)
    {
        /// <summary>
        /// Checks if the Provided File Exists in the System or not 
        /// </summary>
        /// <param name="filepath">Path of the File For Checking if the File Exists</param>
        /// <returns><see langword="true"/> if the File Exists otherwise <see langword="false"/></returns>
        public bool IsFileExist(string filepath)
        {
            return Path.Exists(filepath);
        }

        /// <summary>
        /// Checks if the File have the ValidExtension from the passed <paramref name="allowedextension"/>.
        /// </summary>
        /// <param name="allowedextension">The Extension allowed for Checking the File.</param>
        /// <param name="filepath">FilePath for checking the Extension.</param>
        /// <returns><see langword="true"/> if the File having valid Extension other <see langword="false"/></returns>
        public bool IsFileHaveValidExtension(string allowedextension, string filepath)
        {
            string filenamewithExtension = Path.GetFileName(filepath);
            return filenamewithExtension.EndsWith(allowedextension);
        }


        public async Task RunModel()
        {
            Recognize recognize = new Recognize(_configuration);


            //await recognize.UseHuggingFace_MoonDreamModel();
            //await Chat.ChatWithModel(kernel);

            //Kernel kernel = recognize.UseOllama_LlavaModel();

            //Kernel kernel = recognize.UseHuggingFace_MetaLlamaModel();

            //Kernel kernel = recognize.UseHuggingFace_SalesForceBlipModel();

            //Kernel kernel = recognize.UseAzureOpenAI_GptMiniModel();

            Kernel kernel = recognize.UseOllama_MoonDreamModel();

            await ChatWithModel(kernel);

        }

        public async Task ChatWithModel(Kernel kernel)
        {
            while (true)
            {
                Console.WriteLine("============== Hand Written Digit Recognition ================");
                Console.WriteLine("Bot >>> Enter the Image Path");

                Console.Write("User >>> ");

                // File Path input
                string input = Console.ReadLine() ?? "";

                // Checks the input image path
                if (!String.IsNullOrWhiteSpace(input) && this.IsFileExist(input) && this.IsFileHaveValidExtension(".png", input))
                {
                    //await Chat.ChatWithModel(kernel, input);
                    await Chat.ChatWithModel_Prompting(kernel, input);

#pragma warning disable
                    //await Chat.TextGenerateModel(kernel.GetRequiredService<IImageToTextService>(), input);
                }
                else
                {
                    Console.WriteLine("Bot >>> Enter the Correct Image Path and the Image must have the Extension as .png");
                }
                Console.WriteLine();
            }
        }
    }
}
