#if VUFORIA
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vuforia;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Juniper.Unity.ImageTracking
{
    public class VuforiaTrackerKeeper : AbstractTrackerKeeper
    {
        /// <summary>
        /// The number of times to attempt initializing Vuforia before just giving up.
        /// </summary>
        const int VUFORIA_INIT_ATTEMPTS = 3;

        /// <summary>
        /// The Vuforia Tracker API.
        /// </summary>
        ObjectTracker tracker;

        /// <summary>
        /// Set to true to enable using Vuforia's Persistent Extended Tracking.
        /// </summary>
        public bool usePET;

        /// <summary>
        /// Used for reseting Vuforia's tracker the first time the application is started.
        /// </summary>
        bool firstTime = true;

        /// <summary>
        /// Set to true to set the target framerate lower than normal when the tracker is active.
        /// </summary>
#if UNITY_ANDROID
        [HideInInspector]
        public bool manageVuforiaFrameRate = true;
#else
        public bool manageVuforiaFrameRate;
#endif

        /// <summary>
        /// Is set to true when Persistent Extended Tracking is active.
        /// </summary>
        bool usingPET;

        /// <summary>
        /// The number updates to the AR Background Image texture since tracking started. This value
        /// is used to delay switching from camera initialization to the app until after we are sure
        /// the camera is really ready.
        /// </summary>
        int numUpdates = 0;

        protected override IEnumerable<TrackableFoundEventHandler> FirstTargets =>
            base.FirstTargets
                .Union(from m in this.FindAll<ModelTargetBehaviour>()
                       select m.Ensure<TrackableFoundEventHandler>().Value)
                .Union(from m in this.FindAll<ImageTargetBehaviour>()
                       select m.Ensure<TrackableFoundEventHandler>().Value)
                .Union(from m in this.FindAll<ObjectTargetBehaviour>()
                       select m.Ensure<TrackableFoundEventHandler>().Value);

        protected override void Start()
        {
#if UNITY_ANDROID
            manageVuforiaFrameRate = true;
#endif
            base.Start();
        }

        public override void Update()
        {
            bool cameraRendering = VuforiaRuntime.Instance.InitializationState == VuforiaRuntime.InitState.INITIALIZED
                && VuforiaBehaviour.Instance.enabled
                && VuforiaRenderer.Instance.VideoBackgroundTexture != null;

            Tracking = cameraRendering
                 && numUpdates > Application.targetFrameRate
                 && tracker != null
                 && tracker.IsActive;

            if (cameraRendering)
            {
                ++numUpdates;
            }

            base.Update();
        }

        protected override void OnTrackingStarting(string dataSetName, string targetName)
        {
            base.OnTrackingStarting(dataSetName, targetName);
            StartCoroutine(StartTrackerCoroutine(dataSetName, targetName));
        }

        protected override void OnTrackingStarted()
        {
            base.OnTrackingStarted();

                if (manageVuforiaFrameRate)
                {
                    Application.targetFrameRate = 30;
                }

                if (firstTime)
                {
                    firstTime = false;
                    StopTracking();
                    if (tracker != null)
                    {
                        tracker.Start();
                    }
                    VuforiaBehaviour.Instance.enabled = true;
                }
        }

        protected override void OnTrackingStopping()
        {
            base.OnTrackingStopping();
            StopTracking();
        }

        protected override void OnTrackingStopped()
        {
            base.OnTrackingStopped();

                if (manageVuforiaFrameRate)
                {
                    Application.targetFrameRate = 60;
                }
                numUpdates = 0;
        }

        /// <summary>
        /// Cancel tracking and disable Vuforia. This also forces the Lost event on all of the
        /// currently tracked targets.
        /// </summary>
        void StopTracking()
        {
            if (targets != null)
            {
                foreach (var target in targets)
                {
                    var modelBehaviour = target.GetComponent<ModelTargetBehaviour>();
                    if (modelBehaviour != null)
                    {
                        modelBehaviour.enabled = false;
                    }
                }
            }

            ResetPET();

            if (tracker != null && tracker.IsActive)
            {
                tracker.Stop();
            }

            if (Tracking)
            {
                VuforiaBehaviour.Instance.enabled = false;
            }

            if (targets != null)
            {
                foreach (var target in targets)
                {
                    target.ForceLost();
                }
            }
        }

        /// <summary>
        /// On Vuforia, enables a specific set of data, either image targets, object targets, or
        /// scanned model targets.
        /// </summary>
        /// <param name="dataSetName">Data set name.</param>
        void EnableDataSet(string dataSetName)
        {
            if (dataSetName != null)
            {
                foreach (var ds in tracker.GetActiveDataSets())
                {
                    tracker.DeactivateDataSet(ds);
                }

                LoadDataSet(dataSetName);
            }
        }

        /// <summary>
        /// Loads a set of Vuforia markers from disk and activates it at runtime.
        /// </summary>
        /// <param name="dataSetName">Data set name.</param>
        public void LoadDataSet(string dataSetName)
        {
            if (tracker != null && !string.IsNullOrEmpty(dataSetName))
            {
                var dataSets = tracker.GetActiveDataSets();
                var targetPath = Path.Combine("Vuforia", dataSetName + ".xml");
                var dataSet = dataSets.FirstOrDefault(ds =>
                    ds.Path == targetPath);
                if (dataSet == null)
                {
                    dataSet = tracker.CreateDataSet();
                    dataSet.Load(dataSetName);
                    tracker.ActivateDataSet(dataSet);
                }
            }
        }

        /// <summary>
        /// A coroutine that waits for a Vuforia ObjectTracker to become available.
        /// </summary>
        /// <value>The wait for tracker.</value>
        IEnumerator WaitForTracker
        {
            get
            {
                return new WaitUntil(() =>
                {
                    if (tracker == null)
                    {
                        tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
                    }
                    return tracker != null;
                });
            }
        }

        /// <summary>
        /// Handles marking a target as found, and optionally enables Vuforia's Persistent Extended Tracking.
        /// </summary>
        /// <returns>The coroutine.</returns>
        IEnumerator EnablePETCoroutine()
        {
            if (VuforiaUnityExt.HasPET && usePET && !usingPET)
            {
                yield return WaitForTracker;
                ResetPET();
                usingPET = true;
            }
        }

        /// <summary>
        /// On Vuforia, resets the Persistent Extended Tracking, or starts it if it hasn't already
        /// been running.
        /// </summary>
        public void ResetPET()
        {
            if (usingPET && tracker != null)
            {
                tracker.Stop();
                tracker.Start();
            }
            else
            {
                StartCoroutine(EnablePETCoroutine());
            }
        }

        /// <summary>
        /// What for Vuforia's initialization state to no longer be "INITIALIZING".
        /// </summary>
        /// <value>The wait for not initializing.</value>
        WaitUntil WaitForNotInitializing
        {
            get
            {
                return new WaitUntil(() =>
                    VuforiaRuntime.Instance.InitializationState != VuforiaRuntime.InitState.INITIALIZING);
            }
        }

        /// <summary>
        /// Initializes tracking of a specific target.
        /// </summary>
        /// <returns>The tracker coroutine.</returns>
        /// <param name="dataSetName">Data set name.</param>
        /// <param name="targetName"> Target name.</param>
        IEnumerator StartTrackerCoroutine(string dataSetName, string targetName)
        {
            if (!VuforiaConfiguration.Instance.Vuforia.DelayedInitialization)
            {
                VuforiaBehaviour.Instance.enabled = true;

                yield return WaitForNotInitializing;
            }
            else
            {
                VuforiaConfiguration.Instance.Vuforia.DelayedInitialization = false;

                for (int i = 1; i <= VUFORIA_INIT_ATTEMPTS && VuforiaRuntime.Instance.InitializationState != VuforiaRuntime.InitState.INITIALIZED; ++i)
                {
                    if (i > 1)
                    {
                        Debug.LogFormat("Initializating Vuforia failed. Retrying ({0} of {1})", i, VUFORIA_INIT_ATTEMPTS);
                        VuforiaRuntime.Instance.Deinit();
                    }
                    else
                    {
                        Debug.LogFormat("Initializing Vuforia ({0} of {1})", i, VUFORIA_INIT_ATTEMPTS);
                    }

                    if (VuforiaRuntime.Instance.InitializationState == VuforiaRuntime.InitState.NOT_INITIALIZED)
                    {
                        VuforiaRuntime.Instance.InitVuforia();
                        VuforiaBehaviour.Instance.enabled = true;
                    }

                    yield return WaitForNotInitializing;
                }
            }

            if (VuforiaRuntime.Instance.InitializationState == VuforiaRuntime.InitState.INITIALIZED)
            {
                yield return WaitForTracker;

                if (tracker.IsActive)
                {
                    tracker.Stop();
                    yield return new WaitUntil(() => !tracker.IsActive);
                }

                EnableDataSet(dataSetName);

                if (!tracker.IsActive)
                {
                    tracker.Start();
                    yield return new WaitUntil(() =>
                        tracker.IsActive);
                }
            }
            else
            {
                Debug.LogError("Failed to initialize Vuforia.");
            }
        }

        /// <summary>
        /// If <see cref="DebugReport"/> is set to true, prints a report on the status of tracking.
        /// </summary>
        protected override void PrintDebugReport()
        {
            ScreenDebugger.Print(tracker == null ? "No tracker" : "Has tracker");
            ScreenDebugger.Print(usingPET ? "Using PET" : "Not using PET");
        }
    }
}
#endif
