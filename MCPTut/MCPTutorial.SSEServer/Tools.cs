using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPTutorial.SSEServer.Tools
{
    [McpServerToolType]
    public static class SampleTool
    {
        [McpServerTool("sayhello"), Description("Greets the User")]
        public static string SayHello()
        {
            return "Hello from MCPServer";
        }

        [McpServerTool, Description("Adds the Two Number")]
        public static int AddTwoNumbers(int a, int b)
        {
            return a + b;
        }
    }

    [McpServerToolType]
    public static class GithubTool
    {
        [McpServerTool, Description("Returns the GitHub repository URL constructed from the specified username and repository name.")]
        public static string GetRepository(string repositoryname, string username)
        {
            return $"https://www.github.com/{username}/{repositoryname}";
        }
    }

    [McpServerToolType]
    public class JokeTool
    {
        private readonly HttpClient _httpClient;

        public JokeTool()
        {
            // Initialize HttpClient with the base address
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://v2.jokeapi.dev/joke/"),
                
            };
            
        }

        [McpServerTool, Description("Fetches a collection of jokes from the specified category. Valid categories include 'Programming', 'Misc', and 'Christmas'. If no amount is specified, 10 jokes are returned by default.")]
        public async Task<string> GetJoke(
            [Description("The joke category to filter by (e.g., 'Programming', 'Misc', 'Christmas').")] string category,
            [Description("The number of jokes to retrieve. Defaults to 10 if not provided.")] int amountofjokes = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{category}?amount={amountofjokes}");
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                return "Error";
            }
        }
    }
}
