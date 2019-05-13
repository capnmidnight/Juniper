namespace Juniper.World.GIS
{
    /// <summary>
    /// A model of the Earth.
    /// </summary>
    public static class DatumWGS_84
    {
        /// <summary>
        /// Maximum radius of the earth, in feet.
        /// </summary>
        public const float earthRadius = 20890848;

        /// <summary>
        /// Ratio between <see cref="earthRadius"/> and the radius of the earth at the Equator.
        /// </summary>
        public const float k = 1;

        /// <summary>
        /// Ratio between <see cref="earthRadius"/> and the radius of the earth at the poles.
        /// </summary>
        public const float k0 = 0.9996f;

        /// <summary>
        /// Unexplained
        /// </summary>
        public const float a = 2.093e7f;

        /// <summary>
        /// Unexplained
        /// </summary>
        public const float b = 2.0855486595e7f;

        /// <summary>
        /// Unexplained
        /// </summary>
        public const float f = (a - b) / a;

        /// <summary>
        /// Unexplained
        /// </summary>
        public const float e2 = 2 * f - f * f;

        /// <summary>
        /// Unexplained
        /// </summary>
        public const float ep2 = e2 / (1 - e2);

        /// <summary>
        /// Unexplained
        /// </summary>
        public const float n = (a - b) / (a + b);

        /// <summary>
        /// <see cref="n"/> squared.
        /// </summary>
        public const float n2 = n * n;

        /// <summary>
        /// <see cref="n"/> cubed.
        /// </summary>
        public const float n3 = n2 * n;
    }
}
