using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace TextRpg.Shared.Utils;

public static class GrpcServiceFoundationExtensions
{
    public static WebApplication MapFoundationEndpoints(this WebApplication app, string serviceName)
    {
        app.MapGet("/", () => Results.Ok(new ServiceInfoResponse(
            serviceName,
            "gRPC endpoint is ready. Use a gRPC client for RPC calls.",
            app.Environment.EnvironmentName,
            DateTimeOffset.UtcNow)));

        app.MapGet("/health", () => Results.Ok(new HealthResponse(
            serviceName,
            "Healthy",
            app.Environment.EnvironmentName,
            DateTimeOffset.UtcNow)));

        return app;
    }

    private sealed record ServiceInfoResponse(
        string Service,
        string Message,
        string Environment,
        DateTimeOffset TimestampUtc);

    private sealed record HealthResponse(
        string Service,
        string Status,
        string Environment,
        DateTimeOffset TimestampUtc);
}
