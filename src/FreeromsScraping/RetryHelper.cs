using System;
using System.Threading.Tasks;

namespace FreeromsScraping
{
    public static class RetryHelper
    {
        public static async Task ExecuteAndThrowAsync(Func<Task> funcAsync, Func<Exception, bool> shouldRetry, int retryCount = 3, int delay = 250, Action<Exception> onException = null)
        {
            if (funcAsync == null) throw new ArgumentNullException(nameof(funcAsync));
            if (retryCount < 1) throw new ArgumentOutOfRangeException(nameof(retryCount), "Retry should be at least 2");
            if (delay < 100) throw new ArgumentOutOfRangeException(nameof(delay), "Delay should be at least 100");

            for (var i = 0; i < retryCount; i++)
            {
                try
                {
                    await funcAsync().ConfigureAwait(false);
                    return;
                }
                catch (Exception e)
                {
                    onException?.Invoke(e);

                    if (shouldRetry(e))
                    {
                        Logger.Warning($"An exception has occured, retrying in {delay} ms.");
                        await Task.Delay(delay).ConfigureAwait(false);
                        delay *= 2;
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            throw new TimeoutException();
        }

        public static async Task<T> ExecuteAndThrowAsync<T>(Func<Task<T>> funcAsync, Func<Exception, bool> shouldRetry, int retryCount = 3, int delay = 250, Action<Exception> onException = null)
        {
            if (funcAsync == null) throw new ArgumentNullException(nameof(funcAsync));
            if (retryCount < 1) throw new ArgumentOutOfRangeException(nameof(retryCount), "Retry should be at least 2");
            if (delay < 100) throw new ArgumentOutOfRangeException(nameof(delay), "Delay should be at least 100");

            for (var i = 0; i < retryCount; i++)
            {
                try
                {
                    var result = await funcAsync().ConfigureAwait(false);

                    return result;
                }
                catch (Exception e)
                {
                    onException?.Invoke(e);

                    if (shouldRetry(e))
                    {
                        Logger.Warning($"An exception has occured, retrying in {delay} ms.");
                        await Task.Delay(delay).ConfigureAwait(false);
                        delay *= 2;
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            throw new TimeoutException();
        }
    }
}