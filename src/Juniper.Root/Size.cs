using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Juniper
{
    [Serializable]
    public sealed class Size : ISerializable, IEquatable<Size>
    {
        private static readonly Regex SizePattern = new Regex("^(\\d+)x(\\d+)$", RegexOptions.Compiled);
        private static readonly string WIDTH_FIELD = nameof(Width).ToLowerInvariant();
        private static readonly string HEIGHT_FIELD = nameof(Height).ToLowerInvariant();

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
                throw new FormatException("Input string must be format \"[width]x[height]\", where width and height are positive integers");
            }
        }

        public int Width { get; }
        public int Height { get; }

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        private Size(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            Width = info.GetInt32(WIDTH_FIELD);
            Height = info.GetInt32(HEIGHT_FIELD);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(WIDTH_FIELD, Width);
            info.AddValue(HEIGHT_FIELD, Height);
        }

        public override bool Equals(object obj)
        {
            return obj is Size size && Equals(size);
        }

        public bool Equals(Size other)
        {
            return other is object
                && Width == other.Width
                && Height == other.Height;
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

        public string ToString(IFormatProvider formatter)
        {
            return Width.ToString(formatter)
                + "x"
                + Height.ToString(formatter);
        }

        public override int GetHashCode()
        {
            var hashCode = 859600377;
            hashCode = (hashCode * -1521134295) + Width.GetHashCode();
            hashCode = (hashCode * -1521134295) + Height.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }

        public static explicit operator string(Size size)
        {
            return size?.ToString(CultureInfo.InvariantCulture);
        }
    }
}