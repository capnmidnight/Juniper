using System;
using System.Runtime.InteropServices;
using System.Text;

using static System.Console;

namespace Juniper.Console
{
    public class ConsoleBuffer : IConsoleBuffer
    {
        private ConsoleColor[,] back;
        private ConsoleColor[,] fore;
        private char[,] grid;
        private bool[,] old;

        public ConsoleBuffer(int width, int height)
        {
            BufferWidth = WindowWidth = width;
            BufferHeight = WindowHeight = height + 1;

            CheckGrids();

            OutputEncoding = Encoding.Unicode;
            CursorVisible = false;
        }

        public ConsoleBuffer()
            : this(WindowWidth, WindowHeight)
        { }

        public int AbsoluteLeft => 0;
        public int AbsoluteRight => Width - 1;
        public int AbsoluteTop => 0;
        public int AbsoluteBottom => Height - 1;
        public int Width => grid?.GetWidth() ?? -1;
        public int Height => grid?.GetHeight() ?? -1;

        public ConsoleColor GetBackgroundColor(int x, int y)
        {
            return back[x, y];
        }

        private void CheckGrids()
        {
            var newWidth = WindowWidth;
            var newHeight = WindowHeight - 1;
            if (newWidth != Width
                || newHeight != Height)
            {
                var lastBack = back;
                var lastFore = fore;
                var lastGrid = grid;
                var lastOld = old;

                back = new ConsoleColor[newWidth, newHeight];
                fore = new ConsoleColor[newWidth, newHeight];
                grid = new char[newWidth, newHeight];
                old = new bool[newWidth, newHeight];

                for (var x = 0; x < newWidth; ++x)
                {
                    for (var y = 0; y < newHeight; ++y)
                    {
                        if (x < lastBack?.GetWidth()
                            && y < lastBack?.GetHeight())
                        {
                            back[x, y] = lastBack[x, y];
                            fore[x, y] = lastFore[x, y];
                            grid[x, y] = lastGrid[x, y];
                            old[x, y] = lastOld[x, y];
                        }
                        else
                        {
                            back[x, y] = ConsoleColor.Black;
                            fore[x, y] = ConsoleColor.Gray;
                            grid[x, y] = ' ';
                            old[x, y] = true;
                        }
                    }
                }
            }
        }

        public void Draw(int x, int y, char c, ConsoleColor f, ConsoleColor b)
        {
            old[x, y] = b != back[x, y]
                || f != fore[x, y]
                || c != grid[x, y];
            back[x, y] = b;
            fore[x, y] = f;
            grid[x, y] = c;
        }

        public void Flush()
        {
            for (var y = 0; y < Height; ++y)
            {
                for (var x = 0; x < Width; ++x)
                {
                    if (old[x, y])
                    {
                        CheckCursor(x, y);

                        BackgroundColor = back[x, y];
                        ForegroundColor = fore[x, y];

                        if (x < Width - 1)
                        {
                            Write(grid[x, y]);
                        }
                        else
                        {
                            WriteLine(grid[x, y]);
                        }

                        old[x, y] = false;
                    }
                }
            }

            CheckGrids();
        }

        private void CheckCursor(int x, int y)
        {
            var rowChanged = y != CursorTop;
            var colChanged = x != CursorLeft;
            if (rowChanged || colChanged)
            {
                CursorLeft = x;
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

        public static unsafe bool SetFont(string fontFace)
        {
            if (fontFace is null)
            {
                throw new ArgumentNullException(nameof(fontFace));
            }

            var hnd = NativeMethods.GetStdHandle(NativeMethods.STD_OUTPUT_HANDLE);
            if (hnd != NativeMethods.INVALID_HANDLE_VALUE)
            {
                var info = new NativeMethods.CONSOLE_FONT_INFO_EX();
                info.cbSize = (uint)Marshal.SizeOf(info);
                if (NativeMethods.GetCurrentConsoleFontEx(hnd, false, ref info))
                {
                    var curFontFace = string.Intern(new string(info.FaceName));
                    if (curFontFace != fontFace)
                    {
                        var newInfo = new NativeMethods.CONSOLE_FONT_INFO_EX();
                        newInfo.cbSize = (uint)Marshal.SizeOf(newInfo);
                        newInfo.FontFamily = NativeMethods.TMPF_TRUETYPE;
                        var ptr = new IntPtr(newInfo.FaceName);
                        Marshal.Copy(fontFace.ToCharArray(), 0, ptr, fontFace.Length);
                        newInfo.dwFontSize = new NativeMethods.COORD(info.dwFontSize.X, info.dwFontSize.Y);
                        newInfo.FontWeight = info.FontWeight;
                        return NativeMethods.SetCurrentConsoleFontEx(hnd, false, newInfo);
                    }
                }
            }

            return false;
        }

        internal static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern IntPtr GetStdHandle(int nStdHandle);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern bool GetCurrentConsoleFontEx(
                   IntPtr consoleOutput,
                   bool maximumWindow,
                   ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFontEx);

            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern bool SetCurrentConsoleFontEx(
                   IntPtr consoleOutput,
                   bool maximumWindow,
                   CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

            internal const int STD_OUTPUT_HANDLE = -11;
            internal const int TMPF_TRUETYPE = 4;
            internal const int LF_FACESIZE = 32;
            internal static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

            [StructLayout(LayoutKind.Sequential)]
            internal struct COORD
            {
                internal short X;
                internal short Y;

                internal COORD(short x, short y)
                {
                    X = x;
                    Y = y;
                }
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
