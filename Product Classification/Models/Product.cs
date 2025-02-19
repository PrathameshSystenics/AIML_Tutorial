using Pgvector;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductClassification.Models
{
    public class Product
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        [Column(TypeName = "vector(768)")]
        public Vector DescriptionEmbedding { get; set; }

        [Column(TypeName = "vector(768)")]
        public Vector TitleEmbedding { get; set; }

    }
}
