/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Licensed under the Oculus Utilities SDK License Version 1.31 (the "License"); you may not use
the Utilities SDK except in compliance with the License, which is provided at the time of installation
or download, or which otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at
https://developer.oculus.com/licenses/utilities-1.31

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using System;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Oculus.VR
{
    /// <summary>
    /// Manages an Oculus Rift head-mounted display (HMD).
    /// </summary>
    public class Display
    {
        /// <summary>
        /// Contains latency measurements for a single frame of rendering.
        /// </summary>
        public struct LatencyData
        {
            /// <summary>
            /// The time it took to render both eyes in seconds.
            /// </summary>
            public float render;

            /// <summary>
            /// The time it took to perform TimeWarp in seconds.
            /// </summary>
            public float timeWarp;

            /// <summary>
            /// The time between the end of TimeWarp and scan-out in seconds.
            /// </summary>
            public float postPresent;
            public float renderError;
            public float timeWarpError;
        }

        private bool recenterRequested = false;
        private long recenterRequestedFrameCount = long.MaxValue;
        private Pose previousRelativeTrackingSpacePose;
        private Manager.TrackingOrigin previousTrackingOrigin;

        /// <summary>
        /// Creates an instance of OVRDisplay. Called by OVRManager.
        /// </summary>
        public Display()
        {
            if (Plugin.GetSystemHeadsetType() == Plugin.SystemHeadset.Oculus_Quest)
            {
                previousTrackingOrigin = Manager.trackingOriginType;
                var relativeOrigin = (previousTrackingOrigin != Manager.TrackingOrigin.Stage) ? Manager.TrackingOrigin.Stage : Manager.TrackingOrigin.EyeLevel;
                previousRelativeTrackingSpacePose = Plugin.GetTrackingTransformRelativePose((Plugin.TrackingOrigin)relativeOrigin).ToOVRPose();
            }
        }

        /// <summary>
        /// Updates the internal state of the OVRDisplay. Called by OVRManager.
        /// </summary>
        public void Update()
        {
            if (recenterRequested && DateTime.Now.Ticks > recenterRequestedFrameCount)
            {
                RecenteredPose?.Invoke();
                recenterRequested = false;
                recenterRequestedFrameCount = long.MaxValue;
            }
            if (Plugin.GetSystemHeadsetType() == Plugin.SystemHeadset.Oculus_Quest)
            {
                var relativeOrigin = (Manager.trackingOriginType != Manager.TrackingOrigin.Stage) ? Manager.TrackingOrigin.Stage : Manager.TrackingOrigin.EyeLevel;
                var relativeTrackingSpacePose = Plugin.GetTrackingTransformRelativePose((Plugin.TrackingOrigin)relativeOrigin).ToOVRPose();
                //If the tracking origin type hasn't switched and the relative pose changes, a recenter occurred.
                if (previousTrackingOrigin == Manager.trackingOriginType && previousRelativeTrackingSpacePose != relativeTrackingSpacePose && RecenteredPose != null)
                {
                    RecenteredPose();
                }
                previousRelativeTrackingSpacePose = relativeTrackingSpacePose;
                previousTrackingOrigin = Manager.trackingOriginType;
            }
        }

        /// <summary>
        /// Occurs when the head pose is reset.
        /// </summary>
        public event System.Action RecenteredPose;

        /// <summary>
        /// Recenters the head pose.
        /// </summary>
        public void RecenterPose()
        {
            // The current poses are cached for the current frame and won't be updated immediately
            // after UnityEngine.VR.InputTracking.Recenter(). So we need to wait until next frame
            // to trigger the RecenteredPose delegate. The application could expect the correct pose
            // when the RecenteredPose delegate get called.
            recenterRequested = true;
            recenterRequestedFrameCount = DateTime.Now.Ticks;
        }

        /// <summary>
        /// Gets the current linear acceleration of the head.
        /// </summary>
        public static Vector3 acceleration
        {
            get
            {
                if (!Manager.isHmdPresent)
                {
                    return Vector3.Zero;
                }

                var retVec = Vector3.Zero;
                if (NodeStateProperties.GetNodeStatePropertyVector3(NodeStatePropertyType.Acceleration, Plugin.Node.Head, Plugin.ProcessingStep.Render, out retVec))
                {
                    return retVec;
                }

                return Vector3.Zero;
            }
        }

        /// <summary>
        /// Gets the current angular acceleration of the head in radians per second per second about each axis.
        /// </summary>
        public static Vector3 angularAcceleration
        {
            get
            {
                if (!Manager.isHmdPresent)
                {
                    return Vector3.Zero;
                }

                var retVec = Vector3.Zero;
                if (NodeStateProperties.GetNodeStatePropertyVector3(NodeStatePropertyType.AngularAcceleration, Plugin.Node.Head, Plugin.ProcessingStep.Render, out retVec))
                {
                    return retVec;
                }

                return Vector3.Zero;

            }
        }

        /// <summary>
        /// Gets the current linear velocity of the head in meters per second.
        /// </summary>
        public static Vector3 velocity
        {
            get
            {
                if (!Manager.isHmdPresent)
                {
                    return Vector3.Zero;
                }

                var retVec = Vector3.Zero;
                if (NodeStateProperties.GetNodeStatePropertyVector3(NodeStatePropertyType.Velocity, Plugin.Node.Head, Plugin.ProcessingStep.Render, out retVec))
                {
                    return retVec;
                }

                return Vector3.Zero;
            }
        }

        /// <summary>
        /// Gets the current angular velocity of the head in radians per second about each axis.
        /// </summary>
        public static Vector3 angularVelocity
        {
            get
            {
                if (!Manager.isHmdPresent)
                {
                    return Vector3.Zero;
                }

                var retVec = Vector3.Zero;
                if (NodeStateProperties.GetNodeStatePropertyVector3(NodeStatePropertyType.AngularVelocity, Plugin.Node.Head, Plugin.ProcessingStep.Render, out retVec))
                {
                    return retVec;
                }

                return Vector3.Zero;
            }
        }

        /// <summary>
        /// Gets the current measured latency values.
        /// </summary>
        public static LatencyData latency
        {
            get
            {
                if (!Manager.isHmdPresent)
                {
                    return new LatencyData();
                }

                var latency = Plugin.latency;

                var r = new Regex("Render: ([0-9]+[.][0-9]+)ms, TimeWarp: ([0-9]+[.][0-9]+)ms, PostPresent: ([0-9]+[.][0-9]+)ms", RegexOptions.None);

                var ret = new LatencyData();

                var match = r.Match(latency);
                if (match.Success)
                {
                    ret.render = float.Parse(match.Groups[1].Value);
                    ret.timeWarp = float.Parse(match.Groups[2].Value);
                    ret.postPresent = float.Parse(match.Groups[3].Value);
                }

                return ret;
            }
        }

        /// <summary>
        /// Gets application's frame rate reported by oculus plugin
        /// </summary>
        public static float appFramerate
        {
            get
            {
                if (!Manager.isHmdPresent)
                {
                    return 0;
                }

                return Plugin.GetAppFramerate();
            }
        }

        /// <summary>
        /// Gets the recommended MSAA level for optimal quality/performance the current device.
        /// </summary>
        public static int recommendedMSAALevel
        {
            get
            {
                var result = Plugin.recommendedMSAALevel;

                if (result == 1)
                {
                    result = 0;
                }

                return result;
            }
        }
    }
}