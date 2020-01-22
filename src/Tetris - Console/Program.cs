using System;
using System.Collections.Generic;

using Juniper.Puzzles;

using static System.Console;

namespace Juniper
{
    public static class Program
    {
        private static TetrisGame game;

        private static Dictionary<ConsoleKey, Action> keyPresses;
        private static Dictionary<ConsoleKey, Action> keyReleases;
        private static bool done;

        public static void Main()
        {
            game = new TetrisGame(20, 20);
            keyPresses = new Dictionary<ConsoleKey, Action>
            {
                { ConsoleKey.UpArrow, game.Up_Depress },
                { ConsoleKey.LeftArrow, game.Left_Depress },
                { ConsoleKey.RightArrow, game.Right_Depress },
                { ConsoleKey.DownArrow, game.Down_Depress },
                { ConsoleKey.F4, Quit }
            };
            keyReleases = new Dictionary<ConsoleKey, Action>
            {
                { ConsoleKey.UpArrow, game.Up_Release },
                { ConsoleKey.LeftArrow, game.Left_Release },
                { ConsoleKey.RightArrow, game.Right_Release },
                { ConsoleKey.DownArrow, game.Down_Release }
            };

            var buffer = new ConsoleBuffer(WindowWidth, WindowHeight - 1);

            done = false;
            var last = DateTime.Now;
            while (!done)
            {
                DoInput();
                var now = DateTime.Now;
                game.Update(now - last);
                Draw(game, game.Next, game.Current, buffer, game.CursorX, game.CursorY);
                last = now;
            }
        }

        public static void Quit()
        {
            done = true;
        }

        private static void Draw(Puzzle board, Puzzle next, Puzzle current, ConsoleBuffer buffer, int cursorX, int cursorY)
        {
            buffer.Clear();
            for (var y = 1; y < board.Height + 1; ++y)
            {
                buffer.Set(0, y, '|', ConsoleColor.Green);
                buffer.Set(board.Width + 1, y, '|', ConsoleColor.Green);
            }
            for (var x = 0; x < board.Width; ++x)
            {
                buffer.Set(x + 1, 0, '-', ConsoleColor.Green);
                buffer.Set(x + 1, board.Height + 1, '-', ConsoleColor.Green);
            }
            DrawPuzzle(buffer, board, 1, 1);
            DrawPuzzle(buffer, current, cursorX + 1, cursorY + 1);
            DrawPuzzle(buffer, next, board.Width + 5, 0);
            buffer.Flush();
        }

        private static void DoInput()
        {
            foreach (var action in keyReleases.Values)
            {
                action();
            }

            if (KeyAvailable)
            {
                var key = ReadKey(true).Key;
                if (keyPresses.ContainsKey(key))
                {
                    keyPresses[key]();
                }
            }
        }

        private static void DrawPuzzle(ConsoleBuffer buf, Puzzle p, int x, int y)
        {
            for (var dy = 0; dy < p.Height; ++dy)
            {
                CursorLeft = x;
                CursorLeft = dy + y;
                for (var dx = 0; dx < p.Width; ++dx)
                {
                    if (p[dx, dy] != Puzzle.EmptyTile)
                    {
                        buf.Set(x + dx, y + dy, '#', (ConsoleColor)(p[dx, dy] + 8), (ConsoleColor)p[dx, dy]);
                    }
                }
            }
        }
    }
}
