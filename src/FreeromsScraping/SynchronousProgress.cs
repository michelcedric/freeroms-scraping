using System;

namespace FreeromsScraping
{
    public sealed class SynchronousProgress<T> : IProgress<T>
    {
        private readonly Action<T> _callback;

        public SynchronousProgress(Action<T> callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            _callback = callback;
        }

        void IProgress<T>.Report(T data) => _callback(data);
    }
}
