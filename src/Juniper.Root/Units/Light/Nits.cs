namespace Juniper.Units
{
    /// <summary>
    /// Conversions from Nits
    /// </summary>
    public static class Nits
    {
        /// <summary>
        /// Conversion factor from Lumens to Nits.
        /// </summary>
        public const float PER_LUMEN = 1 / Units.Lumens.PER_NIT;

        /// <summary>
        /// Conversion factor from Brightness to Nits.
        /// </summary>
        public const float PER_BRIGHTNESS = Units.Lumens.PER_BRIGHTNESS / Units.Lumens.PER_NIT;

        /// <summary>
        /// Convert from Nits to Lumens
        /// </summary>
        /// <param name="nits">The number of nits</param>
        /// <returns>The number of lumens</returns>
        public static float Lumens(float nits)
        {
            return nits * Units.Lumens.PER_NIT;
        }

        /// <summary>
        /// Convert from Nits to Brightness
        /// </summary>
        /// <param name="nits">The number of nits</param>
        /// <returns>The brightness</returns>
        public static float Brightness(float nits)
        {
            return nits * Units.Brightness.PER_NIT;
        }
    }
}