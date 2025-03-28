using ModelContextProtocol.Client;
using ModelContextProtocol.Configuration;
using ModelContextProtocol.Protocol.Transport;
using ModelContextProtocol.Protocol.Types;

namespace MCPTutorial.STDIO
{
    public class MCPServer_STDIO
    {
        private readonly IMcpClient mcpClient;
        public MCPServer_STDIO()
        {
            McpServerConfig serverconfig = new()
            {
                Id = "everything",
                Name = "Everything",
                TransportType = TransportTypes.StdIo,
                // Server Executable 
                Location = @"D:\Training\AIML\MCPTut\MCPTutorial.Server\bin\Debug\net8.0\MCPTutorial.Server.exe"
            };

            McpClientOptions clientoptions = new()
            {
                ClientInfo = new Implementation() { Name = "Tool Client", Version = "1.0.0" }
            };

            mcpClient = McpClientFactory.CreateAsync(serverconfig, clientoptions).ConfigureAwait(ConfigureAwaitOptions.ContinueOnCapturedContext).GetAwaiter().GetResult();
        }

        public async Task RunAsync()
        {
            // Listing all the Tools present in the MCP Server
            await foreach (Tool tool in mcpClient.ListToolsAsync(new CancellationTokenSource().Token))
            {
                Console.WriteLine($"Name={tool.Name}\tDescription:{tool.Description}");
            }
        }
    }
}
