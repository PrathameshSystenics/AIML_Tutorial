namespace ProductClassification.Models
{
    public class EvaluationMetrics
    {
        public int Correct { get; set; }
        public int Total { get; set; }
        public decimal Accuracy { get => Math.Round((((decimal)Correct / Total) * 100), 2); }
        public int InCorrect { get => Total - Correct; }
    }
}
