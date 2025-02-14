var builder = DistributedApplication.CreateBuilder(args);


var postgresPasswordParameterResource = builder.AddParameter("postgres-password", secret: true);
var postgresUsernameParameterResource = builder.AddParameter("postgres-user", secret: true);

var postgres = builder.AddPostgres("postgres", postgresUsernameParameterResource, postgresPasswordParameterResource)
                      .WithPgAdmin(containerName: "postgresadmin")
                      .WithLifetime(ContainerLifetime.Persistent)
                      .WithDataVolume("pgvolume");

var promptevaluationdb = postgres.AddDatabase("promptevaldb", "promptevaluationdb");


builder.AddProject<Projects.ProductClassification>("productclassification")
        .WithReference(promptevaluationdb)
        .WaitFor(promptevaluationdb);

builder.AddProject<Projects.ProductClassificationDatabase_MigrationService>("productclassificationdatabase-migrationservice")
        .WithReference(promptevaluationdb)
        .WaitFor(promptevaluationdb);

builder.Build().Run();
