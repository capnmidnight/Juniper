using System;

namespace Juniper.Primrose
{
    public class Token : ICloneable
    {
        public string Value { get; set; }

        public string Type { get; set; }

        public int Index { get; set; }

        public int Line { get; set; }

        public Token(string value, string type, int index, int line)
        {
            Value = value;
            Type = type;
            Index = index;
            Line = line;
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
            return new Token(Value, Type, Index, Line);
        }

        public Token SplitAt(int i)
        {
            var next = Value.Substring(i);
            Value = Value.Substring(0, i);
            return new Token(next, Type, Index + i, Line);
        }

        public override string ToString()
        {
            return $"[{Type}: {Value}]";
        }
    }
}