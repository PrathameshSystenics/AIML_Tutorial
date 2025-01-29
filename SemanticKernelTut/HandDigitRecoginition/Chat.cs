using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.ImageToText;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using OpenAI.Chat;

namespace HandDigitRecoginition
{
    public class Chat
    {
        public static async Task ChatWithModel(Kernel kernel, string filepath)
        {
            // Getting the ChatCompletion Service from the Kernel
            IChatCompletionService chatcompletionservice = kernel.GetRequiredService<IChatCompletionService>();

            // Code For Testing the Text to image
            ChatHistory chathistory = new ChatHistory();

            // Read the Image in Bytes
            byte[] imagebytes = File.ReadAllBytes(filepath);

            // System Message Level Instructions
            chathistory.AddSystemMessage(Prompt.OpenAI_System_Prompt);

            chathistory.AddUserMessage([
                new TextContent($"Which digit is in the image"),

                 // Setting the Image Detail Level As high
                 new ImageContent("data:image/png;base64,"+Convert.ToBase64String(imagebytes)){ Metadata=new Dictionary<string, object?> { ["Detail"] = "high" }}
            ]);

            //PromptExecutionSettings execsettings = new OpenAIPromptExecutionSettings() { Temperature = 0 };

            //HuggingFacePromptExecutionSettings huggingfaceprompt=new HuggingFacePromptExecutionSettings() { }

#pragma warning disable
            PromptExecutionSettings execsettings = new PromptExecutionSettings() { ExtensionData = new Dictionary<string, object>() { { "Temperature", 0.4 } } };
            try
            {
                var r = await chatcompletionservice.GetChatMessageContentAsync(chathistory, execsettings);

                Console.WriteLine(r);

                // Accessing the Token usage from the Generated content
                ChatTokenUsage tokenusage = (ChatTokenUsage)r.Metadata["Usage"];
                Console.WriteLine($"Total Token usage -> {tokenusage.TotalTokenCount}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }


        public async static Task ChatWithModel_Prompting(Kernel kernel, string filepath)
        {
            // Read the Image in Bytes
            byte[] imagebytes = File.ReadAllBytes(filepath);

            // Assigning the Prompt
            string prompt = Prompt.Ollama_HandleBar_Prompt_WithVision;

            // Base64 Encoded Image
            string base64imagestring = "data:image/png;base64," + Convert.ToBase64String(imagebytes);

            //PromptExecutionSettings execsettings = new OpenAIPromptExecutionSettings() { Temperature = 0 };

            //HuggingFacePromptExecutionSettings huggingfaceprompt=new HuggingFacePromptExecutionSettings() { }

            PromptExecutionSettings execsettings = new PromptExecutionSettings()
            {
                ExtensionData = new Dictionary<string, object>() { { "temperature", 0f }, { "repeat_penalty", 0.2f } }
            };

            // Template Format Initialization
            IPromptTemplateFactory templatefactory = new HandlebarsPromptTemplateFactory();

            try
            {
                var r = await kernel.InvokePromptAsync(prompt, new(execsettings) { { "imageData", base64imagestring } }, "handlebars", templatefactory);

                Console.WriteLine(r);

                // Accessing the Token usage from the Generated content
                /*ChatTokenUsage tokenusage = (ChatTokenUsage)r.Metadata["Usage"];
                Console.WriteLine($"Total Token usage -> {tokenusage.TotalTokenCount}\n");*/
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


#pragma warning disable
        public static async Task TextGenerateModel(IImageToTextService imageToTextService, string filepath)
        {
            // Code For Testing the Text to image
            ChatHistory chathistory = new ChatHistory();


            byte[] imagebytes = File.ReadAllBytes(filepath);


            chathistory.AddSystemMessage("""You are the HandWritten Digit Recoginition Bot.<instructions> If the Provided Image doesnot contain number then return the Response as "No Number" detected in Image. If Number is present in the Image then Output as "number" only. Dont' Give Any Further instruction if the Image is not detected</instructions>""");


            //chathistory.AddUserMessage("hello how are you");


        }
    }
}
