using Aspire.Hosting;
using ProductClassification.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

#region Postgres
var postgresPasswordParameterResource = builder.AddParameter("postgres-password", secret: true);
var postgresUsernameParameterResource = builder.AddParameter("postgres-user", secret: true);

var postgres = builder.AddPostgres("postgres", postgresUsernameParameterResource, postgresPasswordParameterResource)
                      .WithImage("pgvector/pgvector","pg17")
                      .WithPgAdmin(containerName: "postgresadmin", configureContainer: configs =>
                      {
                          configs.WithImage("elestio/pgadmin");
                      })
                      .WithLifetime(ContainerLifetime.Persistent)
                      .WithDataVolume("pgvolume")
                      .AddCommandForClearEvaluationData();

var promptevaluationdb = postgres.AddDatabase("promptevaldb", "promptevaluationdb");
#endregion

#region Ollama Container
var ollama = builder.AddContainer("ollama", "ollama/ollama")
                    .WithContainerName("ollama-aspire")
                    .WithHttpEndpoint(11434, 11434, "ollamaendpoint")
                    .WithLifetime(ContainerLifetime.Persistent)
                    .WithVolume("ollmodels", "/root/.ollama")
                    .WithBindMount("./ollamasetup.sh", "/ollamasetup.sh")
                    .WithEntrypoint("/bin/sh")
                    .WithArgs("/ollamasetup.sh");

var ollamacontainerendpoint = ollama.GetEndpoint("ollamaendpoint");
#endregion

#region Product Classification
builder.AddProject<Projects.ProductClassification>("productclassification")
        .WithReference(promptevaluationdb)
        .WaitFor(promptevaluationdb)
        .WaitFor(ollama)
        .AddOllamaEndpointToEnvironmentVariables(ollamacontainerendpoint);
#endregion

#region Product Classification - Migration Service
builder.AddProject<Projects.ProductClassificationDatabase_MigrationService>("productclassificationdatabase-migrationservice")
        .WithReference(promptevaluationdb)
        .WaitFor(promptevaluationdb);
#endregion

builder.Build().Run();
