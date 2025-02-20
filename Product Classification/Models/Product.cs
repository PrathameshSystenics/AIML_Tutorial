using Microsoft.Extensions.VectorData;

namespace ProductClassification.Models
{
    public class Product
    {
        [VectorStoreRecordKey]
        public Guid ID { get; set; }

        [VectorStoreRecordData]
        public string Title { get; set; }

        [VectorStoreRecordData]
        public string Description { get; set; }

        [VectorStoreRecordData]
        public string Category { get; set; }

        [VectorStoreRecordVector(768,DistanceFunction.EuclideanDistance)]
        public ReadOnlyMemory<float> DescriptionEmbedding { get; set; }

/*        [VectorStoreRecordVector(768, DistanceFunction.EuclideanDistance)]
        [Column(TypeName = "vector(768)")]
        public Vector TitleEmbedding { get; set; }*/

    }
}
