using Microsoft.SemanticKernel.ChatCompletion;

namespace ProductClassification.Models
{
    public class ChatRequest
    {
        public string ModelId { get; set; }

        public List<ChatMessage> Messages { get; set; }

        public ChatHistory ToChatHistory
        {
            get
            {
                ChatHistory chathistory = new ChatHistory();
                foreach (ChatMessage message in Messages)
                {
                    if (String.Equals(message.Role,"user",StringComparison.OrdinalIgnoreCase))
                    {
                        chathistory.AddUserMessage(message.Content);
                    }
                    else if (String.Equals(message.Role,"assistant",StringComparison.OrdinalIgnoreCase))
                    {
                        chathistory.AddAssistantMessage(message.Content);
                    }
                }
                return chathistory;
            }
        }

        public class ChatMessage
        {
            public string Role { get; set; }
            public string Content { get; set; }
        }
    }
}