using System;
using System.Globalization;
using System.Text;

namespace Juniper.Puzzles
{
    /// <summary>
    /// A physical representation of a puzzle as a 2D array of tile pieces. The grid can be
    /// manipulated in various ways to help facilitate rapid puzzle creations.
    /// </summary>
    public class Puzzle : IComparable<Puzzle>
    {
        public static readonly int EmptyTile = -1;
        public static readonly bool RowOrder = true;
        public static readonly bool ColumnOrder = !RowOrder;
        public static readonly bool Clockwise = true;
        public static readonly bool CounterClockwise = !Clockwise;
        private int[,] grid;

        /// <summary>
        /// Create an empty puzzle grid with a specified width and height
        /// </summary>
        /// <param name="gridWidth"></param>
        /// <param name="gridHeight"></param>
        public Puzzle(int gridWidth, int gridHeight)
        {
            if (gridWidth <= 0 || gridHeight <= 0)
            {
                throw new ArgumentException("Width and height dimensions must be greater than 0");
            }

            grid = new int[gridWidth, gridHeight];
            for (var y = 0; y < gridHeight; ++y)
            {
                for (var x = 0; x < gridWidth; ++x)
                {
                    grid[x, y] = EmptyTile;
                }
            }
        }

        /// <summary>
        /// Create a puzzle grid from a 2D array of values
        /// </summary>
        /// <param name="grid"></param>
        public Puzzle(int[,] grid)
        {
            Grid = grid;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var y = 0; y < Height; ++y)
            {
                for (var x = 0; x < Width; ++x)
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, "{0},", grid[x, y]);
                }

                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the width of the puzzle grid
        /// </summary>
        public int Width => grid.GetWidth();
        /// <summary>
        /// Gets the height of the puzzle grid
        /// </summary>
        public int Height => grid.GetHeight();

