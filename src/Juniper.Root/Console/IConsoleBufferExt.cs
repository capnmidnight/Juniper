using System;
using Juniper.Puzzles;
using Juniper.Unicode;

namespace Juniper.Console
{
    public static class IConsoleBufferExt
    {
        public static IConsoleBuffer Window(this IConsoleBuffer buffer, int x, int y, int width, int height)
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

        public static void Stroke(this IConsoleBuffer buffer, int x, int y, int width, int height, char vertSideToken, char horizSideToken, char ulToken, char urToken, char llToken, char lrToken, ConsoleColor f)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            buffer.Stroke(x, y, width, height, vertSideToken, horizSideToken, ulToken, urToken, llToken, lrToken, f, buffer.GetBackgroundColor(x, y));
        }

        public static void Stroke(this IConsoleBuffer buffer, int x, int y, int width, int height, BoxDrawingSet set, ConsoleColor f)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            buffer.Stroke(x, y, width, height, set, f, buffer.GetBackgroundColor(x, y));
        }

        public static void Stroke(this IConsoleBuffer buffer, int x, int y, int width, int height, BoxDrawingSet set, ConsoleColor f, ConsoleColor b)
        {
            if (set is null)
            {
                throw new ArgumentNullException(nameof(set));
            }

            buffer.Stroke(x, y, width, height, set.Vertical, set.Horizontal, set.UpperLeft, set.UpperRight, set.LowerLeft, set.LowerRight, f, b);
        }

        public static void Stroke(this IConsoleBuffer buffer, int x, int y, int width, int height, char vertSideToken, char horizSideToken, char ulToken, char urToken, char llToken, char lrToken, ConsoleColor f, ConsoleColor b)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            var right = x + width - 1;
            var bottom = y + height - 1;

            buffer.Draw(x, y, ulToken, f, b);
            buffer.Draw(right, y, urToken, f, b);
            buffer.Draw(x, bottom, llToken, f, b);
            buffer.Draw(right, bottom, lrToken, f, b);

            for (var dx = x + 1; dx < right; ++dx)
            {
                buffer.Draw(dx, y, horizSideToken, f, b);
                buffer.Draw(dx, bottom, horizSideToken, f, b);
            }

            for (var dy = y + 1; dy < bottom; ++dy)
            {
                buffer.Draw(x, dy, vertSideToken, f, b);
                buffer.Draw(right, dy, vertSideToken, f, b);
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