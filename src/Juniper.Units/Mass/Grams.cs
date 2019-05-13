namespace Juniper.Units
{
    /// <summary>
    /// Conversions from grams
    /// </summary>
    public static class Grams
    {
        /// <summary>
        /// Conversion factor from ounces to grams.
        /// </summary>
        public const float PER_OUNCE = 28.349523f;

        /// <summary>
        /// Conversion factor from pounds to grams.
        /// </summary>
        public const float PER_POUND = 453.59233f;

        /// <summary>
        /// Conversion factor from kilograms to grams.
        /// </summary>
        public const float PER_KILOGRAM = 1000;

        /// <summary>
        /// Conversion factor from tons to grams.
        /// </summary>
        public const float PER_TON = PER_KILOGRAM * Units.Kilograms.PER_TON;

        /// <summary>
        /// Convert from grams to ounces.
        /// </summary>
        /// <param name="grams">The number of grams</param>
        /// <returns>The number of ounces</returns>
        public static float Ounces(float grams)
        {
            return grams * Units.Ounces.PER_GRAM;
        }

        /// <summary>
        /// Convert from grams to pounds.
        /// </summary>
        /// <param name="grams">The number of grams</param>
        /// <returns>The number of pounds</returns>
        public static float Pounds(float grams)
        {
            return grams * Units.Pounds.PER_GRAM;
        }

        /// <summary>
        /// Convert from grams to kilograms.
        /// </summary>
        /// <param name="grams">The number of grams</param>
        /// <returns>The number of kilograms</returns>
        public static float Kilograms(float grams)
        {
            return grams * Units.Kilograms.PER_GRAM;
        }

        /// <summary>
        /// Convert from grams to tons.
        /// </summary>
        /// <param name="grams">The number of grams</param>
        /// <returns>The number of tons</returns>
        public static float Tons(float grams)
        {
            return grams * Units.Tons.PER_GRAM;
        }
    }
}
