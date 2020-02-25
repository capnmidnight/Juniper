using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using Juniper.Input;
using Juniper.Puzzles;

namespace Juniper
{
    public static class Program
    {
        private static TetrisGame game;
        private static Brush[] colors;
        private static Thread thread;

        private static DateTime last;

        private static Form form;
        private static Bitmap buffer, block, bgimage;
        private static Graphics back, front;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            form = new Form1();

            colors = new Brush[]{
                Brushes.Black,
                Brushes.Blue,
                Brushes.Green,
                Brushes.Yellow,
                Brushes.Gray,
                Brushes.Orange,
                Brushes.Purple,
                Brushes.White
            };

            game = new TetrisGame(10, 20);
            buffer = new Bitmap(form.ClientSize.Width, form.ClientSize.Height);
            back = Graphics.FromImage(buffer);
            front = form.CreateGraphics();
            last = DateTime.Now;

            block = Properties.Resources.block;
            bgimage = Properties.Resources.back;
            thread = new Thread(new ThreadStart(Run));

            var keys = new WinFormsKeyEventSource(form);
            keys.AddKeyAlias("up", Keys.Up);
            keys.AddKeyAlias("down", Keys.Down);
            keys.AddKeyAlias("left", Keys.Left);
            keys.AddKeyAlias("right", Keys.Right);

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
            thread.Start();

            Application.Run(form);

            keys.Quit();
            thread.Abort();
            buffer.Dispose();
            block.Dispose();
            bgimage.Dispose();
            back.Dispose();
            front.Dispose();
            form.Dispose();
        }

        private static void Run()
        {
            while (thread.ThreadState == ThreadState.Running)
            {
                var now = DateTime.Now;
                var delta = now - last;
                if (delta > TimeSpan.Zero)
                {
                    last = now;
                    game.Update(delta);
                    Draw();
                }
                Application.DoEvents();
            }
        }

        private static void Draw()
        {
            back.DrawImage(bgimage, new Rectangle(form.ClientRectangle.X, form.ClientRectangle.Y, 640, 480));
            DrawPuzzle(game, 1, 1);
            DrawPuzzle(game.Current, game.CursorX + 1, game.CursorY + 1);
            DrawPuzzle(game.Next, game.Width + 3, 1);
            back.Flush();
            front.DrawImage(buffer, form.ClientRectangle.X, form.ClientRectangle.Y);
        }

        private static void DrawPuzzle(Puzzle p, int x, int y)
        {
            for (var dy = 0; dy < p.Height; ++dy)
            {
                for (var dx = 0; dx < p.Width; ++dx)
                {
                    if (p[dx, dy] != Puzzle.EmptyTile)
                    {
                        back.FillRectangle(colors[p[dx, dy]], (x + dx) * 20, (y + dy) * 20, 20, 20);
                        back.DrawImageUnscaled(block, (x + dx) * 20, (y + dy) * 20, 20, 20);
                    }
                }
            }
        }
    }
}