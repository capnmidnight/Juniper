using System.Text;

namespace Juniper.Google.Maps.MapTiles
{
    public struct LinePathStyle
    {
        private readonly string styleDef;

        public LinePathStyle(int weight = 5, string color = null, string fillcolor = null, bool geodesic = false)
        {
            var sb = new StringBuilder();

            void delim(bool check, string name, object value)
            {
                if (check)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append('|');
                    }

                    sb.Append($"{name}:{value}");
                }
            }

            delim(weight != 5, nameof(weight), weight);
            delim(!string.IsNullOrEmpty(color), nameof(color), color);
            delim(!string.IsNullOrEmpty(fillcolor), nameof(fillcolor), fillcolor);
            delim(geodesic, nameof(geodesic), geodesic ? "true" : "false");

            styleDef = sb.ToString();
        }


        public override string ToString()
        {
            return styleDef;
        }

        public static explicit operator string(LinePathStyle style)
        {
            return style.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj != null
                && obj is LinePathStyle style
                && style.styleDef == styleDef;
        }

        public override int GetHashCode()
        {
            return styleDef.GetHashCode();
        }

        public static bool operator ==(LinePathStyle left, LinePathStyle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LinePathStyle left, LinePathStyle right)
        {
            return !(left == right);
        }
    }
}
