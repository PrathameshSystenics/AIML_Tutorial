#region Simple MCP STDIO Server
using MCPTutorial.STDIO;
/*MCPServer_STDIO mcpserverstdio = new MCPServer_STDIO();
await mcpserverstdio.RunAsync();*/
#endregion

#region Simple MCP SSE Server
using MCPTutorial.SSE;
MCPServer_SSE mcpserverSSE = new MCPServer_SSE();
await mcpserverSSE.RunAsync();
#endregion