var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);
var postgres = builder.AddPostgres("postgres", username, password, 54048);
var postgresDb = postgres.AddDatabase("postgresdb", databaseName: "postgres");

var vanillaApi = builder
    .AddProject<Projects.WolverineHoP_VanillaApi>("vanilla-api")
    .WithReference(postgresDb);

// TODO: Wolverine will support Aspire in next major version
//var wolverineDocumentApi = builder.AddProject<Projects.WolverineHoP_WolverineDocumentApi>("wolverine-document-api");

builder.AddProject<Projects.WolverineHoP_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(vanillaApi);


builder.Build().Run();
