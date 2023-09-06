namespace Juniper.Mathematics;

public static class MathExtensions
{

    public static Accord.Math.Matrix4x4 ToAccordMatrix4x4(this Matrix4x4Serializable matrix)
    {
        return new Accord.Math.Matrix4x4
        {
            V00 = matrix.Values[0x0],
            V01 = matrix.Values[0x1],
            V02 = matrix.Values[0x2],
            V03 = matrix.Values[0x3],
            V10 = matrix.Values[0x4],
            V11 = matrix.Values[0x5],
            V12 = matrix.Values[0x6],
            V13 = matrix.Values[0x7],
            V20 = matrix.Values[0x8],
            V21 = matrix.Values[0x9],
            V22 = matrix.Values[0xA],
            V23 = matrix.Values[0xB],
            V30 = matrix.Values[0xC],
            V31 = matrix.Values[0xD],
            V32 = matrix.Values[0xE],
            V33 = matrix.Values[0xF]
        };
    }
    public static Accord.Math.Plane ToAccordPlane(this PlaneSerializable plane)
    {
        return new Accord.Math.Plane(plane.X, plane.Y, plane.Z, plane.D);
    }


    public static Accord.Math.Vector3 ToAccordVector3(this Vector3Serializable vector)
    {
        return new Accord.Math.Vector3(vector.X, vector.Y, vector.Z);
    }

    public static Accord.Math.Point3 ToAccordPoint3(this Vector3Serializable vector)
    {
        return new Accord.Math.Point3(vector.X, vector.Y, vector.Z);
    }

    public static Accord.Math.Vector4 ToAccordVector4(this Vector4Serializable vector)
    {
        return new Accord.Math.Vector4(vector.X, vector.Y, vector.Z, vector.W);
    }
}
