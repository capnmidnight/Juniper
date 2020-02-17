using System;
using System.Collections.Generic;
using System.Globalization;

using Juniper.Console;
using Juniper.Input;
using Juniper.Puzzles;

using static Juniper.Input.VirtualKeyState;
using static Juniper.Unicode.BoxDrawingSet;

namespace Juniper
{
    public static class Program
    {
        private const int PADDING = 1;
        private const int PADDING_SIZE = 2 * PADDING;

        private static ConsoleBuffer frame;
        private static TetrisGame game;

        public static void Main()
        {
            frame = new ConsoleBuffer(16);
            game = new TetrisGame(20, 30);

            var border = frame.Window(0, 0, game.Width + PADDING_SIZE, game.Height + PADDING_SIZE);
            var board = frame.Window(PADDING, PADDING, game.Width, game.Height);
            var nextPiecePanel = frame.Window(border.AbsoluteRight + 1, 2, 7, 5);
            var scorePanel = frame.Window(nextPiecePanel.AbsoluteLeft, nextPiecePanel.AbsoluteBottom + 1, 7, 2);

            var keys = new Win32KeyEventSource();
            keys.AddKeyAlias("up", VK_UP);
            keys.AddKeyAlias("down", VK_DOWN);
            keys.AddKeyAlias("left", VK_LEFT);
            keys.AddKeyAlias("right", VK_RIGHT);

            var keyActions = new Dictionary<string, Action<bool>>()
            {
                ["up"] = game.SetFlip,
                ["down"] = game.SetDrop,
                ["left"] = game.SetLeft,
                ["right"] = game.SetRight
            };

            keys.KeyChanged += delegate (object sender, KeyChangeEvent args)
            {
                keyActions[args.Name](args.State);
            };

            keys.Start();

            var last = DateTime.Now;
            while (!game.GameOver)
            {
                var now = DateTime.Now;
                var delta = now - last;
                if (delta > TimeSpan.Zero)
                {
                    last = now;
                    game.Update(delta);
                    Draw(border, board, nextPiecePanel, scorePanel);
                }
            }

            keys.Stop();
        }

        private static void Draw(IConsoleBuffer border, IConsoleBuffer board, IConsoleBuffer nextPiecePanel, IConsoleBuffer scorePanel)
        {
            frame.Fill(ConsoleColor.DarkGray);

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

            var score = game.Score.ToString(CultureInfo.CurrentCulture);
            scorePanel.Draw(0, 0, $"Score", ConsoleColor.Black);
            scorePanel.Draw(2, 1, score, ConsoleColor.White);

            var (x, y) = ConsoleBuffer.GetCursorPosition();
            scorePanel.Draw(0, 2, "X", ConsoleColor.Black);
            scorePanel.Draw(0, 3, x.ToString(CultureInfo.CurrentCulture), ConsoleColor.White);
            scorePanel.Draw(0, 4, "Y", ConsoleColor.Black);
            scorePanel.Draw(0, 5, y.ToString(CultureInfo.CurrentCulture), ConsoleColor.White);
            frame.Draw(x / 16, y / 16, '*', ConsoleColor.Yellow);


            frame.Flush();
        }
    }
}
