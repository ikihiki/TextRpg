var builder = DistributedApplication.CreateBuilder(args);

var aiOrchestrator = builder.AddProject<Projects.AIOrchestrator_Service>("aiorchestrator");
var localGateway = builder.AddProject<Projects.LocalGateway_Service>("localgateway");
var jobs = builder.AddProject<Projects.Jobs_Service>("jobs")
    .WithReference(localGateway);
var coreBackend = builder.AddProject<Projects.CoreBackend_Service>("corebackend")
    .WithReference(aiOrchestrator)
    .WithReference(jobs);

builder.AddProject<Projects.BffGateway_Service>("bffgateway")
    .WithReference(coreBackend)
    .WithReference(aiOrchestrator);

builder.Build().Run();
