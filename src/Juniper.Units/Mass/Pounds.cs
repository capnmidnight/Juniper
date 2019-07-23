namespace Juniper.Units
{
    /// <summary>
    /// Conversions from pounds
    /// </summary>
    public static class Pounds
    {
        /// <summary>
        /// Conversion factor from grams to pounds.
        /// </summary>
        public const float PER_GRAM = 1 / Units.Grams.PER_POUND;

        /// <summary>
        /// Conversion factor from ounces to pounds.
        /// </summary>
        public const float PER_OUNCE = 1 / Units.Ounces.PER_POUND;

        /// <summary>
        /// Conversion factor from kilograms to pounds.
        /// </summary>
        public const float PER_KILOGRAM = 2.2046228f;

        /// <summary>
        /// Conversion factor from tons to pounds.
        /// </summary>
        public const float PER_TON = 2000f;

        /// <summary>
        /// Convert from pounds to grams.
        /// </summary>
        /// <param name="pounds">The number of pounds</param>
        /// <returns>The number of grams</returns>
        public static float Grams(float pounds)
        {
            return pounds * Units.Grams.PER_POUND;
        }

        /// <summary>
        /// Convert from pounds to ounces.
        /// </summary>
        /// <param name="pounds">The number of pounds</param>
        /// <returns>The number of ounces</returns>
        public static float Ounces(float pounds)
        {
            return pounds * Units.Ounces.PER_POUND;
        }

        /// <summary>
        /// Convert from pounds to kilograms.
        /// </summary>
        /// <param name="pounds">The number of pounds</param>
        /// <returns>The number of kilograms</returns>
        public static float Kilograms(float pounds)
        {
            return pounds * Units.Kilograms.PER_POUND;
        }

        /// <summary>
        /// Convert from pounds to tons.
        /// </summary>
        /// <param name="pounds">The number of pounds</param>
        /// <returns>The number of tons</returns>
        public static float Tons(float pounds)
        {
            return pounds * Units.Tons.PER_POUND;
        }
    }
}