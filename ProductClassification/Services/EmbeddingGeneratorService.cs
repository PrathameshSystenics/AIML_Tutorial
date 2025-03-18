using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using ProductClassification.SemanticKernel;

namespace ProductClassification.Services
{
    public class EmbeddingGeneratorService
    {
        private readonly AIConnectorService _connectorService;
        private readonly ILogger<EmbeddingGeneratorService> _logger;
        private Kernel _kernel;

        public EmbeddingGeneratorService(AIConnectorService connectorService, ILogger<EmbeddingGeneratorService> logger)
        {
            _connectorService = connectorService;
            _logger = logger;
            _kernel = _connectorService.BuildModels();
        }


        public async Task<ReadOnlyMemory<float>> GenerateEmbeddingAsync(string text)
        {
            try
            {
                string serviceid = Enum.GetName<ModelEnum>(ModelEnum.GoogleTextEmbedding_004)!;
                ITextEmbeddingGenerationService kernelembeddinggeneratorService = _kernel.GetRequiredService<ITextEmbeddingGenerationService>(serviceid);
                return await kernelembeddinggeneratorService.GenerateEmbeddingAsync(text);
            }
            catch (Exception ex)
            {
                _logger.LogError("Message = {message}, Type => {type}", ex.Message, ex.GetType());
                throw;
            }
        }

    }
}
