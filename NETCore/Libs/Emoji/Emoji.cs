namespace Juniper;

public partial class Emoji
{
    public static Emoji operator +(Emoji left, Emoji right) =>
        new(left.Value + right.Value, left.Desc + " " + right.Desc);

    public static string operator +(string left, Emoji right) =>
        left + right.Value;

    public static string operator +(Emoji left, string right) =>
        left.Value + right;

    public static string operator +(int left, Emoji right) =>
        char.ConvertFromUtf32(left) + right.Value;

    public static string operator +(Emoji left, int right) =>
        left.Value + char.ConvertFromUtf32(right);

    public static implicit operator string(Emoji emoji) =>
        emoji.Value;

    public override string ToString() =>
        Value;

    public string Value { get; }
    public string Desc { get; }

    public Emoji(string value, string description)
    {
        Value = value;
        Desc = description;
    }

    public Emoji(int value, string description)
        : this(char.ConvertFromUtf32(value), description)
    {
    }

    public Emoji(Emoji copy, string? altDescription = null)
        : this(copy.Value, altDescription ?? copy.Desc)
    {
    }

    public virtual Emoji Join(Emoji right, string? altDescription = null) =>
        new(this + zeroWidthJoiner + right, altDescription ?? (Desc + " " + right.Desc));

    public virtual Emoji Random() => 
        this;

    /// <summary>
    /// Determines of the provided Emoji or EmojiGroup is a subset of this emoji.
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public virtual bool Contains(Emoji e) =>
        Value.Contains(e.Value, System.StringComparison.CurrentCulture);
}
