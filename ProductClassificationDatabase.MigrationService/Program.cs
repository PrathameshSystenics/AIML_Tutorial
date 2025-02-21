using ProductClassification.Data;
using ProductClassificationDatabase.MigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
                .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName))
                .WithLogging()
                .WithMetrics();

builder.AddNpgsqlDbContext<ApplicationDBContext>("promptevaldb");

var host = builder.Build();
host.Run();
