using System;
using System.Runtime.Serialization;

using Juniper.Mathematics;

namespace Juniper.XR
{
    [Serializable]
    public struct Pose : ISerializable, IEquatable<Pose>
    {
        public static readonly Pose Identity = new Pose(0, 0, 0, 0, 0, 0, 1);

        public readonly Vector3Serializable position;
        public readonly QuaternionSerializable orientation;

        public Pose(float px, float py, float pz, float ox, float oy, float oz, float ow)
        {
            position = new Vector3Serializable(px, py, pz);
            orientation = new QuaternionSerializable(ox, oy, oz, ow);
        }

        public Pose(Vector3Serializable position, QuaternionSerializable orientation)
        {
            this.position = position;
            this.orientation = orientation;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private Pose(SerializationInfo info, StreamingContext context)
            : this(info?.GetVector3(nameof(position)) ?? throw new ArgumentNullException(nameof(info)),
                info.GetQuaternion(nameof(orientation)))
        { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

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
