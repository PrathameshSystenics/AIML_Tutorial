using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ProcessFrameworkPOC.Models
{
    public class Articles
    {
        [JsonPropertyName("title")]
        [Description("The title of the article retrieved from the search result.")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        [Description("The URL link to the article.")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        [Description("The summary or snippet of the article content.")]
        public string Content { get; set; } = string.Empty;
    }
}
