var builder = DistributedApplication.CreateBuilder(args);

var k8s = builder.AddKubernetesEnvironment("k8s")
;
var registry = builder.AddContainerRegistry(
	"ghcr",                              // Registry name
	"ghcr.io",                           // Registry endpoint
	"ikihiki/textrpg"     // Repository path
);
var imageTag  = Environment.GetEnvironmentVariable("IMAGE_TAG") ?? "latest";


var postgres = builder.AddPostgres("postgres")
	.WithPgAdmin();
var coreDatabase = postgres.AddDatabase("corebackenddb");
var core = builder
	.AddProject<Projects.CoreBackend_Service>("corebackend")
	.WithReference(coreDatabase)
	.WithHttpEndpoint()
	.WaitFor(coreDatabase)
	.WithContainerRegistry(registry)
	.WithRemoteImageTag(imageTag)
	;

var bff = builder
	.AddProject<Projects.BffGateway_Service>("bffgateway")
	.WithReference(core)
	.WithHttpEndpoint()
	.WaitFor(core)
	.WithContainerRegistry(registry)
	.WithRemoteImageTag(imageTag)
	;

builder.AddProject<Projects.LocalGateway_Service>("localgateway")
	.WithHttpEndpoint()
	.WithContainerRegistry(registry)
	.WithRemoteImageTag(imageTag)
	;
builder.AddProject<Projects.Jobs_Service>("jobs")
	.WithHttpEndpoint()
	.WithContainerRegistry(registry)
	.WithRemoteImageTag(imageTag)
	;
builder.AddProject<Projects.AIOrchestrator_Service>("aiorchestrator")
	.WithHttpEndpoint()
	.WithContainerRegistry(registry)
	.WithRemoteImageTag(imageTag)
	;

builder
	.AddJavaScriptApp("frontend", "../../frontend", "dev")
	.WaitFor(bff)
	.WithHttpEndpoint(env: "PORT")
	.WithContainerRegistry(registry)
	.WithRemoteImageTag(imageTag)
	;

builder.Build().Run();



