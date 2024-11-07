namespace Juniper.Haptics;

/// <summary>
/// Pattern elements combine together into a series to be played together as a pattern. This is
/// not necessary on iOS, as the patterns have already been defined for us. But on Android, there
/// are no defined patterns, so we have to try to match them as closely as possible.
/// </summary>
public struct HapticPatternElement :
    IEquatable<HapticPatternElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Juniper.Haptics.HapticPatternElement"/> struct.
    /// </summary>
    /// <param name="len">Length.</param>
    /// <param name="amp">Amp.</param>
    public HapticPatternElement(long len, float amp)
    {
        Length = len;
        Amplitude = amp;
    }

    /// <summary>
    /// The amount of time, in milliseconds, to play the pulse.
    /// </summary>
    /// <value>The length.</value>
    public long Length
    {
        get; set;
    }

    /// <summary>
    /// The strength of the vibration pulse, on the range [0, 1]
    /// </summary>
    /// <value>The amplitude.</value>
    public float Amplitude
    {
        get; set;
    }

    /// <summary>
    /// Retrieves an array of HapticPatternElements for a given HapticExpression instructs the
    /// device to play that pattern.
    /// </summary>
    /// <param name="device">    Device.</param>
    /// <param name="expression">Expression.</param>
    public static void Play(IHapticDevice device, HapticExpression expression)
    {
        if (device != null
            && expressions.ContainsKey(expression))
        {
            device.Play(expressions[expression]);
        }
    }

    /// <summary>
    /// Stored patterns to use on systems other than iOS that do not have the patterns built in.
    /// </summary>
    private static readonly Dictionary<HapticExpression, HapticPatternElement[]> expressions = new Dictionary<HapticExpression, HapticPatternElement[]>(10)
    {
        { HapticExpression.SelectionChange, new []{
            new HapticPatternElement(10, 1f/6f) } },

        { HapticExpression.Light, new []{
            new HapticPatternElement(125, 1f/3f) } },

        { HapticExpression.Medium, new []{
            new HapticPatternElement(125, 2f/3f) } },

        { HapticExpression.Heavy, new []{
            new HapticPatternElement(125, 1f) } },

        { HapticExpression.Success, new []{
            new HapticPatternElement(50, 1f/3f),
            new HapticPatternElement(20, 0),
            new HapticPatternElement(50, 1f/3f),
            new HapticPatternElement(125, 2f/3f)} },

        { HapticExpression.Warning, new []{
            new HapticPatternElement(125, 0.5f),
            new HapticPatternElement(20, 0),
            new HapticPatternElement(125, 0.5f) } },

        { HapticExpression.Error, new []{
            new HapticPatternElement(125, 1f),
            new HapticPatternElement(20, 0),
            new HapticPatternElement(125, 1f),
            new HapticPatternElement(20, 0),
            new HapticPatternElement(125, 1f) } },

        { HapticExpression.Click, new []{
            new HapticPatternElement(10, 2f / 6f),
            new HapticPatternElement(5, 0),
            new HapticPatternElement(10, 1f / 6f) } },

        { HapticExpression.Press, new []{
            new HapticPatternElement(10, 2f / 6f) } },

        { HapticExpression.Release, new []{
            new HapticPatternElement(10, 1f / 6f) } }
    };

    public override readonly bool Equals(object? obj)
    {
        return obj is HapticPatternElement p
            && Equals(p);
    }

    public readonly bool Equals(HapticPatternElement p)
    {
        return p.Amplitude == Amplitude
            && p.Length == Length;
    }

    public override readonly int GetHashCode()
    {
        return Length.GetHashCode()
            ^ Amplitude.GetHashCode();
    }

    public static bool operator ==(HapticPatternElement left, HapticPatternElement right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(HapticPatternElement left, HapticPatternElement right)
    {
        return !(left == right);
    }
}