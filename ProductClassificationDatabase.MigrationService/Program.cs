using Microsoft.EntityFrameworkCore;
using ProductClassification.Postgres;
using ProductClassificationDatabase.MigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
                .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName))
                .WithLogging()
                .WithMetrics();

builder.Services.AddDbContextPool<ApplicationDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("promptevaldb"), sqlOptions =>
    {
        sqlOptions.MigrationsAssembly("ProductClassification.Postgres");
        sqlOptions.EnableRetryOnFailure();
    })
);

var host = builder.Build();
host.Run();
