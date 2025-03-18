namespace ProductClassification.Models
{
    public enum StatusEnum
    {
        Success,
        Error,
        Warning,
        Correct,
        Wrong,
        Complete
    }

    public class ClassificationResult
    {
        public string ModelId { get; set; } = "";
        public string Content { get; set; } = "";
        public StatusEnum ResultStatus { get; set; }
        public string Status { get => Enum.GetName<StatusEnum>(ResultStatus) ?? ""; }
        public object? Extras { get; set; }
    }
}
