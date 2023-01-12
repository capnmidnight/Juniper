namespace Juniper.Units
{
    /// <summary>
    /// Conversions from furlongs
    /// </summary>
    public static class Furlongs
    {
        /// <summary>
        /// Conversion factor from micrometers to furlongs.
        /// </summary>
        public const double PER_MICROMETER = 1 / Units.Micrometers.PER_FURLONG;

        /// <summary>
        /// Conversion factor from millimeters to furlongs.
        /// </summary>
        public const double PER_MILLIMETER = 1 / Units.Millimeters.PER_FURLONG;

        /// <summary>
        /// Conversion factor from centimeters to furlongs.
        /// </summary>
        public const double PER_CENTIMETER = 1 / Units.Centimeters.PER_FURLONG;

        /// <summary>
        /// Conversion factor from inches to furlongs.
        /// </summary>
        public const double PER_INCH = 1 / Units.Inches.PER_FURLONG;

        /// <summary>
        /// Conversion factor from feet to furlongs.
        /// </summary>
        public const double PER_FOOT = 1 / Units.Feet.PER_FURLONG;

        /// <summary>
        /// Conversion factor from yards to furlongs.
        /// </summary>
        public const double PER_YARD = 1 / Units.Yards.PER_FURLONG;

        /// <summary>
        /// Conversion factor from meters to furlongs.
        /// </summary>
        public const double PER_METER = 1 / Units.Meters.PER_FURLONG;

        /// <summary>
        /// Conversion factor from rods to furlongs.
        /// </summary>
        public const double PER_ROD = 1 / Units.Rods.PER_FURLONG;

        /// <summary>
        /// Conversion factor from kilometers to furlongs.
        /// </summary>
        public const double PER_KILOMETER = PER_METER * Units.Meters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from miles to furlongs.
        /// </summary>
        public const double PER_MILE = 8;

        /// <summary>
        /// Convert from furlongs to micrometers.
        /// </summary>
        /// <param name="furlongs">The number of furlongs</param>
        /// <returns>The number of micrometers</returns>
        public static double Micrometers(double furlongs)
        {
            return furlongs * Units.Micrometers.PER_FURLONG;
        }

        /// <summary>
        /// Convert from furlongs to millimeters.
        /// </summary>
        /// <param name="furlongs">The number of furlongs</param>
        /// <returns>The number of millimeters</returns>
        public static double Millimeters(double furlongs)
        {
            return furlongs * Units.Millimeters.PER_FURLONG;
        }

        /// <summary>
        /// Convert from furlongs to centimeters.
        /// </summary>
        /// <param name="furlongs">The number of furlongs</param>
        /// <returns>The number of centimeters</returns>
        public static double Centimeters(double furlongs)
        {
            return furlongs * Units.Centimeters.PER_FURLONG;
        }

        /// <summary>
        /// Convert from furlongs to inches.
        /// </summary>
        /// <param name="furlongs">The number of furlongs</param>
        /// <returns>The number of inches</returns>
        public static double Inches(double furlongs)
        {
            return furlongs * Units.Inches.PER_FURLONG;
        }

        /// <summary>
        /// Convert from furlongs to feet.
        /// </summary>
        /// <param name="furlongs">The number of furlongs</param>
        /// <returns>The number of feet</returns>
        public static double Feet(double furlongs)
        {
            return furlongs * Units.Feet.PER_FURLONG;
        }

        /// <summary>
        /// Convert from furlongs to yards.
        /// </summary>
        /// <param name="furlongs">The number of furlongs</param>
        /// <returns>The number of yards</returns>
        public static double Yards(double furlongs)
        {
            return furlongs * Units.Yards.PER_FURLONG;
        }

        /// <summary>
        /// Convert from furlongs to meters.
        /// </summary>
        /// <param name="furlongs">The number of furlongs</param>
        /// <returns>The number of meters</returns>
        public static double Meters(double furlongs)
        {
            return furlongs * Units.Meters.PER_FURLONG;
        }

        /// <summary>
        /// Convert from furlongs to rods.
        /// </summary>
        /// <param name="furlongs">The number of furlongs</param>
        /// <returns>The number of rods</returns>
        public static double Rods(double furlongs)
        {
            return furlongs * Units.Rods.PER_FURLONG;
        }

        /// <summary>
        /// Convert from furlongs to kilometers.
        /// </summary>
        /// <param name="furlongs">The number of furlongs</param>
        /// <returns>The number of kilometers</returns>
        public static double Kilometers(double furlongs)
        {
            return furlongs * Units.Kilometers.PER_FURLONG;
        }

        /// <summary>
        /// Convert from furlongs to miles.
        /// </summary>
        /// <param name="furlongs">The number of furlongs</param>
        /// <returns>The number of miles</returns>
        public static double Miles(double furlongs)
        {
            return furlongs * Units.Miles.PER_FURLONG;
        }
    }
}