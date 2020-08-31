using System;
using System.Collections.Generic;

namespace Accord.Math
{
    public static class MathExt
    {
        public static Juniper.Mathematics.Matrix4x4Serializable ToJuniperMatrix4x4Serializable(this Matrix4x4 v)
        {
            return new Juniper.Mathematics.Matrix4x4Serializable(
                v.V00, v.V01, v.V02, v.V03,
                v.V10, v.V11, v.V12, v.V13,
                v.V20, v.V21, v.V22, v.V23,
                v.V30, v.V31, v.V32, v.V33);
        }

        public static System.Numerics.Matrix4x4 ToSystemMatrix4x4(this Matrix4x4 v)
        {
            return new System.Numerics.Matrix4x4(
                v.V00, v.V01, v.V02, v.V03,
                v.V10, v.V11, v.V12, v.V13,
                v.V20, v.V21, v.V22, v.V23,
                v.V30, v.V31, v.V32, v.V33);
        }

        public static Matrix4x4 ToAccordMatrix4x4(this float[] values)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Length != 16)
            {
                throw new ArgumentOutOfRangeException(nameof(values), "values array must be 16 elements long");
            }

            return new Matrix4x4 {
                V00 = values[0], V01 = values[1], V02 = values[2], V03 = values[3],
                V10 = values[4], V11 = values[5], V12 = values[6], V13 = values[7],
                V20 = values[8], V21 = values[9], V22 = values[10],V23 = values[11],
                V30 = values[12],V31 = values[13],V32 = values[14],V33 = values[15]
            };
        }

        public static Juniper.Mathematics.Vector3Serializable ToJuniperVector3Serializable(this Vector3 v)
        {
            return new Juniper.Mathematics.Vector3Serializable(v.X, v.Y, v.Z);
        }

        public static System.Numerics.Vector3 ToSystemVector3(this Vector3 v)
        {
            return new System.Numerics.Vector3(v.X, v.Y, v.Z);
        }

        /// <summary>
        /// Convert a Vector3 with a given UTM Zone to a <see cref="UTMPoint"/>
        /// </summary>
        /// <returns>The utm.</returns>
        /// <param name="v">      Value.</param>
        /// <param name="donatedZone">Donated zone.</param>
        public static Juniper.World.GIS.UTMPoint ToUTM(this Vector3 v, int donatedZone, Juniper.World.GIS.UTMPoint.GlobeHemisphere hemisphere)
        {
            return new Juniper.World.GIS.UTMPoint(v.X, v.Z, v.Y, donatedZone, hemisphere);
        }

        /// <summary>
        /// Calculate the centroid of a cloud of points. The centroid is just a fancy name for the
        /// average of all the vectors together.
        /// </summary>
        /// <returns>The centroid.</returns>
        /// <param name="points">Points.</param>
        public static Vector3 Centroid(this IEnumerable<Vector3> points)
        {
            if (points is null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            var count = 0;
            var avg = new Vector3();
            foreach (var point in points)
            {
                ++count;
                avg += point;
            }

            avg /= count;

            return avg;
        }

        public static Vector3 ToAccordVector3(float[] values)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Length != 3)
            {
                throw new ArgumentOutOfRangeException(nameof(values), "values array must be 3 elements long");
            }

            return new Vector3(values[0], values[1], values[2]);
        }

        public static Juniper.Mathematics.Vector3Serializable ToJuniperVector3Serializable(this Point3 v)
        {
            return new Juniper.Mathematics.Vector3Serializable(v.X, v.Y, v.Z);
        }

        public static System.Numerics.Vector3 ToSystemVector3(this Point3 v)
        {
            return new System.Numerics.Vector3(v.X, v.Y, v.Z);
        }

        public static Point3 ToAccordPoint3(float[] values)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Length != 3)
            {
                throw new ArgumentOutOfRangeException(nameof(values), "values array must be 3 elements long");
            }

            return new Point3(values[0], values[1], values[2]);
        }

        public static float[] ToArray(Point3 v)
        {
            return new float[]
            {
                v.X,
                v.Y,
                v.Z
            };
        }

        public static Juniper.Mathematics.Vector4Serializable ToJuniperVector4Serializable(this Vector4 v)
        {
            return new Juniper.Mathematics.Vector4Serializable(v.X, v.Y, v.Z, v.W);
        }

        public static System.Numerics.Vector4 ToSystemVector4(this Vector4 v)
        {
            return new System.Numerics.Vector4(v.X, v.Y, v.Z, v.W);
        }

        public static Vector4 ToAccordVector4(float[] values)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Length != 4)
            {
                throw new ArgumentOutOfRangeException(nameof(values), "values array must be 4 elements long");
            }

            return new Vector4(values[0], values[1], values[2], values[3]);
        }

        public static System.Numerics.Plane ToSystemPlane(this Plane p)
        {
            if (p is null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            return new System.Numerics.Plane(p.Normal.ToSystemVector3(), p.Offset);
        }

        public static Juniper.Mathematics.PlaneSerializable ToJuniperPlaneSerializable(this Plane p)
        {
            if (p is null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            return new Juniper.Mathematics.PlaneSerializable(p.A, p.B, p.C, p.Offset);
        }

        public static float[] ToArray(Plane p)
        {
            if(p is null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            return new float[]
            {
                p.A,
                p.B,
                p.C,
                p.Offset
            };
        }

        public static Plane ToAccordPlane(float[] values)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Length != 4)
            {
                throw new ArgumentOutOfRangeException(nameof(values), "values array must be 4 elements long");
            }

            return new Plane(values[0], values[1], values[2], values[3]);
        }
    }
}
