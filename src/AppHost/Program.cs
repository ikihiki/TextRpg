var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume();

var coreBackendDb = postgres.AddDatabase("corebackenddb");

// Add Core Backend service
var coreBackend = builder.AddProject<Projects.CoreBackend_Service>("corebackend")
    .WithReference(coreBackendDb)
    .WaitFor(coreBackendDb);

// Add BFF Gateway service
var bffGateway = builder.AddProject<Projects.BffGateway_Service>("bffgateway")
    .WithReference(coreBackend)
    .WaitFor(coreBackend);

builder.Build().Run();
