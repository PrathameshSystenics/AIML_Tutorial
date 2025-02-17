using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Store;
using TinyShop.AppHost;

// Distributed Application Builder Pattern.
var builder = DistributedApplication.CreateBuilder(args);

// getting the parameter from the secrets.json
var postgrespasswordparameter = builder.AddParameter("postgres-password", true);
var postgrespasswordusername = builder.AddParameter("postgres-username", true);

// Adding the Postgres Database specifying the name, image, and Tag
var postgres = builder.AddPostgres("postgres", postgrespasswordusername, postgrespasswordparameter)
                .WithImage("postgres")
                .WithImageTag("16.6-alpine3.21")
                .WithPgAdmin((res) =>
                {
                    res.WithContainerName("pg-admin");
                })
                // Creates the volume with the provided name
                .WithDataVolume("pgvol");

// Subscribing to the resource events
builder.Eventing.Subscribe<ResourceReadyEvent>(postgres.Resource, (rev, token) =>
{
    var logger = rev.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Postgres Resource is Ready");

    return Task.CompletedTask;
});


// adding the container with the image, specifying the container, exposing the port.
var ollama = builder.AddContainer("ollama", "ollama/ollama")
                    .WithContainerName("ollama-aspire")
                    .WithHttpEndpoint(11434, 11434, "ollamaendpoint")
                    .WithLifetime(ContainerLifetime.Persistent)
                    .WithVolume("ollmodels","/root/.ollama")
                    .WithBindMount("./ollamasetup.sh", "/ollamasetup.sh")
                    .WithEntrypoint("/bin/sh")
                    .WithArgs("/ollamasetup.sh");

// Adding the database to the created postgres db. Here we need to specify the connection string name and database name
var productdb = postgres.AddDatabase("productcontext", "productdb")
                        .WithClearDatabaseDataCommand();

// Adding the Product Project in to the Aspire Orchestrator
// Here we added the support for the Postgres Database to the Products Project
var products = builder.AddProject<Projects.Products>("products")
                .WithReference(productdb)
                .WaitFor(productdb);

// Adding the Migration service before creating the Product API. 
builder.AddProject<Projects.MigrationService>("migrationservice")
                              .WithReference(productdb)
                              .WaitFor(productdb);

// Getting the endpoint for the container
var ollamacontainer = ollama.GetEndpoint("ollamaendpoint");

// Adding the support for lifecycle hooks
builder.Services.AddLifecycleHook<LifeCyleLogger>();

// Subscribing to the events
builder.Eventing.Subscribe<BeforeStartEvent>((sta, token) =>
{
    var logger = sta.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("BeforeStartEvent");

    return Task.CompletedTask;

});

// Add the Parameter to the application resource
var smtpmailparameter = builder.AddParameter("smtpmail");

// Adding the Project or Microservice.
// The store project is also dependent on the Products.
builder.AddProject<Projects.Store>("store")
    .WithExternalHttpEndpoints()
    .WithReference(products)
    .WaitFor(products)
    .WithReference(ollamacontainer)
    // Providing the Evironment as the parameter to it. 
    .WithEnvironment("SMTP_MAIL", smtpmailparameter);


builder.Build().Run();
