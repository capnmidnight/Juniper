using System;
using System.Collections.Generic;

using Juniper.Console;
using Juniper.Puzzles;

using static Juniper.Console.VirtualKeyState;
using static Juniper.Unicode.BoxDrawingSet;

namespace Juniper
{
    public static class Program
    {
        private const int PADDING = 1;
        private const int PADDING_SIZE = 2 * PADDING;

        public static void Main()
        {
            var window = new ConsoleBuffer(16);
            var game = new TetrisGame(20, 30);

            var keyActions = new Dictionary<int, Action<bool>>()
            {
                [VK_UP] = game.SetFlip,
                [VK_DOWN] = game.SetDrop,
                [VK_LEFT] = game.SetLeft,
                [VK_RIGHT] = game.SetRight
            };

            var border = window.Window(0, 0, game.Width + PADDING_SIZE, game.Height + PADDING_SIZE);
            var board = window.Window(PADDING, PADDING, game.Width, game.Height);
            var nextPiecePanel = window.Window(border.AbsoluteRight + 1, 2, 7, 5);
            var scorePanel = window.Window(nextPiecePanel.AbsoluteLeft, nextPiecePanel.AbsoluteBottom + 1, 7, 2);

            var last = DateTime.Now;

            while (!game.GameOver)
            {
                var now = DateTime.Now;
                var delta = now - last;
                last = now;
                foreach (var entry in keyActions)
                {
                    entry.Value(ConsoleBuffer.IsKeyDown(entry.Key));
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