        /// <summary>
        /// Gets or sets a specific value on the puzzle grid
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int this[int x, int y]
        {
            get
            {
                if (IsInBounds(x, y))
                {
                    return grid[x, y];
                }

                return EmptyTile;
            }
            set
            {
                if (IsInBounds(x, y))
                {
                    grid[x, y] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the raw puzzle grid array
        /// </summary>
        public int[,] Grid
        {
            get => grid;
            set
            {
                if (value != null && value.GetWidth() != 0 && value.GetHeight() != 0)
                {
                    grid = new int[value.GetWidth(), value.GetHeight()];
                    Array.Copy(value, grid, value.Length);
                }
                else
                {
                    throw new ArgumentException("Invalid initialization grid");
                }
            }
        }

        /// <summary>
        /// Clear the entire board
        /// </summary>
        public void Clear()
        {
            Clear(0, 0, Width, Height);
        }

        /// <summary>
        /// Clear a single point on the puzzle grid
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Clear(int x, int y)
        {
            Clear(x, y, 1, 1);
        }

        /// <summary>
        /// Clear a row or a column on the puzzle grid
        /// </summary>
        /// <param name="order">Puzzle.RowOrder to clear a row, Puzzle.ColumnOrder to clear a column</param>
        /// <param name="ordinal">the row or column index to clear</param>
        public void Clear(bool order, int ordinal)
        {
            if (order == RowOrder)
            {
                Clear(0, ordinal, Width, 1);
            }
            else
            {
                Clear(ordinal, 0, 1, Height);
            }
        }

        /// <summary>
        /// Clear a rectangular block on the puzzle grid
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Clear(int left, int top, int width, int height)
        {
            Fill(left, top, width, height, EmptyTile);
        }

        /// <summary>
        /// Batch clear specific points on the grid
        /// </summary>
        /// <param name="mask">a 2D array of flags marking points that should be cleared. The array should be as
        /// large as the puzzle grid</param>
        public void Clear(int[,] mask)
        {
            if (mask != null
                && mask.GetWidth() == Width
                && mask.GetHeight() == Height)
            {
                for (var x = 0; x < Width; ++x)
                {
                    for (var y = 0; y < Height; ++y)
                    {
                        if (mask[x, y] != EmptyTile)
                        {
                            grid[x, y] = EmptyTile;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clear an irregularly shaped area of the board
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="shape">a 2D array of flags marking points that should be cleared.</param>
        public void Clear(int left, int top, int[,] shape)
        {
            if (shape != null)
            {
                for (var x = left; x < shape.GetWidth() + left; ++x)
                {
                    if (x >= 0 && x < Width)
                    {
                        for (var y = top; y < shape.GetHeight() + top; ++y)
                        {
                            if (y >= 0 && y < Height && shape[x - left, y - top] != EmptyTile)
                            {
                                grid[x, y] = EmptyTile;
                            }
                        }
                    }
                }
            }
        }

        public void Clear(int left, int top, Puzzle shape)
        {
            if (shape != null)
            {
                Clear(left, top, shape.grid);
            }
        }

        /// <summary>
        /// Fill the entire puzzle grid with a single value
        /// </summary>
        /// <param name="value"></param>
        public void Fill(int value)
        {
            Fill(0, 0, Width, Height, value);
        }

        /// <summary>
        /// Fill a row or column with a single value
        /// </summary>
        /// <param name="order">Puzzle.RowOrder to fill a row, Puzzle.ColumnOrder to fill a column</param>
        /// <param name="ordinal">the row or column index to fill</param>
        /// <param name="value"></param>
        public void Fill(bool order, int ordinal, int value)
        {
            if (order == RowOrder)
            {
                Fill(0, ordinal, Width, 1, value);
            }
            else
            {
                Fill(ordinal, 0, 1, Height, value);
            }
        }

        /// <summary>
        /// Fill a rectangular portion of the board
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="value"></param>
        public void Fill(int left, int top, int width, int height, int value)
        {
            for (var x = left; x < left + width && x < Width; ++x)
            {
                if (x >= 0)
                {
                    for (var y = top; y < top + height && y < Height; ++y)
                    {
                        if (y >= 0)
                        {
                            grid[x, y] = value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Use a pseudorandom number generator to fill the entire board
        /// </summary>
        /// <param name="prand"></param>
        public void Fill(Random prand)
        {
            if (prand != null)
            {
                for (var x = 0; x < Width; ++x)
                {
                    for (var y = 0; y < Height; ++y)
                    {
                        grid[x, y] = prand.Next();
                    }
                }
            }
        }

        /// <summary>
        /// Use a pseudorandom number generator to fill the entire board
        /// </summary>
        public void Fill()
        {
            Fill(new Random());
        }

        /// <summary>
        /// Copy a puzzle piece on top of the current puzzle at a specific location.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="piece"></param>
        public void Fill(int left, int top, Puzzle piece)
        {
            if (piece != null)
            {
                Fill(left, top, piece.grid);
            }
        }

        /// <summary>
        /// Fill a specific area with a shape. Ignores empty tiles in the shape.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="shape"></param>
        public void Fill(int left, int top, int[,] shape)
        {
            if (shape != null)
            {
                for (var x = left; x < left + shape.GetWidth() && x < Width; ++x)
                {
                    if (x >= 0)
                    {
                        for (var y = top; y < top + shape.GetHeight() && y < Height; ++y)
                        {
                            if (y >= 0 && shape[x - left, y - top] != EmptyTile)
                            {
                                grid[x, y] = shape[x - left, y - top];
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Make tiles above empty spots shift down into the empty spot, leaving an empty spot behind them.
        /// This is a modification of Insertion Sort, and is an O(n^3) operation.
        /// </summary>
        public void ShiftColumnsDown()
        {
            for (var x = 0; x < Width; ++x)
            {
                for (var y = Height - 1; y > 0; --y)
                {
                    for (var y2 = y - 1; y2 >= 0; --y2)
                    {
                        if (grid[x, y] == EmptyTile)
                        {
                            grid[x, y] = grid[x, y];
                            grid[x, y2] = EmptyTile;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Make tiles below empty spots shift up into the empty spot, leaving an empty spot behind them.
        /// This is a modification of Insertion Sort, and is an O(n^3) operation.
        /// </summary>
        public void ShiftColumnsUp()
        {
            for (var x = 0; x < Width; ++x)
            {
                for (var y = 0; y < Height - 1; ++y)
                {
                    for (var y2 = y + 1; y2 < Height; ++y2)
                    {
                        if (grid[x, y] == EmptyTile)
                        {
                            grid[x, y] = grid[x, y2];
                            grid[x, y2] = EmptyTile;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Make tiles to the right empty spots shift left into the empty spot, leaving an empty spot behind them.
        /// This is a modification of Insertion Sort, and is an O(n^3) operation.
        /// </summary>
        public void ShiftColumnsLeft()
        {
            for (var x = 0; x < Width - 1; ++x)
            {
                for (var x2 = x + 1; x2 < Width; ++x2)
                {
                    for (var y = 0; y < Height; ++y)
                    {
                        if (grid[x, y] == EmptyTile)
                        {
                            grid[x, y] = grid[x2, y];
                            grid[x2, y] = EmptyTile;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Make tiles to the left empty spots shift right into the empty spot, leaving an empty spot behind them.
        /// This is a modification of Insertion Sort, and is an O(n^3) operation.
        /// </summary>
        public void ShiftColumnsRight()
        {
            for (var x = Width - 1; x > 0; --x)
            {
                for (var x2 = x - 1; x2 >= 0; --x2)
                {
                    for (var y = 0; y < Height; ++y)
                    {
                        if (grid[x, y] == EmptyTile)
                        {
                            grid[x, y] = grid[x2, y];
                            grid[x2, y] = EmptyTile;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determine if a point is within the bounds of the board
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Width
                && y >= 0 && y < Height;
        }

        /// <summary>
        /// Determine if a rectangle is completely contained within the board
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public bool IsInBounds(int x, int y, int width, int height)
        {
            return IsInBounds(x, y) && IsInBounds(x + width - 1, y + height - 1);
        }

        /// <summary>
        /// Determine if a shape is completely contained within the board
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public bool IsInBounds(int x, int y, int[,] shape)
        {
            if (shape != null && shape.Length > 0)
            {
                for (var dx = 0; dx < shape.GetWidth(); ++dx)
                {
                    for (var dy = 0; dy < shape.GetHeight(); ++dy)
                    {
                        {
                            if (shape[dx, dy] != EmptyTile && !IsInBounds(x + dx, y + dy))
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Determine if a puzzle piece is completely contained within the board
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public bool IsInBounds(int x, int y, Puzzle shape)
        {
            return shape != null && IsInBounds(x, y, shape.grid);
        }

        /// <summary>
        /// Compares two puzzles to see if they match.
        /// </summary>
        /// <param name="p"></param>
        /// <returns>+1 if the dimensions do not match
        /// -1 if at least one of the points do not match
        /// 0 if the puzzles are equivalent</returns>
        public int CompareTo(Puzzle p)
        {
            if (p is null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            if (Width == p.Width && Height == p.Height)
            {
                for (var x = 0; x < Width; ++x)
                {
                    for (var y = 0; y < Height; ++y)
                    {
                        if (this[x, y] != p[x, y])
                        {
                            return -1;
                        }
                    }
                }

                return 0;
            }
            return +1;
        }

        /// <summary>
        /// Compare two puzzles to see if they match element-for-element
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Puzzle)
            {
                return CompareTo(obj as Puzzle) == 0;
            }

            return false;
        }

        /// <summary>
        /// A recommended override if one overrides the Equals method
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return grid.GetHashCode();
        }

        /// <summary>
        /// Swap two specific points on the puzzle
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void Swap(int x1, int y1, int x2, int y2)
        {
            Swap(x1, y1, x2, y2, 1, 1);
        }

        /// <summary>
        /// Swap two rows or two columns
        /// </summary>
        /// <param name="order"></param>
        /// <param name="ordinal1"></param>
        /// <param name="ordinal2"></param>
        public void Swap(bool order, int ordinal1, int ordinal2)
        {
            if (order == RowOrder)
            {
                Swap(0, ordinal1, 0, ordinal2, Width, 1);
            }
            else
            {
                Swap(ordinal1, 0, ordinal2, 0, 1, Height);
            }
        }

        /// <summary>
        /// Swap two rectangular areas that have the same dimensions but do not overlap
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Swap(int x1, int y1, int x2, int y2, int width, int height)
        {
            int temp;
            if (IsInBounds(x1, y1, width, height) && IsInBounds(x2, y2, width, height)
                && !RectsIntersect(x1, y1, x2, y2, width, height))
            {
                for (var x = 0; x < width; ++x)
                {
                    for (var y = 0; y < height; ++y)
                    {
                        temp = grid[x1 + x, y1 + y];
                        grid[x1 + x, y1 + y] = grid[x2 + x, y2 + y];
                        grid[x2 + x, y2 + y] = temp;
                    }
                }
            }
        }

        /// <summary>
        /// Check if two rectangles, of the same size in different locations, overlap each other.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static bool RectsIntersect(int x1, int y1, int x2, int y2, int width, int height)
        {
            System.Drawing.Rectangle a, b;
            a = new System.Drawing.Rectangle(x1, y1, width, height);
            b = new System.Drawing.Rectangle(x2, y2, width, height);
            return a.IntersectsWith(b);
        }

        /// <summary>
        /// Rotate the puzzle by 90 degrees in either a clockwise or counterclockwise direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns>A new puzzle that represents the rotated puzzle</returns>
        public Puzzle Rotate(bool direction)
        {
            var q = new Puzzle(Height, Width);

            if (direction == Clockwise)
            {
                for (var x = 0; x < Width; ++x)
                {
                    for (var y = 0; y < Height; ++y)
                    {
                        q[q.Width - y - 1, x] = this[x, y];
                    }
                }
            }
            else
            {
                for (var x = 0; x < Width; ++x)
                {
                    for (var y = 0; y < Height; ++y)
                    {
                        q[y, q.Height - x - 1] = this[x, y];
                    }
                }
            }

            return q;
        }

        /// <summary>
        /// Creates a copy of the current puzzle
        /// </summary>
        /// <returns></returns>
        public Puzzle Duplicate()
        {
            return new Puzzle(grid);
        }

        /// <summary>
        /// Check to see if the board is full
        /// </summary>
        /// <returns></returns>
        public bool IsFull()
        {
            return IsFull(0, 0, Width, Height);
        }

        /// <summary>
        /// Check to see if a row or column is full
        /// </summary>
        /// <param name="order"></param>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public bool IsFull(bool order, int ordinal)
        {
            if (order == RowOrder)
            {
                return IsFull(0, ordinal, Width, 1);
            }
            else
            {
                return IsFull(ordinal, 0, 1, Height);
            }
        }

        /// <summary>
        /// Check to see if a rectangular area is full
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public bool IsFull(int x, int y, int width, int height)
        {
            for (var dx = 0; dx < width; ++dx)
            {
                for (var dy = 0; dy < height; ++dy)
                {
                    if (this[x + dx, y + dy] == EmptyTile)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Check to see if a mask shape is full
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public bool IsFull(int x, int y, int[,] shape)
        {
            if (shape == null || shape.Length == 0)
            {
                return false;
            }

            for (var dx = 0; dx < shape.GetWidth(); ++dx)
            {
                for (var dy = 0; dy < shape.GetHeight(); ++dy)
                {
                    if (shape[dx, dy] != EmptyTile && this[x + dx, y + dy] == EmptyTile)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Check to see if a puzzle piece shape is full
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public bool IsFull(int x, int y, Puzzle shape)
        {
            return shape != null && IsFull(x, y, shape.grid);
        }

        public bool IsEmpty()
        {
            return IsEmpty(0, 0, Width, Height);
        }

        public bool IsEmpty(bool order, int ordinal)
        {
            if (order == RowOrder)
            {
                return IsEmpty(0, ordinal, Width, 1);
            }
            else
            {
                return IsEmpty(ordinal, 0, 1, Height);
            }
        }

        public bool IsEmpty(int x, int y, int width, int height)
        {
            for (var dx = 0; dx < width; ++dx)
            {
                for (var dy = 0; dy < height; ++dy)
                {
                    if (this[x + dx, y + dy] != EmptyTile)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsEmpty(int x, int y, int[,] shape)
        {
            if (shape == null || shape.Length == 0)
            {
                return false;
            }

            for (var dx = 0; dx < shape.GetWidth(); ++dx)
            {
                for (var dy = 0; dy < shape.GetHeight(); ++dy)
                {
                    if (shape[dx, dy] != EmptyTile && this[x + dx, y + dy] != EmptyTile)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsEmpty(int x, int y, Puzzle shape)
        {
            return shape != null && IsEmpty(x, y, shape.grid);
        }

        public static bool operator ==(Puzzle left, Puzzle right)
        {
            return (left is null && right is null)
                || (left is object && left.Equals(right));
        }

        public static bool operator !=(Puzzle left, Puzzle right)
        {
            return !(left == right);
        }

        public static bool operator <(Puzzle left, Puzzle right)
        {
            return (left is null && right is object)
                || (left is object && left.CompareTo(right) < 0);
        }

        public static bool operator <=(Puzzle left, Puzzle right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(Puzzle left, Puzzle right)
        {
            return left is object && left.CompareTo(right) > 0;
        }

        public static bool operator >=(Puzzle left, Puzzle right)
        {
            return (left is null && right is null)
                || (left is object && left.CompareTo(right) >= 0);
        }
    }
}