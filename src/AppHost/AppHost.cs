var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL Database
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("textrpgdb");

// Core Backend Service
var coreBackend = builder.AddProject<Projects.CoreBackend>("corebackend")
    .WithReference(postgres);

// AI Orchestrator Service
var aiOrchestrator = builder.AddProject<Projects.AIOrchestrator>("aiorchestrator")
    .WithReference(coreBackend);

// Jobs Worker Service
var jobs = builder.AddProject<Projects.Jobs>("jobs")
    .WithReference(postgres)
    .WithReference(coreBackend)
    .WithReference(aiOrchestrator);

// BFF Gateway Service
builder.AddProject<Projects.BffGateway>("bffgateway")
    .WithReference(coreBackend)
    .WithReference(aiOrchestrator);

builder.Build().Run();
