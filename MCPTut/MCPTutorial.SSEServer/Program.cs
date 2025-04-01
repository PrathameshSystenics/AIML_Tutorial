using MCPTutorial.SSEServer.Extensions;
using ModelContextProtocol;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add default services
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<HttpListener>();
builder.Services.AddMcpServer().WithToolsFromAssembly();

builder.Services.AddLogging((configure) =>
{
    configure.SetMinimumLevel(LogLevel.Debug);
});


// add MCP server
var app = builder.Build();

// Initialize default endpoints
app.UseHttpsRedirection();

// map endpoints
app.MapGet("/", () => $"Hello World! {DateTime.Now}");
app.MapMcpSse();

app.Run();