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
        public const double PER_OUNCE = 28.349523;

        /// <summary>
        /// Conversion factor from pounds to grams.
        /// </summary>
        public const double PER_POUND = 453.59233;

        /// <summary>
        /// Conversion factor from kilograms to grams.
        /// </summary>
        public const double PER_KILOGRAM = 1000;

        /// <summary>
        /// Conversion factor from tons to grams.
        /// </summary>
        public const double PER_TON = PER_KILOGRAM * Units.Kilograms.PER_TON;

        /// <summary>
        /// Convert from grams to ounces.
        /// </summary>
        /// <param name="grams">The number of grams</param>
        /// <returns>The number of ounces</returns>
        public static double Ounces(double grams)
        {
            return grams * Units.Ounces.PER_GRAM;
        }

        /// <summary>
        /// Convert from grams to pounds.
        /// </summary>
        /// <param name="grams">The number of grams</param>
        /// <returns>The number of pounds</returns>
        public static double Pounds(double grams)
        {
            return grams * Units.Pounds.PER_GRAM;
        }

        /// <summary>
        /// Convert from grams to kilograms.
        /// </summary>
        /// <param name="grams">The number of grams</param>
        /// <returns>The number of kilograms</returns>
        public static double Kilograms(double grams)
        {
            return grams * Units.Kilograms.PER_GRAM;
        }

        /// <summary>
        /// Convert from grams to tons.
        /// </summary>
        /// <param name="grams">The number of grams</param>
        /// <returns>The number of tons</returns>
        public static double Tons(double grams)
        {
            return grams * Units.Tons.PER_GRAM;
        }
    }
}