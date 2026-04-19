using TextRpg.Shared.Utils;

var builder = Host.CreateApplicationBuilder(args);

builder.AddTextRpgTelemetry();
// TODO: Add Hangfire configuration
// TODO: Add job handlers

var host = builder.Build();
host.Run();
