using CsvHelper;
using System.Globalization;

namespace ProductClassification.CSVReader
{
    public static class ProductCsvReader
    {
        public static IEnumerable<ProductCsvModel> ReadProducts(string csvfilepath)
        {
            try
            {
                List<ProductCsvModel> products = new List<ProductCsvModel>();
                StreamReader streamreader = new StreamReader(csvfilepath);
                using (CsvReader csvreader = new CsvReader(streamreader, CultureInfo.InvariantCulture))
                {
                    products = csvreader.GetRecords<ProductCsvModel>().ToList();
                }
                return products;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
