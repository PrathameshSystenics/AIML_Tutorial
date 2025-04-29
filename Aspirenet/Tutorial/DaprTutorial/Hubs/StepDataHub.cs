using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;

namespace DaprTutorial.Hubs
{

    // Creating the Strongly Typed Hub
    public sealed class StepDataHub : Hub<IStepMessage>
    {
        // Called when the Client Connects with the hub
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendMessage("Welcome To the Step Data Hub");
            await base.OnConnectedAsync();
        }

        // Simple method for sending the data to the client
        public async Task SendStepData(string data)
        {
            await Clients.Caller.SendMessage(data);
        }

        // Gets the Connection ID
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        public async IAsyncEnumerable<int> Counter(
       int count,
       int delay,
       [EnumeratorCancellation]
        CancellationToken cancellationToken)
        {
            for (var i = 0; i < count; i++)
            {
                // Check the cancellation token regularly so that the server will stop
                // producing items if the client disconnects.
                cancellationToken.ThrowIfCancellationRequested();

                yield return i;

                // Use the cancellationToken in other APIs that accept cancellation
                // tokens so the cancellation can flow down to them.
                await Task.Delay(delay, cancellationToken);
            }
        }



    }
}
