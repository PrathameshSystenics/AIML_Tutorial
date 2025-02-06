namespace ProductClassification.Models
{
    public class EvaluationBatch
    {
        public int ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ModelName { get; set; } = String.Empty;

        public ICollection<EvaluatedResult> EvaluatedResults { get; set; } = [];
    }
}
