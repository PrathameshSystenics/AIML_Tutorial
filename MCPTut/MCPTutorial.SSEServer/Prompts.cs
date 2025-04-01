using ModelContextProtocol.Server;

namespace MCPTutorial.SSEServer
{
    [McpServerPromptType]
    public class Prompts
    {
        [McpServerPrompt]
        public static int CalculateMinus(int a, int b)
        {
            return b - a;
        }
    }
}
