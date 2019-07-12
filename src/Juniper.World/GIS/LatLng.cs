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

            double N0 = hemisphere == UTMPoint.GlobeHemisphere.Northern ? 0.0 : 10000000.0;

            int zone = (int)Math.Floor(latlng.Longitude / 6 + 31);

            double a = DatumWGS_84.equatorialRadius_a;
            double f = DatumWGS_84.flattening_f;
            double b = a * (1 - f);   // polar radius

            double e = Math.Sqrt(1 - Math.Pow(b, 2) / Math.Pow(a, 2));
            double k0 = 0.9996;

            double phi = Degrees.Radians(latlng.Latitude);
            double utmz = 1 + Math.Floor((latlng.Longitude + 180) / 6.0);
            double zcm = 3 + 6.0 * (utmz - 1) - 180;

            double esq = (1 - (b / a) * (b / a));
            double e0sq = e * e / (1 - Math.Pow(e, 2));


            double N = a / Math.Sqrt(1 - Math.Pow(e * Math.Sin(phi), 2));
            double T = Math.Pow(Math.Tan(phi), 2);
            double C = e0sq * Math.Pow(Math.Cos(phi), 2);
            double A = Degrees.Radians((float)(latlng.Longitude - zcm)) * Math.Cos(phi);

            double M = phi * (1 - esq * (1.0 / 4.0 + esq * (3.0 / 64.0 + 5.0 * esq / 256.0)));
            M -= Math.Sin(2.0 * phi) * (esq * (3.0 / 8.0 + esq * (3.0 / 32.0 + 45.0 * esq / 1024.0)));
            M += Math.Sin(4.0 * phi) * (esq * esq * (15.0 / 256.0 + esq * 45.0 / 1024.0));
            M -= Math.Sin(6.0 * phi) * (esq * esq * esq * (35.0 / 3072.0));
            M *= a;

            double M0 = 0;

            // Easting
            var easting = k0 * N * A * (1 + A * A * ((1 - T + C) / 6 + A * A * (5 - 18 * T + T * T + 72.0 * C - 58 * e0sq) / 120.0));
            easting += DatumWGS_84.E0;


            // Northing

            double northing = k0 * (M - M0 + N * Math.Tan(phi) * (A * A * (1 / 2.0 + A * A * ((5 - T + 9 * C + 4 * C * C) / 24.0 + A * A * (61 - 58 * T + T * T + 600 * C - 330 * e0sq) / 720.0))));    // first from the equator
            if (northing < 0)
            {
                northing = N0 + northing;
            }


            return new UTMPoint(
                (float)easting,
                (float)northing,
                latlng.Altitude,
                zone,
                hemisphere);
        }

        private static double Atanh(double value)
        {
            return Math.Log((1 + value) / (1 - value)) / 2;
        }
    }
}
