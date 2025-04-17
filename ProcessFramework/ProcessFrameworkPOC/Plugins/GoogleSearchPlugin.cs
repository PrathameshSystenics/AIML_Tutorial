using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using ProcessFrameworkPOC.Models;
using ProcessFrameworkPOC.Services;
using System.ComponentModel;

namespace ProcessFrameworkPOC.Plugins
{
    public class GoogleSearchPlugin
    {

        [KernelFunction("search"), Description("search with the specified query")]
        public async Task<List<Articles>> SearchAsync([Description("Query to Search in Google")] string query)
        {
            string path = "D:\\Training\\AIML\\ProcessFramework\\ProcessFrameworkPOC\\appsettings.json";
            IConfiguration config = new ConfigurationBuilder().AddJsonFile(path).Build();
            GoogleSearchService googleSearchService = new GoogleSearchService(config);
            var results = await googleSearchService.GetContentBasedOnQueryAsync(query);
            return results;
        }
    }
}
