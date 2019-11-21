using System;

namespace Juniper.Primrose
{
    public class Token : ICloneable
    {
        public string value, type;
        public int index, line;

        public Token(string value, string type, int index, int line)
        {
            this.value = value;
            this.type = type;
            this.index = index;
            this.line = line;
        }

        public Token(string value, string type)
            : this(value, type, -1, -1)
        { }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public Token Clone()
        {
            return new Token(value, type, index, line);
        }

        public Token SplitAt(int i)
        {
            var next = value.Substring(i);
            value = value.Substring(0, i);
            return new Token(next, type, index + i, line);
        }

        public override string ToString()
        {
            return $"[{type}: {value}]";
        }
    }
}