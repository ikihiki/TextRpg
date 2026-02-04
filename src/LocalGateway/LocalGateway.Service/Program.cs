var builder = Host.CreateApplicationBuilder(args);

// TODO: Add gRPC client configuration for bidirectional streaming
// TODO: Add work stream client service

var host = builder.Build();
host.Run();
