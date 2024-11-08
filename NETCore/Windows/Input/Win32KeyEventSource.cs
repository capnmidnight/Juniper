using System.Runtime.InteropServices;
using Keys = System.Windows.Forms.Keys;

namespace Juniper.Input;

public sealed class Win32KeyEventSource :
    AbstractPollingKeyEventSource<Keys>
{
    public static bool IsAvailable { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    public Win32KeyEventSource(CancellationToken token)
        : base(token)
    { }

    private static bool HasMod(Keys key, Keys mod)
    {
        return (key & mod) != 0;
    }

    private static bool RawKeyDown(Keys modKey)
    {
        return NativeMethods.GetKeyState(modKey) < 0;
    }

    private static bool Check(Keys key, Keys mod, Keys modKey)
    {
        return HasMod(key, mod) == RawKeyDown(modKey);
    }

    public override bool IsKeyDown(Keys key)
    {
        var baseKey = key & ~Keys.Modifiers;
        return RawKeyDown(baseKey)
            && Check(key, Keys.Shift, Keys.ShiftKey)
            && Check(key, Keys.Control, Keys.ControlKey);
    }

    private static class NativeMethods
    {
        [DllImport("user32", CallingConvention = CallingConvention.Winapi)]
        internal static extern short GetKeyState(Keys key);
    }
}