using Microsoft.SemanticKernel;
using ProductClassification.Data;
using ProductClassification.Models;
using System.ComponentModel;

namespace ProductClassification.SemanticKernel.Plugins
{
    public class ProductPlugin
    {
        private ProductDataRepository _productdatarepo;
        private ILogger<ProductPlugin> _logger;

        public ProductPlugin(ProductDataRepository productdatarepo, ILogger<ProductPlugin> logger)
        {
            _productdatarepo = productdatarepo;
            _logger = logger;
        }

        [KernelFunction("get_similar_products_by_description"), Description("Retrieves a list of products that are similar to the provided description.")]
        public async Task<List<Product>> GetSimilarProducts(string description)
        {
            try
            {
                _logger.LogInformation("Using the Product Plugin");
                return await _productdatarepo.SearchProductsByDescription(description, 10);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new List<Product>();
            }
        }

        [KernelFunction("get_categories_for_classification")]
        [Description("Get the list of categories which are useful for classifying the product based on description")]
        public List<string> GetCategories()
        {
            List<string> categories = new List<string>
            {
                "Arts, Crafts & Sewing",
                "Cell Phones & Accessories",
                "Clothing, Shoes & Jewelry",
                "Tools & Home Improvement",
                "Health & Personal Care",
                "Baby Products",
                "Baby",
                "Patio, Lawn & Garden",
                "Beauty",
                "Sports & Outdoors",
                "Electronics",
                "All Electronics",
                "Automotive",
                "Toys & Games",
                "All Beauty",
                "Office Products",
                "Appliances",
                "Musical Instruments",
                "Industrial & Scientific",
                "Grocery & Gourmet Food",
                "Pet Supplies",
                "Unknown"
            };
            return categories;
        }


    }
}
