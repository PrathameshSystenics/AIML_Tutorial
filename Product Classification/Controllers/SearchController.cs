using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.VectorData;
using ProductClassification.Data;
using ProductClassification.Models;
using System.Net;

namespace ProductClassification.Controllers
{
    public class SearchController : Controller
    {
        private readonly ILogger<SearchController> _logger;
        private ProductDataRepository _productdatarepository;

        public SearchController(ILogger<SearchController> logger, ProductDataRepository productdatarepo)
        {
            _logger = logger;
            _productdatarepository = productdatarepo;
        }


        [HttpGet]
        public async Task<IActionResult> SearchProducts(string searchtext, int noofproductstosearch = 15)
        {
            List<Product> products = new List<Product>();
            try
            {
                if (String.IsNullOrWhiteSpace(searchtext))
                {
                    return RedirectToAction("Index", "Home");
                }

                VectorSearchResults<Product> productsearchresults = await _productdatarepository.SearchProductsByDescription(searchtext, noofproductstosearch);

                await foreach (var product in productsearchresults.Results)
                {
                    products.Add(product.Record);
                }
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<ObjectResult> GetProductDetails(Guid id)
        {
            try
            {
                Product product = await _productdatarepository.GetProductById(id);
                if (product == null)
                {
                    return NotFound(new
                    {
                        message = "No Product Found"
                    });
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new
                {
                    message = "Error Occured While Fetching the Product Details"
                });
            }
        }
    }
}
