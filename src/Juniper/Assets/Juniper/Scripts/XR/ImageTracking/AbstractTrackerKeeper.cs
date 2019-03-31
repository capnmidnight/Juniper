using System;
using System.Collections.Generic;
using System.Linq;

using Juniper.Unity.Display;

using UnityEngine;
using UnityEngine.Events;

namespace Juniper.Unity.ImageTracking
{
    public class AbstractTrackerKeeper : MonoBehaviour
    {
        /// <summary>
        /// When true, indicates that targets where available and tracked on the previous frame. Used
        /// to detect when all targets have been lost.
        /// </summary>
        private bool hadAnyTargets;

        /// <summary>
        /// All of the targets in the system.
        /// </summary>
        protected readonly List<AbstractTrackableFoundEventHandler> targets = new List<AbstractTrackableFoundEventHandler>();

        /// <summary>
        /// Is set to true when the tracking system is initialized and running.
        /// </summary>
        public bool Tracking
        {
            get; protected set;
        }

        /// <summary>
        /// The value of <see cref="Tracking"/> from the previous frame, to detect changes.
        /// </summary>
        private bool wasTracking;

        /// <summary>
        /// Set to true to print out a debug report on the screen.
        /// </summary>
        public bool DebugReport;

        /// <summary>
        /// Gets set to true when the application is shutting down, to prevent the invocation of any methods.
        /// </summary>
        private bool done;

