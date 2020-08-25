using System;
using System.Runtime.Serialization;

namespace Juniper.Mathematics
{
    [Serializable]
    public struct PoseSerializable : ISerializable, IEquatable<PoseSerializable>
    {
        private const string TYPE_NAME = "Pose";

        public static readonly PoseSerializable Identity = new PoseSerializable(0, 0, 0, 0, 0, 0, 1);

        public Vector3Serializable Position { get; }
        public QuaternionSerializable Orientation { get; }

        public PoseSerializable(Vector3Serializable position, QuaternionSerializable orientation)
        {
            Position = position;
            Orientation = orientation;
        }

        public PoseSerializable(float px, float py, float pz, float ox, float oy, float oz, float ow)
            : this(new Vector3Serializable(px, py, pz), new QuaternionSerializable(ox, oy, oz, ow))
        { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private PoseSerializable(SerializationInfo info, StreamingContext context)
            : this(info?.GetVector3(nameof(Position)) ?? throw new ArgumentNullException(nameof(info)),
                info.GetQuaternion(nameof(Orientation)))
        {
            info.CheckForType(TYPE_NAME);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("Type", TYPE_NAME);
            info.AddVector3(nameof(Position), Position);
            info.AddQuaternion(nameof(Orientation), Orientation);
        }

        public override bool Equals(object obj)
        {
            return obj is PoseSerializable other
                && Equals(other);
        }

        public bool Equals(PoseSerializable other)
        {
            return Position.Equals(other.Position)
                && Orientation.Equals(other.Orientation);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode()
                ^ Orientation.GetHashCode();
        }

        public static bool operator ==(PoseSerializable left, PoseSerializable right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PoseSerializable left, PoseSerializable right)
        {
            return !(left == right);
        }
    }
}
