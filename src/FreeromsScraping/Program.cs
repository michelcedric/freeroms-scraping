using Nito.AsyncEx;
using System;
using System.Threading.Tasks;

namespace FreeromsScraping
{
    internal class Program
    {
        private static Task MainAsync()
        {
            throw new NotImplementedException();
        }

        private static void Main()
        {
            AsyncContext.Run(async () => await MainAsync().ConfigureAwait(false));
        }
    }
}
