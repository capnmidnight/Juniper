using Juniper.Units;
using Juniper.World.GIS;

namespace System.Numerics;

public static class MathExt
{
    /// <summary>
    /// Convert a Vector3 with a given UTM Zone to a <see cref="UTMPoint"/>
    /// </summary>
    /// <returns>The utm.</returns>
    /// <param name="v">      Value.</param>
    /// <param name="donatedZone">Donated zone.</param>
    public static UTMPoint ToUTM(this Vector3 v, int donatedZone, GlobeHemisphere hemisphere)
    {
        return new UTMPoint(v.X, v.Z, v.Y, donatedZone, hemisphere);
    }

    /// <summary>
    /// Convert a Vector3 with a given UTM Zone to a <see cref="UTMPoint"/>
    /// </summary>
    /// <returns>The utm.</returns>
    /// <param name="v">      Value.</param>
    /// <param name="donatedZone">Donated zone.</param>
    public static Vector3 ToSystemVector3(this UTMPoint v)
    {
        if (v is null)
        {
            throw new ArgumentNullException(nameof(v));
        }

        return new Vector3(
            (float)v.Easting,
            (float)v.Altitude,
            (float)v.Northing);
    }

    /// <summary>
    /// Convert this UTMPoint to a Unity Vector3, projected to the screen using the provided matrix.
    /// </summary>
    /// <param name="v">   </param>
    /// <param name="toScreen"></param>
    /// <returns></returns>
    public static Vector3 ToScreen(this UTMPoint v, Matrix4x4 toScreen)
    {
        if (v is null)
        {
            throw new ArgumentNullException(nameof(v));
        }

        return Vector3.Transform(v.ToSystemVector3(), toScreen);
    }

    /// <summary>
    /// Converts this LatLngPoint to a UTM point that is stored as a Unity Vector3 object.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Vector3 ToVector3(this LatLngPoint value)
    {
        return value.ToUTM().ToSystemVector3();
    }

    /// <summary>
    /// Converts this LatLngPoint to an intermediate value roughly corresponding to feet that
    /// maintains perpendicular and parallel line relationships. The spherical model of the Earth
    /// that this function uses ruins it for distance calculations.
    /// </summary>
    /// <returns></returns>
    [Obsolete("This should really only ever be used when trying to match visuals to Google Maps.")]
    public static Vector3 SphericalMercator(this LatLngPoint value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var lat = Degrees.Radians(value.Lat);
        var lng = Degrees.Radians(value.Lng);
        var x = DatumWGS_84.equatorialRadius * lng;
        var y = DatumWGS_84.equatorialRadius * Math.Log(Math.Tan((0.25 * Math.PI) + (0.5 * lat)));
        return new Vector3(
            (float)x,
            (float)y,
            (float)value.Alt);
    }

    /// <summary>
    /// Converts an AltitudeAzimuth to Unity's standard rotation construct.
    /// </summary>
    public static Quaternion ToQuaternion(this HorizontalSphericalPosition p)
    {
        if (p is null)
        {
            throw new ArgumentNullException(nameof(p));
        }

        return Quaternion.CreateFromYawPitchRoll(
            (float)p.AzimuthDegrees,
            (float)p.AltitudeDegrees,
            0);
    }

    /// <summary>
    /// Converts an AltitudeAzimuth to Unity's standard Euler rotation construct.
    /// </summary>
    public static Vector3 ToEuler(this HorizontalSphericalPosition p)
    {
        if (p is null)
        {
            throw new ArgumentNullException(nameof(p));
        }

        return new Vector3(
            (float)p.AltitudeDegrees,
            (float)p.AzimuthDegrees,
            0);
    }
}
