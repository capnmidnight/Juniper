namespace Juniper.World.GIS
{
    /// <summary>
    /// A model of the Earth.
    /// </summary>
    public static class DatumWGS_84
    {
        /// <summary>
        /// Maximum radius of the earth, in meters.
        /// </summary>
        public const double equatorialRadius_a = 6378137;
        public const double invF = 298.257223563;
        public const double pointScaleFactor_k0 = 0.9996;
        public const double E0 = 500000;

        public const double flattening_f = 1 / invF;

        public const double n = flattening_f / (2 - flattening_f);
        public const double A = (equatorialRadius_a / (1 + n)) * (1 + n * n / 4 + n * n * n * n / 64);
        public static readonly double[] alpha = {
            n / 2 - 2 * n * n / 3 + 5 * n * n * n / 16,
            13 * n * n / 48 - 3 * n * n * n / 5,
            61 * n * n * n / 240
        };
        public static readonly double[] beta = {
            n / 2 - 2 * n * n / 3 + 37 * n * n * n / 96,
            n * n / 48 + n * n * n / 15,
            17 * n * n * n / 480
        };
        public static readonly double[] delta = {
            2 * n - 2 * n * n / 3,
            7 * n * n / 3 - 8 * n * n * n / 5,
            56 * n * n * n / 15
        };
    }
}
