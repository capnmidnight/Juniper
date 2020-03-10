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

namespace Oculus.VR
{
    /// <summary>
    /// An affine transformation built from a Unity position and orientation.
    /// </summary>
    public static class PoseExt
    {
        // Warning: this function is not a strict reverse of OVRPlugin.Posef.ToOVRPose(), even after flipZ()
        public static Plugin.Posef ToPosef_Legacy(this Juniper.Mathematics.Pose p)
        {
            return new Plugin.Posef()
            {
                Position = p.Position.ToVector3f(),
                Orientation = p.Orientation.ToQuatf()
            };
        }

        public static Plugin.Posef ToPosef(this Juniper.Mathematics.Pose pose)
        {
            var result = new Plugin.Posef();

            var p = pose.Position;
            result.Position.x = p.X;
            result.Position.y = p.Y;
            result.Position.z = -p.Z;

            var q = pose.Orientation;
            result.Orientation.x = -q.X;
            result.Orientation.y = -q.Y;
            result.Orientation.z = q.Z;
            result.Orientation.w = q.W;
            return result;
        }
    }
}