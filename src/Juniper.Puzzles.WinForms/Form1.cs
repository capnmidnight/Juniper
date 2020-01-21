using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using PuzzleFramework;
using Puzzles;
namespace Thumbnail2
{
    public partial class Form1 : Form
    {
        TetrisGame game;
        Brush[] colors;
        Thread thread;
        Bitmap buffer, block, bgimage;
        Graphics back, front;
        delegate void Action();
        Dictionary<object, Action> command;
        bool done, finished;
        DateTime last;
        public Form1()
        {
            InitializeComponent();
            Initialize();

            thread = new Thread(new ThreadStart(Run));
            thread.Start();
        }
        void Initialize()
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
            command = new Dictionary<object, Action>();
            command.Add(Keys.Up, new Action(game.Up_Depress));
            command.Add(Keys.Left, new Action(game.Left_Depress));
            command.Add(Keys.Right, new Action(game.Right_Depress));
            command.Add(Keys.Down, new Action(game.Down_Depress));

            buffer = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            back = Graphics.FromImage(buffer);
            front = this.CreateGraphics();
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

        void Run()
        {
            while (!done)
            {
                if (game.Update(DateTime.Now - last))
                    last = DateTime.Now;
                Draw();
                Application.DoEvents();
            }
            finished = true;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            done = true;
            while (!finished)
                Application.DoEvents();
            base.OnClosing(e);
        }
        private void Draw()
        {
            back.DrawImage(bgimage, new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y, 640, 480));
            //back.DrawRectangle(Pens.LightGreen, 20, 20, game.Width * 20, game.Height * 20);
            DrawPuzzle(game, 1, 1);
            DrawPuzzle(game.Current, game.CursorX + 1, game.CursorY + 1);
            DrawPuzzle(game.Next, game.Width + 3, 1);
            back.Flush();
            front.DrawImage(buffer, this.ClientRectangle.X, this.ClientRectangle.Y);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            Keys key = e.KeyCode;
            if (command.ContainsKey(key))
                command[key]();
        }
        void DrawPuzzle(Puzzle p, int x, int y)
        {
            for (int dy = 0; dy < p.Height; ++dy)
                for (int dx = 0; dx < p.Width; ++dx)
                    if (p[dx, dy] != Puzzle.EmptyTile)
                    {
                        back.FillRectangle(colors[p[dx, dy]], (x + dx) * 20, (y + dy) * 20, 20, 20);
                        back.DrawImageUnscaled(block, (x + dx) * 20, (y + dy) * 20, 20, 20);
                    }
        }
    }
}