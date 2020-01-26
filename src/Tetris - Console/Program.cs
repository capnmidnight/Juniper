using System;
using System.Collections.Generic;

using Juniper.Console;
using Juniper.Puzzles;

using static Juniper.Unicode.BoxDrawingSet;

namespace Juniper
{
    public static class Program
    {
        private const int PADDING = 1;
        private const int PADDING_SIZE = 2 * PADDING;

        private static TetrisGame game;
        private static IConsoleBuffer border;
        private static IConsoleBuffer board;
        private static IConsoleBuffer nextPiecePanel;
        private static IConsoleBuffer scorePanel;

        public static void Main()
        {
            game = new TetrisGame(20, 25);
            var keyActions = new Dictionary<VirtualKeyState, (Action pressed, Action released)>()
            {
                [VirtualKeyState.VK_UP] = (game.Up_Depress, game.Up_Release),
                [VirtualKeyState.VK_DOWN] = (game.Down_Depress, game.Down_Release),
                [VirtualKeyState.VK_LEFT] = (game.Left_Depress, game.Left_Release),
                [VirtualKeyState.VK_RIGHT] = (game.Right_Depress, game.Right_Release)
            };

            using var window = new ConsoleBuffer(game.Width + PADDING_SIZE + 10, game.Height + PADDING_SIZE + 1);

            border = window.Window(0, 0, game.Width + PADDING_SIZE, game.Height + PADDING_SIZE);
            board = window.Window(PADDING, PADDING, game.Width, game.Height);
            nextPiecePanel = window.Window(border.AbsoluteRight + 1, 2, 7, 5);
            scorePanel = window.Window(nextPiecePanel.AbsoluteLeft, nextPiecePanel.AbsoluteBottom + 1, 7, 2);

            var last = DateTime.Now;

            while (!game.GameOver)
            {
                var now = DateTime.Now;
                var delta = now - last;
                last = now;
                foreach (var entry in keyActions)
                {
                    var key = entry.Key;
                    var (press, release) = entry.Value;
                    if (ConsoleBuffer.IsKeyDown(key))
                    {
                        press();
                    }
                    else
                    {
                        release();
                    }
                }

                game.Update(delta);

                var fps = 1 / delta.TotalSeconds;

                window.Fill(ConsoleColor.DarkGray);

                for (var i = 0; i < PADDING; ++i)
                {
                    var j = 2 * i;
                    border.Stroke(
                        i, i,
                        border.Width - j, border.Height - j,
                        DoubleLight,
                        ConsoleColor.Gray);
                }

                board.Fill(ConsoleColor.Black);
                board.DrawPuzzle(0, 0, game);
                board.DrawPuzzle(game.CursorX, game.CursorY, game.Current);

                nextPiecePanel.Draw(0, 0, "Next", ConsoleColor.Black);
                nextPiecePanel.DrawPuzzle(2, 1, game.Next);

                var score = game.Score.ToString(System.Globalization.CultureInfo.CurrentCulture);
                scorePanel.Draw(0, 0, "Score", ConsoleColor.Black);
                scorePanel.Draw(2, 1, score, ConsoleColor.White);

                window.Flush();
            }
        }
    }
}
