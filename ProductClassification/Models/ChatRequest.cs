using Microsoft.SemanticKernel.ChatCompletion;

namespace ProductClassification.Models
{
    public class ChatRequest
    {
        public string ModelId { get; set; } = string.Empty;

        public List<ChatMessage> Messages { get; set; }= new List<ChatMessage>();

        public ChatHistory ToChatHistory
        {
            get
            {
                ChatHistory chathistory = new ChatHistory();
                if (Messages == null)
                    return chathistory;
                foreach (ChatMessage message in Messages)
                {
                    if (String.Equals(message.Role, "user", StringComparison.OrdinalIgnoreCase))
                    {
                        chathistory.AddUserMessage(message.Content);
                    }
                    else if (String.Equals(message.Role, "assistant", StringComparison.OrdinalIgnoreCase))
                    {
                        chathistory.AddAssistantMessage(message.Content);
                    }
                }
                return chathistory;
            }
        }

        public class ChatMessage
        {
            public string Role { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
        }
    }
}