using System;
using System.Runtime.Serialization;

using Juniper.Mathematics;

namespace Juniper.XR
{
    [Serializable]
    public struct Pose : ISerializable, IEquatable<Pose>
    {
        public static readonly Pose Identity = new Pose(0, 0, 0, 0, 0, 0, 1);

        public Vector3Serializable Position { get; }
        public QuaternionSerializable Orientation { get; }

        public Pose(Vector3Serializable position, QuaternionSerializable orientation)
        {
            Position = position;
            Orientation = orientation;
        }

        public Pose(float px, float py, float pz, float ox, float oy, float oz, float ow)
            : this(new Vector3Serializable(px, py, pz), new QuaternionSerializable(ox, oy, oz, ow))
        {  }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private Pose(SerializationInfo info, StreamingContext context)
            : this(info?.GetVector3(nameof(Position)) ?? throw new ArgumentNullException(nameof(info)),
                info.GetQuaternion(nameof(Orientation)))
        { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddVector3(nameof(Position), Position);
            info.AddQuaternion(nameof(Orientation), Orientation);
        }

        public override bool Equals(object obj)
        {
            return obj is Pose other
                && Equals(other);
        }

        public bool Equals(Pose other)
        {
            return Position.Equals(other.Position)
                && Orientation.Equals(other.Orientation);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode()
                ^ Orientation.GetHashCode();
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
