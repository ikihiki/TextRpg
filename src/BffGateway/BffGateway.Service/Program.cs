using BffGateway.Service.Auth;
using BffGateway.Service.Services;
using TextRpg.Shared.Utils;

var builder = WebApplication.CreateBuilder(args);
var authOptions = BffAuthOptions.FromConfiguration(builder.Configuration);

builder.AddTextRpgTelemetry();
builder.Services.AddGrpc(options => options.EnableDetailedErrors = builder.Environment.IsDevelopment());
builder.Services.AddSingleton(authOptions);
builder.Services.AddSingleton<BootstrapPasswordLoginService>();
builder.Services.AddBffAuthentication(authOptions);
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.WithOrigins(authOptions.AllowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<GameApiGrpcService>();
app.MapAuthEndpoints();
app.MapFoundationEndpoints("bffgateway");

app.Run();
