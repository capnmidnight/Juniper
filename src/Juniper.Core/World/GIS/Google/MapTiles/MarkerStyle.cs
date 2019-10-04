using System;
using System.Runtime.Serialization;
using System.Text;

namespace Juniper.GIS.Google.MapTiles
{
    [Serializable]
    public sealed class MarkerStyle : ISerializable, IEquatable<MarkerStyle>
    {
        public static MarkerStyle CustomIcon(Uri image)
        {
            return new MarkerStyle($"icon:{image}");
        }

        public static MarkerStyle CustomIcon(Uri image, MarkerAnchorPosition anchorPosition)
        {
            return new MarkerStyle($"anchor:{anchorPosition.ToString()}:icon:{image}");
        }

        public static MarkerStyle CustomIcon(Uri image, int anchorX, int anchorY)
        {
            return new MarkerStyle($"anchor:{anchorX.ToString()},{anchorY.ToString()}|icon:{image}");
        }

        private readonly string styleDef;

        private MarkerStyle(string style)
        {
            styleDef = style;
        }

        public MarkerStyle(string color = null, char label = char.MinValue, MarkerSize size = MarkerSize.normal)
        {
            if (label != char.MinValue && !char.IsLetterOrDigit(label))
            {
                throw new ArgumentException(nameof(label), "Value must be an uppercase letter A-Z or a digit 0-9.");
            }

            var sb = new StringBuilder();

            void delim(bool check, string name, string value)
            {
                if (check)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append('|');
                    }
                    sb.Append(name);
                    sb.Append(':');
                    sb.Append(value);
                }
            }

            delim(size != MarkerSize.normal, nameof(size), size.ToString());
            delim(!string.IsNullOrEmpty(color), nameof(color), color);
            delim(char.IsLetterOrDigit(label), nameof(label), label.ToString());

            styleDef = sb.ToString();
        }

        private MarkerStyle(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return styleDef;
        }

        public static explicit operator string(MarkerStyle style)
        {
            return style.ToString();
        }

        public override int GetHashCode()
        {
            return styleDef.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is MarkerStyle style && Equals(style);
        }

        public bool Equals(MarkerStyle other)
        {
            return other is object
                && styleDef == other.styleDef;
        }

        public static bool operator ==(MarkerStyle left, MarkerStyle right)
        {
            return ReferenceEquals(left, right)
                || left is object && left.Equals(right);
        }

        public static bool operator !=(MarkerStyle left, MarkerStyle right)
        {
            return !(left == right);
        }
    }
}