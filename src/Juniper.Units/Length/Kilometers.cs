namespace Juniper.Units
{
    /// <summary>
    /// Conversions from kilometers
    /// </summary>
    public static class Kilometers
    {
        /// <summary>
        /// Conversion factor from micrometers to kilometers.
        /// </summary>
        public const double PER_MICROMETER = 1 / Units.Micrometers.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from millimeters to kilometers.
        /// </summary>
        public const double PER_MILLIMETER = 1 / Units.Millimeters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from centimeters to kilometers.
        /// </summary>
        public const double PER_CENTIMETER = 1 / Units.Centimeters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from inches to kilometers.
        /// </summary>
        public const double PER_INCH = 1 / Units.Inches.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from feet to kilometers.
        /// </summary>
        public const double PER_FOOT = 1 / Units.Feet.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from yards to kilometers.
        /// </summary>
        public const double PER_YARD = 1 / Units.Yards.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from meters to kilometers.
        /// </summary>
        public const double PER_METER = 1 / Units.Meters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from rods to kilometers.
        /// </summary>
        public const double PER_ROD = 1 / Units.Rods.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from furlongs to kilometers.
        /// </summary>
        public const double PER_FURLONG = 1 / Units.Furlongs.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from miles to kilometers.
        /// </summary>
        public const double PER_MILE = PER_FURLONG * Units.Furlongs.PER_MILE;

        /// <summary>
        /// Convert from kilometers to micrometers.
        /// </summary>
        /// <param name="kilometers">The number of kilometers</param>
        /// <returns>The number of micrometers</returns>
        public static double Micrometers(double kilometers)
        {
            return kilometers * Units.Micrometers.PER_MILLIMETER;
        }

        /// <summary>
        /// Convert from kilometers to millimeters.
        /// </summary>
        /// <param name="kilometers">The number of kilometers</param>
        /// <returns>The number of millimeters</returns>
        public static double Millimeters(double kilometers)
        {
            return kilometers * Units.Millimeters.PER_KILOMETER;
        }

        /// <summary>
        /// Convert from kilometers to centimeters.
        /// </summary>
        /// <param name="kilometers">The number of kilometers</param>
        /// <returns>The number of centimeters</returns>
        public static double Centimeters(double kilometers)
        {
            return kilometers * Units.Centimeters.PER_KILOMETER;
        }

        /// <summary>
        /// Convert from kilometers to inches.
        /// </summary>
        /// <param name="kilometers">The number of kilometers</param>
        /// <returns>The number of inches</returns>
        public static double Inches(double kilometers)
        {
            return kilometers * Units.Inches.PER_KILOMETER;
        }

        /// <summary>
        /// Convert from kilometers to feet.
        /// </summary>
        /// <param name="kilometers">The number of kilometers</param>
        /// <returns>The number of feet</returns>
        public static double Feet(double kilometers)
        {
            return kilometers * Units.Feet.PER_KILOMETER;
        }

        /// <summary>
        /// Convert from kilometers to yards.
        /// </summary>
        /// <param name="kilometers">The number of kilometers</param>
        /// <returns>The number of yards</returns>
        public static double Yards(double kilometers)
        {
            return kilometers * Units.Yards.PER_KILOMETER;
        }

        /// <summary>
        /// Convert from kilometers to meters.
        /// </summary>
        /// <param name="kilometers">The number of kilometers</param>
        /// <returns>The number of meters</returns>
        public static double Meters(double kilometers)
        {
            return kilometers * Units.Meters.PER_KILOMETER;
        }

        /// <summary>
        /// Convert from kilometers to rods.
        /// </summary>
        /// <param name="kilometers">The number of kilometers</param>
        /// <returns>The number of rods</returns>
        public static double Rods(double kilometers)
        {
            return kilometers * Units.Rods.PER_KILOMETER;
        }
        /// <summary>
        /// Convert from kilometers to furlongs.
        /// </summary>
        /// <param name="kilometers">The number of kilometers</param>
        /// <returns>The number of furlongs</returns>
        public static double Furlongs(double kilometers)
        {
            return kilometers * Units.Furlongs.PER_KILOMETER;
        }

        /// <summary>
        /// Convert from kilometers to miles.
        /// </summary>
        /// <param name="kilometers">The number of kilometers</param>
        /// <returns>The number of miles</returns>
        public static double Miles(double kilometers)
        {
            return kilometers * Units.Miles.PER_KILOMETER;
        }
    }
}