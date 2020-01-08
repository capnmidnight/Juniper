using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Juniper.Primrose
{
    public class Cursor : IComparable<Cursor>
    {
        private static readonly Regex WordBoundary = new Regex("(\\s|\\W)+", RegexOptions.Compiled);

        public static Cursor Min(Cursor a, Cursor b)
        {
            if (a is object
                && b is object
                && a.i <= b.i)
            {
                return a;
            }

            return b;
        }

        public static Cursor Max(Cursor a, Cursor b)
        {
            if (a is object
                && b is object
                && a.i > b.i)
            {
                return a;
            }

            return b;
        }

        private int i;
        private int x;
        private int y;
        private bool moved;

        public Cursor(int i = 0, int x = 0, int y = 0)
        {
            this.i = i;
            this.x = x;
            this.y = y;
            moved = true;
        }

        public int CompareTo(Cursor other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return i.CompareTo(other.i);
        }

        public Cursor Clone()
        {
            return new Cursor(i, x, y);
        }

        public override string ToString()
        {
            return $"[i:{i.ToString(CultureInfo.CurrentCulture)} x:{x.ToString(CultureInfo.CurrentCulture)} y:{y.ToString(CultureInfo.CurrentCulture)}]";
        }

        public override int GetHashCode()
        {
            return i.GetHashCode()
                ^ x.GetHashCode()
                ^ y.GetHashCode();
        }

        public void Copy(Cursor other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            i = other.i;
            x = other.x;
            y = other.y;
            moved = false;
        }

        public void FullHome()
        {
            i = 0;
            x = 0;
            y = 0;
            moved = true;
        }

        public void FullEnd(string[] lines)
        {
            if (lines is null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            i = 0;
            var lastLength = 0;
            for (var y = 0; y < lines.Length; ++y)
            {
                var line = lines[y];
                lastLength = line.Length;
                i += lastLength;
            }

            y = lines.Length - 1;
            x = lastLength;
            moved = true;
        }

        public void SkipLeft(string[] lines)
        {
            if (lines is null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            if (x == 0)
            {
                Left(lines);
            }
            else
            {
                var x = this.x - 1;
                var line = lines[y];
                var word = line
                    .Substring(0, x)
                    .Reverse();
                var m = WordBoundary.Match(word);
                var dx = m.Success
                    ? (m.Index + m.Captures[0].Length + 1)
                    : word.Length;
                i -= dx;
                this.x -= dx;
            }

            moved = true;
        }

        public void Left(string[] lines)
        {
            if (lines is null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            if (i > 0)
            {
                --i;
                --x;
                if (x < 0)
                {
                    --y;
                    var line = lines[y];
                    x = line.Length;
                }

                if (ReverseFromNewline(lines))
                {
                    ++i;
                }
            }

            moved = true;
        }

        public void SkipRight(string[] lines)
        {
            if (lines is null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            var line = lines[y];
            if (x == line.Length || line[x] == '\n')
            {
                Right(lines);
            }
            else
            {
                var x = this.x + 1;
                line = line.Substring(x);
                var m = WordBoundary.Match(line);
                var dx = m.Success
                    ? (m.Index + m.Captures[0].Length + 1)
                    : (line.Length - x);
                i += dx;
                this.x += dx;
                ReverseFromNewline(lines);
            }

            moved = true;
        }

        public bool FixCursor(string[] lines)
        {
            if (lines is null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            x = i;
            y = 0;
            var total = 0;
            var line = lines[y];
            while (x > line.Length)
            {
                x -= line.Length;
                total += line.Length;
                if (y >= lines.Length - 1)
                {
                    i = total;
                    x = line.Length;
                    moved = true;
                    break;
                }

                ++y;
                line = lines[y];
            }

            return moved;
        }

        public void Right(string[] lines)
        {
            AdvanceN(lines, 1);
        }

        public void AdvanceN(string[] lines, int n)
        {
            if (lines is null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            var line = lines[y];
            if (y < lines.Length - 1 || x < line.Length)
            {
                i += n;
                FixCursor(lines);
                line = lines[y];
                if (x > 0 && line[x - 1] == '\n')
                {
                    ++y;
                    x = 0;
                }
            }

            moved = true;
        }

        public void Home()
        {
            i -= x;
            x = 0;
            moved = true;
        }

        public void End(string[] lines)
        {
            if (lines is null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            var line = lines[y];
            var dx = line.Length - x;
            i += dx;
            x += dx;
            ReverseFromNewline(lines);
            moved = true;
        }

        public void Up(string[] lines)
        {
            if (lines is null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            if (y > 0)
            {
                --y;
                var line = lines[y];
                var dx = Math.Min(0, line.Length - x);
                x += dx;
                i -= line.Length - dx;
                ReverseFromNewline(lines);
            }

            moved = true;
        }

        public void Down(string[] lines)
        {
            if (lines is null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            if (y < lines.Length - 1)
            {
                ++y;
                var line = lines[y];
                var pLine = lines[y - 1];
                var dx = Math.Min(0, line.Length - x);
                x += dx;
                i += pLine.Length + dx;
                ReverseFromNewline(lines);
            }

            moved = true;
        }

        public void IncY(int dy, string[] lines)
        {
            if (lines is null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            y = Math.Max(0, Math.Min(lines.Length - 1, y + dy));
            var line = lines[y];
            x = Math.Max(0, Math.Min(line.Length, x));
            i = x;
            for (var i = 0; i < y; ++i)
            {
                i += lines[i].Length;
            }

            ReverseFromNewline(lines);
            moved = true;
        }

        public void SetXY(int x, int y, string[] lines)
        {
            if (lines is null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            y = Math.Max(0, Math.Min(lines.Length - 1, y));
            var line = lines[y];
            x = Math.Max(0, Math.Min(line.Length, x));
            i = x;
            for (var i = 0; i < y; ++i)
            {
                i += lines[i].Length;
            }

            ReverseFromNewline(lines);
            moved = true;
        }

        public void SetI(int i, string[] lines)
        {
            this.i = i;
            FixCursor(lines);
            moved = true;
        }

        public bool ReverseFromNewline(string[] lines)
        {
            if (lines is null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            var line = lines[y];
            if (x > 0 && line[x - 1] == '\n')
            {
                --x;
                --i;
                return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is Cursor other && CompareTo(other) == 0;
        }

        public static bool operator ==(Cursor left, Cursor right)
        {
            return left is null ? right is null : left.Equals(right);
        }

        public static bool operator !=(Cursor left, Cursor right)
        {
            return !(left == right);
        }

        public static bool operator <(Cursor left, Cursor right)
        {
            return left is null ? right is object : left.CompareTo(right) < 0;
        }

        public static bool operator <=(Cursor left, Cursor right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(Cursor left, Cursor right)
        {
            return left is object && left.CompareTo(right) > 0;
        }

        public static bool operator >=(Cursor left, Cursor right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }
    }
}