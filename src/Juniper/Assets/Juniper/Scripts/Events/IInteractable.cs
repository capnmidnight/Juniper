namespace Juniper.Events
{
    /// <summary>
    /// An interface for custom controls to report to the
    /// <see cref="Input.UnifiedInputModule"/> that they
    /// are enabled/disabled for UI interaction (but still
    /// enabled for processing purposes). The input module
    /// uses this information to play the right "disabled"
    /// sound if the control is clicked.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Should interaction with this control continue, or
        /// be prevented with an "denied" notification to the
        /// user?
        /// </summary>
        /// <returns>
        /// True when the control can proceed with processing,
        /// False when the user needs to be told to stahp.
        /// </returns>
        bool IsInteractable();
    }
}
