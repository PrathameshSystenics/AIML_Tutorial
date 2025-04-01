
using System.Net;
using ModelContextProtocol.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add default services
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<HttpListener>();
builder.Services.AddMcpServer()
                .WithToolsFromAssembly()
                .WithPromptsFromAssembly(); // New in Preview 4

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

app.MapMcp(); // new in Preview 4
app.Run();