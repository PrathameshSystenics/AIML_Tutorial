using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.VectorData;
using OpenAI.VectorStores;
using OpenTelemetry.Resources;
using ProductClassification.CSVReader;
using ProductClassification.Data;
using ProductClassification.Models;
using System.Diagnostics;

namespace VectorStore.SeedingService
{
    public class VectorDataSeeder : BackgroundService
    {
        private readonly ILogger<VectorDataSeeder> _logger;
        private readonly IServiceProvider _serviceprovider;
        private readonly IHostApplicationLifetime _hostapplicationlifetime;

        public VectorDataSeeder(ILogger<VectorDataSeeder> logger, IServiceProvider serviceProvider,
IHostApplicationLifetime hostApplicationLifetime, IConfiguration configuration)
        {
            _logger = logger;
            _serviceprovider = serviceProvider;
            _hostapplicationlifetime = hostApplicationLifetime;
        }

        public const string ActivitySourceName = "Migrations";
        private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var activity = s_activitySource.StartActivity("Seeding Collections", ActivityKind.Client);

            try
            {
                await CreateCollectionIfNotExists();
                await SeedVectorData(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            _hostapplicationlifetime.StopApplication();
        }

        private async Task CreateCollectionIfNotExists()
        {
            IVectorStore vectorstore = _serviceprovider.GetRequiredService<IVectorStore>();
            IVectorStoreRecordCollection<int, Product> collection = vectorstore.GetCollection<int, Product>("Products");

            await collection.CreateCollectionIfNotExistsAsync();
        }

        private async Task SeedVectorData(CancellationToken stoppingToken)
        {
            using (var Scope = _serviceprovider.CreateScope())
            {
                ProductDataRepository productdatarepo = Scope.ServiceProvider.GetRequiredService<ProductDataRepository>();
                string csvfilepath = Path.Combine(AppContext.BaseDirectory, Path.Join("Samples", "train.csv"));

                IEnumerable<ProductCsvModel> productscsv = ProductCsvReader.ReadProducts(csvfilepath).Take(500);

                await Parallel.ForEachAsync(productscsv, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, async (source, stoppingToken) =>
                {
                    try
                    {
                        await productdatarepo.GenerateAndStoreEmbedding(source);
                    }
                    catch (Exception ex)
                    {

                        _logger.LogError(ex.Message);
                    }
                });
            }
        }
    }
}
