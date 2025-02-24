using Microsoft.SemanticKernel;
using ProductClassification.Data;

namespace ProductClassification.Extensions
{
    public static class VectorDbExtensions
    {

        public static IHostApplicationBuilder AddVectorDbSupport(this IHostApplicationBuilder builder,IConfiguration config)
        {
            builder.Services.AddPostgresVectorStore(config.GetConnectionString("promptevaldb"));
            builder.Services.AddScoped<ProductDataRepository>();
            return builder;
        }
    }
}
