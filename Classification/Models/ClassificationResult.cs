namespace Classification.Models
{
    public enum Status
    {
        Success,
        Error,
        Warning
    }

    public class ClassificationResult
    {
        public string ModelId { get; set; } = "";

        public string Content { get; set; } = "";

        public Status ResultStatus { get; set; }

        public string Status { get => Enum.GetName<Status>(ResultStatus) ?? ""; }

        public object? Extras { get; set; }
    }
}
