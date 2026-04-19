using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace TextRpg.Shared.Utils;

public static class TelemetryDefaultsExtensions
{
    public static TBuilder AddTextRpgTelemetry<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        var serviceName = builder.Environment.ApplicationName;
        var hasOtlpEndpoint =
            !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]) ||
            !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_TRACES_ENDPOINT"]) ||
            !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_METRICS_ENDPOINT"]) ||
            !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_LOGS_ENDPOINT"]);

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                if (hasOtlpEndpoint)
                {
                    tracing.AddOtlpExporter();
                }
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                if (hasOtlpEndpoint)
                {
                    metrics.AddOtlpExporter();
                }
            });

        builder.Logging.AddOpenTelemetry();
        builder.Services.Configure<OpenTelemetryLoggerOptions>(options =>
        {
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
            options.ParseStateValues = true;

            if (hasOtlpEndpoint)
            {
                options.AddOtlpExporter();
            }
        });

        return builder;
    }
}
