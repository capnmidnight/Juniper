namespace Juniper.Units
{
    /// <summary>
    /// Conversions from feet per second squared
    /// </summary>
    public static class FeetPerSecondSquared
    {
        /// <summary>
        /// Gravitational acceleration constant
        /// </summary>
        public const float GRAVITY = 32.174f;

        /// <summary>
        /// Conversion factor from meters per second squared to feet per second squared.
        /// </summary>
        public const float PER_METER_PER_SECOND_SQUARED = Units.Feet.PER_METER;

        /// <summary>
        /// Convert from feet per second squared to meters per second squared.
        /// </summary>
        /// <param name="fpsps">the number feet per second squared</param>
        /// <returns>the number of meters per second squared</returns>
        public static float MetersPerSecondSquared(float fpsps)
        {
            return fpsps * Units.MetersPerSecondSquared.PER_FOOT_PER_SECOND_SQUARED;
        }
    }
}
