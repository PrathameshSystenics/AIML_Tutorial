namespace ProductClassification.Models
{
    public class EvaluationMetrics
    {
        public int Correct { get; set; }
        public int Total { get; set; }
        public double Accuracy { get => (((double)Correct / Total) * 100); }
        public int InCorrect { get => Total - Correct; }
    }
}
