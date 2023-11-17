namespace Juniper.Puzzles;

public struct Tile : IEquatable<Tile?>
{
    public ConsoleColor Fore { get; set; }
    public ConsoleColor Back { get; set; }
    public char Token { get; set; }

    public readonly bool Equals(Tile? other)
    {
        return other is not null
            && other.Value.Fore == Fore
            && other.Value.Back == Back
            && other.Value.Token == Token;
    }

    public readonly override bool Equals(object? obj)
    {
        return obj is Tile t
            && Equals(t);
    }

    public readonly override int GetHashCode()
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
