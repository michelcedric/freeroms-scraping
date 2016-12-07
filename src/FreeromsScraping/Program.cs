using FreeromsScraping.Configuration;
using Nito.AsyncEx;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace FreeromsScraping
{
    internal class Program
    {
        private static Task MainAsync()
        {
            var configuration = (ScrapingSection)ConfigurationManager.GetSection("scraping");

            foreach (var source in configuration.Sources.Cast<SourceElement>())
            {
                Console.WriteLine($"Fetching source {source.Name}");
            }

            throw new NotImplementedException();
        }

        private static void Main()
        {
            AsyncContext.Run(async () => await MainAsync().ConfigureAwait(false));
        }
    }
}
