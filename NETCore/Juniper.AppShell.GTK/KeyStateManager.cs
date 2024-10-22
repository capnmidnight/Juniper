using Gdk;

namespace Gtk;

public class KeyStateManager
{
    private readonly HashSet<Gdk.Key> isPressed = new();
    private readonly HashSet<Gdk.Key> wasPressed = new();

    public void OnKeyPressEvent(EventKey evnt) =>
        isPressed.Add(evnt.Key);

    public void OnKeyReleaseEvent(EventKey evnt)
    {
        wasPressed.Clear();
        foreach (var key in isPressed)
        {
            wasPressed.Add(key);
        }

        isPressed.Remove(evnt.Key);
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
}
