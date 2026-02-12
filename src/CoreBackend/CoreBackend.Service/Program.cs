using CoreBackend.Domain.Users;
using CoreBackend.Infrastructure;
using CoreBackend.Infrastructure.Repositories;
using CoreBackend.Infrastructure.Services;
using CoreBackend.Service.Services;
using Microsoft.EntityFrameworkCore;

// Feature usings
using FeatureRegisterUser = CoreBackend.Features.RegisterUser;
using FeatureLoginUser = CoreBackend.Features.LoginUser;
using FeatureLogoutUser = CoreBackend.Features.LogoutUser;
using FeatureGetUserProfile = CoreBackend.Features.GetUserProfile;
using FeatureUpdateUserProfile = CoreBackend.Features.UpdateUserProfile;
using FeatureDeleteUser = CoreBackend.Features.DeleteUser;
using FeatureValidateSession = CoreBackend.Features.ValidateSession;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

// Add Aspire service defaults
builder.AddServiceDefaults();

// Add PostgreSQL with EF Core via Aspire
builder.AddNpgsqlDbContext<CoreBackendDbContext>("corebackenddb");

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();

// Register services
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// Register use cases
builder.Services.AddScoped<FeatureRegisterUser.RegisterUser>();
builder.Services.AddScoped<FeatureLoginUser.LoginUser>();
builder.Services.AddScoped<FeatureLogoutUser.LogoutUser>();
builder.Services.AddScoped<FeatureGetUserProfile.GetUserProfile>();
builder.Services.AddScoped<FeatureUpdateUserProfile.UpdateUserProfile>();
builder.Services.AddScoped<FeatureDeleteUser.DeleteUser>();
builder.Services.AddScoped<FeatureValidateSession.ValidateSession>();

var app = builder.Build();

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CoreBackendDbContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
app.MapGrpcService<UserServiceImpl>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
