namespace Juniper.Units
{
    /// <summary>
    /// Conversions from gradians
    /// </summary>
    public static class Gradians
    {
        /// <summary>
        /// Conversion factor from circles to gradians.
        /// </summary>
        public const double PER_CIRCLE = 400;

        /// <summary>
        /// Conversion factor from degrees to gradians.
        /// </summary>
        public const double PER_DEGREE = PER_CIRCLE / Units.Degrees.PER_CIRCLE;

        /// <summary>
        /// Conversion factor from radians to gradians.
        /// </summary>
        public const double PER_RADIAN = PER_CIRCLE / Units.Radians.PER_CIRCLE;

        /// <summary>
        /// Conversion factor from hours to gradians.
        /// </summary>
        public const double PER_HOUR = PER_CIRCLE / Units.Hours.PER_CIRCLE;

        /// <summary>
        /// Convert from gradians to degrees.
        /// </summary>
        /// <param name="gradians">The number of gradians</param>
        /// <returns>The number of degrees</returns>
        public static double Degrees(double gradians)
        {
            return gradians * Units.Degrees.PER_GRADIAN;
        }

        /// <summary>
        /// Convert from gradians to radians.
        /// </summary>
        /// <param name="gradians">The number of gradians</param>
        /// <returns>The number of radians</returns>
        public static double Radians(double gradians)
        {
            return gradians * Units.Radians.PER_GRADIAN;
        }

        /// <summary>
        /// Convert from gradians to hours.
        /// </summary>
        /// <param name="gradians">The number of gradians</param>
        /// <returns>The number of hours</returns>
        public static double Hours(double gradians)
        {
            return gradians * Units.Hours.PER_GRADIAN;
        }
    }
}