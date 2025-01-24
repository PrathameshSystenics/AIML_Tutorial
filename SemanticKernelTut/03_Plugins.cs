using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace SemanticKernelTut
{
    public class _03_Plugins
    {
        public async void Perform()
        {
            #region Creating Kernel
            IKernelBuilder kernelbuilder = Kernel.CreateBuilder();
            kernelbuilder.AddAzureOpenAIChatCompletion("deplyname", "endpoint", "apikey", "serviceid", "modelId");

            // Add the Plugin to the Kernel
            kernelbuilder.Plugins.AddFromType<OrderPlugin>("Orders");

            Kernel kernel = kernelbuilder.Build();
            #endregion
        }
    }

    #region Define Plugin
    // Defining the Plugin
    class OrderPlugin
    {
        // annotate the function to be used a the function/plugin.
        [KernelFunction("get_current_order")]
        // Give the Proper Description for the AI model to understand about what does the function does.
        [Description("Gets the Current Order, Ordered by the User.")]
        public string GetCurrentOrder()
        {
            return "Pizza";
        }
    }
    #endregion
}
