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

using System.Numerics;

namespace Oculus.VR
{
    /// <summary>
    /// An infrared camera that tracks the position of a head-mounted display.
    /// </summary>
    public static class Tracker
    {
        /// <summary>
        /// The (symmetric) visible area in front of the sensor.
        /// </summary>
        public struct Frustum
        {
            /// <summary>
            /// The sensor's minimum supported distance to the HMD.
            /// </summary>
            public float nearZ;
            /// <summary>
            /// The sensor's maximum supported distance to the HMD.
            /// </summary>
            public float farZ;
            /// <summary>
            /// The sensor's horizontal and vertical fields of view in degrees.
            /// </summary>
            public Vector2 fov;
        }

        /// <summary>
        /// If true, a sensor is attached to the system.
        /// </summary>
        public static bool IsPresent
        {
            get
            {
                if (!Manager.isHmdPresent)
                {
                    return false;
                }

                return Plugin.positionSupported;
            }
        }

        /// <summary>
        /// If true, the sensor is actively tracking the HMD's position. Otherwise the HMD may be temporarily occluded, the system may not support position tracking, etc.
        /// </summary>
        public static bool IsPositionTracked => Plugin.positionTracked;

        /// <summary>
        /// If this is true and a sensor is available, the system will use position tracking when isPositionTracked is also true.
        /// </summary>
        public static bool IsEnabled
        {
            get
            {
                if (!Manager.isHmdPresent)
                {
                    return false;
                }

                return Plugin.position;
            }

            set
            {
                if (!Manager.isHmdPresent)
                {
                    return;
                }

                Plugin.position = value;
            }
        }

        /// <summary>
        /// Returns the number of sensors currently connected to the system.
        /// </summary>
        public static int Count
        {
            get
            {
                var count = 0;

                for (var i = 0; i < (int)Plugin.Tracker.Count; ++i)
                {
                    if (GetPresent(i))
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the sensor's viewing frustum.
        /// </summary>
        public static Frustum GetFrustum(int tracker = 0)
        {
            if (!Manager.isHmdPresent)
            {
                return new Frustum();
            }

            return Plugin.GetTrackerFrustum((Plugin.Tracker)tracker).ToFrustum();
        }

        /// <summary>
        /// Gets the sensor's pose, relative to the head's pose at the time of the last pose recentering.
        /// </summary>
        public static Pose GetPose(int tracker = 0)
        {
            if (!Manager.isHmdPresent
                || 0 > tracker
                || tracker > 3)
            {
                return Pose.Identity;
            }

            var p = tracker switch
            {
                0 => Plugin.GetNodePose(Plugin.Node.TrackerZero, Plugin.ProcessingStep.Render).ToOVRPose(),
                1 => Plugin.GetNodePose(Plugin.Node.TrackerOne, Plugin.ProcessingStep.Render).ToOVRPose(),
                2 => Plugin.GetNodePose(Plugin.Node.TrackerTwo, Plugin.ProcessingStep.Render).ToOVRPose(),
                _ => Plugin.GetNodePose(Plugin.Node.TrackerThree, Plugin.ProcessingStep.Render).ToOVRPose(),
            };

            return new Pose(
                p.Position,
                p.Orientation * Quaternion.CreateFromYawPitchRoll(180, 0, 0));
        }

        /// <summary>
        /// If true, the pose of the sensor is valid and is ready to be queried.
        /// </summary>
        public static bool GetPoseValid(int tracker = 0)
        {
            if (!Manager.isHmdPresent)
            {
                return false;
            }

            return tracker switch
            {
                0 => Plugin.GetNodePositionTracked(Plugin.Node.TrackerZero),
                1 => Plugin.GetNodePositionTracked(Plugin.Node.TrackerOne),
                2 => Plugin.GetNodePositionTracked(Plugin.Node.TrackerTwo),
                3 => Plugin.GetNodePositionTracked(Plugin.Node.TrackerThree),
                _ => false,
            };
        }

        public static bool GetPresent(int tracker = 0)
        {
            if (!Manager.isHmdPresent)
            {
                return false;
            }

            return tracker switch
            {
                0 => Plugin.GetNodePresent(Plugin.Node.TrackerZero),
                1 => Plugin.GetNodePresent(Plugin.Node.TrackerOne),
                2 => Plugin.GetNodePresent(Plugin.Node.TrackerTwo),
                3 => Plugin.GetNodePresent(Plugin.Node.TrackerThree),
                _ => false,
            };
        }
    }
}