using System.Text.Json.Serialization;

namespace ProductClassification.Models
{

    public class EvaluationListModel
    {
        [JsonPropertyName("Evaluate")]
        public EvaluationData[] Evaluate { get; set; } = [];
    }
}
