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
    /// Provides test results of boundary system queries.
    /// </summary>
    public struct BoundaryTestResult
    {
        public bool IsTriggering;                              ///< Returns true if the queried test would violate and/or trigger the tested boundary types.
        public float ClosestDistance;                          ///< Returns the distance between the queried test object and the closest tested boundary type.
        public Vector3 ClosestPoint;                           ///< Returns the closest point to the queried test object.
        public Vector3 ClosestPointNormal;                     ///< Returns the normal of the closest point to the queried test object.
    }
}