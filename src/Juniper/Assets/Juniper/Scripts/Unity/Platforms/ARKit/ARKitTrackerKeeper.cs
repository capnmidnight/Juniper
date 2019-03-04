#if ARKIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using UnityEngine.XR.iOS;

namespace Juniper.ImageTracking
{
    public class ARKitTrackerKeeper : AbstractTrackerKeeper
    {
        /// <summary>
        /// On ARKit, a lookup of targets by name.
        /// </summary>
        private Dictionary<string, TrackableFoundEventHandler> targetsByName;

        /// <summary>
        /// On ARKit, a lookup of the image sets by name.
        /// </summary>
        private Dictionary<string, ARReferenceImagesSet> dataSetsByName;
        protected override void Start()
        {
            base.Start();
            targetsByName = targets.ToDictionary(target =>
                target.image.imageName);
            dataSetsByName = targets.Select(target =>
                target.imagesSet)
                                    .Distinct()
                                    .ToDictionary(set =>
                                        set.resourceGroupName);

            ScreenDebugger.Print($"Found {targets.Count} targets: {string.Join(", ", targetsByName.Keys.ToArray())}");
            ScreenDebugger.Print($"Found {dataSetsByName.Count} data sets: {string.Join(", ", dataSetsByName.Keys.ToArray())}");

            UnityARSessionNativeInterface.ARImageAnchorAddedEvent += UnityARSessionNativeInterface_ARImageAnchorAddedEvent;
            UnityARSessionNativeInterface.ARImageAnchorUpdatedEvent += UnityARSessionNativeInterface_ARImageAnchorUpdatedEvent;

            UnityARSessionNativeInterface.ARImageAnchorRemovedEvent += UnityARSessionNativeInterface_ARImageAnchorRemovedEvent;
            if (dataSetsByName.Count == 1)
            {
                StartTrackerWithDataSetForTarget(dataSetsByName.Keys.First(), null);
            }
        }

        /// <summary>
        /// Finds a target object by name and updates its state.
        /// </summary>
        /// <param name="label">A label to aid in debugging.</param>
        /// <param name="anchorData">Anchor data.</param>
        /// <param name="act">The action to perform once the target is found.</param>
        private void WithTarget(string label, ARImageAnchor anchorData, Action<TrackableFoundEventHandler> act = null)
        {
            var tracking = Tracking ? "tracking" : "not tracking";
            ScreenDebugger.Print($"{label} {anchorData.referenceImageName} [{tracking}]");

            if (Tracking)
            {
                var targetName = anchorData.referenceImageName;
                if (targetsByName.ContainsKey(targetName))
                {
                    var target = targetsByName[targetName];
                    act?.Invoke(target);
                    target.transform.position = UnityARMatrixOps.GetPosition(anchorData.transform);
                    target.transform.rotation = UnityARMatrixOps.GetRotation(anchorData.transform);
                }
                else
                {
                    ScreenDebugger.Print($"Couldn't find {targetName}");
                    ScreenDebugger.Print($"Names we recognize: {string.Join(", ", targetsByName.Keys.ToArray())}");
                }
            }
        }

        /// <summary>
        /// An event handler for the first time an image is found by ARKit.
        /// </summary>
        /// <param name="anchorData">Anchor data.</param>
        private void UnityARSessionNativeInterface_ARImageAnchorAddedEvent(ARImageAnchor anchorData) =>
            WithTarget("Added", anchorData, target =>
                target.ForceFound());

        /// <summary>
        /// An event handler that executes every time ARKit updates its understanding of an image.
        /// </summary>
        /// <param name="anchorData">Anchor data.</param>
        private void UnityARSessionNativeInterface_ARImageAnchorUpdatedEvent(ARImageAnchor anchorData) =>
            WithTarget("Updated", anchorData);

        /// <summary>
        /// An event handler that executes when ARKit loses track of an image.
        /// </summary>
        /// <param name="anchorData">Anchor data.</param>
        private void UnityARSessionNativeInterface_ARImageAnchorRemovedEvent(ARImageAnchor anchorData) =>
            WithTarget("Removed", anchorData, target =>
                target.ForceLost());

        protected override void OnTrackingStarting(string dataSetName, string targetName)
        {
            base.OnTrackingStarting(dataSetName, targetName);

            if (dataSetsByName.ContainsKey(dataSetName))
            {
                var dataSet = dataSetsByName[dataSetName];
                var camMgr = ComponentExt.FindAny<UnityARCameraManager>();
                if (camMgr == null)
                {
                    Debug.LogError("No Unity AR Camera Manager!");
                }
                else
                {
                    camMgr.detectionImages = dataSet;
                }

                Tracking = true;
            }
        }
    }
}
#endif
