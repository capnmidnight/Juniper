namespace Juniper.Puzzles;

public class TetrisGame : Puzzle
{
    private static readonly Puzzle[] pieces =
    [
        new (new int[,] { {  1,  1, 1, 1 } }),

        new (new int[,] { {  2,  2 },
                          {  2,  2 } }),

        new (new int[,] { { -1,  3, -1 },
                          {  3,  3,  3 } }),

        new (new int[,] { {  4,  4, -1 },
                          { -1,  4,  4 } }),

        new (new int[,] { { -1,  5,  5 },
                          {  5,  5, -1 } }),

        new (new int[,] { {  6, -1, -1 },
                          {  6,  6,  6 } }),

        new (new int[,] { { -1, -1,  7 },
                          {  7,  7,  7 } })
    ];

    private static readonly Random rand = new();

    private bool isLeftDown;
    private bool isRightDown;
    private bool isFlipDown;
    private bool flipDirection;
    private bool isDropDown;
    private double advanceRate;
    private double moveRate;
    private double flipRate;
    private double millisPerAdvance;
    private double millisPerMove;
    private double millisPerFlip;
    private double millisPerDrop;
    private double sinceLastMove;
    private double sinceLastAdvance;
    private double sinceLastFlip;
    private double sinceLastDrop;
    private double sincePieceEntered;

    public Puzzle Next { get; private set; } = Empty;

    public Puzzle Current { get; private set; } = Empty;

    public int CursorX { get; private set; }
    public int CursorY { get; private set; }

    public int Score { get; private set; }

    public bool GameOver { get; private set; }

    public event EventHandler? Thump;
    public event EventHandler<IntegerEventArgs>? LineClear;
    public event EventHandler? Flip;

    private static int[,] MakeEmptyGrid(int width, int height)
    {
        var grid = new int[height, width];
        for (var y = 0; y < height; ++y)
        {
            for (var x = 0; x < width; ++x)
            {
                grid[y, x] = -1;
            }
        }

        return grid;
    }

    public TetrisGame(int width, int height)
        : this(MakeEmptyGrid(width, height))
    { }

    public TetrisGame(int[,] grid)
        : base(grid)
    {
        Reset();
    }

    public void Reset()
    {
        advanceRate = 3;
        flipRate = 8;
        moveRate = 10;

        millisPerFlip = 1000 / flipRate;
        millisPerMove = 1000 / moveRate;

        UpdateSpeed(0);

        CursorX = Width / 2;
        CursorY = 0;
        Score = 0;
        GameOver = false;
        sinceLastMove = sinceLastAdvance = sinceLastFlip = sinceLastDrop = sincePieceEntered = 0.0;

        Current = pieces[rand.Next(pieces.Length)];
        Next = pieces[rand.Next(pieces.Length)];
    }

    private void UpdateSpeed(double deltaLinesPerSecond)
    {
        advanceRate += deltaLinesPerSecond;
        millisPerAdvance = 1000 / advanceRate;
        millisPerDrop = millisPerAdvance / 10;
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

    public void Update(TimeSpan sinceLastDraw)
    {
        if (!IsEmpty(RowOrder, 0))
        {
            GameOver = true;
        }

        if (!GameOver)
        {
            if (isLeftDown || isRightDown)
            {
                sinceLastMove += sinceLastDraw.TotalMilliseconds;
                if (sinceLastMove >= millisPerMove)
                {
                    sinceLastMove -= millisPerMove;
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

            if (isFlipDown)
            {
                sinceLastFlip += sinceLastDraw.TotalMilliseconds;
                if (sinceLastFlip >= millisPerFlip)
                {
                    sinceLastFlip -= millisPerFlip;
                    var pre = Current.Rotate(flipDirection);
                    PlayFlip();
                    if (isFlipDown && IsInBounds(CursorX, CursorY, pre)
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

            if (isDropDown && sincePieceEntered >= 500.0)
            {
                sinceLastDrop += sinceLastDraw.TotalMilliseconds;
                if (sinceLastDrop >= millisPerDrop)
                {
                    sinceLastDrop -= millisPerDrop;
                    if (IsInBounds(CursorX, CursorY + 1, Current)
                            && IsEmpty(CursorX, CursorY + 1, Current))
                    {
                        CursorY++;
                    }
                }
            }
            else
            {
                sinceLastDrop = millisPerDrop;
            }

            if (!isDropDown)
            {
                sinceLastAdvance += sinceLastDraw.TotalMilliseconds;
                sincePieceEntered += sinceLastDraw.TotalMilliseconds;

                if (sinceLastAdvance >= millisPerAdvance)
                {
                    sinceLastAdvance -= millisPerAdvance;

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
                            clearCount++;
                        }
                    }

                    if (clearCount > 0)
                    {
                        PlayLineClear(clearCount);
                        Score += clearCount * clearCount * 100;
                        UpdateSpeed(1);
                    }
                }
            }
        }
    }

    public void SetLeft(bool value)
    {
        isLeftDown = value;
    }

    public void SetRight(bool value)
    {
        isRightDown = value;
    }

    public void SetFlip(bool value)
    {
        isFlipDown = value;
        if (value)
        {
            flipDirection = Puzzle.Clockwise;
        }
    }

    public void SetReverseFlip(bool value)
    {
        isFlipDown = value;
        if (value)
        {
            flipDirection = Puzzle.CounterClockwise;
        }
    }

    public void SetDrop(bool value)
    {
        isDropDown = value;
    }

    public void TetrisClearRow(int row)
    {
        if (IsInBounds(0, row))
        {
            Clear(RowOrder, row);
            ShiftRowsDown();
        }
    }

    private void PlayThump()
    {
        Thump?.Invoke(this, EventArgs.Empty);
    }

    private void PlayLineClear(int numLines)
    {
        LineClear?.Invoke(this, new IntegerEventArgs(numLines));
    }

    private void PlayFlip()
    {
        Flip?.Invoke(this, EventArgs.Empty);
    }
}