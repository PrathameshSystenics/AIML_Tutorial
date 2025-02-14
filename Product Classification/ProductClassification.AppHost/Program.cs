using Aspire.Hosting;
using ProductClassification.AppHost;

var builder = DistributedApplication.CreateBuilder(args);


var postgresPasswordParameterResource = builder.AddParameter("postgres-password", secret: true);
var postgresUsernameParameterResource = builder.AddParameter("postgres-user", secret: true);

var postgres = builder.AddPostgres("postgres", postgresUsernameParameterResource, postgresPasswordParameterResource)
                      .WithPgAdmin(containerName: "postgresadmin")
                      .WithLifetime(ContainerLifetime.Persistent)
                      .WithDataVolume("pgvolume")
                      .AddCommandForClearEvaluationData();

var promptevaluationdb = postgres.AddDatabase("promptevaldb", "promptevaluationdb");

var ollama = builder.AddContainer("ollama", "ollama/ollama")
                    .WithContainerName("ollama-aspire")
                    .WithHttpEndpoint(11434, 11434, "ollamaendpoint")
                    .WithLifetime(ContainerLifetime.Persistent)
                    .WithVolume("ollmodels", "/root/.ollama")
                    .WithBindMount("./ollamasetup.sh", "/ollamasetup.sh")
                    .WithEntrypoint("/bin/sh")
                    .WithArgs("/ollamasetup.sh");

var ollamacontainerendpoint = ollama.GetEndpoint("ollamaendpoint");

builder.AddProject<Projects.ProductClassification>("productclassification")
        .WithReference(promptevaluationdb)
        .WaitFor(promptevaluationdb)
        .WaitFor(ollama)
        .AddOllamaEndpointToEnvironmentVariables(ollamacontainerendpoint);

builder.AddProject<Projects.ProductClassificationDatabase_MigrationService>("productclassificationdatabase-migrationservice")
        .WithReference(promptevaluationdb)
        .WaitFor(promptevaluationdb);

builder.Build().Run();
