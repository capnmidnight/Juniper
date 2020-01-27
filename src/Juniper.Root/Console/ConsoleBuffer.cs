using System;
using System.Text;

using System.Runtime.InteropServices;

using static System.Console;
using System.ComponentModel;

namespace Juniper.Console
{
    public sealed class ConsoleBuffer :
        IConsoleBuffer
    {
        private static readonly bool IsWindows
            = Environment.OSVersion.Platform == PlatformID.Win32NT
#if !NETSTANDARD && !NETCOREAPP
            || Environment.OSVersion.Platform == PlatformID.Win32S
            || Environment.OSVersion.Platform == PlatformID.Win32Windows
            || Environment.OSVersion.Platform == PlatformID.WinCE
#endif
            ;

        private static readonly IntPtr outputHandle = IsWindows
            ? NativeMethods.GetStdHandle(NativeMethods.STD_OUTPUT_HANDLE)
            : NativeMethods.INVALID_HANDLE_VALUE;

        private static readonly IntPtr inputHandle = IsWindows
            ? NativeMethods.GetStdHandle(NativeMethods.STD_INPUT_HANDLE)
            : NativeMethods.INVALID_HANDLE_VALUE;

        private readonly ConsoleColor startFore;
        private readonly ConsoleColor startBack;
        private ConsoleColor lastBack;
        private ConsoleColor lastFore;
        private ConsoleColor[,] back1;
        private ConsoleColor[,] back2;
        private ConsoleColor[,] fore1;
        private ConsoleColor[,] fore2;
        private char[,] grid1;
        private char[,] grid2;

        public int AbsoluteLeft => 0;
        public int AbsoluteRight => Width - 1;
        public int AbsoluteTop => 0;
        public int AbsoluteBottom => Height - 1;
        public int Width => grid1?.GetWidth() ?? -1;
        public int Height => grid1?.GetHeight() ?? -1;

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
                SetFontSize(gridSize, gridSize);
            }

            width = Math.Min(width, LargestWindowWidth);
            height = Math.Min(height, LargestWindowHeight - 1);

            OutputEncoding = Encoding.Unicode;

            lastFore = startFore = ForegroundColor;
            lastBack = startBack = BackgroundColor;

            SetWindowSize(width, height + 1);
            SetBufferSize(width, height + 1);
            SetWindowPosition(0, 0);

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
                return back1[x, y];
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
                var lastBack1 = back1;
                var lastBack2 = back2;
                var lastFore1 = fore1;
                var lastFore2 = fore2;
                var lastGrid1 = grid1;
                var lastGrid2 = grid2;

                back1 = new ConsoleColor[newWidth, newHeight];
                back2 = new ConsoleColor[newWidth, newHeight];
                fore1 = new ConsoleColor[newWidth, newHeight];
                fore2 = new ConsoleColor[newWidth, newHeight];
                grid1 = new char[newWidth, newHeight];
                grid2 = new char[newWidth, newHeight];

                Clear();
                CursorVisible = false;

