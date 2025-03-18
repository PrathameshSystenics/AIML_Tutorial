using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using ProductClassification.Models;
using ProductClassification.CSVReader;
using ProductClassification.Services;

namespace ProductClassification.Data
{
    public class ProductDataRepository
    {
        private EmbeddingGeneratorService _embeddingGeneratorService;
        private ILogger<ProductDataRepository> _logger;
        private IVectorStore _vectorstore;

        public ProductDataRepository(EmbeddingGeneratorService embeddingGeneratorService, ILogger<ProductDataRepository> logger, IVectorStore vectorstore)
        {
            _embeddingGeneratorService = embeddingGeneratorService;
            _logger = logger;
            _vectorstore = vectorstore;

        }

        public async Task UpsertProductEmbeddingAsync(ProductCsvModel product)
        {
            try
            {
                IVectorStoreRecordCollection<Guid, Product> collection = _vectorstore.GetCollection<Guid, Product>("Products");

                ReadOnlyMemory<float> vector = await _embeddingGeneratorService.GenerateEmbeddingAsync(product.Description);

                Guid insertedID = await collection.UpsertAsync(new Product()
                {
                    Category = product.Category,
                    Description = product.Description,
                    DescriptionEmbedding = vector,
                    Title = product.Title,
                    ID = Guid.NewGuid()
                });

                _logger.LogInformation("Embedding Generated for ID = {id}, Title = {description}", insertedID, product.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError("Message = {message}, Type => {type}", ex.Message, ex.GetType());
                throw;

            }
        }


        public async Task<List<Product>> SearchProductsByDescription(string searchtext, int noofProductsToSearch)
        {
            List<Product> products = new List<Product>();
            try
            {
                IVectorStoreRecordCollection<Guid, Product> collection = _vectorstore.GetCollection<Guid, Product>("Products");

                ReadOnlyMemory<float> embeddings = await _embeddingGeneratorService.GenerateEmbeddingAsync(searchtext);

                VectorSearchOptions searchoptions = new VectorSearchOptions()
                {
                    Top = noofProductsToSearch
                };

                var searchResults = await collection.VectorizedSearchAsync(embeddings, searchoptions);
                await foreach (var product in searchResults.Results)
                {
                    if (product.Score < 1)
                        products.Add(product.Record);
                }
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError("Message = {message}, Type => {type}", ex.Message, ex.GetType());
                throw;
            }
        }


        public async Task<Product?> GetProductById(Guid id)
        {
            try
            {
                IVectorStoreRecordCollection<Guid, Product> collection = _vectorstore.GetCollection<Guid, Product>("Products");
                return await collection.GetAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}