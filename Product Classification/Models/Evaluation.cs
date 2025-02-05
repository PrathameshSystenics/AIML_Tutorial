using System.Text.Json.Serialization;

namespace Classification.Models
{
    public class Evaluation
    {
        [JsonPropertyName("Description")]
        public string Description { get; set; } = String.Empty;

        [JsonPropertyName("Answer")]
        public string Answer { get; set; } = String.Empty;
    }

    public class EvaluationList
    {
        [JsonPropertyName("Evaluate")]
        public Evaluation[] Evaluate { get; set; }
    }
}
