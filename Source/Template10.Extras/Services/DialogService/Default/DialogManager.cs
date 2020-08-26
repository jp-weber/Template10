using System;
using System.Threading;
using System.Threading.Tasks;

namespace Template10.Services.Dialog
{
    internal static class DialogManager
    {
        static SemaphoreSlim _oneAtATimeAsync = new SemaphoreSlim(1, 1);

        internal static Task<bool> IsDialogRunning()
        {
            return Task.FromResult(_oneAtATimeAsync.CurrentCount < 1);
        }

        internal static async Task<T> OneAtATimeAsync<T>(Func<Task<T>> show, TimeSpan? timeout, CancellationToken? token)
        {
            var to = timeout ?? TimeSpan.FromHours(1);
            var tk = token ?? new CancellationToken(false);
            if (!await _oneAtATimeAsync.WaitAsync(to, tk))
            {
                throw new Exception($"{nameof(DialogManager)}.{nameof(OneAtATimeAsync)} has timed out.");
            }
            try { return await show(); }
            finally { _oneAtATimeAsync.Release(); }
        }

        internal static async Task<T> OneAtATimeAsync<T>(Func<Task<T>> show, TimeSpan? timeout, CancellationToken token)
        {
            var to = timeout ?? TimeSpan.FromHours(1);
            if (!await _oneAtATimeAsync.WaitAsync(to, token))
            {
                throw new Exception($"{nameof(DialogManager)}.{nameof(OneAtATimeAsync)} has timed out.");
            }
            try { return await show().WaitAsync(token); }
            finally { _oneAtATimeAsync.Release(); }
        }

        public static async Task<T> WaitAsync<T>(this Task<T> task, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<T>();
            using (token.Register(() => tcs.TrySetCanceled(token), useSynchronizationContext: false))
                return await await Task.WhenAny(task, tcs.Task).ConfigureAwait(false);
        }
    }
}
