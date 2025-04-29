using Dapr.Client;
using DaprTutorial.Hubs;
using DaprTutorial.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel;

namespace ProcessFramework.Steps
{
    // Create the Step Class by inheriting the KernelProcessStep Class
    public class GatherInformationStep : KernelProcessStep
    {
        private DaprClient _daprclient;
        private IHubContext<StepDataHub,IStepMessage> _hubContext;

        public GatherInformationStep(DaprClient client,IHubContext<StepDataHub,IStepMessage> hubcontext)
        {
            _daprclient = client;
            _hubContext = hubcontext;
        }

        // process step to gather the product information.
        // Create the kernel Function
        [KernelFunction]
        public UserInputs GatherProductInformation(UserInputs inputs)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"[GatherInformationStep]: Gathering Product Information for {inputs.ProductName}");

            string docs = """
            Product Description:
            GlowBrew is a revolutionary AI driven coffee machine with industry leading number of LEDs and programmable light shows. The machine is also capable of brewing coffee and has a built in grinder.

        
            Product Features:
            1. **Luminous Brew Technology**: Customize your morning ambiance with programmable LED lights that sync with your brewing process.
            2. **AI Taste Assistant**: Learns your taste preferences over time and suggests new brew combinations to explore.
            3. **Gourmet Aroma Diffusion**: Built-in aroma diffusers enhance your coffee's scent profile, energizing your senses before the first sip.

            Troubleshooting:
            - **Issue**: LED Lights Malfunctioning
                - **Solution**: Reset the lighting settings via the app. Ensure the LED connections inside the GlowBrew are secure. Perform a factory reset if necessary.
            """;

            /*            Dictionary<string, string> metadata = new()
                        {
                            {"ttlInSeconds","60" },
                            {"productname",productname }
                        };

                        // Publishing the Events to the Dapr Pubsub
                        _daprclient.PublishEventAsync<string>("pubsub", "stepsdata", docs, metadata).Wait();*/

            // Sending the data through the SignalR Hub
            _hubContext.Clients.Client(inputs.ConnectionID).SendMessage(inputs.ProcessId).Wait();
            _hubContext.Clients.Client(inputs.ConnectionID).SendMessage(docs).Wait();
            _hubContext.Clients.Client(inputs.ConnectionID).SendMessage("<next>").Wait();
            inputs.PreviousProcessOutput = docs;

            Console.ResetColor();
            return inputs;
        }
    }
}
