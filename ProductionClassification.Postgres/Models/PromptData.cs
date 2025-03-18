using System.ComponentModel.DataAnnotations.Schema;

namespace ProductClassification.Models
{
    public class PromptData
    {
        public int ID { get; set; }

        public int EvaluationBatchID { get; set; }

        [Column(TypeName = "text")]
        public string SystemPrompt { get; set; }

        public EvaluationBatch EvaluationBatch { get; set; }
    }
}
