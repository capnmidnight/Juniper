using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Juniper.Imaging
{
    [Serializable]
    public sealed class Size : ISerializable, IEquatable<Size>
    {
        private static readonly Regex SizePattern = new Regex("^(\\d+)x(\\d+)$", RegexOptions.Compiled);

        public static bool TryParse(string widthXHeight, out Size size)
        {
            var match = SizePattern.Match(widthXHeight);
            if (!match.Success)
            {
                size = default;
            }
            else
            {
                var widthString = match.Captures[1].Value;
                var heightString = match.Captures[2].Value;
                var width = int.Parse(widthString, CultureInfo.InvariantCulture);
                var height = int.Parse(heightString, CultureInfo.InvariantCulture);
                size = new Size(width, height);
            }

            return match.Success;
        }

        public static Size Parse(string widthXHeight)
        {
            if (TryParse(widthXHeight, out var size))
            {
                return size;
            }
            else
            {
                throw new FormatException("input string must be format \"[width]x[height]\", where width and height are positive integers");
            }
        }

        public readonly int width;
        public readonly int height;

        public Size(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        private Size(SerializationInfo info, StreamingContext context)
        {
            width = info.GetInt32(nameof(width));
            height = info.GetInt32(nameof(height));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(width), width);
            info.AddValue(nameof(height), height);
        }

        public override int GetHashCode()
        {
            return width.GetHashCode()
                ^ height.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Size size && Equals(size);
        }

        public bool Equals(Size other)
        {
            return other is object
                && width == other.width
                && height == other.height;
        }

        public static bool operator ==(Size left, Size right)
        {
            return ReferenceEquals(left, right)
                || (left is object && left.Equals(right));
        }

        public static bool operator !=(Size left, Size right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return width.ToString(CultureInfo.InvariantCulture)
                + "x"
                + height.ToString(CultureInfo.InvariantCulture);
        }

        public static explicit operator string(Size size)
        {
            return size.ToString();
        }
    }
}