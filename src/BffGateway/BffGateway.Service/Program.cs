using BffGateway.Service.Services;
using TextRpg.Shared.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(options => options.EnableDetailedErrors = builder.Environment.IsDevelopment());

var app = builder.Build();

app.MapGrpcService<GameApiGrpcService>();
app.MapFoundationEndpoints("bffgateway");

app.Run();
