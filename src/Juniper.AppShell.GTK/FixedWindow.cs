using Gdk;

namespace Gtk;

public class FixedWindow : Window
{
    private readonly HashSet<Gdk.Key> isPressed = new();
    private readonly HashSet<Gdk.Key> wasPressed = new();

    private bool isIconified = false;
    private bool isFullscreen = false;

    public FixedWindow(nint raw) : base(raw) { Init(); }
    public FixedWindow(string title) : base(title) { Init(); }
    public FixedWindow(WindowType type) : base(type) { Init(); }

    private void Init()
    {
        AddEvents(
            EventMask.KeyReleaseMask
            | EventMask.KeyPressMask
        );

        WindowStateEvent += delegate (object? sender, WindowStateEventArgs e)
        {
            isIconified = (e.Event.NewWindowState & WindowState.Iconified) != 0;
            isFullscreen = (e.Event.NewWindowState & WindowState.Fullscreen) != 0;
        };
    }

    protected override bool OnKeyPressEvent(EventKey evnt)
    {
        isPressed.Add(evnt.Key);
        return false;
    }

    protected override bool OnKeyReleaseEvent(EventKey evnt)
    {
        wasPressed.Clear();
        foreach (var key in isPressed)
        {
            wasPressed.Add(key);
        }

        isPressed.Remove(evnt.Key);
        return false;
    }

    private static bool KeyPressedCheck(HashSet<Gdk.Key> keyState, Gdk.Key[] keys)
    {
        foreach (var key in keys)
        {
            if (!keyState.Contains(key))
            {
                return false;
            }
        }

        return true;
    }

    public bool IsPressed(params Gdk.Key[] keys) =>
        KeyPressedCheck(isPressed, keys);

    public bool WasPressed(params Gdk.Key[] keys) =>
        KeyPressedCheck(wasPressed, keys);


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
