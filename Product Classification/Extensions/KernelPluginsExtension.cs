using ProductClassification.SemanticKernel.Plugins;

namespace ProductClassification.Extensions
{
    public static class KernelPluginsExtension
    {
        public static void AddKernelPlugins(this IHostApplicationBuilder builder)
        {
            builder.Services.AddScoped<ProductPlugin>();
        }
    }
}
