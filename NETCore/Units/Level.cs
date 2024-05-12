namespace Juniper.Units;

/// <summary>
/// A low/medium/high value. Usable for difficulty settings, or other, vague settings of levels.
/// </summary>
public enum Level
{
    None,

    /// <summary>
    /// The minimum setting.
    /// </summary>
    Low,

    /// <summary>
    /// The midpoint setting.
    /// </summary>
    Medium,

    /// <summary>
    /// The maximum setting.
    /// </summary>
    High
}