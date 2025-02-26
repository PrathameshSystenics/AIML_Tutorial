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

        [KernelFunction("getsimilarproducts"), Description("Retrieve a list of products similar to the given description.")]
        public async Task<List<Product>> GetSimilarProducts(string description)
        {
            try
            {
                return await _productdatarepo.SearchProductsByDescription(description, 10);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new List<Product>();
            }
        }


    }
}
