namespace Juniper.Units
{
    /// <summary>
    /// Conversions from feet
    /// </summary>
    public static class Feet
    {
        /// <summary>
        /// Conversion factor from micrometers to feet.
        /// </summary>
        public const double PER_MICROMETER = 1 / Units.Micrometers.PER_FOOT;

        /// <summary>
        /// Conversion factor from millimeters to feet.
        /// </summary>
        public const double PER_MILLIMETER = 1 / Units.Millimeters.PER_FOOT;

        /// <summary>
        /// Conversion factor from centimeters to feet.
        /// </summary>
        public const double PER_CENTIMETER = 1 / Units.Centimeters.PER_FOOT;

        /// <summary>
        /// Conversion factor from inches to feet.
        /// </summary>
        public const double PER_INCH = 1 / Units.Inches.PER_FOOT;

        /// <summary>
        /// Conversion factor from yards to feet.
        /// </summary>
        public const double PER_YARD = 3;

        /// <summary>
        /// Conversion factor from meters to feet.
        /// </summary>
        public const double PER_METER = PER_INCH * Units.Inches.PER_METER;

        /// <summary>
        /// Conversion factor from rods to feet.
        /// </summary>
        public const double PER_ROD = 16.5;

        /// <summary>
        /// Conversion factor from furlongs to feet.
        /// </summary>
        public const double PER_FURLONG = PER_ROD * Units.Rods.PER_FURLONG;

        /// <summary>
        /// Conversion factor from kilometers to feet.
        /// </summary>
        public const double PER_KILOMETER = PER_METER * Units.Meters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from miles to feet.
        /// </summary>
        public const double PER_MILE = PER_FURLONG * Units.Furlongs.PER_MILE;

        /// <summary>
        /// Convert from feet to micrometers.
        /// </summary>
        /// <param name="feet">The number of feet</param>
        /// <returns>The number of micrometers</returns>
        public static double Micrometers(double feet)
        {
            return feet * Units.Micrometers.PER_FOOT;
        }

        /// <summary>
        /// Convert from feet to millimeters.
        /// </summary>
        /// <param name="feet">The number of feet</param>
        /// <returns>The number of millimeters</returns>
        public static double Millimeters(double feet)
        {
            return feet * Units.Millimeters.PER_FOOT;
        }

        /// <summary>
        /// Convert from feet to centimeters.
        /// </summary>
        /// <param name="feet">The number of feet</param>
        /// <returns>The number of centimeters</returns>
        public static double Centimeters(double feet)
        {
            return feet * Units.Centimeters.PER_FOOT;
        }

        /// <summary>
        /// Convert from feet to inches.
        /// </summary>
        /// <param name="feet">The number of feet</param>
        /// <returns>The number of inches</returns>
        public static double Inches(double feet)
        {
            return feet * Units.Inches.PER_FOOT;
        }

        /// <summary>
        /// Convert from feet to yards.
        /// </summary>
        /// <param name="feet">The number of feet</param>
        /// <returns>The number of yards</returns>
        public static double Yards(double feet)
        {
            return feet * Units.Yards.PER_FOOT;
        }

        /// <summary>
        /// Convert from feet to meters.
        /// </summary>
        /// <param name="feet">The number of feet</param>
        /// <returns>The number of meters</returns>
        public static double Meters(double feet)
        {
            return feet * Units.Meters.PER_FOOT;
        }

        /// <summary>
        /// Convert from feet to rods.
        /// </summary>
        /// <param name="feet">The number of feet</param>
        /// <returns>The number of rods</returns>
        public static double Rods(double feet)
        {
            return feet * Units.Rods.PER_FOOT;
        }

        /// <summary>
        /// Convert from feet to furlongs.
        /// </summary>
        /// <param name="feet">The number of feet</param>
        /// <returns>The number of furlongs</returns>
        public static double Furlongs(double feet)
        {
            return feet * Units.Furlongs.PER_FOOT;
        }

        /// <summary>
        /// Convert from feet to kilometers.
        /// </summary>
        /// <param name="feet">The number of feet</param>
        /// <returns>The number of kilometers</returns>
        public static double Kilometers(double feet)
        {
            return feet * Units.Kilometers.PER_FOOT;
        }

        /// <summary>
        /// Convert from feet to miles.
        /// </summary>
        /// <param name="feet">The number of feet</param>
        /// <returns>The number of miles</returns>
        public static double Miles(double feet)
        {
            return feet * Units.Miles.PER_FOOT;
        }
    }
}