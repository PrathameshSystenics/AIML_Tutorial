using System.ComponentModel.DataAnnotations.Schema;

namespace ProductClassification.Models
{
    public class EvaluationData
    {
        public int ID { get; set; }

        [Column(TypeName = "Text")]
        public string Description { get; set; } = String.Empty;
        public string Answer { get; set; } = String.Empty;
        public string Reason { get; set; } = String.Empty;
        public ICollection<EvaluatedResult> EvaluatedResults { get; set; } = [];
    }
}
