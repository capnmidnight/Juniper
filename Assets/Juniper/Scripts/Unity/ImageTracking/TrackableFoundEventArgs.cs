using System;

namespace Juniper.ImageTracking
{
    /// <summary>
    /// Event arguments for AR tracked objects coming into and out of view.
    /// </summary>
    public class TrackingStateChangeEventArgs : EventArgs
    {
        /// <summary>
        /// The object that was found.
        /// </summary>
        public readonly TrackableFoundEventHandler Source;

        /// <summary>
        /// The new state of tracking, either true (Tracking) or false (Lost).
        /// </summary>
        public readonly bool Tracking;

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Juniper.XR.TrackingStateChangeEventArgs"/> class.
        /// </summary>
        /// <param name="src">Source.</param>
        /// <param name="tracking">If set to <c>true</c> tracking.</param>
        public TrackingStateChangeEventArgs(TrackableFoundEventHandler src, bool tracking)
        {
            Source = src;
            Tracking = tracking;
        }
    }
}
