using System;
using System.Collections.Generic;
using System.Globalization;
using Juniper.Input;
using Juniper.Puzzles;
using Juniper.Terminal;
using static System.Windows.Forms.Keys;
using static Juniper.Unicode.BoxDrawingSet;

namespace Juniper
{
    public static class Program
    {
        private const int PADDING = 1;
        private const int PADDING_SIZE = 2 * PADDING;

        private static ConsoleBuffer frame;
        private static TetrisGame game;

        [STAThread]
        public static void Main()
        {
            var keys = new Win32KeyEventSource();
            while (true)
            {
                System.Console.WriteLine($"D {keys.IsKeyDown(D)}, ControlKey {keys.IsKeyDown(ControlKey)}, Control+D {keys.IsKeyDown(D | Control)}");
            }
        }

        private static void RunGame()
        { 
            frame = new ConsoleBuffer(16);
            game = new TetrisGame(20, 30);

            var border = frame.Window(0, 0, game.Width + PADDING_SIZE, game.Height + PADDING_SIZE);
            var board = frame.Window(PADDING, PADDING, game.Width, game.Height);
            var nextPiecePanel = frame.Window(border.AbsoluteRight + 1, 2, 7, 5);
            var scorePanel = frame.Window(nextPiecePanel.AbsoluteLeft, nextPiecePanel.AbsoluteBottom + 1, 7, 2);
            var lastCommand = "";


            var keys = new Win32KeyEventSource();
            keys.AddKeyAlias("reverse flip", Control | Up);
            keys.AddKeyAlias("flip", Up);
            keys.AddKeyAlias("drop", Down);
            keys.AddKeyAlias("left", Left);
            keys.AddKeyAlias("right", Right);

            var keyActions = new Dictionary<string, Action<bool>>()
            {
                ["flip"] = game.SetFlip,
                ["reverse flip"] = game.SetReverseFlip,
                ["drop"] = game.SetDrop,
                ["left"] = game.SetLeft,
                ["right"] = game.SetRight
            };

            keys.KeyChanged += delegate (object sender, KeyChangeEvent args)
            {
                lastCommand = args.Name;
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
                    Draw(border, board, nextPiecePanel, scorePanel, lastCommand);
                }
            }

            keys.Quit();
        }

        private static void Draw(IConsoleBuffer border, IConsoleBuffer board, IConsoleBuffer nextPiecePanel, IConsoleBuffer scorePanel, string lastCommand)
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
            scorePanel.Draw(0, 2, lastCommand, ConsoleColor.Black);

            frame.Flush();
        }
    }
}
