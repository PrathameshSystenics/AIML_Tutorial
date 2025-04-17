using AngleSharp;
using AngleSharp.Dom;
using Google.Apis.CustomSearchAPI.v1;
using Google.Apis.CustomSearchAPI.v1.Data;
using Google.Apis.Services;
using ProcessFrameworkPOC.Models;
using System.Text.RegularExpressions;

namespace ProcessFrameworkPOC.Services
{
    public class GoogleSearchService
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration configuration;

        public GoogleSearchService(Microsoft.Extensions.Configuration.IConfiguration config)
        {
            configuration = config;
        }

        private async Task<Search> SearchQueryAsync(string query)
        {
            BaseClientService.Initializer initializer = new BaseClientService.Initializer()
            {
                ApiKey = configuration["GoogleSearch:ApiKey"]
            };

            CustomSearchAPIService service = new CustomSearchAPIService(initializer);

            var cselist = service.Cse.List();
            cselist.Cx = configuration["GoogleSearch:EngineId"];
            cselist.SiteSearchFilter = CseResource.ListRequest.SiteSearchFilterEnum.E;
            cselist.SiteSearch = "https://www.youtube.com";
            cselist.Num = 10;
            cselist.Q = query;
            Search data = await cselist.ExecuteAsync();
            return data;
        }

        /*        private string ConvertHtmlContentToString(string htmlcontent)
                {
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(htmlcontent);

                    // Remove <script> and <style> nodes if you want to avoid their text.
                    foreach (var script in document.DocumentNode.SelectNodes("//script|//style"))
                    {
                        script.Remove();
                    }

                    string content = document.DocumentNode.InnerText.Trim();
                    string cleanedText = Regex.Replace(content, @"\s+", " ").Trim();
                    return cleanedText;
                }

                private async Task<string> GetHtmlContentAsync(string url)
                {
                    HttpClient httpclient = new HttpClient();
                    HttpResponseMessage response = await httpclient.GetAsync(url, new CancellationTokenSource().Token);
                    string htmlresponse = await response.Content.ReadAsStringAsync();
                    return htmlresponse;
                }*/

        private async Task<string> ConvertHtmlContentToStringWithAngleSharp(string url)
        {
            var config = Configuration.Default.WithDefaultLoader().WithJs();
            var context = BrowsingContext.New(config);

            IDocument document = await context.OpenAsync(url);

            foreach (var element in document.QuerySelectorAll("script, style, meta, head, iframe"))
            {
                element.Remove();
            }

            // Get text content from the body (or the whole document if needed).
            string textContent = document.Body?.TextContent ?? document.DocumentElement.TextContent;

            // Remove excessive whitespace.
            string cleanedText = Regex.Replace(textContent, @"\s+", " ").Trim();

            return cleanedText;
        }

        public async Task<List<Articles>> GetContentBasedOnQueryAsync(string query)
        {
            List<Articles> articles = new List<Articles>();

            Search searchresults = await this.SearchQueryAsync(query);
            foreach (var item in searchresults.Items)
            {
                string renderedhtml = await ConvertHtmlContentToStringWithAngleSharp(item.Link);
                articles.Add(new Articles() { Content = renderedhtml, Title = item.Title, Url = item.Link });
            }
            return articles;
        }
    }
}
