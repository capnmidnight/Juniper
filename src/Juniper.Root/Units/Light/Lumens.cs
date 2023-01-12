namespace Juniper.Units
{
    /// <summary>
    /// Conversions from Lumens
    /// </summary>
    public static class Lumens
    {
        /// <summary>
        /// Conversion factor from Nits to Lumens.
        /// </summary>
        public const double PER_NIT = 3.426;

        /// <summary>
        /// Conversion factor from Brightness to Lumens.
        /// </summary>
        public const double PER_BRIGHTNESS = 1000;

        /// <summary>
        /// Convert from Lumens to Nits
        /// </summary>
        /// <param name="lumens">The number of lumens</param>
        /// <returns>The number of nits</returns>
        public static double Nits(double lumens)
        {
            return lumens * Units.Nits.PER_LUMEN;
        }

        /// <summary>
        /// Convert from Lumens to Brightness
        /// </summary>
        /// <param name="lumens">The number of lumens</param>
        /// <returns>The number of brightness</returns>
        public static double Brightness(double lumens)
        {
            return lumens * Units.Brightness.PER_LUMEN;
        }
    }
}