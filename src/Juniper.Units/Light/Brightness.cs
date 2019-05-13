namespace Juniper.Units
{
    /// <summary>
    /// Conversions from Brightness
    /// </summary>
    public static class Brightness
    {
        /// <summary>
        /// Conversion factor from Lumens to Brightness.
        /// </summary>
        public const float PER_LUMEN = 1 / Units.Lumens.PER_BRIGHTNESS;

        /// <summary>
        /// Conversion factor from Nits to Brightness.
        /// </summary>
        public const float PER_NIT = Units.Lumens.PER_NIT / Units.Lumens.PER_BRIGHTNESS;

        /// <summary>
        /// Convert from Brightness to Lumens
        /// </summary>
        /// <param name="brightness">The number of brightness</param>
        /// <returns>The number of lumens</returns>
        public static float Lumens(float brightness)
        {
            return brightness * Units.Lumens.PER_BRIGHTNESS;
        }

        /// <summary>
        /// Convert from Brightness to Nits
        /// </summary>
        /// <param name="brightness">The number of brightness</param>
        /// <returns>The number of nits</returns>
        public static float Nits(float brightness)
        {
            return brightness * Units.Nits.PER_BRIGHTNESS;
        }
    }
}
