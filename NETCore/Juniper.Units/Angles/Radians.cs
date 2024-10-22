using static System.Math;

namespace Juniper.Units
{
    /// <summary>
    /// Conversions from radians
    /// </summary>
    public static class Radians
    {
        /// <summary>
        /// Conversion factor from circles to radians.
        /// </summary>
        public const double PER_CIRCLE = (double)(2 * PI);

        /// <summary>
        /// Conversion factor from degrees to radians.
        /// </summary>
        public const double PER_DEGREE = PER_CIRCLE / Units.Degrees.PER_CIRCLE;

        /// <summary>
        /// Conversion factor from gradians to radians.
        /// </summary>
        public const double PER_GRADIAN = PER_CIRCLE / Units.Gradians.PER_CIRCLE;

        /// <summary>
        /// Conversion factor from hours to radians.
        /// </summary>
        public const double PER_HOUR = PER_CIRCLE / Units.Hours.PER_CIRCLE;

        /// <summary>
        /// Convert from radians to degrees.
        /// </summary>
        /// <param name="radians">The number of radians</param>
        /// <returns>The number of degrees</returns>
        public static double Degrees(double radians)
        {
            return radians * Units.Degrees.PER_RADIAN;
        }

        /// <summary>
        /// Convert from radians to gradians.
        /// </summary>
        /// <param name="radians">The number of radians</param>
        /// <returns>The number of gradians</returns>
        public static double Gradians(double radians)
        {
            return radians * Units.Gradians.PER_RADIAN;
        }

        /// <summary>
        /// Convert from radians to hours.
        /// </summary>
        /// <param name="radians">The number of radians</param>
        /// <returns>The number of hours</returns>
        public static double Hours(double radians)
        {
            return radians * Units.Hours.PER_RADIAN;
        }
    }
}