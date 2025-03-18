using Microsoft.Extensions.VectorData;
using System.Text.Json.Serialization;

namespace ProductClassification.Models
{
    public class Product
    {
        [VectorStoreRecordKey]
        [JsonPropertyName("id")]
        public Guid ID { get; set; }= Guid.NewGuid();

        [VectorStoreRecordData]
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [VectorStoreRecordData]
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [VectorStoreRecordData]
        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [VectorStoreRecordVector(768, DistanceFunction.EuclideanDistance)]
        [JsonPropertyName("descriptionembedding")]
        public ReadOnlyMemory<float> DescriptionEmbedding { get; set; }

        /*        [VectorStoreRecordVector(768, DistanceFunction.EuclideanDistance)]
                [Column(TypeName = "vector(768)")]
                public Vector TitleEmbedding { get; set; }*/

    }
}
