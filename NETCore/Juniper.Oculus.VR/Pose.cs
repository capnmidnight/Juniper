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

namespace Oculus.VR
{
    public struct Pose : IEquatable<Pose>
    {
        public static Pose Identity { get; } = new Pose(Vector3.Zero, Quaternion.Identity);

        public Quaternion Orientation;
        public Vector3 Position;

        public Pose(Vector3 position, Quaternion orientation)
        {
            Orientation = orientation;
            Position = position;
        }

        public override bool Equals(object obj)
        {
            return obj is Pose pose && Equals(pose);
        }

        public bool Equals(Pose other)
        {
            return Orientation.Equals(other.Orientation) &&
                   Position.Equals(other.Position);
        }

        public override int GetHashCode()
        {
            var hashCode = 1543317779;
            hashCode = hashCode * -1521134295 + Orientation.GetHashCode();
            hashCode = hashCode * -1521134295 + Position.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Pose left, Pose right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Pose left, Pose right)
        {
            return !(left == right);
        }
    }
}