using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using Juniper.Puzzles;

namespace Thumbnail2
{
    public partial class Form1 : Form
    {
        private TetrisGame game;
        private Brush[] colors;
        private readonly Thread thread;
        private Bitmap buffer, block, bgimage;
        private Graphics back, front;

        private Dictionary<object, Action> keyPresses;
        private Dictionary<object, Action> keyReleases;
        private bool done, finished;
        private DateTime last;

        public Form1()
        {
            InitializeComponent();
            Initialize();

            thread = new Thread(new ThreadStart(Run));
            thread.Start();
        }

        private void Initialize()
        {
            colors = new Brush[8];
            colors[0] = Brushes.Black;
            colors[1] = Brushes.Blue;
            colors[2] = Brushes.Green;
            colors[3] = Brushes.Yellow;
            colors[4] = Brushes.Gray;
            colors[5] = Brushes.Orange;
            colors[6] = Brushes.Purple;
            colors[7] = Brushes.White;


            game = new TetrisGame(10, 20);
            keyPresses = new Dictionary<object, Action>
            {
                { Keys.Up, game.Up_Depress },
                { Keys.Left, game.Left_Depress },
                { Keys.Right, game.Right_Depress },
                { Keys.Down, game.Down_Depress }
            };

            keyReleases = new Dictionary<object, Action>
            {
                { Keys.Up, game.Up_Release },
                { Keys.Left, game.Left_Release},
                { Keys.Right, game.Right_Release},
                { Keys.Down, game.Down_Release }
            };

            buffer = new Bitmap(ClientSize.Width, ClientSize.Height);
            back = Graphics.FromImage(buffer);
            front = CreateGraphics();
            done = false;
            finished = false;
            last = DateTime.Now;

            block = new Bitmap("../../block2.png");
            //for (int y = 0; y < block.Height; ++y)
            //    for (int x = 0; x < block.Width; ++x)
            //    {
            //        Color c = block.GetPixel(x, y);
            //        c = Color.FromArgb(255 - c.R, c.R, c.R, c.R);
            //        block.SetPixel(x, y, c);
            //    }
            bgimage = new Bitmap("../../back.jpg");
        }

        private void Run()
        {
            while (!done)
            {
                if (game.Update(DateTime.Now - last))
                {
                    last = DateTime.Now;
                }

                Draw();
                Application.DoEvents();
            }
            finished = true;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            done = true;
            while (!finished)
            {
                Application.DoEvents();
            }

            base.OnClosing(e);
        }
        private void Draw()
        {
            back.DrawImage(bgimage, new Rectangle(ClientRectangle.X, ClientRectangle.Y, 640, 480));
            //back.DrawRectangle(Pens.LightGreen, 20, 20, game.Width * 20, game.Height * 20);
            DrawPuzzle(game, 1, 1);
            DrawPuzzle(game.Current, game.CursorX + 1, game.CursorY + 1);
            DrawPuzzle(game.Next, game.Width + 3, 1);
            back.Flush();
            front.DrawImage(buffer, ClientRectangle.X, ClientRectangle.Y);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            var key = e.KeyCode;
            if (keyPresses.ContainsKey(key))
            {
                keyPresses[key]();
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            var key = e.KeyCode;
            if (keyReleases.ContainsKey(key))
            {
                keyReleases[key]();
            }
        }

        private void DrawPuzzle(Puzzle p, int x, int y)
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