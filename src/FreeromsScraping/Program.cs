using Fizzler.Systems.HtmlAgilityPack;
using FreeromsScraping.Configuration;
using HtmlAgilityPack;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FreeromsScraping
{
    internal class Program
    {
        private static async Task MainAsync()
        {
            var configuration = (ScrapingSection)ConfigurationManager.GetSection("scraping");

            foreach (var source in configuration.Sources.Cast<SourceElement>())
            {
                await DownloadCatalogAsync(source.Name, source.Url);
            }

            Logger.Info("END");
            Console.Read();
        }

        private static async Task DownloadCatalogAsync(string name, string url)
        {
            Logger.Info($"Fetching source {name}...");

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Logger.Error($"Error while fetching source {name} !");
                    return;
                }

                Logger.Info("Content read, looking for menu links...");
                var content = await response.Content.ReadAsStringAsync();
                var menuLinks = ParseContentForMenuLink(content);

                foreach (var link in menuLinks)
                {
                    await DownloadPageCatalogAsync(link);
                }
            }
        }

        private static Task DownloadPageCatalogAsync(string link)
        {
            Logger.Info($"Fetching {link}...");

            throw new NotImplementedException();
        }

        private static IEnumerable<string> ParseContentForMenuLink(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            foreach (var node in htmlDoc.DocumentNode.QuerySelectorAll("tr.letters a"))
            {
                yield return node.Attributes["href"].Value;
            }
        }

        private static void Main()
        {
            AsyncContext.Run(async () => await MainAsync().ConfigureAwait(false));
        }
    }
}
