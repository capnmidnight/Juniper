﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace Juniper.AppShell;

public partial class WpfAppShell : Window, IAppShell
{
    public WpfAppShell()
    {
        InitializeComponent();
    }

    private Task<T> Do<T>(Func<T> action) =>
        Dispatcher.InvokeAsync(action).Task;

    private Task Do(Action action) =>
        Dispatcher.InvokeAsync(action).Task;

    public Task<Uri> GetSourceAsync() =>
        Do(() => WebView.Source);

    public Task SetSourceAsync(Uri value) =>
        Do(() =>
        {
            if (value == WebView.Source)
            {
                WebView.Reload();
            }
            else
            {
                WebView.Source = value;
            }
        });

    public Task<string> GetTitleAsync() =>
        Do(() => Title);

    public Task SetTitleAsync(string title) =>
        Do(() => Title = title);

    public Task CloseAsync()
    {
        try
        {
            Dispatcher.Invoke(Close);
        }
        catch(TaskCanceledException)
        {
            // do nothing
        }
        closing.TrySetResult();
        return Task.CompletedTask;
    }

    private readonly TaskCompletionSource closing = new();
    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        closing.TrySetResult();
    }

    public async Task WaitForCloseAsync()
    {
        await closing.Task;
    }

}