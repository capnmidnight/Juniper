using System;
using Juniper.Puzzles;

namespace Juniper.Console
{
    public interface IConsoleBuffer
    {
        int AbsoluteLeft { get; }
        int AbsoluteRight { get; }
        int AbsoluteTop { get; }
        int AbsoluteBottom { get; }
        int Height { get; }
        int Width { get; }
        void Draw(int x, int y, char c, ConsoleColor f, ConsoleColor b);
        ConsoleColor GetBackgroundColor(int x, int y);
    }

    public static class IConsoleBufferExt
    {
        public static IConsoleBuffer Window(this IConsoleBuffer buffer, int x, int y, int height, int width)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return new ConsoleBufferSubdivision(buffer, x, y, width, height);
        }

        public static void Clear(this IConsoleBuffer buffer)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            buffer.Fill(ConsoleColor.Black);
        }

        public static void Fill(this IConsoleBuffer buffer, ConsoleColor b)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            buffer.Fill(0, 0, buffer.Width, buffer.Height, b);
        }

        public static void Fill(this IConsoleBuffer buffer, int x, int y, int width, int height, ConsoleColor b)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            for (var dx = 0; dx < width; ++dx)
            {
                for (var dy = 0; dy < height; ++dy)
                {
                    buffer.Draw(x + dx, y + dy, ' ', ConsoleColor.Gray, b);
                }
            }
        }

        public static void Stroke(this IConsoleBuffer buffer, int x, int y, int width, int height, char vertSideToken, char horizSideToken, char cornerToken, ConsoleColor f)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            buffer.Stroke(x, y, width, height, vertSideToken, horizSideToken, cornerToken, f, buffer.GetBackgroundColor(x, y));
        }

        public static void Stroke(this IConsoleBuffer buffer, int cx, int cy, int width, int height, char vertSideToken, char horizSideToken, char cornerToken, ConsoleColor f, ConsoleColor b)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            var right = width - 1;
            var bottom = height - 1;

            buffer.Draw(0, 0, cornerToken, f, b);
            buffer.Draw(0, bottom, cornerToken, f, b);
            buffer.Draw(right, 0, cornerToken, f, b);
            buffer.Draw(right, bottom, cornerToken, f, b);

            for (var x = 1; x < right; ++x)
            {
                buffer.Draw(x, 0, horizSideToken, f, b);
                buffer.Draw(x, bottom, horizSideToken, f, b);
            }

            for (var y = 1; y < bottom; ++y)
            {
                buffer.Draw(0, y, vertSideToken, f, b);
                buffer.Draw(right, y, vertSideToken, f, b);
            }
        }


        public static void Draw(this IConsoleBuffer buffer, int x, int y, char c, ConsoleColor f)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            buffer.Draw(x, y, c, f, buffer.GetBackgroundColor(x, y));
        }

        public static void Draw(this IConsoleBuffer buffer, int x, int y, string s, ConsoleColor f, ConsoleColor b)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            for (var dx = 0; dx < s.Length; ++dx)
            {
                buffer.Draw(x + dx, y, s[dx], f, b);
            }
        }

        public static void Draw(this IConsoleBuffer buffer, int x, int y, string s, ConsoleColor f)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            for (var dx = 0; dx < s.Length; ++dx)
            {
                buffer.Draw(x + dx, y, s[dx], f);
            }
        }

        public static void DrawPuzzle(this IConsoleBuffer buf, int x, int y, Puzzle p)
        {
            if (buf is null)
            {
                throw new ArgumentNullException(nameof(buf));
            }

            if (p is null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            for (var dx = 0; dx < p.Width; ++dx)
            {
                for (var dy = 0; dy < p.Height; ++dy)
                {
                    if (p[dx, dy] != Puzzle.EmptyTile)
                    {
                        buf.Draw(x + dx, y + dy, '#', (ConsoleColor)(p[dx, dy] + 8), (ConsoleColor)p[dx, dy]);
                    }
                }
            }
        }
    }
}