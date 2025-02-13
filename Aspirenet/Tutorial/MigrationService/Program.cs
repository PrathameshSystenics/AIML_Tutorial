using MigrationService;
using Products.Data;

var builder = Host.CreateApplicationBuilder(args);

// Add the Extension method of the Service Defaults
builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

// Add the support for opentelemetry
builder.Services.AddOpenTelemetry().WithLogging()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddNpgsqlDbContext<ProductDataContext>("productcontext");

var host = builder.Build();
host.Run();
