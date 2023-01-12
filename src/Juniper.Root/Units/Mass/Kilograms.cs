namespace Juniper.Units
{
    /// <summary>
    /// Conversions from kilograms
    /// </summary>
    public static class Kilograms
    {
        /// <summary>
        /// Conversion factor from grams to kilograms.
        /// </summary>
        public const double PER_GRAM = 1 / Units.Grams.PER_KILOGRAM;

        /// <summary>
        /// Conversion factor from ounces to kilograms.
        /// </summary>
        public const double PER_OUNCE = 1 / Units.Ounces.PER_KILOGRAM;

        /// <summary>
        /// Conversion factor from pounds to kilograms.
        /// </summary>
        public const double PER_POUND = 1 / Units.Pounds.PER_KILOGRAM;

        /// <summary>
        /// Conversion factor from tons to kilograms.
        /// </summary>
        public const double PER_TON = 907.18475;

        /// <summary>
        /// Convert from kilograms to grams.
        /// </summary>
        /// <param name="kilograms">The number of kilograms</param>
        /// <returns>The number of grams</returns>
        public static double Grams(double kilograms)
        {
            return kilograms * Units.Grams.PER_KILOGRAM;
        }

        /// <summary>
        /// Convert from kilograms to ounces.
        /// </summary>
        /// <param name="kilograms">The number of kilograms</param>
        /// <returns>The number of ounces</returns>
        public static double Ounces(double kilograms)
        {
            return kilograms * Units.Ounces.PER_KILOGRAM;
        }

        /// <summary>
        /// Convert from kilograms to pounds.
        /// </summary>
        /// <param name="kilograms">The number of kilograms</param>
        /// <returns>The number of pounds</returns>
        public static double Pounds(double kilograms)
        {
            return kilograms * Units.Pounds.PER_KILOGRAM;
        }

        /// <summary>
        /// Convert from kilograms to tons.
        /// </summary>
        /// <param name="kilograms">The number of kilograms</param>
        /// <returns>The number of tons</returns>
        public static double Tons(double kilograms)
        {
            return kilograms * Units.Tons.PER_KILOGRAM;
        }
    }
}