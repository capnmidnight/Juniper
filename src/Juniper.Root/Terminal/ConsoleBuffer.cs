using System;
using System.Text;

using Juniper.Puzzles;

using static System.Console;

namespace Juniper.Terminal
{

    public sealed class ConsoleBuffer :
        IConsoleBuffer
    {
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

        public ConsoleBuffer()
            : this(WindowWidth, WindowHeight - 1)
        { }

        public ConsoleBuffer(int width, int height)
        {
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
    }
}
