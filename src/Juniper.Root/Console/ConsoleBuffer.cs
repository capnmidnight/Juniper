//Copyright, Sean T. McBeth, 2007
//sean.mcbeth@gmail.com
//www.seanmcbeth.com
using System;
using System.Runtime.InteropServices;
using System.Text;

using static System.Console;

namespace Juniper
{
    public class ConsoleBuffer
    {
        private ConsoleColor[,] back;
        private ConsoleColor[,] fore;
        private char[,] grid;
        private bool[,] old;

        public ConsoleBuffer(string fontFace = "Lucida Console")
        {
            if (fontFace is null)
            {
                throw new ArgumentNullException(nameof(fontFace));
            }

            CheckGrids();

            OutputEncoding = Encoding.Unicode;
            CursorVisible = false;

            unsafe
            {
                var hnd = GetStdHandle(STD_OUTPUT_HANDLE);
                if (hnd != INVALID_HANDLE_VALUE)
                {
                    var info = new CONSOLE_FONT_INFO_EX();
                    info.cbSize = (uint)Marshal.SizeOf(info);
                    if (GetCurrentConsoleFontEx(hnd, false, ref info))
                    {
                        var curFaceName = string.Intern(new string(info.FaceName));
                        if (curFaceName != fontFace)
                        {
                            var newInfo = new CONSOLE_FONT_INFO_EX();
                            newInfo.cbSize = (uint)Marshal.SizeOf(newInfo);
                            newInfo.FontFamily = TMPF_TRUETYPE;
                            var ptr = new IntPtr(newInfo.FaceName);
                            Marshal.Copy(fontFace.ToCharArray(), 0, ptr, fontFace.Length);
                            newInfo.dwFontSize = new COORD(info.dwFontSize.X, info.dwFontSize.Y);
                            newInfo.FontWeight = info.FontWeight;
                            _ = SetCurrentConsoleFontEx(hnd, false, newInfo);
                        }
                    }
                }
            }
        }

        public int Width => grid?.GetWidth() ?? -1;

        public int Height => grid?.GetHeight() ?? -1;

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

        public void Set(int x, int y, char c, ConsoleColor f, ConsoleColor b)
        {
            if (0 <= x && x < Width
                && 0 <= y && y < Height)
            {
                old[x, y] = b != back[x, y]
                    || f != fore[x, y]
                    || c != grid[x, y];
                back[x, y] = b;
                fore[x, y] = f;
                grid[x, y] = c;
            }
        }

        public void Set(int x, int y, char c, ConsoleColor f)
        {
            Set(x, y, c, f, BackgroundColor);
        }

        public void Set(int x, int y, string s, ConsoleColor f, ConsoleColor b)
        {
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            for (var dx = 0; dx < s.Length; ++dx)
            {
                Set(x + dx, y, s[dx], f, b);
            }
        }

        public void Set(int x, int y, string s, ConsoleColor f)
        {
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            for (var dx = 0; dx < s.Length; ++dx)
            {
                Set(x + dx, y, s[dx], f);
            }
        }

        public void SetWrap(int x, int y, string s, ConsoleColor f, ConsoleColor b)
        {
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            for (var dx = 0; dx < s.Length; ++dx)
            {
                var v = x + dx;
                Set(v % Width, y + (v / Width), s[dx], f, b);
            }
        }

        public void Flush()
        {
            for (var y = 0; y < Height; ++y)
            {
                for (var x = 0; x < Width; ++x)
                {
                    if (old[x, y])
                    {
                        if (back[x, y] != BackgroundColor)
                        {
                            BackgroundColor = back[x, y];
                        }

                        if (fore[x, y] != ForegroundColor)
                        {
                            ForegroundColor = fore[x, y];
                        }

                        CheckCursor(x, y);

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

        private void CheckCursor(int y, int x)
        {
            var isSameRow = y == CursorTop;
            var isNextRow = y == CursorTop + 1;
            var isNextCol = x == CursorLeft + 1;
            var isStartCol = x == 0;
            var advancedRight = isSameRow && isNextCol;
            var advancedNewLine = isNextRow && isStartCol;
            var cursorSkipped = !(advancedRight || advancedNewLine);
            if (cursorSkipped)
            {
                CursorLeft = x;
                CursorTop = y;
            }
        }

        public void Clear()
        {
            Clear(ConsoleColor.Black);
        }

        public void Clear(ConsoleColor b)
        {
            for (var x = 0; x < Width; ++x)
            {
                for (var y = 0; y < Height; ++y)
                {
                    Set(x, y, ' ', ConsoleColor.Gray, b);
                }
            }
        }

        public void Prompt(string message, bool pause)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Set(0, Height - 1, message, ConsoleColor.Red, ConsoleColor.DarkRed);
            Flush();

            CursorLeft = message.Length;
            CursorTop = Height - 1;

            if (pause)
            {
                _ = ReadKey(true);
            }
        }


        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool GetCurrentConsoleFontEx(
               IntPtr consoleOutput,
               bool maximumWindow,
               ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFontEx);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetCurrentConsoleFontEx(
               IntPtr consoleOutput,
               bool maximumWindow,
               CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

        private const int STD_OUTPUT_HANDLE = -11;
        private const int TMPF_TRUETYPE = 4;
        private const int LF_FACESIZE = 32;
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

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
