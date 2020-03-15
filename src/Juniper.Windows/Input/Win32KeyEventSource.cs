#if !NETCOREAPP

using System.Runtime.InteropServices;

using Keys = System.Windows.Forms.Keys;
using static System.Windows.Forms.Keys;
using Juniper.Terminal;

namespace Juniper.Input
{
    public sealed class Win32KeyEventSource :
        AbstractPollingKeyEventSource<Keys>
    {
        public static readonly bool IsAvailable = ConsoleBuffer.IsWindows;

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
            var baseKey = key & ~Modifiers;
            return RawKeyDown(baseKey)
                && Check(key, Shift, ShiftKey)
                && Check(key, Control, ControlKey);
        }

        private static class NativeMethods
        {
            [DllImport("user32", CallingConvention = CallingConvention.Winapi)]
            internal static extern short GetKeyState(Keys key);
        }
    }
}

#endif