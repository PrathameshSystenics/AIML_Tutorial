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

        public ProductPlugin()
        {
   /*         _productdatarepo = productdatarepo;
            _logger = logger;*/
        }

        [KernelFunction("getsimilarproducts"), Description("Retrieve a list of products similar to the given description.")]
        public async Task<List<Product>> GetSimilarProducts(string description)
        {
            try
            {
                _logger.LogInformation("Using the Product Plugin");
                return new List<Product>();
                //return await _productdatarepo.SearchProductsByDescription(description, 10);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new List<Product>();
            }
        }


    }
}
