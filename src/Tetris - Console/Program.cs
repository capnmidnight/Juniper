using System;
using System.Collections.Generic;

using Juniper.Console;
using Juniper.Puzzles;

using static System.Console;

namespace Juniper
{
    public static class Program
    {
        private static readonly Dictionary<ConsoleKey, Action> keyPresses = new Dictionary<ConsoleKey, Action>
        {
            [ConsoleKey.UpArrow] = () => game.Up_Depress(),
            [ConsoleKey.LeftArrow] = () => game.Left_Depress(),
            [ConsoleKey.RightArrow] = () => game.Right_Depress(),
            [ConsoleKey.DownArrow] = () => game.Down_Depress()
        };

        private static readonly Dictionary<ConsoleKey, Action> keyReleases = new Dictionary<ConsoleKey, Action>
        {
            [ConsoleKey.UpArrow] = () => game.Up_Release(),
            [ConsoleKey.LeftArrow] = () => game.Left_Release(),
            [ConsoleKey.RightArrow] = () => game.Right_Release(),
            [ConsoleKey.DownArrow] = () => game.Down_Release()
        };

        private const int PADDING = 2;
        private const int PADDING_SIZE = 2 * PADDING;

        private static TetrisGame game;
        private static ConsoleBuffer window;
        private static IConsoleBuffer border;
        private static IConsoleBuffer board;
        private static IConsoleBuffer nextPiecePanel;
        private static IConsoleBuffer scorePanel;

        public static void Main()
        {
            ConsoleBuffer.SetFont("Consolas");

            game = new TetrisGame(20, 25);

            window = new ConsoleBuffer(game.Width + PADDING_SIZE + 10, game.Height + PADDING_SIZE + 1);
            border = window.Window(0, 0, game.Width + PADDING_SIZE, game.Height + PADDING_SIZE);
            board = window.Window(PADDING, PADDING, game.Width, game.Height);
            nextPiecePanel = window.Window(border.AbsoluteRight + 1, 2, 4, 5);
            scorePanel = window.Window(nextPiecePanel.AbsoluteLeft, 7, 7, 2);
            
            var last = DateTime.Now;
            while (!game.GameOver)
            {
                DoInput();

                var now = DateTime.Now;
                game.Update(now - last);
                last = now;

                Draw();
            }
        }

        private static void Draw()
        {
            window.Clear();

            for (var i = 0; i < PADDING; ++i)
            {
                border.Stroke(i, i, border.Width - i, border.Height - i, '|', '-', '+', ConsoleColor.Green);
            }

            board.DrawPuzzle(0, 0, game);
            board.DrawPuzzle(game.CursorX, game.CursorY, game.Current);

            nextPiecePanel.Fill(ConsoleColor.DarkGray);
            nextPiecePanel.Draw(0, 0, "Next", ConsoleColor.Cyan);
            nextPiecePanel.DrawPuzzle(2, 1, game.Next);

            var score = game.Score.ToString(System.Globalization.CultureInfo.CurrentCulture);
            scorePanel.Fill(ConsoleColor.DarkGray);
            scorePanel.Draw(0, 0, "Score", ConsoleColor.Cyan);
            scorePanel.Draw(2, 1, score, ConsoleColor.Yellow);

            window.Flush();
        }

        private static void DoInput()
        {
            if (KeyAvailable)
            {
                var key = ReadKey(true).Key;

                foreach (var keyedAction in keyReleases)
                {
                    if (key != keyedAction.Key)
                    {
                        keyedAction.Value();
                    }
                }

                if (keyPresses.ContainsKey(key))
                {
                    keyPresses[key]();
                }
            }
            else
            {
                foreach (var action in keyReleases.Values)
                {
                    action();
                }
            }
        }
    }
}
