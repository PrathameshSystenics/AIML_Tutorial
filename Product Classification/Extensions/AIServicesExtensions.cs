using ProductClassification.Services;


namespace ProductClassification.Extensions
{
    public static class AIServicesExtensions
    {
        public static IHostApplicationBuilder AddServicesRequiredForAI(this IHostApplicationBuilder builder)
        {
            builder.AddServicesRequiredForEmbedding();
            builder.Services.AddScoped<EvaluationService>();
            builder.Services.AddScoped<ClassificationService>();
            
            return builder;
        }

        public static IHostApplicationBuilder AddServicesRequiredForEmbedding(this IHostApplicationBuilder builder) {
            builder.Services.AddScoped<AIConnectorService>();
            builder.Services.AddScoped<EmbeddingGeneratorService>();
            return builder;
        }
    }
}
