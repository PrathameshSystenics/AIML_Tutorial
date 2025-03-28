using ModelContextProtocol;

var builder = Host.CreateEmptyApplicationBuilder(null);

// adding the MCP server, loading all the tools which are present in the Assembly with Attribute ToolType, Creating it as a StdioServer Protocol
builder.Services.AddMcpServer().WithToolsFromAssembly().WithStdioServerTransport();
builder.Logging.AddConsole();

var app = builder.Build();
app.Run();