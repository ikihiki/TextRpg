using CoreBackend.Domain.Scenarios;
using CoreBackend.Service.Infrastructure.Persistence;
using CoreBackend.Service.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

// Configure EF Core with PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register repositories
builder.Services.AddScoped<IScenarioRepository, ScenarioRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// TODO: Map gRPC services as they are created
// app.MapGrpcService<SessionService>();
// app.MapGrpcService<NotesService>();
// app.MapGrpcService<AssetService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
