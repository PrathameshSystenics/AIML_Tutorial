using ProductClassification.Data;
using ProductClassification.Extensions;
using VectorStore.SeedingService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddServicesRequiredForEmbedding();
builder.AddVectorDbSupport(builder.Configuration);

builder.Services.AddHostedService<VectorDataSeeder>();

builder.Services.AddOpenTelemetry()
                .WithTracing(tracing => tracing.AddSource(VectorDataSeeder.ActivitySourceName))
                .WithLogging()
                .WithMetrics();

var host = builder.Build();

host.Run();
