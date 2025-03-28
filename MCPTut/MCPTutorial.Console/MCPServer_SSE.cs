
using ModelContextProtocol.Client;
using ModelContextProtocol.Configuration;
using ModelContextProtocol.Protocol.Transport;
using ModelContextProtocol.Protocol.Types;

namespace MCPTutorial.SSE
{
    public class MCPServer_SSE
    {
        private readonly IMcpClient mcpclient;
        public MCPServer_SSE()
        {
            HttpClient httpClient = new()
            {
                BaseAddress = new("http://localhost:5249/sse")  //"https +http://aspnetsseserver" + "/sse")
            };


            McpServerConfig serverconfig = new()
            {
                Id = "Sample",
                Name = "SampleSSEServer",
                TransportType = TransportTypes.Sse,
                Location = httpClient.BaseAddress.ToString()
            };

            McpClientOptions clientoptions = new()
            {
                ClientInfo = new Implementation() { Name = "Tool Client", Version = "1.0.0" }
            };

            mcpclient = McpClientFactory.CreateAsync(serverconfig, clientoptions).ConfigureAwait(ConfigureAwaitOptions.ContinueOnCapturedContext).GetAwaiter().GetResult();

        }

        public async Task RunAsync()
        {
            await foreach (Tool tool in mcpclient.ListToolsAsync(new CancellationTokenSource().Token))
            {
                Console.WriteLine($"Name={tool.Name}\tDescription:{tool.Description}");
            }
        }
    }
}
