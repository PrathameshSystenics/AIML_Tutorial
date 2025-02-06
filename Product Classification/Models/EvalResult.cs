namespace ProductClassification.Models
{
    public class EvalResult
    {
        public string Status { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string Expected { get; set; } = String.Empty;
        public string Result { get; set; } = String.Empty;
        public int Correct { get; set; }
        public int Total { get; set; }
        public double Accuracy { get; set; }

    }
}
