using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FreeromsScraping.IO
{
    public static class Extensions
    {
        // From stream.cs:
        // We pick a value that is the largest multiple of 4096 that is still smaller than the large object heap threshold (85K).
        // The CopyTo/CopyToAsync buffer is short-lived and is likely to be collected at Gen0, and it offers a significant
        // improvement in Copy performance.
        public static Task CopyToAsync(this Stream source, Stream destination, IProgress<long> progress, int bufferSize = 81920)
        {
            return source.CopyToAsync(destination, progress, CancellationToken.None, bufferSize);
        }

        public static async Task CopyToAsync(this Stream source, Stream destination, IProgress<long> progress, CancellationToken cancellationToken, int bufferSize = 81920)
        {
            int bytesRead;
            var buffer = new byte[bufferSize];
            long totalRead = 0;

            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                totalRead += bytesRead;
                progress.Report(totalRead);
            }
        }
    }
}
