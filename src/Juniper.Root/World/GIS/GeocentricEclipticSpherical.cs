using Juniper.World.GIS;

using System;

using static System.Math;

namespace Juniper.Units
{
    public static class GeocentricEclipticSpherical
    {
        /// <summary>
        /// Calculates the position of the Sun on the Ecliptic for the given Julian Day
        /// </summary>
        /// <param name="n">The Julian Day</param>
        /// <returns>The position of the sun on the ecliptic</returns>
        public static GeocentricEclipticSphericalPosition ToGeocentricEclipticSphericalFromJulianDay(this float n)
        {
            var L_deg = (280.460f + (0.9856474f * n)).Repeat(360f);
            var g_deg = (357.528f + (0.9856003f * n)).Repeat(360f);
            var g_rad = Degrees.Radians(g_deg);
            var sin_g = Sin(g_rad);
            var cos_g = Cos(g_rad);
            var lambda_deg = ((float)(L_deg + (1.915f * sin_g) + (0.020f * 2 * sin_g * cos_g))).Repeat(360);
            var R = 1.00014f - (0.01671f * cos_g) - (0.00014f * ((cos_g * cos_g) - (sin_g * sin_g)));
            var ec = new GeocentricEclipticSphericalPosition(0, lambda_deg, (float)R);
            return ec;
        }

        /// <summary>
        /// This calculation is only good for the Sun, as it does not take the Ecliptic Latitude into consideration.
        /// </summary>
        /// <param name="n">The Julian Day</param>
        /// <returns>The position of the son on the equitorial plane</returns>
        public static EquitorialSphericalPosition ToEquitorial(this GeocentricEclipticSphericalPosition p, float n)
        {
            if (p is null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            var epsilon_deg = 23.439f - (0.0000004f * n);
            var epsilon_rad = Degrees.Radians(epsilon_deg);
            var sin_epsilon = Sin(epsilon_rad);
            var cos_epsilon = Cos(epsilon_rad);
            var lambda_rad = Degrees.Radians(p.LongitudeDegrees);
            var sin_lambda = Sin(lambda_rad);
            var cos_lambda = Cos(lambda_rad);

            var alpha_rad = (float)Atan2(cos_epsilon * sin_lambda, cos_lambda);
            var delta_rad = (float)Asin(sin_epsilon * sin_lambda);

            return new EquitorialSphericalPosition(
                Radians.Degrees(alpha_rad),
                Radians.Degrees(delta_rad),
                p.RadiusAU);
        }
    }
}