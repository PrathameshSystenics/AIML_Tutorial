using AngleSharp.Dom;
using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace ProcessFrameworkPOC.Plugins
{
    public class VisitSitePlugin
    {
        [KernelFunction("visit_site"),Description("Visits the Url and returns the content")]
        public async  Task<string> GetStringContentOfWebsite(string url)
        {
            return await ConvertHtmlContentToStringWithAngleSharp(url);
        }

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
    }
}
