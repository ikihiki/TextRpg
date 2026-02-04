using CoreBackend.Service.Services;
using FeatureCreateSession = CoreBackend.Features.CreateSession;
using FeatureGetSession = CoreBackend.Features.GetSession;
using FeatureUpdateSessionState = CoreBackend.Features.UpdateSessionState;
using FeatureCreateTurn = CoreBackend.Features.CreateTurn;
using FeatureRewindTurn = CoreBackend.Features.RewindTurn;
using FeatureCreateNote = CoreBackend.Features.CreateNote;
using FeatureGetNotes = CoreBackend.Features.GetNotes;
using FeatureUpdateNote = CoreBackend.Features.UpdateNote;
using FeatureCreateAsset = CoreBackend.Features.CreateAsset;
using FeatureGetAssets = CoreBackend.Features.GetAssets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

// Register use cases
builder.Services.AddScoped<FeatureCreateSession.CreateSession>();
builder.Services.AddScoped<FeatureGetSession.GetSession>();
builder.Services.AddScoped<FeatureUpdateSessionState.UpdateSessionState>();
builder.Services.AddScoped<FeatureCreateTurn.CreateTurn>();
builder.Services.AddScoped<FeatureRewindTurn.RewindTurn>();
builder.Services.AddScoped<FeatureCreateNote.CreateNote>();
builder.Services.AddScoped<FeatureGetNotes.GetNotes>();
builder.Services.AddScoped<FeatureUpdateNote.UpdateNote>();
builder.Services.AddScoped<FeatureCreateAsset.CreateAsset>();
builder.Services.AddScoped<FeatureGetAssets.GetAssets>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<SessionServiceImpl>();
app.MapGrpcService<NotesServiceImpl>();
app.MapGrpcService<AssetServiceImpl>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