                SizeChanged?.Invoke(this, new SizeChangedEventArgs(oldWidth, oldHeight, newWidth, newHeight));
            }
        }

        public void Draw(int x, int y, char c, ConsoleColor f, ConsoleColor b)
        {
            if (IsInBounds(x, y))
            {
                back1[x, y] = b;
                fore1[x, y] = f;
                grid1[x, y] = c;
            }
        }

        public void Flush()
        {
            for (var y = 0; y < Height; ++y)
            {
                for (var x = 0; x < Width; ++x)
                {
                    var changed = grid1[x, y] != grid2[x, y]
                        || fore1[x, y] != fore2[x, y]
                        || back1[x, y] != back2[x, y];

                    if (changed)
                    {
                        CheckCursor(x, y);

                        if (fore1[x, y] != lastFore)
                        {
                            lastFore = ForegroundColor = fore1[x, y];
                        }

                        if (back1[x, y] != lastBack)
                        {
                            lastBack = BackgroundColor = back1[x, y];
                        }

                        if (x < Width - 1)
                        {
                            Write(grid1[x, y]);
                        }
                        else
                        {
                            WriteLine(grid1[x, y]);
                        }
                    }

                    fore2[x, y] = fore1[x, y];
                    back2[x, y] = back1[x, y];
                    grid2[x, y] = grid1[x, y];
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

        public static bool IsKeyDown(int key)
        {
            if (!IsWindows)
            {
                return false;
            }

            return NativeMethods.GetKeyState(key) < 0;
        }

        public static (int x, int y) CursorPosition
        {
            get
            {
                if (!IsWindows
                    || !NativeMethods.GetCursorPos(out var point))
                {
                    return default;
                }

                return (point.X, point.Y);
            }
        }

        public static void SetFont(int index)
        {
            if (!IsWindows)
            {
                return;
            }

            var info = new NativeMethods.CONSOLE_FONT_INFO_EX();
            info.cbSize = (uint)Marshal.SizeOf(info);
            info.nFont = (uint)index;
            if (!NativeMethods.SetCurrentConsoleFontEx(outputHandle, false, info))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public static void SetFont(string fontFace)
        {
            if (!IsWindows)
            {
                return;
            }

            if (fontFace is null)
            {
                throw new ArgumentNullException(nameof(fontFace));
            }

            unsafe
            {
                var info = new NativeMethods.CONSOLE_FONT_INFO_EX();
                info.cbSize = (uint)Marshal.SizeOf(info);
                if (!NativeMethods.GetCurrentConsoleFontEx(outputHandle, false, ref info))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                var curFontFace = string.Intern(new string(info.FaceName));
                if (curFontFace != fontFace)
                {
                    var newInfo = new NativeMethods.CONSOLE_FONT_INFO_EX();
                    newInfo.cbSize = (uint)Marshal.SizeOf(newInfo);
                    newInfo.FontFamily = NativeMethods.TMPF_TRUETYPE;
                    var ptr = new IntPtr(newInfo.FaceName);
                    Marshal.Copy(fontFace.ToCharArray(), 0, ptr, fontFace.Length);
                    var size = Math.Max(info.dwFontSize.X, info.dwFontSize.Y);
                    newInfo.dwFontSize = new NativeMethods.COORD(size, size);
                    newInfo.FontWeight = info.FontWeight;
                    if (!NativeMethods.SetCurrentConsoleFontEx(outputHandle, false, newInfo))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
            }
        }

        public static void SetFontSize(int width, int height)
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

        private static bool IsConsoleModeEnabled(IntPtr handle, NativeMethods.CONSOLE_MODES desiredMode)
        {
            if (!IsWindows)
            {
                return false;
            }

            if (!NativeMethods.GetConsoleMode(handle, out var mode))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return (mode & desiredMode) != 0;
        }

        private static void SetConsoleModeEnabled(IntPtr handle, NativeMethods.CONSOLE_MODES addMode, bool enabled)
        {
            if (!IsWindows)
            {
                return;
            }

            if (!NativeMethods.GetConsoleMode(handle, out var curMode))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            var nextMode = curMode;
            if (enabled)
            {
                nextMode |= addMode;
            }
            else
            {
                nextMode &= ~addMode;
            }

            if (!NativeMethods.SetConsoleMode(outputHandle, nextMode))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public static bool EchoInputEnabled
        {
            get
            {
                return IsConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_ECHO_INPUT);
            }

            set
            {
                SetConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_ECHO_INPUT, value);
            }
        }

        public static bool InsertEnabled
        {
            get
            {
                return IsConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_INSERT_MODE);
            }

            set
            {
                SetConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_INSERT_MODE | NativeMethods.CONSOLE_MODES.ENABLE_EXTENDED_FLAGS, value);
            }
        }

        public static bool LineInputEnabled
        {
            get
            {
                return IsConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_LINE_INPUT);
            }

            set
            {
                SetConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_LINE_INPUT, value);
            }
        }

        public static bool MouseInputEnabled
        {
            get
            {
                return IsConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_MOUSE_INPUT);
            }

            set
            {
                SetConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_MOUSE_INPUT, value);
            }
        }

        public static bool ProcessedInputEnabled
        {
            get
            {
                return IsConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_PROCESSED_INPUT);
            }

            set
            {
                SetConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_PROCESSED_INPUT, value);
            }
        }

        public static bool ProcessedOutputEnabled
        {
            get
            {
                return IsConsoleModeEnabled(outputHandle, NativeMethods.CONSOLE_MODES.ENABLE_PROCESSED_OUTPUT);
            }

            set
            {
                SetConsoleModeEnabled(outputHandle, NativeMethods.CONSOLE_MODES.ENABLE_PROCESSED_OUTPUT, value);
            }
        }

        public static bool WrapAtEOLEnabled
        {
            get
            {
                return IsConsoleModeEnabled(outputHandle, NativeMethods.CONSOLE_MODES.ENABLE_WRAP_AT_EOL_OUTPUT);
            }
            set
            {
                SetConsoleModeEnabled(outputHandle, NativeMethods.CONSOLE_MODES.ENABLE_WRAP_AT_EOL_OUTPUT, value);
            }
        }

        public static bool QuickEditModeEnabled
        {
            get
            {
                return IsConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_QUICK_EDIT_MODE);
            }

            set
            {
                SetConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_QUICK_EDIT_MODE | NativeMethods.CONSOLE_MODES.ENABLE_EXTENDED_FLAGS, value);
            }
        }

        public static bool ExtendedModeEnabled =>
            IsConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_EXTENDED_FLAGS);

        public static bool WindowInputEnabled
        {
            get
            {
                return IsConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_WINDOW_INPUT);
            }

            set
            {
                SetConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_WINDOW_INPUT, value);
            }
        }

        public static bool VirtualTerminalInputEnabled
        {
            get
            {
                return IsConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_VIRTUAL_TERMINAL_INPUT);
            }

            set
            {
                SetConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_VIRTUAL_TERMINAL_INPUT, value);
            }
        }

        public static bool VirtualTerminalProcessingEnabled
        {
            get
            {
                return IsConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_VIRTUAL_TERMINAL_PROCESSING);
            }

            set
            {
                SetConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_VIRTUAL_TERMINAL_PROCESSING, value);
            }
        }

        public static bool NewLineAutoReturnEnabled
        {
            get
            {
                return !IsConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.DISABLE_NEWLINE_AUTO_RETURN);
            }

            set
            {
                SetConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.DISABLE_NEWLINE_AUTO_RETURN, !value);
            }
        }

        public static bool LVBGridWorldWideEnabled
        {
            get
            {
                return IsConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_LVB_GRID_WORLDWIDE);
            }

            set
            {
                SetConsoleModeEnabled(inputHandle, NativeMethods.CONSOLE_MODES.ENABLE_LVB_GRID_WORLDWIDE, value);
            }
        }

        internal static class NativeMethods
        {
            [DllImport("user32", CallingConvention = CallingConvention.Winapi)]
            internal static extern short GetKeyState(int key);

            [DllImport("user32", CallingConvention = CallingConvention.Winapi)]
            internal static extern bool GetCursorPos(out POINT point);

            [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
            internal static extern IntPtr GetStdHandle(int nStdHandle);

            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.Winapi)]
            internal static extern bool GetCurrentConsoleFontEx(
                IntPtr consoleOutput,
                bool maximumWindow,
                ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFontEx);

            [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
            internal static extern bool SetCurrentConsoleFontEx(
                IntPtr consoleOutput,
                bool maximumWindow,
                CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

            [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
            internal static extern bool GetConsoleMode(
                IntPtr console,
                out CONSOLE_MODES mode);

            [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
            internal static extern bool SetConsoleMode(
                IntPtr consoleOutput,
                CONSOLE_MODES mode);

            [Flags]
            internal enum CONSOLE_MODES
            {
                ENABLE_PROCESSED_INPUT = 0x0001,
                ENABLE_PROCESSED_OUTPUT = 0x0001,
                ENABLE_LINE_INPUT = 0x0002,
                ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
                ENABLE_ECHO_INPUT = 0x0004,
                ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
                ENABLE_WINDOW_INPUT = 0x0008,
                DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
                ENABLE_MOUSE_INPUT = 0x0010,
                ENABLE_LVB_GRID_WORLDWIDE = 0x0010,
                ENABLE_INSERT_MODE = 0x0020,
                ENABLE_QUICK_EDIT_MODE = 0x0040,
                ENABLE_EXTENDED_FLAGS = 0x0080,
                ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200
            }

            internal const int STD_INPUT_HANDLE = -10;
            internal const int STD_OUTPUT_HANDLE = -11;
            internal const int STD_ERROR_HANDLE = -12;
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
            internal struct POINT
            {
                internal int X;
                internal int Y;

                internal POINT(int x, int y)
                {
                    X = x;
                    Y = y;
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left, Top, Right, Bottom;

                public RECT(int left, int top, int right, int bottom)
                {
                    Left = left;
                    Top = top;
                    Right = right;
                    Bottom = bottom;
                }

                public int X
                {
                    get { return Left; }
                    set { Right -= (Left - value); Left = value; }
                }

                public int Y
                {
                    get { return Top; }
                    set { Bottom -= (Top - value); Top = value; }
                }

                public int Height
                {
                    get { return Bottom - Top; }
                    set { Bottom = value + Top; }
                }

                public int Width
                {
                    get { return Right - Left; }
                    set { Right = value + Left; }
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
