using TextRpg.Jobs;

var builder = Host.CreateApplicationBuilder(args);

// Add service defaults & Aspire integrations.
builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
