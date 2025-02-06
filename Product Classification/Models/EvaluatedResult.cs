using System.ComponentModel.DataAnnotations.Schema;

namespace ProductClassification.Models
{
    public class EvaluatedResult
    {
        public int ID { get; set; }
        public int EvaluationBatchID { get; set; }
        public int EvaluationDataID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Result { get; set; } = String.Empty;
        public bool IsCorrect { get; set; }

        public EvaluationBatch EvaluationBatch { get; set; }
        public EvaluationData EvaluationData { get; set; }

        [NotMapped]
        public EvaluationMetrics EvaluationMetrics { get; set; }
    }
}
