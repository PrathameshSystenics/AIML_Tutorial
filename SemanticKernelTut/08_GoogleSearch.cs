using Google.Apis.CustomSearchAPI.v1;
using Google.Apis.CustomSearchAPI.v1.Data;
using Google.Apis.Services;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace SemanticKernelTut
{
    public class _08_GoogleSearch
    {
        private readonly IConfiguration configuration;

        public _08_GoogleSearch(IConfiguration config)
        {
            configuration = config;
        }


        public async Task<Search> RunGoogleSearch(string query)
        {
            BaseClientService.Initializer initializer = new BaseClientService.Initializer()
            {
                ApiKey = configuration["GoogleSearch:ApiKey"]
            };

            CustomSearchAPIService service = new CustomSearchAPIService(initializer);

            var cselist = service.Cse.List();
            cselist.Cx = configuration["GoogleSearch:EngineId"];
            cselist.Num = 5;
            cselist.Q = query;
            Search data = await cselist.ExecuteAsync();
            return data;
        }

        public async Task ReadAsTextFromHtml(string query)
        {
            var data = await RunGoogleSearch(query);

            foreach (var item in data.Items)
            {
                HttpClient httpclient = new HttpClient();
                HttpResponseMessage response = await httpclient.GetAsync(item.Link, new CancellationTokenSource().Token);
                string htmlresponse = await response.Content.ReadAsStringAsync();

                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(htmlresponse);

                // Remove <script> and <style> nodes if you want to avoid their text.
                foreach (var script in document.DocumentNode.SelectNodes("//script|//style"))
                {
                    script.Remove();
                }

                string content = document.DocumentNode.InnerText.Trim();
                string cleanedText = Regex.Replace(content, @"\s+", " ").Trim();


                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine(item.Title);
                Console.WriteLine(item.Link);
                Console.ResetColor();
                Console.WriteLine(cleanedText);
                Console.WriteLine();

            }
        }
    }
}
