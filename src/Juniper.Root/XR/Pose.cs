using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.XR
{
    [Serializable]
    public struct Pose : ISerializable, IEquatable<Pose>
    {
        public readonly float px, py, pz, ox, oy, oz, ow;

        public Pose(float px, float py, float pz, float ox, float oy, float oz, float ow)
        {
            this.px = px;
            this.py = py;
            this.pz = pz;
            this.ox = ox;
            this.oy = oy;
            this.oz = oz;
            this.ow = ow;
        }

        private Pose(SerializationInfo info, StreamingContext context)
            : this(info.GetSingle(nameof(px)),
                 info.GetSingle(nameof(py)),
                 info.GetSingle(nameof(pz)),
                 info.GetSingle(nameof(ox)),
                 info.GetSingle(nameof(oy)),
                 info.GetSingle(nameof(oz)),
                 info.GetSingle(nameof(ow)))
        { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(px), px);
            info.AddValue(nameof(py), py);
            info.AddValue(nameof(pz), pz);
            info.AddValue(nameof(ox), ox);
            info.AddValue(nameof(oy), oy);
            info.AddValue(nameof(oz), oz);
            info.AddValue(nameof(ow), ow);

        }

        public override bool Equals(object obj)
        {
            return obj is Pose other
                && Equals(other);
        }

        public bool Equals(Pose other)
        {
            return px == other.px
                && py == other.py
                && pz == other.pz
                && ox == other.ox
                && oy == other.oy
                && oz == other.oz
                && ow == other.ow;
        }

        public override int GetHashCode()
        {
            return px.GetHashCode()
                ^ py.GetHashCode()
                ^ pz.GetHashCode()
                ^ ox.GetHashCode()
                ^ oy.GetHashCode()
                ^ oz.GetHashCode()
                ^ ow.GetHashCode();
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
