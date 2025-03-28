using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPTutorial.Server
{
    [McpToolType]
    public static class Tool
    {
        [McpTool("sayhello"), Description("Greets the User")]
        public static string SayHello()
        {
            return "Hello from MCPServer";
        }

        [McpTool, Description("Adds the Two Number")]
        public static int AddTwoNumbers(int a, int b)
        {
            return a + b;
        }
    }
}
