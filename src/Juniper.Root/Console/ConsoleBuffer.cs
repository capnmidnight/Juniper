using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

using Juniper.Puzzles;

using static System.Console;

namespace Juniper.Console
{

    public sealed class ConsoleBuffer :
        IConsoleBuffer
    {
        public static readonly bool IsWindows
            = Environment.OSVersion.Platform == PlatformID.Win32NT
#if !NETSTANDARD && !NETCOREAPP
            || Environment.OSVersion.Platform == PlatformID.Win32S
            || Environment.OSVersion.Platform == PlatformID.Win32Windows
            || Environment.OSVersion.Platform == PlatformID.WinCE
#endif
            ;

        private readonly IntPtr outputHandle;

        private readonly ConsoleColor startFore;
        private readonly ConsoleColor startBack;
        private ConsoleColor lastBack;
        private ConsoleColor lastFore;
        private Tile[,] tiles1;
        private Tile[,] tiles2;

        public int AbsoluteLeft => 0;
        public int AbsoluteRight => Width - 1;
        public int AbsoluteTop => 0;
        public int AbsoluteBottom => Height - 1;
        public int Width => tiles1?.GetWidth() ?? -1;
        public int Height => tiles1?.GetHeight() ?? -1;

        public event EventHandler<SizeChangedEventArgs> SizeChanged;

        public ConsoleBuffer(int gridSize)
            : this(gridSize, WindowWidth, WindowHeight - 1)
        { }

        internal ConsoleBuffer(int gridSize, NativeMethods.COORD size)
            : this(gridSize, size.X, size.Y)
        { }

        public ConsoleBuffer(int gridSize, int width, int height)
        {
            if (IsWindows)
            {
                outputHandle = IsWindows
                   ? NativeMethods.GetStdHandle(NativeMethods.STD_OUTPUT_HANDLE)
                   : NativeMethods.INVALID_HANDLE_VALUE;
                SetFontSize(gridSize, gridSize);
            }

            width = Math.Min(width, LargestWindowWidth);
            height = Math.Min(height, LargestWindowHeight - 1);

            OutputEncoding = Encoding.Unicode;

            lastFore = startFore = ForegroundColor;
            lastBack = startBack = BackgroundColor;

            SetWindowSize(width, height + 1);
            SetBufferSize(width, height + 1);

            CheckGrids();
        }

        private void ConsoleBuffer_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Teardown();
            e.Cancel = true;
        }

        ~ConsoleBuffer()
        {
            Teardown();
        }

        private void Teardown()
        {
            ForegroundColor = startFore;
            BackgroundColor = startBack;
        }

        /// <summary>
        /// Determine if a point is within the bounds of the board
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Width
                && y >= 0 && y < Height;
        }

        public ConsoleColor GetBackgroundColor(int x, int y)
        {
            if (IsInBounds(x, y))
            {
                return tiles1[x, y].Back;
            }
            else
            {
                return ConsoleColor.Black;
            }
        }

        private void CheckGrids()
        {
            var oldWidth = Width;
            var oldHeight = Height;
            var newWidth = WindowWidth;
            var newHeight = WindowHeight - 1;
            if (newWidth != oldWidth
                || newHeight != oldHeight)
            {
                var lastTiles1 = tiles1;
                var lastTiles2 = tiles2;

                tiles1 = new Tile[newWidth, newHeight];
                tiles2 = new Tile[newWidth, newHeight];

                Clear();
                CursorVisible = false;

                SizeChanged?.Invoke(this, new SizeChangedEventArgs(oldWidth, oldHeight, newWidth, newHeight));
            }
        }

        public void Draw(int x, int y, char c, ConsoleColor f, ConsoleColor b)
        {
            if (IsInBounds(x, y))
            {
                tiles1[x, y].Back = b;
                tiles1[x, y].Fore = f;
                tiles1[x, y].Token = c;
            }
        }

        public void Flush()
        {
            for (var y = 0; y < Height; ++y)
            {
                for (var x = 0; x < Width; ++x)
                {
                    var t1 = tiles1[x, y];

                    if (!t1.Equals(tiles2[x, y]))
                    {
                        CheckCursor(x, y);

                        if (t1.Fore != lastFore)
                        {
                            lastFore = ForegroundColor = t1.Fore;
                        }

                        if (t1.Back != lastBack)
                        {
                            lastBack = BackgroundColor = t1.Back;
                        }

                        if (x < Width - 1)
                        {
                            Write(t1.Token);
                        }
                        else
                        {
                            WriteLine(t1.Token);
                        }
                    }

                    tiles2[x, y] = t1;
                }
            }

            CheckGrids();
        }

        private static void CheckCursor(int x, int y)
        {
            var rowChanged = y != CursorTop;
            var colChanged = x != CursorLeft;
            if (rowChanged && colChanged)
            {
                SetCursorPosition(x, y);
            }
            else if (colChanged)
            {
                CursorLeft = x;
            }
            else if (rowChanged)
            {
                CursorTop = y;
            }
        }

        public void Prompt(string message, bool pause)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.Draw(0, Height - 1, message, ConsoleColor.Red, ConsoleColor.DarkRed);
            Flush();

            CursorLeft = message.Length;
            CursorTop = Height - 1;

            if (pause)
            {
                _ = ReadKey(true);
            }
        }

        public static (int x, int y) GetCursorPosition()
        {
            if (!IsWindows)
            {
                return default;
            }

            if (!NativeMethods.GetCursorPos(out var point))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return (point.X, point.Y);
        }

        public void SetFontSize(int width, int height)
        {
            if (!IsWindows)
            {
                return;
            }

            var info = new NativeMethods.CONSOLE_FONT_INFO_EX();
            info.cbSize = (uint)Marshal.SizeOf(info);
            info.dwFontSize.X = (short)width;
            info.dwFontSize.Y = (short)height;
            if (!NativeMethods.SetCurrentConsoleFontEx(outputHandle, false, info))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        internal static class NativeMethods
        {
            [DllImport("user32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
            internal static extern bool GetCursorPos(out POINT point);

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
            internal struct POINT
            {
                internal int X;
                internal int Y;
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
