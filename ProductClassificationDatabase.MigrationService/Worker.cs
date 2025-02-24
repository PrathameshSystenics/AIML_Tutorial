using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using OpenTelemetry.Trace;
using ProductClassification.Data;
using System.Diagnostics;

namespace ProductClassificationDatabase.MigrationService
{
    public class Worker(IServiceProvider serviceProvider,
IHostApplicationLifetime hostApplicationLifetime, ILogger<Worker> logger) : BackgroundService
    {

        public const string ActivitySourceName = "Migrations";
        private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);
            try
            {
                logger.LogInformation("Executing the Migration Service");

                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                await RunMigrationAsync(dbContext, stoppingToken);
                await SeedData(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError("Error Occurred While Running Migration Service");
                logger.LogError(ex.Message);
                activity?.RecordException(ex);
                throw;
            }

            hostApplicationLifetime.StopApplication();
        }

        private static async Task RunMigrationAsync(ApplicationDBContext dbContext, CancellationToken cancellationToken)
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();
            await EnsureDatabaseAsync(dbContext, cancellationToken);

            await strategy.ExecuteAsync(async () =>
            {
                await dbContext.Database.MigrateAsync(cancellationToken);
            });

        }

        private static async Task SeedData(ApplicationDBContext dbContext)
        {
            await DBInitializer.SeedEvaluationData(dbContext);
        }

        /// <summary>
        /// Make sure if the database exists else create one 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task EnsureDatabaseAsync(ApplicationDBContext dbContext, CancellationToken cancellationToken)
        {
            var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                // Create the database if it does not exist.
                // Do this first so there is then a database to start a transaction against.
                if (!await dbCreator.ExistsAsync(cancellationToken))
                {
                    await dbCreator.CreateAsync(cancellationToken);
                }
            });
        }
    }
}

