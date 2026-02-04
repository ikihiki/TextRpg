var builder = Host.CreateApplicationBuilder(args);

// TODO: Add Hangfire configuration
// TODO: Add job handlers

var host = builder.Build();
host.Run();