        /// <summary>
        /// Returns true when any target has been found and is tracking.
        /// </summary>
        public bool TargetFound
        {
            get
            {
                foreach (var target in targets)
                {
                    if (target.Found)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// A Unity Event that occurs when the tracking system is about to be initialized. and started.
        /// </summary>
        public UnityEvent onTrackingStarting = new UnityEvent();

        /// <summary>
        /// A Unity Event that occurs when the tracking system has been initialized and started.
        /// </summary>
        public UnityEvent onTrackingStarted = new UnityEvent();

        /// <summary>
        /// A Unity Event that occurs when the tracking system is about to be shut down. and started.
        /// </summary>
        public UnityEvent onTrackingStopping = new UnityEvent();

        /// <summary>
        /// A Unity Event that occurs when the tracking system has been shut down.
        /// </summary>
        public UnityEvent onTrackingStopped = new UnityEvent();

        /// <summary>
        /// A Unity Event that occurs when the first target is found, after having no targets
        /// tracked. This event does not fire if any subsequent targets are found.
        /// </summary>
        public UnityEvent onAnyTargetFound = new UnityEvent();

        /// <summary>
        /// A Unity Event that occurs every time any target is found.
        /// </summary>
        public TransformEvent onTargetFound = new TransformEvent();

        /// <summary>
        /// A Unity Event that occurs every time any target is lost.
        /// </summary>
        public TransformEvent onTargetLost = new TransformEvent();

        /// <summary>
        /// A UnityEvent that occurs when all targets have been lost. If more than one target is
        /// currently being tracked, this event will not fire until all of them are lost.
        /// </summary>
        public UnityEvent onAllTargetsLost = new UnityEvent();

        /// <summary>
        /// A UnityEvent that occurs when the user is at a safe distance from the target.
        /// </summary>
        public TransformEvent onTargetSafe = new TransformEvent();

        /// <summary>
        /// A UnityEvent that occurs when the user gets too close to a target.
        /// </summary>
        public TransformEvent onTargetProximity = new TransformEvent();

        private DisplayManager xr;

        protected virtual IEnumerable<TrackableFoundEventHandler> FirstTargets
        {
            get
            {
                return ComponentExt.FindAll<TrackableFoundEventHandler>();
            }
        }

        /// <summary>
        /// Finds all of the targetable objects and makes sure they all have <see
        /// cref="TrackableFoundEventHandler"/> s, wiring up the onFound and onLost events of each.
        /// </summary>
        protected virtual void Start()
        {
            xr = ComponentExt.FindAny<DisplayManager>();

            foreach (var target in FirstTargets.Distinct())
            {
                Register(target);
            }
        }

        /// <summary>
        /// Register's a target and its events for the aggregated events issued by TrackerKeeper.
        /// </summary>
        /// <param name="target">Target.</param>
        public AbstractTrackableFoundEventHandler Register(AbstractTrackableFoundEventHandler target)
        {
            if (targets?.Contains(target) == false)
            {
                targets.Add(target);
                target.onFound.AddListener(TargetFinder(target));
                target.onLost.AddListener(TargetLoser(target));
                target.onProximity.AddListener(TargetProximity(target));
                target.onSafe.AddListener(TargetSafe(target));
            }

            return target;
        }

        protected virtual void OnTrackingStarting(string dataSetName, string targetName)
        {
            onTrackingStarting?.Invoke();
        }

        protected virtual void OnTrackingStarted()
        {
            onTrackingStarted?.Invoke();
        }

        protected virtual void OnTrackingStopping()
        {
            onTrackingStopping?.Invoke();
        }

        protected virtual void OnTrackingStopped()
        {
            onTrackingStopped?.Invoke();
        }

        protected virtual void PrintDebugReport()
        {
        }

        /// <summary>
        /// Checks the current tracking status and fires the events to indicate any change in the
        /// tracking status.
        /// </summary>
        public virtual void Update()
        {
            if (Tracking && !wasTracking)
            {
                xr.StartAR();
                OnTrackingStarted();

                foreach (var target in targets)
                {
                    if (target.Found)
                    {
                        target.onFound?.Invoke();

                        if (!target.Safe)
                        {
                            target.onProximity?.Invoke();
                        }
                    }
                }

                if (targets.All(target => !target.Found))
                {
                    OnAllTargetsLost();
                }
            }
            else if (!Tracking && wasTracking)
            {
                xr.StopAR();
                OnTrackingStopped();
            }

            wasTracking = Tracking;

            if (DebugReport)
            {
                PrintDebugReport();
            }
        }

        /// <summary>
        /// Tears down the tracker keeper.
        /// </summary>
        public void OnDestroy()
        {
            done = true;
        }

        /// <summary>
        /// Creates an event handler that can handle finding a given target.
        /// </summary>
        /// <returns>The finder.</returns>
        /// <param name="target">Target.</param>
        private UnityAction TargetFinder(AbstractTrackableFoundEventHandler target)
        {
            return () =>
            {
                CancelInvoke(nameof(OnAllTargetsLost));

                if (HasTargets && !hadAnyTargets && Tracking)
                {
                    onAnyTargetFound?.Invoke();
                }

                onTargetFound?.Invoke(target.transform);

                foreach (var otherTarget in targets)
                {
                    if (otherTarget != target && otherTarget.Found)
                    {
                        otherTarget.ForceLost();
                    }
                }

                hadAnyTargets = HasTargets;
            };
        }

        /// <summary>
        /// Creates an event handler that can handle finding a given target.
        /// </summary>
        /// <returns>The finder.</returns>
        /// <param name="target">Target.</param>
        private UnityAction TargetProximity(AbstractTrackableFoundEventHandler target)
        {
            return () => onTargetProximity.Invoke(target.transform);
        }

        /// <summary>
        /// Creates an event handler that can handle finding a given target.
        /// </summary>
        /// <returns>The finder.</returns>
        /// <param name="target">Target.</param>
        private UnityAction TargetSafe(AbstractTrackableFoundEventHandler target)
        {
            return () => onTargetSafe.Invoke(target.transform);
        }

        /// <summary>
        /// Creates an event handler that can handle losing a given target. The event handler will
        /// fire the targetChanged event if the lost target was the <see cref="CurrentTarget"/>. If
        /// losing this target means that there are no tracked targets, then the <see
        /// cref="onAllTargetsLost"/> event is fired. Finally, Vuforia's Persistent Extended Tracking
        /// is reset, if it was active.
        /// </summary>
        /// <returns>The loser.</returns>
        /// <param name="target">Target.</param>
        private UnityAction TargetLoser(AbstractTrackableFoundEventHandler target)
        {
            return () =>
            {
                onTargetLost?.Invoke(target.transform);

                if (!HasTargets && hadAnyTargets)
                {
                    hadAnyTargets = HasTargets;
                    if (!done)
                    {
                        Invoke(nameof(OnAllTargetsLost), 1);
                    }
                }
            };
        }

        /// <summary>
        /// Triggers the <see cref="onAllTargetsLost"/> event.
        /// </summary>
        private void OnAllTargetsLost()
        {
            if (Tracking)
            {
                onAllTargetsLost?.Invoke();
            }
        }

        /// <summary>
        /// Returns true when there are any <see cref="GoodTargets"/>.
        /// </summary>
        /// <value><c>true</c> if has targets; otherwise, <c>false</c>.</value>
        public bool HasTargets
        {
            get
            {
                foreach (var target in targets)
                {
                    if (target.Found)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// This value is set to true when the user is at a safe distance from the target.
        /// </summary>
        public bool Safe
        {
            get
            {
                foreach (var target in targets)
                {
                    if (target.Found && !target.Safe)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Starts the tracker without changing any dataset data that has already been configured.
        /// </summary>
        public void StartTracker()
        {
            Invoke(nameof(StartTrackerDelayed), 1);
        }

        /// <summary>
        /// Starts the tracker without changing any dataset data that has already been configured.
        /// This method is the target of <see cref="StartTracker"/> to start after a second of delay
        /// to allow the camera feed to catch up.
        /// </summary>
        private void StartTrackerDelayed()
        {
            StartTrackerWithDataSetForTarget(null, null);
        }

        /// <summary>
        /// Initializes tracking of a set of targets.
        /// </summary>
        /// <param name="dataSetName">Data set name.</param>
        public void StartTrackerWithDataSet(string dataSetName)
        {
            ScreenDebugger.Print($"Activating data set: {dataSetName}");
            StartTrackerWithDataSetForTarget(dataSetName, null);
        }

        /// <summary>
        /// Initializes tracking of a specific target.
        /// </summary>
        /// <param name="dataSetName">Data set name.</param>
        /// <param name="targetName"> Target name.</param>
        public void StartTrackerWithDataSetForTarget(string dataSetName, string targetName)
        {
            OnTrackingStarting(dataSetName, targetName);
            Update();
        }

#if UNITY_EDITOR

        /// <summary>
        /// Stop object tracking while in picker
        /// </summary>
        public void Reset()
        {
            OnTrackingStopping();
            Tracking = false;
            Update();
        }

#endif
    }
}
