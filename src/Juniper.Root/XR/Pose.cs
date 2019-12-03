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

        public Pose(float px, float py, float pz, float ox, float oy, float oz, float ow)
        {
            position = new Vector3(px, py, pz);
            orientation = new Quaternion(ox, oy, oz, ow);
        }

        public Pose(Vector3 position, Quaternion orientation)
        {
            this.position = position;
            this.orientation = orientation;
        }

        private Pose(SerializationInfo info, StreamingContext context)
            : this(info.GetVector3(nameof(position)),
                info.GetQuaternion(nameof(orientation)))
        { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddVector3(nameof(position), position);
            info.AddQuaternion(nameof(orientation), orientation);
        }

        public override bool Equals(object obj)
        {
            return obj is Pose other
                && Equals(other);
        }

        public bool Equals(Pose other)
        {
            return position.Equals(other.position)
                && orientation.Equals(other.orientation);
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
