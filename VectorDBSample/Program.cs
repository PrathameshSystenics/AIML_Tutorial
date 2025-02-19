using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Qdrant.Client;
using VectorDBSample;

IKernelBuilder kernelbuilder = Kernel.CreateBuilder();

// Adding the Ollama Text Embedding Generation
kernelbuilder.AddOllamaTextEmbeddingGeneration(
    modelId: "all-minilm:33m",
    endpoint: new Uri("http://localhost:11434/")
    );

// Adding the Qdrant Vector DB Support
kernelbuilder.AddQdrantVectorStore("localhost", 6334);

Kernel kernel = kernelbuilder.Build();

// Getting the Required Services
IVectorStore vectorstore = kernel.GetRequiredService<IVectorStore>();
ITextEmbeddingGenerationService embeddingservice = kernel.GetRequiredService<ITextEmbeddingGenerationService>();

// Creating the Collection
var productcollections = vectorstore.GetCollection<Guid, ProductVectorCollection>("products");

await productcollections.CreateCollectionIfNotExistsAsync();

// Reading the top 500 products list
ProductCSVReader reader = new ProductCSVReader();
IEnumerable<ProductCSVModel> products = reader.GetProducts().Skip(15992); // Process all rows

foreach (ProductCSVModel product in products)
{
    // Generating the Embedding of each description
    ReadOnlyMemory<float> descriptionembedding = await embeddingservice.GenerateEmbeddingAsync(product.Description);

    // Inserting the Embedding record into the Vector DB
    Guid inserted = await productcollections.UpsertAsync(new ProductVectorCollection()
    {
        ID = Guid.NewGuid(),
        Category = product.Category,
        Description = product.Description,
        DescriptionEmbedding = descriptionembedding,
        Title = product.Title
    });

    Console.WriteLine($"{inserted} Inserted");
}

// FIXME : HIT it again after the Rows 15992
