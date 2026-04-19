using Aspire.Hosting.JavaScript;
using System.Net;
using System.Net.Sockets;

EnsureAspireDashboardDefaults();

var builder = DistributedApplication.CreateBuilder(args);

var aiOrchestrator = builder.AddProject<Projects.AIOrchestrator_Service>("aiorchestrator")
    .WithHttpEndpoint(env: "ASPNETCORE_HTTP_PORTS")
    .AsHttp2Service();
var localGateway = builder.AddProject<Projects.LocalGateway_Service>("localgateway");
var jobs = builder.AddProject<Projects.Jobs_Service>("jobs")
    .WithReference(localGateway);
var coreBackend = builder.AddProject<Projects.CoreBackend_Service>("corebackend")
    .WithHttpEndpoint(env: "ASPNETCORE_HTTP_PORTS")
    .AsHttp2Service()
    .WithReference(aiOrchestrator)
    .WithReference(jobs);
var bffGateway = builder.AddProject<Projects.BffGateway_Service>("bffgateway")
    .WithHttpEndpoint(env: "ASPNETCORE_HTTP_PORTS")
    .WithExternalHttpEndpoints()
    .WithReference(coreBackend)
    .WithReference(aiOrchestrator);
var frontend = builder.AddViteApp("frontend", Path.Combine("..", "..", "frontend"))
    .WithExternalHttpEndpoints()
    .WithReference(bffGateway)
    .WithEnvironment("BFF_PROXY_TARGET", bffGateway.GetEndpoint("http"));

bffGateway
    .WithEnvironment("AUTH_FRONTEND_URL", frontend.GetEndpoint("http"))
    .WithEnvironment("AUTH_ALLOWED_ORIGINS", frontend.GetEndpoint("http"));

builder.Build().Run();

static void EnsureAspireDashboardDefaults()
{
    SetIfMissing("ASPIRE_ALLOW_UNSECURED_TRANSPORT", "true");
    SetIfMissing("ASPNETCORE_URLS", $"http://127.0.0.1:{GetAvailablePort()}");
    SetIfMissing("ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL", $"http://127.0.0.1:{GetAvailablePort()}");
    SetIfMissing("ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL", $"http://127.0.0.1:{GetAvailablePort()}");

    static void SetIfMissing(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key)))
        {
            Environment.SetEnvironmentVariable(key, value);
        }
    }

    static int GetAvailablePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }
}
