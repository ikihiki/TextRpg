var builder = DistributedApplication.CreateBuilder(args);

// TODO: Add service references as they are created
// Example:
// var coreBackend = builder.AddProject<Projects.CoreBackend>("corebackend");
// var bffGateway = builder.AddProject<Projects.BffGateway>("bffgateway")
//     .WithReference(coreBackend);

builder.Build().Run();
