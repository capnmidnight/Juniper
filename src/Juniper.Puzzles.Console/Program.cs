using System;
using System.Collections.Generic;
using System.Text;
using PuzzleFramework;
using Puzzles;
namespace Thumbnail
{
    class Program
    {
        static TetrisGame game;
        delegate void Action();
        static Dictionary<object, Action> command;
        static bool done;
        static void Main(string[] args)
        {
            game = new TetrisGame(20, 20);
            command = new Dictionary<object, Action>();
            command.Add(ConsoleKey.UpArrow, new Action(game.Up_Depress));
            command.Add(ConsoleKey.LeftArrow, new Action(game.Left_Depress));
            command.Add(ConsoleKey.RightArrow, new Action(game.Right_Depress));
            command.Add(ConsoleKey.DownArrow, new Action(game.Down_Depress));
            command.Add(ConsoleKey.F4, new Action(Quit));

            ConsoleBuffer buffer = new ConsoleBuffer(Console.WindowWidth, Console.WindowHeight - 1);

            done = false;
            DateTime last = DateTime.Now;
            while (!done)
            {
                DoInput();
                if (game.Update(DateTime.Now - last))
                    last = DateTime.Now;
                Draw(game, game.Next, game.Current, buffer, game.CursorX, game.CursorY);
            }
        }
        public static void Quit()
        {
            done = true;
        }
        private static void Draw(Puzzle board, Puzzle next, Puzzle current, ConsoleBuffer buffer, int cursorX, int cursorY)
        {
            buffer.Clear();
            for (int y = 1; y < board.Height + 1; ++y)
            {
                buffer.Set(0, y, '|', ConsoleColor.Green);
                buffer.Set(board.Width + 1, y, '|', ConsoleColor.Green);
            }
            for (int x = 0; x < board.Width; ++x)
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
            if (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                if (command.ContainsKey(key))
                    command[key]();
            }
        }
        static void DrawPuzzle(ConsoleBuffer buf, Puzzle p, int x, int y)
        {
            for (int dy = 0; dy < p.Height; ++dy)
            {
                Console.CursorLeft = x;
                Console.CursorLeft = dy + y;
                for (int dx = 0; dx < p.Width; ++dx)
                    if (p[dx, dy] != Puzzle.EmptyTile)
                        buf.Set(x + dx, y + dy, '#', (ConsoleColor)(p[dx, dy] + 8), (ConsoleColor)p[dx, dy]);
            }
        }
    }
}
