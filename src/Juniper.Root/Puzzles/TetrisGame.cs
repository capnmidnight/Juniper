using System;

namespace Juniper.Puzzles
{
    public class TetrisGame : Puzzle
    {
        private static readonly Puzzle[] pieces;
        private static readonly Random rand;

        static TetrisGame()
        {
            pieces = new Puzzle[7];
            pieces[0] = new Puzzle(new int[,] { { 1, 1, 1, 1 } });
            pieces[1] = new Puzzle(new int[,] { { 2, 2 },
                                                { 2, 2 } });
            pieces[2] = new Puzzle(new int[,] { { -1, 3, -1 },
                                                {  3, 3,  3 } });
            pieces[3] = new Puzzle(new int[,] { { 4, 4, -1 }, { -1, 4, 4 } });
            pieces[4] = new Puzzle(new int[,] { { -1, 5, 5 }, { 5, 5, -1 } });
            pieces[5] = new Puzzle(new int[,] { { 6, -1, -1 }, { 6, 6, 6 } });
            pieces[6] = new Puzzle(new int[,] { { -1, -1, 7 }, { 7, 7, 7 } });

            rand = new Random();
        }

        private bool isLeftDown, isRightDown, isUpDown, isDownDown;
        private int millisPerDrop, lessMillisPerLine, millisPerMove, millisPerFlip, millisPerAdvance;
        private double sinceLastMove, sinceLastDrop, sinceLastFlip, sinceLastAdvance, sincePieceEntered;

        public Puzzle Next { get; private set; }

        public Puzzle Current { get; private set; }

        public int CursorX { get; private set; }
        public int CursorY { get; private set; }

        public int Score { get; private set; }

        public bool GameOver { get; private set; }

        public void Reset()
        {
            millisPerDrop = 5000;
            millisPerFlip = 2500;
            millisPerMove = 2000;
            millisPerAdvance = 2500;
            lessMillisPerLine = 100;

            Current = pieces[rand.Next(pieces.Length)];
            Next = pieces[rand.Next(pieces.Length)];
            CursorX = Width / 2;
            CursorY = 0;
            Score = 0;
            GameOver = false;
            sinceLastMove = sinceLastDrop = sinceLastFlip = sinceLastAdvance = sincePieceEntered = 0.0;
        }

        public TetrisGame(int width, int height)
            : base(width, height)
        {
            Reset();
        }
        public TetrisGame(int[,] grid)
            : base(grid)
        {
            Reset();
        }

        private void MoveLeft()
        {
            if (IsInBounds(CursorX - 1, CursorY, Current)
                        && IsEmpty(CursorX - 1, CursorY, Current))
            {
                CursorX--;
            }
        }

        private void MoveRight()
        {
            if (IsInBounds(CursorX + 1, CursorY, Current)
                        && IsEmpty(CursorX + 1, CursorY, Current))
            {
                CursorX++;
            }
        }

        public bool Update(TimeSpan sinceLastDraw)
        {
            if (isLeftDown || isRightDown)
            {
                sinceLastMove += sinceLastDraw.TotalMilliseconds;
                if (sinceLastMove >= millisPerMove)
                {
                    sinceLastMove = 0.0;
                    if (isLeftDown)
                    {
                        MoveLeft();
                    }

                    if (isRightDown)
                    {
                        MoveRight();
                    }
                }
            }
            else
            {
                sinceLastMove = millisPerMove;
            }

            if (isUpDown)
            {
                sinceLastFlip += sinceLastDraw.TotalMilliseconds;
                if (sinceLastFlip >= millisPerFlip)
                {
                    sinceLastFlip = 0.0;
                    var pre = Current.Rotate(Clockwise);
                    PlayFlip();
                    if (isUpDown && IsInBounds(CursorX, CursorY, pre)
                        && IsEmpty(CursorX, CursorY, pre))
                    {
                        Current = pre;
                    }
                }
            }
            else
            {
                sinceLastFlip = millisPerFlip;
            }

            if (isDownDown && sincePieceEntered >= 500.0)
            {
                sinceLastAdvance += sinceLastDraw.TotalMilliseconds;
                if (sinceLastAdvance >= millisPerAdvance)
                {
                    sinceLastAdvance = 0.0;
                    if (IsInBounds(CursorX, CursorY + 1, Current)
                            && IsEmpty(CursorX, CursorY + 1, Current))
                    {
                        CursorY++;
                    }
                }
            }
            else
            {
                sinceLastAdvance = millisPerAdvance;
            }

            sinceLastDrop += sinceLastDraw.TotalMilliseconds;
            sincePieceEntered += sinceLastDraw.TotalMilliseconds;

            if (sinceLastDrop >= millisPerDrop)
            {
                sinceLastDrop = 0.0;
                if (!IsEmpty(RowOrder, 0))
                {
                    GameOver = true;
                }
                if (!GameOver)
                {
                    if (IsEmpty(CursorX, CursorY + 1, Current)
                        && IsInBounds(CursorX, CursorY + 1, Current))
                    {
                        CursorY++;
                    }
                    else
                    {
                        Fill(CursorX, CursorY, Current);
                        PlayThump();
                        Current = Next;
                        Next = pieces[rand.Next(pieces.Length)];
                        sincePieceEntered = 0.0;
                        CursorX = Width / 2;
                        CursorY = 0;
                    }
                    var clearCount = 0;
                    for (var y = 0; y < Height; ++y)
                    {
                        if (IsFull(RowOrder, y))
                        {
                            TetrisClearRow(y);
                            y--; //rewind, because TetrisClearRow changes the layout of the board
                            clearCount++;
                        }
                    }
                    if (clearCount > 0)
                    {
                        PlayLineClear(clearCount);
                        Score += clearCount * clearCount * 100;
                        millisPerDrop -= lessMillisPerLine;
                    }
                }
                return true;
            }
            return false;
        }

        public void Left_Depress()
        {
            isLeftDown = true;
        }

        public void Left_Release()
        {
            isLeftDown = false;
        }

        public void Right_Depress()
        {
            isRightDown = true;
        }

        public void Right_Release()
        {
            isRightDown = false;
        }

        public void Up_Depress()
        {
            isUpDown = true;
        }

        public void Up_Release()
        {
            isUpDown = false;
        }

        public void Down_Depress()
        {
            isDownDown = true;
        }

        public void Down_Release()
        {
            isDownDown = false;
        }

        public void TetrisClearRow(int row)
        {
            if (IsInBounds(0, row))
            {
                for (var x = 0; x < Width; ++x)
                {
                    for (var y = row; y > 0; --y)
                    {
                        this[x, y] = this[x, y - 1];
                    }

                    this[x, 0] = -1;
                }
            }
        }

        protected virtual void PlayThump()
        {
        }

        protected virtual void PlayLineClear(int numLines)
        {
        }

        protected virtual void PlayFlip()
        {
        }
    }
}