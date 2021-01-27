using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Juniper.World.GIS.Google.MapTiles
{
    [Serializable]
    public sealed class LinePathStyle : ISerializable, IEquatable<LinePathStyle>
    {
        private readonly string styleDef;

        public LinePathStyle(int weight = 5, string color = null, string fillcolor = null, bool geodesic = false)
        {
            var sb = new StringBuilder();

            void delim(bool check, string name, string value)
            {
                if (check)
                {
                    if (sb.Length > 0)
                    {
                        _ = sb.Append('|');
                    }

                    _ = sb.Append(name)
                        .Append(':')
                        .Append(value);
                }
            }

            delim(weight != 5, nameof(weight), weight.ToString(CultureInfo.InvariantCulture));
            delim(!string.IsNullOrEmpty(color), nameof(color), color);
            delim(!string.IsNullOrEmpty(fillcolor), nameof(fillcolor), fillcolor);
            delim(geodesic, nameof(geodesic), geodesic ? "true" : "false");

            styleDef = sb.ToString();
        }

        private LinePathStyle(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            styleDef = info.GetString(nameof(styleDef));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(styleDef), styleDef);
        }

        public override string ToString()
        {
            return styleDef;
        }

        public static explicit operator string(LinePathStyle style)
        {
            return style?.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is LinePathStyle style && Equals(style);
        }

        public bool Equals(LinePathStyle other)
        {
            return other is object
                && styleDef == other.styleDef;
        }

        public override int GetHashCode()
        {
            return -340221435 + EqualityComparer<string>.Default.GetHashCode(styleDef);
        }

        public static bool operator ==(LinePathStyle left, LinePathStyle right)
        {
            return ReferenceEquals(left, right)
                || (left is object && left.Equals(right));
        }

        public static bool operator !=(LinePathStyle left, LinePathStyle right)
        {
            return !(left == right);
        }
    }
}