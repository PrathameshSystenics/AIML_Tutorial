using Dapr.Client;
using DaprTutorial.Hubs;
using DaprTutorial.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel;

namespace ProcessFramework.Steps
{
    // The Last Step
    public class PublishDocumentationStep : KernelProcessStep
    {
        private readonly DaprClient _darpclient;
        private IHubContext<StepDataHub, IStepMessage> _hubcontext;

        public PublishDocumentationStep(DaprClient darpclient, IHubContext<StepDataHub, IStepMessage> hubcontext)
        {
            _darpclient = darpclient;
            _hubcontext = hubcontext;
        }

        [KernelFunction]
        public UserInputs PublishDocumentation(UserInputs inputs)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"[{nameof(PublishDocumentationStep)}]:Publishing the Documentation");
            Console.ResetColor();

            Console.WriteLine(inputs.Content);

            this._hubcontext.Clients.Client(inputs.ConnectionID).SendMessage(inputs.Content).Wait();
            //this._darpclient.PublishEventAsync<string>("pubsub", "stepsdata", "Docs Published").Wait();

            return inputs;
        }
    }
}
