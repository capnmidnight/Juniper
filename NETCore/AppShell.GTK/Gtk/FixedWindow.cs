using Gdk;

namespace Gtk;

public class FixedWindow : Window
{

    private bool isIconified = false;
    private bool isFullscreen = false;

    public FixedWindow(nint raw) : base(raw) { Init(); }
    public FixedWindow(string title) : base(title) { Init(); }
    public FixedWindow(WindowType type) : base(type) { Init(); }

    private void Init()
    {
        WindowStateEvent += delegate (object? sender, WindowStateEventArgs e)
        {
            isIconified = (e.Event.NewWindowState & WindowState.Iconified) != 0;
            isFullscreen = (e.Event.NewWindowState & WindowState.Fullscreen) != 0;
        };
    }

    public new bool IsMaximized
    {
        get
        {
            return base.IsMaximized;
        }
        set
        {
            if (IsMaximized != value)
            {
                if (value)
                {
                    Maximize();
                }
                else
                {
                    Unmaximize();
                }
            }
        }
    }

    public bool IsFullscreen
    {
        get
        {
            return isFullscreen;
        }
        set
        {
            if (isFullscreen != value)
            {
                if (value)
                {
                    Fullscreen();
                }
                else
                {
                    Unfullscreen();
                }
            }
        }
    }

    public bool IsIconified
    {
        get
        {
            return isIconified;
        }
        set
        {
            if (isIconified != value)
            {
                if (value)
                {
                    Iconify();
                }
                else
                {
                    Deiconify();
                }
            }
        }
    }

    public void AddEvents(EventMask mask) =>
        AddEvents((int)mask);
}
