using CoreBackend.Service.Services;
using TextRpg.Shared.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(options => options.EnableDetailedErrors = builder.Environment.IsDevelopment());

var app = builder.Build();

app.MapGrpcService<SessionGrpcService>();
app.MapGrpcService<NotesGrpcService>();
app.MapGrpcService<AssetGrpcService>();
app.MapFoundationEndpoints("corebackend");

app.Run();
