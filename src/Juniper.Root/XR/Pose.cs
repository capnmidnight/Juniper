using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.XR
{
    [Serializable]
    public struct Pose : ISerializable, IEquatable<Pose>
    {
        public readonly Vector3 position;
        public readonly Quaternion orientation;

        public Pose(Vector3 position, Quaternion orientation)
        {
            this.position = position;
            this.orientation = orientation;
        }

        public Pose(Vector3 position)
            : this(position, Quaternion.Identity)
        { }

        private Pose(SerializationInfo info, StreamingContext context)
            : this(info.GetValue<Vector3>(nameof(position)), info.GetValue<Quaternion>(nameof(orientation)))
        { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(position), position);
            info.AddValue(nameof(orientation), orientation);
        }

        public override bool Equals(object obj)
        {
            return obj is Pose other
                && Equals(other);
        }

        public bool Equals(Pose other)
        {
            return position == other.position
                && orientation == other.orientation;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode()
                ^ orientation.GetHashCode();
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
