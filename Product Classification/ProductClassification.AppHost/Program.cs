using ProductClassification.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

#region Postgres
var postgresPasswordParameterResource = builder.AddParameter("postgres-password", secret: true);
var postgresUsernameParameterResource = builder.AddParameter("postgres-user", secret: true);

var postgres = builder.AddPostgres("postgres", postgresUsernameParameterResource, postgresPasswordParameterResource)
                      .WithImage("pgvector/pgvector", "pg17")
                      .WithPgAdmin(containerName: "postgresadmin", configureContainer: configs =>
                      {
                          configs.WithImage("elestio/pgadmin");
                          configs.WithBindMount("./Postgres", "/home/Postgres");
                      })
                      .WithEnvironment("POSTGRES_DB", "promptevaluationdb")
                      .WithBindMount("./Postgres/Init", "/docker-entrypoint-initdb.d")
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


#region Product Classification - Migration Service
var migrationservice = builder.AddProject<Projects.ProductClassificationDatabase_MigrationService>("productclassificationdatabase-migrationservice")
        .WithReference(promptevaluationdb)
        .WaitFor(promptevaluationdb);
#endregion

#region Product Classification - PgVector Data Seeding Service
var vectordataseedingservice = builder.AddProject<Projects.VectorStore_SeedingService>("vectorstore-seedingservice")
       .WithReference(promptevaluationdb)
       .WaitFor(promptevaluationdb)
       .WaitForCompletion(migrationservice)
       .WaitFor(ollama)
       .AddOllamaEndpointToEnvironmentVariables(ollamacontainerendpoint);
#endregion

#region Product Classification
builder.AddProject<Projects.ProductClassification>("productclassification")
        .WithReference(promptevaluationdb)
        .WaitFor(promptevaluationdb)
        .WaitFor(ollama)
        .WaitForCompletion(migrationservice)
        .AddOllamaEndpointToEnvironmentVariables(ollamacontainerendpoint);
#endregion




builder.Build().Run();
