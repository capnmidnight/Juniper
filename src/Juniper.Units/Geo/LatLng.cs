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
            var long0 = 3 + (6 * Math.Floor((latlng.Longitude + 180) / 6)) - 180;
            var lat = latlng.Latitude * Math.PI / 180;
            var lng = (latlng.Longitude - long0) * Math.PI / 180;
            var lng2 = lng * lng;
            var lng3 = lng2 * lng;
            var lng4 = lng3 * lng;
            var lng5 = lng4 * lng;
            var lng6 = lng5 * lng;
            var lng7 = lng6 * lng;
            var lng8 = lng7 * lng;
            var s = Math.Sin(lat);
            var c = Math.Cos(lat);
            var c2 = c * c;
            var c3 = c2 * c;
            var c4 = c3 * c;
            var c5 = c4 * c;
            var c6 = c5 * c;
            var c7 = c6 * c;
            var c8 = c7 * c;
            var t = Math.Tan(lat);
            var t2 = t * t;
            var t4 = t2 * t2;
            var t6 = t4 * t2;
            var v = DatumWGS_84.a / Math.Sqrt(1 - DatumWGS_84.e2 * s * s);
            var rho = Math.Pow(v, 3) * (1 - DatumWGS_84.e2) / (DatumWGS_84.a * DatumWGS_84.a);
            var beta = v / rho;
            var beta2 = beta * beta;
            var beta3 = beta2 * beta;
            var beta4 = beta2 * beta2;
            //var nu2 = beta - 1;

            // the following is actually an infinite series, but to this many terms it should be
            // accurate to 0.1mm
            var B0 = DatumWGS_84.b * (1 + DatumWGS_84.n + 5 * (DatumWGS_84.n2 + DatumWGS_84.n3) / 4);
            var B2 = -DatumWGS_84.b * 3 * (DatumWGS_84.n + DatumWGS_84.n2 + 7 * DatumWGS_84.n3 / 8) / 2;
            var B4 = DatumWGS_84.b * 15 * (DatumWGS_84.n2 + DatumWGS_84.n3) / 16;
            var B6 = -DatumWGS_84.b * 35 * DatumWGS_84.n3 / 48;
            var m = B0 * lat
                + B2 * Math.Sin(2 * lat)
                + B4 * Math.Sin(4 * lat)
                + B6 * Math.Sin(6 * lat);
            var W3 = beta - t2;
            var W5 = 4 * beta3 * (1 - 6 * t2) + beta2 * (1 + 8 * t2) - 2 * beta * t2 + t4;
            var W7 = 61 - 479 * t2 + 179 * t4 - t6 + DatumWGS_84.ep2;
            var W4 = 4 * beta2 + beta - t2;
            var W6 = 8 * beta4 * (11 - 24 * t2) - 28 * beta3 * (1 - 6 * t2) + beta2 * (1 - 32 * t2) - 2 * beta * t2 + t4;
            var W8 = 1385 - 3111 * t2 + 543 * t4 - t6 + DatumWGS_84.ep2;
            var x = DatumWGS_84.k0 * v * (lng * c + lng3 * c3 * W3 / 6 + lng5 * c5 * W5 / 120 + lng7 * c7 * W7 / 5040);
            var y = DatumWGS_84.k0 * (m + v * (lng2 * c2 * t / 2 + lng4 * c4 * t * W4 / 24 + lng6 * c6 * t * W6 / 720 + lng8 * c8 * t * W8 / 40320));
            var z = 0;

            if (latlng.Longitude >= 8
                && latlng.Longitude <= 13
                && latlng.Latitude > 54.5
                && latlng.Latitude < 58)
            {
                z = 32;
            }
            else if (latlng.Latitude >= 56.0
                && latlng.Latitude < 64.0
                && latlng.Longitude >= 3.0
                && latlng.Longitude < 12.0)
            {
                z = 32;
            }
            else
            {
                z = (int)((latlng.Longitude + 180) / 6) + 1;

                if (latlng.Latitude >= 72.0
                    && latlng.Latitude < 84.0)
                {
                    if (latlng.Longitude >= 0.0
                        && latlng.Longitude < 9.0)
                    {
                        z = 31;
                    }
                    else if (latlng.Longitude >= 9.0
                        && latlng.Longitude < 21.0)
                    {
                        z = 33;
                    }
                    else if (latlng.Longitude >= 21.0
                        && latlng.Longitude < 33.0)
                    {
                        z = 35;
                    }
                    else if (latlng.Longitude >= 33.0
                        && latlng.Longitude < 42.0)
                    {
                        z = 37;
                    }
                }
            }
            return new UTMPoint(
                Units.Feet.Meters((float)x),
                Units.Feet.Meters((float)y),
                latlng.Altitude,
                z,
                latlng.Latitude > 0
                    ? UTMPoint.GlobeHemisphere.Southern
                    : UTMPoint.GlobeHemisphere.Northern);
        }
    }
}
