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
        private static IConsoleBuffer border;
        private static IConsoleBuffer board;
        private static IConsoleBuffer nextPiecePanel;
        private static IConsoleBuffer scorePanel;
        private static IConsoleBuffer fpsPanel;

        public static void Main()
        {
            _ = ConsoleBuffer.SetFont("Consolas");

            game = new TetrisGame(20, 25);

            using var window = new ConsoleBuffer(game.Width + PADDING_SIZE + 10, game.Height + PADDING_SIZE + 1);

            border = window.Window(0, 0, game.Width + PADDING_SIZE, game.Height + PADDING_SIZE);
            board = window.Window(PADDING, PADDING, game.Width, game.Height);
            nextPiecePanel = window.Window(border.AbsoluteRight + 1, 2, 7, 5);
            scorePanel = window.Window(nextPiecePanel.AbsoluteLeft, nextPiecePanel.AbsoluteBottom + 1, 7, 2);
            fpsPanel = window.Window(nextPiecePanel.AbsoluteLeft, scorePanel.AbsoluteBottom + 1, 7, 2);
            var last = DateTime.Now;
            while (!game.GameOver)
            {
                DoInput();

                var now = DateTime.Now;
                var delta = now - last;
                last = now;

                game.Update(delta);

                var fps = 1 / delta.TotalSeconds;

                Draw(window, fps);
            }
        }

        private static void Draw(ConsoleBuffer window, double fps)
        {
            window.Clear();

            for (var i = 0; i < PADDING; ++i)
            {
                var j = 2 * i;
                border.Stroke(i, i, border.Width - j, border.Height - j, '║', '═', '╔', '╗', '╚', '╝', ConsoleColor.Green);
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

            fpsPanel.Fill(ConsoleColor.DarkGray);
            fpsPanel.Draw(0, 0, "FPS", ConsoleColor.Cyan);
            fpsPanel.Draw(2, 1, $"{fps:0.00}", ConsoleColor.Yellow);

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
