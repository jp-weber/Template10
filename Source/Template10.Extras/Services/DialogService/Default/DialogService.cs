using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Template10.Services.Dialog
{
    public class DialogService : IDialogService
    {
        private static CancellationTokenSource _tokenSource;
        public async Task<MessageBoxResult> AlertAsync(string content, IDialogResourceResolver resolver = null)
            => await AlertAsync(string.Empty, content, resolver);

        public async Task<MessageBoxResult> AlertAsync(string title, string content, IDialogResourceResolver resolver = null)
            => await new MessageBoxEx(title, content, MessageBoxType.Ok, resolver).ShowAsync();

        public async void CancelDialogs()
        {
            if (await IsDialogRunning())
            {
                if (_tokenSource != null)
                {
                    _tokenSource.Cancel();
                    _tokenSource = new CancellationTokenSource();
                }
            }
        }

        public async Task<bool> IsDialogRunning()
        {
            return await DialogManager.IsDialogRunning();
        }

        public async Task<MessageBoxResult> PromptAsync(string content, MessageBoxType type = MessageBoxType.YesNo, IDialogResourceResolver resolver = null)
            => await PromptAsync(string.Empty, content, type, resolver);

        public async Task<MessageBoxResult> PromptAsync(string title, string content, MessageBoxType type = MessageBoxType.YesNo, IDialogResourceResolver resolver = null)
            => await new MessageBoxEx(title, content, type, resolver).ShowAsync();

        public async Task<bool> PromptAsync(string content, MessageBoxType type, MessageBoxResult expected, IDialogResourceResolver resolver = null)
            => await PromptAsync(string.Empty, content, type, expected, resolver);

        public async Task<bool> PromptAsync(string title, string content, MessageBoxType type, MessageBoxResult expected, IDialogResourceResolver resolver = null)
            => (await PromptAsync(title, content, type, resolver)).Equals(expected);

        public async Task<ContentDialogResult> ShowAsync(ContentDialog dialog, TimeSpan? timeout = null, CancellationToken? token = null)
        {
            var tk = token ?? new CancellationToken(false);
            if (_tokenSource is null)
            {
                _tokenSource = new CancellationTokenSource();
            }
            if (token is null)
            {
                return await DialogManager.OneAtATimeAsync(async () => await dialog.ShowAsync(_tokenSource.Token), timeout, _tokenSource.Token);

            }
            else
            {
                return await DialogManager.OneAtATimeAsync(async () => await dialog.ShowAsync(_tokenSource.Token), timeout, tk);
            }
        }

        public async Task<IUICommand> ShowAsync(MessageDialog dialog, TimeSpan? timeout = null, CancellationToken? token = null)
        {
            return await DialogManager.OneAtATimeAsync(async () => await dialog.ShowAsync(), timeout, token);
        }
    }
}
