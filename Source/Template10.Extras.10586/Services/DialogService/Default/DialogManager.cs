using System;
using System.Threading;
using System.Threading.Tasks;

namespace Template10.Services.Dialog
{
    internal static class DialogManager
    {
        static SemaphoreSlim _oneAtATimeAsync = new SemaphoreSlim(1, 1);

        public static SemaphoreSlim OneAtATimeAsyncSemaphore { get => _oneAtATimeAsync; private set => _oneAtATimeAsync = value; }

        internal static async Task<T> OneAtATimeAsync<T>(Func<Task<T>> show, TimeSpan? timeout, CancellationToken? token)
        {
            var to = timeout ?? TimeSpan.FromHours(1);
            var tk = token ?? new CancellationToken(false);
            if (!await OneAtATimeAsyncSemaphore.WaitAsync(to, tk))
            {
                throw new Exception($"{nameof(DialogManager)}.{nameof(OneAtATimeAsync)} has timed out.");
            }
            try { return await show(); }
            finally { OneAtATimeAsyncSemaphore.Release(); }
        }
    }
}
