using Microsoft.Extensions.VectorData;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorDBSample
{
    public class ProductVectorCollection
    {
        [VectorStoreRecordKey]
        public Guid ID { get; set; }

        [VectorStoreRecordData]
        public string Title { get; set; }

        [VectorStoreRecordData(IsFullTextSearchable = true)]
        public string Description { get; set; }

        [VectorStoreRecordVector(384,DistanceFunction.CosineSimilarity,IndexKind.Hnsw)]
        public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }

        [VectorStoreRecordData]
        public string Category { get; set; }
    }
}
