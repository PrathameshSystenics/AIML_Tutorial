namespace DaprTutorial.Models
{
    public class UserInputs
    {
        public string ProcessId { get; set; } = string.Empty;
        public string PreviousProcessOutput { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public bool IsStreaming { get; set; } = false;
        public string StreamingContent { get; set; } = string.Empty;
        public string ConnectionID { get; set; } = string.Empty;
    }
}
