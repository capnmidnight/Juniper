#if !WINDOWS

using System.ComponentModel;

namespace Juniper.AppShell;

public class Fake
{
    protected static void DIE(Fake inst) => throw KILLER;
    protected static T DIE<T>(Fake inst) => throw KILLER;
    protected static Exception KILLER => new PlatformNotSupportedException("WPF is only supported on Windows");
    protected static Task HIT(Fake inst) => throw KILLER;
}

public class FakeWebView : Fake
{
#pragma warning disable CS0067 // The event 'FakeWebView.ZoomFactorChanged' is never used
    public event Action? ZoomFactorChanged;
#pragma warning restore CS0067 // The event 'FakeWebView.ZoomFactorChanged' is never used
    public double ZoomFactor { get; set; }
    public Task EnsureCoreWebView2Async() => HIT(this);
    public Uri Source { get => DIE<Uri>(this); set => DIE(this); }
    public void Reload() => DIE(this);
    public void NavigateToString(string html) => DIE(this);
    public bool CanGoBack { get => DIE<bool>(this); set => DIE(this); }
    public bool CanGoForward { get => DIE<bool>(this); set => DIE(this); }
}

public class FakeDispatcherOperation<TResult> : Fake
{
    public Task<TResult> Task { get; set; } = System.Threading.Tasks.Task.FromException<TResult>(KILLER);
}

public class FakeDispatcherOperation : Fake
{
    public Task Task { get; set; } = Task.FromException(KILLER);
}

public class FakeDispatcher : Fake
{
    public FakeDispatcherOperation<TResult> InvokeAsync<TResult>(Func<TResult> callback) => DIE<FakeDispatcherOperation<TResult>>(this);
    public FakeDispatcherOperation InvokeAsync(Action callback) => DIE<FakeDispatcherOperation>(this);
    public void Invoke(Action callback) => DIE(this);
}

public enum WindowStyle
{
    None,
    SingleBorderWindow
}

public enum WindowState
{
    Minimized,
    Maximized,
    Normal
}

public class FakeWindow : Fake
{
    protected void InitializeComponent() => DIE(this);
    protected FakeWebView WebView => DIE<FakeWebView>(this);
    protected FakeDispatcher Dispatcher => DIE<FakeDispatcher>(this);
    protected void Show() => DIE(this);
    protected void Hide() => DIE(this);
    protected void Close() => DIE(this);
    protected virtual void OnClosing(CancelEventArgs e) => DIE(this);
    public string Title { get => DIE<string>(this); set => DIE(this); }
    public double Width { get => DIE<double>(this); set => DIE(this); }
    public double Height { get => DIE<double>(this); set => DIE(this); }
    public bool Topmost { get => DIE<bool>(this); set => DIE(this); }
    public WindowStyle WindowStyle { get => DIE<WindowStyle>(this); set => DIE(this); }
    public WindowState WindowState { get => DIE<WindowState>(this); set => DIE(this); }
}

public class Window : FakeWindow { }

#endif
