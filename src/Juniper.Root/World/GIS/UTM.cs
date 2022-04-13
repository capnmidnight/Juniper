using Juniper.World.GIS;

using System;

using static System.Math;

namespace Juniper.Units
{
    public static class UTM
    {
        /// <summary>
        /// Converts this UTMPoint to a Latitude/Longitude point using the WGS-84 datum. The
        /// coordinate pair's units will be in meters, and should be usable to make distance
        /// calculations over short distances. /// reference: http://www.uwgb.edu/dutchs/usefuldata/utmformulas.htm
        /// </summary>
        /// <param name="utm">The UTM point to convert</param>
        /// <returns>The latitude/longitude</returns>
        public static LatLngPoint ToLatLng(this UTMPoint utm)
        {
            if (utm is null)
            {
                throw new ArgumentNullException(nameof(utm));
            }

            var N0 = (utm.Hemisphere == GlobeHemisphere.Northern || utm.Northing < 0)
                ? 0.0 
                : DatumWGS_84.FalseNorthing;
            var xi = (utm.Northing - N0) / (DatumWGS_84.pointScaleFactor * DatumWGS_84.A);
            var eta = (utm.Easting - DatumWGS_84.E0) / (DatumWGS_84.pointScaleFactor * DatumWGS_84.A);
            var xiPrime = xi;
            var etaPrime = eta;
            double sigmaPrime = 1;
            double tauPrime = 0;

            for (var j = 1; j <= 3; ++j)
            {
                var beta = DatumWGS_84.beta[j - 1];
                var je2 = 2 * j * xi;
                var jn2 = 2 * j * eta;
                var sinje2 = Sin(je2);
                var coshjn2 = Cosh(jn2);
                var cosje2 = Cos(je2);
                var sinhjn2 = Sinh(jn2);

                xiPrime -= beta * sinje2 * coshjn2;
                etaPrime -= beta * cosje2 * sinhjn2;
                sigmaPrime -= 2 * j * beta * cosje2 * coshjn2;
                tauPrime -= 2 * j * beta * sinje2 * sinhjn2;
            }

            var chi = Asin(Sin(xiPrime) / Cosh(etaPrime));

            var lat = chi;

            for (var j = 1; j <= 3; ++j)
            {
                lat += DatumWGS_84.delta[j - 1] * Sin(2 * j * chi);
            }

            var long0 = (utm.Zone * 6) - 183;
            var lng = Atan(Sinh(etaPrime) / Cos(xiPrime));

            return new LatLngPoint(
                Radians.Degrees((float)lat),
                long0 + Radians.Degrees((float)lng),
                utm.Altitude);
        }
    }
}