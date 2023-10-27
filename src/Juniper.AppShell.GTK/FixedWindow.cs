namespace Gtk;

public class FixedWindow : Window
{
    public FixedWindow(nint raw) : base(raw) { }
    public FixedWindow(string title) : base(title) { }
    public FixedWindow(WindowType type) : base(type) { }

    public void AddEvents(Gdk.EventMask mask)
    {
        AddEvents((int)mask);
    }
}