namespace Juniper.Puzzles
{
    public struct Tile : IEquatable<Tile>
    {
        public ConsoleColor Fore { get; set; }
        public ConsoleColor Back { get; set; }
        public char Token { get; set; }

        public bool Equals(Tile other)
        {
            return other.Fore == Fore
                && other.Back == Back
                && other.Token == Token;
        }

        public override bool Equals(object obj)
        {
            return obj is Tile t
                && Equals(t);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Fore, Back, Token);
        }

        public static bool operator ==(Tile left, Tile right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Tile left, Tile right)
        {
            return !(left == right);
        }
    }
}
