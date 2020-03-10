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
using Juniper.Mathematics;

namespace Oculus.VR
{
    /// <summary>
    /// Miscellaneous extension methods that any script can use.
    /// </summary>
    public static class Extensions
    {
        public static Pose ToOVRPose(this Plugin.Posef p)
        {
            return new Pose(
                new Vector3(p.Position.x, p.Position.y, -p.Position.z),
                new Quaternion(-p.Orientation.x, -p.Orientation.y, p.Orientation.z, p.Orientation.w));
        }

        public static Tracker.Frustum ToFrustum(this Plugin.Frustumf f)
        {
            return new Tracker.Frustum()
            {
                nearZ = f.zNear,
                farZ = f.zFar,

                fov = new Vector2()
                {
                    X = Juniper.Units.Radians.Degrees(f.fovX),
                    Y = Juniper.Units.Radians.Degrees(f.fovY)
                }
            };
        }

        public static Vector3 FromVector3f(this Plugin.Vector3f v)
        {
            return new Vector3() { X = v.x, Y = v.y, Z = v.z };
        }

        public static Vector3 FromFlippedXVector3f(this Plugin.Vector3f v)
        {
            return new Vector3() { X = -v.x, Y = v.y, Z = v.z };
        }

        public static Vector3 FromFlippedZVector3f(this Plugin.Vector3f v)
        {
            return new Vector3() { X = v.x, Y = v.y, Z = -v.z };
        }

        public static Plugin.Vector3f ToVector3f(this Vector3 v)
        {
            return new Plugin.Vector3f() { x = v.X, y = v.Y, z = v.Z };
        }

        public static Plugin.Vector3f ToFlippedXVector3f(this Vector3 v)
        {
            return new Plugin.Vector3f() { x = -v.X, y = v.Y, z = v.Z };
        }

        public static Plugin.Vector3f ToFlippedZVector3f(this Vector3 v)
        {
            return new Plugin.Vector3f() { x = v.X, y = v.Y, z = -v.Z };
        }

        public static Quaternion FromQuatf(this Plugin.Quatf q)
        {
            return new Quaternion() { X = q.x, Y = q.y, Z = q.z, W = q.w };
        }

        public static Quaternion FromFlippedXQuatf(this Plugin.Quatf q)
        {
            return new Quaternion() { X = q.x, Y = -q.y, Z = -q.z, W = q.w };
        }

        public static Quaternion FromFlippedZQuatf(this Plugin.Quatf q)
        {
            return new Quaternion() { X = -q.x, Y = -q.y, Z = q.z, W = q.w };
        }

        public static Plugin.Quatf ToQuatf(this Quaternion q)
        {
            return new Plugin.Quatf() { x = q.X, y = q.Y, z = q.Z, w = q.W };
        }

        public static Plugin.Quatf ToFlippedXQuatf(this Quaternion q)
        {
            return new Plugin.Quatf() { x = q.X, y = -q.Y, z = -q.Z, w = q.W };
        }

        public static Plugin.Quatf ToFlippedZQuatf(this Quaternion q)
        {
            return new Plugin.Quatf() { x = -q.X, y = -q.Y, z = q.Z, w = q.W };
        }
    }
}