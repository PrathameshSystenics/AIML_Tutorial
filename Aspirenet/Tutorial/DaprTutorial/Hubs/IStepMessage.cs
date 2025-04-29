using System.Runtime.CompilerServices;

namespace DaprTutorial.Hubs
{
    public interface IStepMessage
    {
        Task SendMessage(string message);
       
    }
}
