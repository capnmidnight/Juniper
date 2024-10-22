namespace Juniper.Haptics;

/// <summary>
/// Pre-canned feedback patterns. These are defined by iOS's Taptic feedback system, which is the
/// only feedback system that is readily available to us. It's fairly useful, so we reimplement
/// it on Android.
/// </summary>
public enum HapticExpression
{
    /// <summary>
    /// No interaction, do nothing.
    /// </summary>
    None,

    /// <summary>
    /// A light tap to indicate a change in selection.
    /// </summary>
    SelectionChange,

    /// <summary>
    /// A light tap, slightly stronger than <see cref="SelectionChange"/>.
    /// </summary>
    Light,

    /// <summary>
    /// A medium tap, stronger than <see cref="Light"/>/
    /// </summary>
    Medium,

    /// <summary>
    /// A heavy thud, stronger than <see cref="Medium"/>.
    /// </summary>
    Heavy,

    /// <summary>
    /// A pattern of feedback pulses that evokes "Warning".
    /// </summary>
    Warning,

    /// <summary>
    /// A pattern of feedback pulses that evokes "Error".
    /// </summary>
    Error,

    /// <summary>
    /// A pattern of feedback pulses that evokes "Success".
    /// </summary>
    Success,

    /// <summary>
    /// A button press and release.
    /// </summary>
    Click,

    /// <summary>
    /// The first half of a click.
    /// </summary>
    Press,

    /// <summary>
    /// The second half of a click.
    /// </summary>
    Release
}