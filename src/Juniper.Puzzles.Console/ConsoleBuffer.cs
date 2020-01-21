//Copyright, Sean T. McBeth, 2007
//sean.mcbeth@gmail.com
//www.seanmcbeth.com
using System;
using System.Collections.Generic;
using System.Text;

namespace Thumbnail
{
    class ConsoleBuffer
    {
        private ConsoleColor[,] back, fore;
        private char[,] grid;
        private bool[,] old;
        private int width, height;
        public ConsoleBuffer(int width, int height)
        {
            this.width = width;
            this.height = height;
            back = new ConsoleColor[width, height];
            fore = new ConsoleColor[width, height];
            grid = new char[width, height];
            old = new bool[width, height];
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    back[x, y] = ConsoleColor.Black;
                    fore[x, y] = ConsoleColor.Gray;
                    grid[x, y] = ' ';
                    old[x, y] = true;
                }
            }
        }

        private void Set(int x, int y, char c, ConsoleColor f, ConsoleColor b, bool ignoreBG)
        {
            if (0 <= x && x < width && 0 <= y && y < height &&
                ((!ignoreBG && back[x, y] != b)
                || fore[x, y] != f || grid[x, y] != c))
            {
                if (!ignoreBG)
                    back[x, y] = b;
                fore[x, y] = f;
                grid[x, y] = c;
                old[x, y] = true;
            }
        }

        public void Set(int x, int y, char c, ConsoleColor f, ConsoleColor b)
        {
            this.Set(x, y, c, f, b, false);
        }
        public void Set(int x, int y, char c, ConsoleColor f)
        {
            this.Set(x, y, c, f, ConsoleColor.Black, true);
        }

        public void Set(int x, int y, string s, ConsoleColor f, ConsoleColor b)
        {
            for (int dx = 0; dx < s.Length; ++dx)
            {
                this.Set(x + dx, y, s[dx], f, b);
            }
        }

        public void Set(int x, int y, string s, ConsoleColor f)
        {
            for (int dx = 0; dx < s.Length; ++dx)
            {
                this.Set(x + dx, y, s[dx], f);
            }
        }
        public void SetWrap(int x, int y, string s, ConsoleColor f, ConsoleColor b)
        {
            for (int dx = 0; dx < s.Length; ++dx)
            {
                int v = x + dx;
                this.Set(v % width, y + v / width, s[dx], f, b);
            }
        }
        int lastX = -1;
        int lastY = -1;
        ConsoleColor lastB, lastF;
        public void Flush()
        {
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    if (old[x, y])
                    {
                        if (lastB != back[x, y])
                        {
                            Console.BackgroundColor = back[x, y];
                            lastB = back[x, y];
                        }
                        if (lastF != fore[x, y])
                        {
                            Console.ForegroundColor = fore[x, y];
                            lastF = fore[x, y];
                        }
                        if (!(y == lastY && x == lastX + 1))
                        {
                            Console.CursorLeft = x;
                            Console.CursorTop = y;
                        }
                        Console.Write(grid[x, y]);
                        old[x, y] = false;
                    }
                }
            }
        }
        public void Clear()
        {
            this.Clear(ConsoleColor.Black);
        }

        public void Clear(ConsoleColor b)
        {
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    this.Set(x, y, ' ', ConsoleColor.Gray, b);
                }
            }
        }

        int startX, startY;
        public void Prompt(string message, bool pause)
        {
            this.Set(0, height - 1, message, ConsoleColor.Red, ConsoleColor.DarkRed);
            this.Flush();
            startX = message.Length;
            startY = height - 1;
            Console.CursorTop = startY;
            Console.CursorLeft = startX;
            if (pause)
            {
                Console.ReadKey(true);
            }
        }

        public void CorrectInputBuffer(string input)
        {
            this.Set(startX, startY, input, ConsoleColor.Red, ConsoleColor.DarkRed);
        }

        public void Set(int x, int y, ConsoleColor b)
        {
            if (this.back[x, y] != b)
            {
                this.back[x, y] = b;
                this.old[x, y] = true;
            }
        }
    }
}
