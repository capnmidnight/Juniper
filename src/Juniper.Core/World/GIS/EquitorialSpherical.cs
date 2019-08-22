using System;

using Juniper.World.GIS;

namespace Juniper.Units
{
    public static class EquitorialSpherical
    {
        /// <summary>
        /// Convert Equitorial Spherical Position to Horizontal Spherical Position.
        /// </summary>
        /// <param name="location">The point on earth</param>
        /// <param name="n">The Julian day</param>
        /// <returns>The elevation above the horizon</returns>
        public static HorizontalSphericalPosition ToHorizontal(this EquitorialSphericalPosition value, LatLngPoint location, float n)
        {
            var GMST = (18.697374558f + 24.06570982441908f * n).Repeat(24);
            var LST = GMST + Units.Degrees.Hours(location.Longitude);
            var RA = Units.Degrees.Hours(value.RightAscensionDegrees);
            var H = Units.Hours.Radians(LST - RA);
            var sin_H = Math.Sin(H);
            var cos_H = Math.Cos(H);
            var lat_rad = Units.Degrees.Radians(location.Latitude);
            // var lng_rad = lng_deg * Math.Deg2Rad;
            var delta_rad = Units.Degrees.Radians(value.DeclinationDegrees);
            var sin_delta = Math.Sin(delta_rad);
            var cos_delta = Math.Cos(delta_rad);
            var sin_lat = Math.Sin(lat_rad);
            var cos_lat = Math.Cos(lat_rad);
            var sin_alt = sin_delta * sin_lat + cos_delta * cos_lat * cos_H;
            var cos_alt = Math.Sqrt(1 - sin_alt * sin_alt);
            var sin_azm = sin_H * cos_delta / cos_alt;
            var cos_azm = (sin_delta - sin_lat * sin_alt) / (cos_lat * cos_alt);

            var altitude_rad = (float)Math.Atan2(sin_alt, cos_alt);
            var azimuth_rad = (float)Math.Atan2(sin_azm, cos_azm);

            return new HorizontalSphericalPosition(
                Units.Radians.Degrees(altitude_rad),
                (180 - Units.Radians.Degrees(azimuth_rad)).Repeat(360),
                value.RadiusAU);
        }
    }
}