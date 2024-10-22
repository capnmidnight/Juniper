using Juniper.World.GIS;

using static System.Math;

namespace Juniper.Units;

public static class EquitorialSpherical
{
    /// <summary>
    /// Convert Equitorial Spherical Position to Horizontal Spherical Position.
    /// </summary>
    /// <param name="location">The point on earth</param>
    /// <param name="n">The Julian day</param>
    /// <returns>The elevation above the horizon</returns>
    public static HorizontalSphericalPosition ToHorizontal(this EquitorialSphericalPosition value, LatLngPoint location, double n)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (location is null)
        {
            throw new ArgumentNullException(nameof(location));
        }

        var GMST = (18.697374558 + (24.06570982441908 * n)).Repeat(24);
        var LST = GMST + Degrees.Hours(location.Lng);
        var RA = Degrees.Hours(value.RightAscensionDegrees);
        var H = Hours.Radians(LST - RA);
        var sin_H = Sin(H);
        var cos_H = Cos(H);
        var lat_rad = Degrees.Radians(location.Lat);
        // var lng_rad = lng_deg * Deg2Rad;
        var delta_rad = Degrees.Radians(value.DeclinationDegrees);
        var sin_delta = Sin(delta_rad);
        var cos_delta = Cos(delta_rad);
        var sin_lat = Sin(lat_rad);
        var cos_lat = Cos(lat_rad);
        var sin_alt = (sin_delta * sin_lat) + (cos_delta * cos_lat * cos_H);
        var cos_alt = Sqrt(1 - (sin_alt * sin_alt));
        var sin_azm = sin_H * cos_delta / cos_alt;
        var cos_azm = (sin_delta - (sin_lat * sin_alt)) / (cos_lat * cos_alt);

        var altitude_rad = Atan2(sin_alt, cos_alt);
        var azimuth_rad = Atan2(sin_azm, cos_azm);

        return new HorizontalSphericalPosition(
            Radians.Degrees(altitude_rad),
            (180 - Radians.Degrees(azimuth_rad)).Repeat(360),
            value.RadiusAU);
    }
}