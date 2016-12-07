using FreeromsScraping.Configuration;
using Nito.AsyncEx;
using System;
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

                Logger.Info("Content read, looking for links...");
                var content = await response.Content.ReadAsStringAsync();
            }
        }

        private static void Main()
        {
            AsyncContext.Run(async () => await MainAsync().ConfigureAwait(false));
        }
    }
}
