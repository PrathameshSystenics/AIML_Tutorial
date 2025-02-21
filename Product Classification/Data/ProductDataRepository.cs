using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using ProductClassification.CSVReader;
using ProductClassification.Models;
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
            catch (HttpOperationException ex)
            {
                _logger.LogError("Message = {message}, Type => {type}", ex.Message, ex.GetType());
                if (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    _logger.LogWarning("Rate limit exceeded. Waiting 1 minute before retrying...");
                    await Task.Delay(1000);
                    await UpsertProductEmbeddingAsync(product);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Message = {message}, Type => {type}", ex.Message, ex.GetType());
                throw;

            }
        }


        public async Task<VectorSearchResults<Product>> SearchProductsByDescription(string description, int noofProductsToSearch)
        {
            VectorSearchResults<Product> searchResults;
            try
            {
                IVectorStoreRecordCollection<Guid, Product> collection = _vectorstore.GetCollection<Guid, Product>("Products");

                ReadOnlyMemory<float> embeddings = await _embeddingGeneratorService.GenerateEmbeddingAsync(description);

                VectorSearchOptions searchoptions = new VectorSearchOptions()
                {
                    Top = noofProductsToSearch
                };

                searchResults = await collection.VectorizedSearchAsync(embeddings, searchoptions);
                return searchResults;
            }
            catch (Exception ex)
            {
                _logger.LogError("Message = {message}, Type => {type}", ex.Message, ex.GetType());
                throw;
            }

        }
    }
}