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
        public const double PER_GRAM = 1 / Units.Grams.PER_POUND;

        /// <summary>
        /// Conversion factor from ounces to pounds.
        /// </summary>
        public const double PER_OUNCE = 1 / Units.Ounces.PER_POUND;

        /// <summary>
        /// Conversion factor from kilograms to pounds.
        /// </summary>
        public const double PER_KILOGRAM = 2.20462262;

        /// <summary>
        /// Conversion factor from tons to pounds.
        /// </summary>
        public const double PER_TON = 2000;

        /// <summary>
        /// Convert from pounds to grams.
        /// </summary>
        /// <param name="pounds">The number of pounds</param>
        /// <returns>The number of grams</returns>
        public static double Grams(double pounds)
        {
            return pounds * Units.Grams.PER_POUND;
        }

        /// <summary>
        /// Convert from pounds to ounces.
        /// </summary>
        /// <param name="pounds">The number of pounds</param>
        /// <returns>The number of ounces</returns>
        public static double Ounces(double pounds)
        {
            return pounds * Units.Ounces.PER_POUND;
        }

        /// <summary>
        /// Convert from pounds to kilograms.
        /// </summary>
        /// <param name="pounds">The number of pounds</param>
        /// <returns>The number of kilograms</returns>
        public static double Kilograms(double pounds)
        {
            return pounds * Units.Kilograms.PER_POUND;
        }

        /// <summary>
        /// Convert from pounds to tons.
        /// </summary>
        /// <param name="pounds">The number of pounds</param>
        /// <returns>The number of tons</returns>
        public static double Tons(double pounds)
        {
            return pounds * Units.Tons.PER_POUND;
        }
    }
}