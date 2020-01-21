using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Juniper.World.GIS.Google.MapTiles
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
            return new MarkerStyle($"anchor:{anchorX.ToString(CultureInfo.InvariantCulture)},{anchorY.ToString(CultureInfo.InvariantCulture)}|icon:{image}");
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
                throw new ArgumentException("Value must be an uppercase letter A-Z or a digit 0-9.", nameof(label));
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
            delim(char.IsLetterOrDigit(label), nameof(label), label.ToString(CultureInfo.InvariantCulture));

            styleDef = sb.ToString();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private MarkerStyle(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return styleDef;
        }

        public static explicit operator string(MarkerStyle style)
        {
            return style?.ToString();
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

        public override int GetHashCode()
        {
            return -340221435 + EqualityComparer<string>.Default.GetHashCode(styleDef);
        }

        public static bool operator ==(MarkerStyle left, MarkerStyle right)
        {
            return ReferenceEquals(left, right)
                || (left is object && left.Equals(right));
        }

        public static bool operator !=(MarkerStyle left, MarkerStyle right)
        {
            return !(left == right);
        }
    }
}