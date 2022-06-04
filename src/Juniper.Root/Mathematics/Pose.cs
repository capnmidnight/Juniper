using System.Numerics;

namespace Juniper.Mathematics
{
    [Serializable]
    public struct Pose : IEquatable<Pose>
    {
        public static readonly Pose Identity = new(Vector3.Zero, Quaternion.Identity);

        public Vector3 Position { get; set; }

        public Quaternion Orientation { get; set; }

        public Pose(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Orientation = rotation;
        }

        public override bool Equals(object obj)
        {
            return obj is Pose pose && Equals(pose);
        }

        public bool Equals(Pose other)
        {
            return Position.Equals(other.Position)
                && Orientation.Equals(other.Orientation);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Position, Orientation);
        }

        public static Pose operator *(Pose a, Pose b)
        {
            return new Pose
            {
                Orientation = a.Orientation * b.Orientation,
                Position = Vector3.Transform(b.Position, a.Orientation) + a.Position
            };
        }

        /// <summary>
        /// Converts the pose from left- to right-handed or vice-versa.
        /// </summary>
        public Pose FlipZ()
        {
            var v = Position;
            var q = Orientation;
            v.Z = -v.Z;
            q.Z = -q.Z;
            q.W = -q.W;
            return new Pose(v, q);
        }

        public Pose Inverse()
        {
            var q = Quaternion.Inverse(Orientation);
            return new Pose(Vector3.Transform(-Position, q), q);
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
