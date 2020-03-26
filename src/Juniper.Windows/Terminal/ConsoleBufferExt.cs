using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Juniper.Terminal
{
    public static class ConsoleBufferExt
    {
        public static void SetFontSize(this ConsoleBuffer buffer, int size)
        {
            buffer.SetFontSize(size, size);
        }

        public static void SetFontSize(this ConsoleBuffer buffer, int width, int height)
        {
            if (!ConsoleBuffer.IsWindows)
            {
                return;
            }

            var info = new NativeMethods.CONSOLE_FONT_INFO_EX();
            info.cbSize = (uint)Marshal.SizeOf(info);
            info.dwFontSize.X = (short)width;
            info.dwFontSize.Y = (short)height;
            var handle = NativeMethods.GetStdHandle(NativeMethods.STD_OUTPUT_HANDLE);
            if (handle != NativeMethods.INVALID_HANDLE_VALUE
                && !NativeMethods.SetCurrentConsoleFontEx(handle, false, info))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        internal static class NativeMethods
        {

            [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
            internal static extern IntPtr GetStdHandle(int nStdHandle);

            [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
            internal static extern bool SetCurrentConsoleFontEx(
                IntPtr consoleOutput,
                bool maximumWindow,
                CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

            internal const int STD_OUTPUT_HANDLE = -11;
            internal const int LF_FACESIZE = 32;
            internal static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

            [StructLayout(LayoutKind.Sequential)]
            internal struct COORD
            {
                internal short X;
                internal short Y;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal unsafe struct CONSOLE_FONT_INFO_EX
            {
                internal uint cbSize;
                internal uint nFont;
                internal COORD dwFontSize;
                internal int FontFamily;
                internal int FontWeight;
                internal fixed char FaceName[LF_FACESIZE];
            }
        }
    }
}