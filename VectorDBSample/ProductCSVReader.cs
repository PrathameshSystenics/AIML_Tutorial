using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorDBSample
{
    public class ProductCSVReader
    {
        public ProductCSVReader() { }

        public IEnumerable<ProductCSVModel> GetProducts()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "train.csv");
            IEnumerable<ProductCSVModel> products = new List<ProductCSVModel>();
            StreamReader streamreader = new StreamReader(path);
            using (CsvReader csvreader = new CsvReader(streamreader, CultureInfo.InvariantCulture))
            {
                products = csvreader.GetRecords<ProductCSVModel>().ToList();
            }
            return products;
        }
    }

    public class ProductCSVModel
    {
        [Name("ImgId")]
        public string ImgId { get; set; }
        [Name("title")]
        public string Title { get; set; }
        [Name("description")]
        public string Description { get; set; }
        [Name("categories")]
        public string Category { get; set; }
    }
}
