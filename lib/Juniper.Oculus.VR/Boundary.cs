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
using System.Runtime.InteropServices;

namespace Oculus.VR
{
    /// <summary>
    /// Provides access to the Oculus boundary system.
    /// </summary>
    public static class Boundary
    {
        /// <summary>
        /// Specifies a tracked node that can be queried through the boundary system.
        /// </summary>
        public enum Node
        {
            HandLeft = Plugin.Node.HandLeft,  ///< Tracks the left hand node.
            HandRight = Plugin.Node.HandRight, ///< Tracks the right hand node.
            Head = Plugin.Node.Head,      ///< Tracks the head node.
        }

        /// <summary>
        /// Specifies a boundary type surface.
        /// </summary>
        public enum BoundaryType
        {
            OuterBoundary = Plugin.BoundaryType.OuterBoundary, ///< Outer boundary that closely matches the user's configured walls.
            PlayArea = Plugin.BoundaryType.PlayArea,      ///< Smaller convex area inset within the outer boundary.
        }

        /// <summary>
        /// Returns true if the boundary system is currently configured with valid boundary data.
        /// </summary>
        public static bool Configured => Plugin.GetBoundaryConfigured();

        /// <summary>
        /// Returns the results of testing a tracked node against the specified boundary type.
        /// All points are returned in local tracking space shared by tracked nodes and accessible through OVRCameraRig's trackingSpace anchor.
        /// </summary>
        public static BoundaryTestResult TestNode(Boundary.Node node, Boundary.BoundaryType boundaryType)
        {
            var ovrpRes = Plugin.TestBoundaryNode((Plugin.Node)node, (Plugin.BoundaryType)boundaryType);

            var res = new BoundaryTestResult()
            {
                IsTriggering = (ovrpRes.IsTriggering == Plugin.Bool.True),
                ClosestDistance = ovrpRes.ClosestDistance,
                ClosestPoint = ovrpRes.ClosestPoint.FromFlippedZVector3f(),
                ClosestPointNormal = ovrpRes.ClosestPointNormal.FromFlippedZVector3f(),
            };

            return res;
        }

        /// <summary>
        /// Returns the results of testing a 3d point against the specified boundary type.
        /// The test point is expected in local tracking space.
        /// All points are returned in local tracking space shared by tracked nodes and accessible through OVRCameraRig's trackingSpace anchor.
        /// </summary>
        public static BoundaryTestResult TestPoint(Vector3 point, Boundary.BoundaryType boundaryType)
        {
            var ovrpRes = Plugin.TestBoundaryPoint(point.ToFlippedZVector3f(), (Plugin.BoundaryType)boundaryType);

            var res = new BoundaryTestResult()
            {
                IsTriggering = (ovrpRes.IsTriggering == Plugin.Bool.True),
                ClosestDistance = ovrpRes.ClosestDistance,
                ClosestPoint = ovrpRes.ClosestPoint.FromFlippedZVector3f(),
                ClosestPointNormal = ovrpRes.ClosestPointNormal.FromFlippedZVector3f(),
            };

            return res;
        }

        private static readonly int cachedVector3fSize = Marshal.SizeOf(typeof(Plugin.Vector3f));
        private static readonly NativeBuffer cachedGeometryNativeBuffer = new NativeBuffer(0);
        private static float[] cachedGeometryManagedBuffer = Array.Empty<float>();

        /// <summary>
        /// Returns an array of 3d points (in clockwise order) that define the specified boundary type.
        /// All points are returned in local tracking space shared by tracked nodes and accessible through OVRCameraRig's trackingSpace anchor.
        /// </summary>
        public static Vector3[] GetGeometry(Boundary.BoundaryType boundaryType)
        {
            var pointsCount = 0;
            if (Plugin.GetBoundaryGeometry2((Plugin.BoundaryType)boundaryType, IntPtr.Zero, ref pointsCount))
            {
                if (pointsCount > 0)
                {
                    var requiredNativeBufferCapacity = pointsCount * cachedVector3fSize;
                    if (cachedGeometryNativeBuffer.GetCapacity() < requiredNativeBufferCapacity)
                    {
                        cachedGeometryNativeBuffer.Reset(requiredNativeBufferCapacity);
                    }

                    var requiredManagedBufferCapacity = pointsCount * 3;
                    if (cachedGeometryManagedBuffer.Length < requiredManagedBufferCapacity)
                    {
                        cachedGeometryManagedBuffer = new float[requiredManagedBufferCapacity];
                    }

                    if (Plugin.GetBoundaryGeometry2((Plugin.BoundaryType)boundaryType, cachedGeometryNativeBuffer.GetPointer(), ref pointsCount))
                    {
                        Marshal.Copy(cachedGeometryNativeBuffer.GetPointer(), cachedGeometryManagedBuffer, 0, requiredManagedBufferCapacity);

                        var points = new Vector3[pointsCount];

                        for (var i = 0; i < pointsCount; i++)
                        {
                            points[i] = new Plugin.Vector3f()
                            {
                                x = cachedGeometryManagedBuffer[3 * i + 0],
                                y = cachedGeometryManagedBuffer[3 * i + 1],
                                z = cachedGeometryManagedBuffer[3 * i + 2],
                            }.FromFlippedZVector3f();
                        }

                        return points;
                    }
                }
            }

            return Array.Empty<Vector3>();
        }

        /// <summary>
        /// Returns a vector that indicates the spatial dimensions of the specified boundary type. (x = width, y = height, z = depth)
        /// </summary>
        public static Vector3 GetDimensions(Boundary.BoundaryType boundaryType)
        {
            return Plugin.GetBoundaryDimensions((Plugin.BoundaryType)boundaryType).FromVector3f();
        }

        /// <summary>
        /// Get: Returns true if the boundary system is currently visible.
        /// Set: Requests that the boundary system visibility be set to the specified value.
        /// The actual visibility can be overridden by the system (i.e., proximity trigger) or by the user (boundary system disabled)
        /// </summary>
        public static bool Visible
        {
            get
            {
                return Plugin.GetBoundaryVisible();
            }

            set
            {
                Plugin.SetBoundaryVisible(value);
            }
        }
    }
}