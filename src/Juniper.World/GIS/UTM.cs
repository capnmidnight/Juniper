using System;
using Juniper.World.GIS;

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
            var northing = Units.Meters.Feet(utm.Y);
            var easting = Units.Meters.Feet(utm.X);

            //if(northing < 0 || northing > 10000000)
            //{
            //    throw new Exception($"Northing must be between 0 and 10000000. Was {northing}");
            //}

            //if(easting < 160000 || easting > 834000)
            //{
            //    throw new Exception($"Easting coordinate crosses zone boundries, results should be used with caution. Was {easting}");
            //}

            //Central meridian of zone
            var zcm = 3 + 6 * (utm.Zone - 1) - 180;
            var e1 = (1 - Math.Sqrt(1 - DatumWGS_84.e2)) / (1 + Math.Sqrt(1 - DatumWGS_84.e2));

            //In case origin other than zero lat - not needed for standard UTM
            var M0 = 0f;
            var M = 0f;

            if (utm.Hemisphere == UTMPoint.GlobeHemisphere.Southern)
            {
                M = M0 + (northing - 10000000) / DatumWGS_84.k0;
            }
            else
            {
                //Arc length along standard meridian.
                M = M0 + northing / DatumWGS_84.k;
            }

            var mu = M / (DatumWGS_84.a * (1 - DatumWGS_84.e2 * (1 / 4 + DatumWGS_84.e2 * (3 / 64 + 5 * DatumWGS_84.e2 / 256))));
            //Footprint Latitude
            var phi1 = mu + e1 * (3 / 2 - 27 * e1 * e1 / 32) * Math.Sin(2 * mu) + e1 * e1 * (21 / 16 - 55 * e1 * e1 / 32) * Math.Sin(4 * mu);
            phi1 += e1 * e1 * e1 * (Math.Sin(6 * mu) * 151 / 96 + e1 * Math.Sin(8 * mu) * 1097 / 512);
            var C1 = DatumWGS_84.ep2 * Math.Pow(Math.Cos(phi1), 2);
            var T1 = Math.Pow(Math.Tan(phi1), 2);
            var N1 = DatumWGS_84.a / Math.Sqrt(1 - DatumWGS_84.e2 * Math.Pow(Math.Sin(phi1), 2));
            var R1 = N1 * (1 - DatumWGS_84.e2) / (1 - DatumWGS_84.e2 * Math.Pow(Math.Sin(phi1), 2));
            var D = (easting - 500000) / (N1 * DatumWGS_84.k0);
            var phi = (D * D) * (1 / 2 - D * D * (5 + 3 * T1 + 10 * C1 - 4 * C1 * C1 - 9 * DatumWGS_84.ep2) / 24);
            phi = phi + Math.Pow(D, 6) * (61 + 90 * T1 + 298 * C1 + 45 * T1 * T1 - 252 * DatumWGS_84.ep2 - 3 * C1 * C1) / 720;
            phi = phi1 - (N1 * Math.Tan(phi1) / R1) * phi;

            var lat = Math.Floor(1000000 * Units.Radians.Degrees((float)phi)) / 1000000;
            var lng = D * (1 + D * D * ((-1 - 2 * T1 - C1) / 6 + D * D * (5 - 2 * C1 + 28 * T1 - 3 * C1 * C1 + 8 * DatumWGS_84.ep2 + 24 * T1 * T1) / 120)) / Math.Cos(phi1);
            lng = zcm + Units.Radians.Degrees((float)lng);

            return new LatLngPoint((float)lat, (float)lng, utm.Z);
        }
    }
}
