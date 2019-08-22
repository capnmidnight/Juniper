using System;
using Juniper.World.GIS;

namespace Juniper.Units
{
    public static class LatLng
    {
        /// <summary>
        /// Calculate a rough distance, in meters, between two LatLngPoints.
        /// </summary>
        /// <param name="p">The second point to which to measure</param>
        /// <returns>The distance between the two points, after conversion to UTM</returns>
        public static float Distance(this LatLngPoint A, LatLngPoint B)
        {
            var a = A.ToUTM();
            var b = B.ToUTM();
            var dx = b.X - a.X;
            var dy = b.Y - a.Y;
            return (float)Math.Sqrt((dx * dx) + (dy * dy));
        }

        /// <summary>
        /// Calculate the position of the Sun at the given time as an azimuth and altitude bearing
        /// from the given location on spaceship Earth. /// SEE: https://en.wikipedia.org/wiki/Position_of_the_Sun
        /// </summary>
        /// <param name="location">The position of the viewer on earth</param>
        /// <param name="time">The time of day</param>
        /// <returns>The position of the sun in the sky</returns>
        public static HorizontalSphericalPosition ToSunPosition(this LatLngPoint location, DateTime time)
        {
            var n = time.ToJulianDays();
            var ec = n.ToGeocentricEclipticSphericalFromJulianDay();
            var eq = ec.ToEquitorial(n);
            return eq.ToHorizontal(location, n);
        }

        /// <summary>
        /// Converts this LatLngPoint to a Universal Transverse Mercator point using the WGS-84
        /// datum. The coordinate pair's units will be in meters, and should be usable to make
        /// distance calculations over short distances. /// reference: http://www.uwgb.edu/dutchs/usefuldata/utmformulas.htm
        /// </summary>
        /// <param name="latlng">The point on Earth to convert to UTM</param>
        /// <returns>The UTM point</returns>
        public static UTMPoint ToUTM(this LatLngPoint latlng)
        {
            var hemisphere = latlng.Latitude < 0
                    ? UTMPoint.GlobeHemisphere.Southern
                    : UTMPoint.GlobeHemisphere.Northern;

            var k0 = 0.9996;

            double phi = Degrees.Radians(latlng.Latitude);
            var sinPhi = Math.Sin(phi);
            var cosPhi = Math.Cos(phi);
            var sin2Phi = 2 * sinPhi * cosPhi;
            var cos2Phi = 2 * cosPhi * cosPhi - 1;
            var sin4Phi = 2 * sin2Phi * cos2Phi;
            var cos4Phi = 2 * cos2Phi * cos2Phi - 1;
            var sin6Phi = sin4Phi * cos2Phi + cos4Phi * sin2Phi;
            var tanPhi = sinPhi / cosPhi;
            var ePhi = DatumWGS_84.e * sinPhi;
            var N = DatumWGS_84.equatorialRadius / Math.Sqrt(1 - ePhi * ePhi);

            var utmz = 1 + (int)Math.Floor((latlng.Longitude + 180) / 6.0);
            var zcm = 3 + 6.0 * (utmz - 1) - 180;
            var A = Degrees.Radians((float)(latlng.Longitude - zcm)) * cosPhi;

            var M = DatumWGS_84.equatorialRadius * (
                phi * DatumWGS_84.alpha1
                - sin2Phi * DatumWGS_84.alpha2
                + sin4Phi * DatumWGS_84.alpha3
                - sin6Phi * DatumWGS_84.alpha4);

            // Easting
            var T = tanPhi * tanPhi;
            var C = DatumWGS_84.e0sq * cosPhi * cosPhi;
            var Asqr = A * A;
            var Tsqr = T * T;
            var x0 = 1 - T + C;
            var x1 = 5 - 18 * T + Tsqr + 72.0 * C - 58 * DatumWGS_84.e0sq;
            var x2 = Asqr * x1 / 120.0;
            var x3 = x0 / 6 + x2;
            var x4 = 1 + Asqr * x3;
            var easting = k0 * N * A * x4;
            easting += DatumWGS_84.E0;

            // Northing
            var northing = k0 * (M + N * tanPhi * (Asqr * (1 / 2.0 + Asqr * ((5 - T + 9 * C + 4 * C * C) / 24.0 + Asqr * (61 - 58 * T + Tsqr + 600 * C - 330 * DatumWGS_84.e0sq) / 720.0))));
            if (hemisphere == UTMPoint.GlobeHemisphere.Southern)
            {
                northing = 10000000.0 + northing;
            }

            return new UTMPoint(
                (float)easting,
                (float)northing,
                latlng.Altitude,
                utmz,
                hemisphere);
        }
    }
}