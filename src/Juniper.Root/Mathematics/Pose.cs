using System;
using System.Numerics;

namespace Juniper.Mathematics
{
    public struct Pose : IEquatable<Pose>
    {
        public static readonly Pose Identity = new Pose(Vector3.Zero, Quaternion.Identity);

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
            var hashCode = -388643783;
            hashCode = hashCode * -1521134295 + Position.GetHashCode();
            hashCode = hashCode * -1521134295 + Orientation.GetHashCode();
            return hashCode;
        }

        public static Pose operator *(Pose a, Pose b)
        {
            return new Pose
            {
                Orientation = a.Orientation * b.Orientation,
                Position = Vector3.Transform(b.Position, a.Orientation) + a.Position
            };
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
