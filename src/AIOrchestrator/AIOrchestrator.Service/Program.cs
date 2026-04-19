using AIOrchestrator.Service.Services;
using TextRpg.Shared.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.AddTextRpgTelemetry();
builder.Services.AddGrpc(options => options.EnableDetailedErrors = builder.Environment.IsDevelopment());

var app = builder.Build();

app.MapGrpcService<OrchestratorGrpcService>();
app.MapFoundationEndpoints("aiorchestrator");

app.Run();
