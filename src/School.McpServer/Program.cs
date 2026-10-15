using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using School.Infrastructure;
using School.McpServer;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services.AddSchoolInfrastructure();
builder.Services.AddSingleton<ISchoolService, SchoolService>();
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<SchoolTools>();

var host = builder.Build();
host.Services.InitializeSchoolDatabase();
await host.RunAsync();
