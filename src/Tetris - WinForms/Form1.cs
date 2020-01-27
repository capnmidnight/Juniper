using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using Juniper.Puzzles;

namespace Juniper
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

        private float shakeMagnitude;
        private float shakeDuration;

        public Form1()
        {
            InitializeComponent();
            Initialize();

            thread = new Thread(new ThreadStart(Run));
            thread.Start();
        }

        private void Initialize()
        {
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
            game.Flip += Game_Flip;
            game.Thump += Game_Thump;
            game.LineClear += Game_LineClear;

            keyPresses = new Dictionary<object, Action>
            {
                { Keys.Up, game.Flip_Depress },
                { Keys.Left, game.Left_Depress },
                { Keys.Right, game.Right_Depress },
                { Keys.Down, game.Drop_Depress }
            };

            keyReleases = new Dictionary<object, Action>
            {
                { Keys.Up, game.Flip_Release },
                { Keys.Left, game.Left_Release},
                { Keys.Right, game.Right_Release},
                { Keys.Down, game.Drop_Release }
            };

            buffer = new Bitmap(ClientSize.Width, ClientSize.Height);
            back = Graphics.FromImage(buffer);
            front = CreateGraphics();
            done = false;
            finished = false;
            last = DateTime.Now;

            block = Properties.Resources.block;
            bgimage = Properties.Resources.back;
        }

        private void Game_LineClear(object sender, IntegerEventArgs e)
        {
            shakeMagnitude = e.Value * 10;
            shakeDuration = e.Value * 300;
        }

        private void Game_Thump(object sender, EventArgs e)
        {
            shakeMagnitude = 5;
            shakeDuration = 250;
        }

        private void Game_Flip(object sender, EventArgs e)
        {
            shakeMagnitude = 5;
            shakeDuration = 250;
        }

        private void Run()
        {
            while (!done)
            {
                var now = DateTime.Now;
                game.Update(now - last);
                last = DateTime.Now;
                Draw();
                Application.DoEvents();
            }
            finished = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (e is object)
            {
                done = true;
                while (!finished)
                {
                    Application.DoEvents();
                }

                base.OnClosing(e);
            }
        }

        private static readonly Random rand = new Random();

        private static DateTime lastShake = DateTime.Now;
        private void Draw()
        {
            if(shakeDuration > 0)
            {
                shakeDuration -= (float)((DateTime.Now - lastShake).TotalMilliseconds);
                lastShake = DateTime.Now;
                var r = rand.Number(shakeMagnitude);
                var a = Math.PI * rand.Number(2);
                var dx = r * Math.Cos(a);
                var dy = r * Math.Sin(a);
                back.TranslateTransform((float)dx, (float)dy);
            }

            back.DrawImage(bgimage, new Rectangle(ClientRectangle.X, ClientRectangle.Y, 640, 480));
            DrawPuzzle(game, 1, 1);
            DrawPuzzle(game.Current, game.CursorX + 1, game.CursorY + 1);
            DrawPuzzle(game.Next, game.Width + 3, 1);
            back.Flush();
            front.DrawImage(buffer, ClientRectangle.X, ClientRectangle.Y);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e is object)
            {
                base.OnKeyDown(e);
                var key = e.KeyCode;
                if (keyPresses.ContainsKey(key))
                {
                    keyPresses[key]();
                }
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e is object)
            {
                base.OnKeyUp(e);
                var key = e.KeyCode;
                if (keyReleases.ContainsKey(key))
                {
                    keyReleases[key]();
                }
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