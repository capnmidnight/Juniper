using System;

namespace Juniper.Animation
{
    /// <summary>
    /// Changes in the transition value can be captured in other scripts by subscribing to the
    /// ValueChanged event. This EventArgs object includes both the old and the new value.
    /// </summary>
    public class TransitionValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The value before the change happens.
        /// </summary>
        public float OldValue { get; }

        /// <summary>
        /// The value after the change happens.
        /// </summary>
        public float NewValue { get; }

        /// <summary>
        /// Creates a new transition value change event.
        /// </summary>
        /// <param name="oldf">Oldf.</param>
        /// <param name="newf">Newf.</param>
        public TransitionValueChangedEventArgs(float oldf, float newf)
        {
            OldValue = oldf;
            NewValue = newf;
        }
    }
}