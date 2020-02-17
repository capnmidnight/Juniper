using System.Runtime.InteropServices;

using Juniper.Console;

namespace Juniper.Input
{
    public sealed class Win32KeyEventSource :
        AbstractPollingKeyEventSource<int>
    {
        public static readonly bool IsAvailable = ConsoleBuffer.IsWindows;

        protected override bool IsKeyDown(int key)
        {
            return NativeMethods.GetKeyState(key) < 0;
        }

        private static class NativeMethods
        {
            [DllImport("user32", CallingConvention = CallingConvention.Winapi)]
            internal static extern short GetKeyState(int key);
        }
    }
}
