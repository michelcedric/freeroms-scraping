using Fizzler.Systems.HtmlAgilityPack;
using FreeromsScraping.Configuration;
using FreeromsScraping.IO;
using HtmlAgilityPack;
using Nito.AsyncEx;
using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FreeromsScraping
{
    internal class Program
    {
        private static ScrapingSection _configuration;

        private static ScrapingSection Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = (ScrapingSection)ConfigurationManager.GetSection("scraping");
                }
                return _configuration;
            }
        }

        private static async Task MainAsync()
        {
            foreach (var source in Configuration.Sources.Cast<SourceElement>())
            {
                Logger.Info($"Downloading catalog from source {source.Name}...");
                await DownloadCatalogAsync(source.Name, source.Url, Configuration.DestinationFolder);
            }

            Logger.Info("End");
            Console.ReadKey();
        }

        private static async Task DownloadCatalogAsync(string name, string url, string destinationFolder)
        {
            var menuPage = await RetryHelper.ExecuteAndThrowAsync(() => GetContentAsStringAsync(url), e => true);
            if (String.IsNullOrWhiteSpace(menuPage))
            {
                return;
            }

            foreach (var catalogLink in ParseContentForMenuLink(menuPage))
            {
                var listPage = await RetryHelper.ExecuteAndThrowAsync(() => GetContentAsStringAsync(catalogLink), e => true);
                if (String.IsNullOrWhiteSpace(listPage))
                {
                    continue;
                }

                var links = ParseContentForRomLink(listPage);
                await links.ParallelForEachAsync(async romLink =>
                {
                    var romPage = await RetryHelper.ExecuteAndThrowAsync(() => GetContentAsStringAsync(romLink), e => true);
                    if (!String.IsNullOrWhiteSpace(romPage))
                    {
                        var fileLink = ParseContentForFileLink(romPage);
                        if (!String.IsNullOrWhiteSpace(fileLink))
                        {
                            var folder = Path.Combine(destinationFolder, name);
                            if (!Directory.Exists(folder))
                            {
                                Directory.CreateDirectory(folder);
                            }

                            var fileName = Path.GetFileName(fileLink);
                            var path = Path.Combine(folder, fileName);
                            if (!File.Exists(path))
                            {

                                await RetryHelper.ExecuteAndThrowAsync(() => SaveContentAsync(fileLink, path), e => true);
                            }
                            else
                            {
                                Logger.Info("--> File already exists, skipping.");
                            }
                        }
                    }
                }, maxDegreeOfParalellism: Configuration.ClientNumber);

            }
        }

        private static string ParseContentForFileLink(string html)
        {
            var regex = new Regex(@"document\.getElementById\(""romss""\)\.innerHTML='&nbsp;<a href=""(?<link>http:\/\/(?:\w|\d)+\.freeroms\.com\/(?:\/|\w|\.|-|,|!|\(|\)|\+|\[|\]|%)+)"">Direct&nbsp;Download<\/a>&nbsp;';", RegexOptions.Compiled);
            var match = regex.Match(html);

            if (!match.Success)
            {
                Logger.Error("No link to rom found in html !");
                return null;
            }

            return match.Groups["link"].Value;
        }

        private static IEnumerable<string> ParseContentForRomLink(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            foreach (var node in htmlDoc.DocumentNode.QuerySelectorAll("a[href*='rom_download.php']"))
            {
                yield return node.Attributes["href"].Value;
            }
        }

        private static async Task SaveContentAsync(string url, string path, bool extracFile = false)
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        Logger.Error($"Error while fetching {url} !");
                        return;
                    }

                    var fileSize = response.Content.Headers.ContentLength;
                    var left = Console.CursorLeft;
                    var top = Console.CursorTop;
                    Console.Write($"Downloading file {url}... ");

                    using (var stream = await RetryHelper.ExecuteAndThrowAsync(() => response.Content.ReadAsStreamAsync(), e => true))
                    {
                        using (var destination = new FileStream(path, FileMode.Create))
                        {
                            var sw = new Stopwatch();
                            var progress = new SynchronousProgress<long>(value =>
                            {
                                Console.CursorLeft = left;
                                Console.CursorTop = top;
                                var speed = value / sw.Elapsed.TotalSeconds / 1024;

                                if (fileSize.HasValue)
                                {
                                    var pct = (decimal)(value * 100) / fileSize;
                                    Console.WriteLine($"{pct:0.00} % @ {speed:0.00} kb/s ");
                                }
                                else
                                {
                                    Console.Write($"{value} bytes @ {speed:0.00} kb/s ");
                                }
                            });

                            sw.Start();
                            Console.CursorVisible = false;
                            await stream.CopyToAsync(destination, progress);

                            if (extracFile)
                            {
                                ZipArchive zipFile = new ZipArchive(destination);
                                foreach (ZipArchiveEntry file in zipFile.Entries)
                                {
                                    file.ExtractToFile(Path.Combine(Path.GetDirectoryName(path), file.FullName), true);
                                }
                            }


                            Console.CursorVisible = true;
                            Console.WriteLine();
                            sw.Stop();
                        }
                    }

                    if (extracFile)
                    {
                        File.Delete(path);
                    }

                }

            }
        }

        private static async Task<string> GetContentAsStringAsync(string url)
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(url))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        Logger.Error($"Error while fetching {url} !");
                        return null;
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    return content;
                }
            }
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
            AsyncContext.Run(async () => await MainAsync());
        }
    }
}
