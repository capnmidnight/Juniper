#if VUFORIA
using System;
using Vuforia;

namespace Juniper.Unity.ImageTracking
{
    [RequireComponent(typeof(TrackableBehaviour))]
    public abstract class VuforiaTrackableFoundEventHandler :
        AbstractTrackableFoundEventHandler,
        ITrackableEventHandler
    {
        /// <summary>
        /// The Vuforia tracked object.
        /// </summary>
        TrackableBehaviour trackable;

        /// <summary>
        /// Set to true to make the update loop print a debug report.
        /// </summary>
        public bool DebugReport;

        public override void Awake()
        {
            trackable = GetComponentInChildren<TrackableBehaviour>();
            trackable.RegisterTrackableEventHandler(this);

            base.Awake();
        }

        public override void Update()
        {
            base.Update();

            if (DebugReport)
            {
                PrintDebugReport();
            }
        }

        /// <summary>
        /// When running on Vuforia, de-registers the listeners for Vuforia's tracking state changed events.
        /// </summary>
        void OnDestroy()
        {
            trackable.UnregisterTrackableEventHandler(this);
        }

        /// <summary>
        /// Updates the tracking state of the target when Unity's OnTrackableStateChange event is fired.
        /// </summary>
        /// <param name="previousStatus">Previous status.</param>
        /// <param name="newStatus">New status.</param>
        public void OnTrackableStateChanged(
            TrackableBehaviour.Status previousStatus,
            TrackableBehaviour.Status newStatus)
        {
            if (DebugReport)
            {
                ScreenDebugger.Print("[{0}] {1} => {2}", trackable.TrackableName, previousStatus, newStatus);
            }

            Extended = trackable.CurrentStatus == TrackableBehaviour.Status.EXTENDED_TRACKED;
            Found = trackable.CurrentStatus == TrackableBehaviour.Status.DETECTED
                || trackable.CurrentStatus == TrackableBehaviour.Status.TRACKED
                || Extended;
        }

        /// <summary>
        /// If <see cref="DebugReport"/> is set to true, prints a report on the status of tracking.
        /// </summary>
        private void PrintDebugReport()
        {
            if (DebugReport)
            {
                if (trackable == null)
                {
                    ScreenDebugger.Print($"[{name}] No trackable object!");
                }
                else
                {
                    var msg = Extended ? "extended" : Found ? "FOUND" : "";
                    ScreenDebugger.Print($"[{name}] {trackable.TrackableName} -> {msg}");
                }
            }
        }
    }
}
#endif