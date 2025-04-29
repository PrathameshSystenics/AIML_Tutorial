using Microsoft.AspNetCore.SignalR;

namespace SignalRTut.SignalR
{
    // Creating the Hub Class by Inheriting the Base class Hub
    public class ChatHub : Hub
    {
        // Creating the Method to send the message to all the clients
        public async Task SendMessage(string user, string message)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", user, message);
        }
    }
}
