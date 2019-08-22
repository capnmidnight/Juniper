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
        public const float PER_NIT = 3.426f;

        /// <summary>
        /// Conversion factor from Brightness to Lumens.
        /// </summary>
        public const float PER_BRIGHTNESS = 1000;

        /// <summary>
        /// Convert from Lumens to Nits
        /// </summary>
        /// <param name="lumens">The number of lumens</param>
        /// <returns>The number of nits</returns>
        public static float Nits(float lumens)
        {
            return lumens * Units.Nits.PER_LUMEN;
        }

        /// <summary>
        /// Convert from Lumens to Brightness
        /// </summary>
        /// <param name="lumens">The number of lumens</param>
        /// <returns>The number of brightness</returns>
        public static float Brightness(float lumens)
        {
            return lumens * Units.Brightness.PER_LUMEN;
        }
    }
}