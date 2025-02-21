using CsvHelper.Configuration.Attributes;

namespace ProductClassification.CSVReader
{
    public class ProductCsvModel
    {
        [Name("ImgId")]
        public string Id { get; set; }
        [Name("title")]
        public string Title { get; set; }
        [Name("description")]
        public string Description { get; set; }
        [Name("categories")]
        public string Category { get; set; }    
    }
}
