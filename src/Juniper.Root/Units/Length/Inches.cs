namespace Juniper.Units
{
    /// <summary>
    /// Conversions from inches
    /// </summary>
    public static class Inches
    {
        /// <summary>
        /// Conversion factor from micrometers to inches.
        /// </summary>
        public const float PER_MICROMETER = 1 / Units.Micrometers.PER_INCH;

        /// <summary>
        /// Conversion factor from millimeters to inches.
        /// </summary>
        public const float PER_MILLIMETER = 1 / Units.Millimeters.PER_INCH;

        /// <summary>
        /// Conversion factor from centimeters to inches.
        /// </summary>
        public const float PER_CENTIMETER = 1 / Units.Centimeters.PER_INCH;

        /// <summary>
        /// Conversion factor from feet to inches.
        /// </summary>
        public const float PER_FOOT = 12;

        /// <summary>
        /// Conversion factor from yards to inches.
        /// </summary>
        public const float PER_YARD = PER_FOOT * Units.Feet.PER_YARD;

        /// <summary>
        /// Conversion factor from meters to inches.
        /// </summary>
        public const float PER_METER = PER_CENTIMETER * Units.Centimeters.PER_METER;

        /// <summary>
        /// Conversion factor from rods to inches.
        /// </summary>
        public const float PER_ROD = PER_FOOT * Units.Feet.PER_ROD;

        /// <summary>
        /// Conversion factor from furlongs to inches.
        /// </summary>
        public const float PER_FURLONG = PER_ROD * Units.Rods.PER_FURLONG;

        /// <summary>
        /// Conversion factor from kilometers to inches.
        /// </summary>
        public const float PER_KILOMETER = PER_METER * Units.Meters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from miles to inches.
        /// </summary>
        public const float PER_MILE = PER_FURLONG * Units.Furlongs.PER_MILE;

        /// <summary>
        /// Convert from inches to micrometers.
        /// </summary>
        /// <param name="inches">The number of inches</param>
        /// <returns>The number of micrometers</returns>
        public static float Micrometers(float inches)
        {
            return inches * Units.Micrometers.PER_INCH;
        }

        /// <summary>
        /// Convert from inches to millimeters.
        /// </summary>
        /// <param name="inches">The number of inches</param>
        /// <returns>The number of millimeters</returns>
        public static float Millimeters(float inches)
        {
            return inches * Units.Millimeters.PER_INCH;
        }

        /// <summary>
        /// Convert from inches to centimeters.
        /// </summary>
        /// <param name="inches">The number of inches</param>
        /// <returns>The number of centimeters</returns>
        public static float Centimeters(float inches)
        {
            return inches * Units.Centimeters.PER_INCH;
        }

        /// <summary>
        /// Convert from inches to feet.
        /// </summary>
        /// <param name="inches">The number of inches</param>
        /// <returns>The number of feet</returns>
        public static float Feet(float inches)
        {
            return inches * Units.Feet.PER_INCH;
        }

        /// <summary>
        /// Convert from inches to yards.
        /// </summary>
        /// <param name="inches">The number of inches</param>
        /// <returns>The number of yards</returns>
        public static float Yards(float inches)
        {
            return inches * Units.Yards.PER_INCH;
        }

        /// <summary>
        /// Convert from inches to meters.
        /// </summary>
        /// <param name="inches">The number of inches</param>
        /// <returns>The number of meters</returns>
        public static float Meters(float inches)
        {
            return inches * Units.Meters.PER_INCH;
        }

        /// <summary>
        /// Convert from inches to rods.
        /// </summary>
        /// <param name="inches">The number of inches</param>
        /// <returns>The number of rods</returns>
        public static float Rods(float inches)
        {
            return inches * Units.Rods.PER_INCH;
        }

        /// <summary>
        /// Convert from inches to furlongs.
        /// </summary>
        /// <param name="inches">The number of inches</param>
        /// <returns>The number of furlongs</returns>
        public static float Furlongs(float inches)
        {
            return inches * Units.Furlongs.PER_INCH;
        }

        /// <summary>
        /// Convert from inches to kilometers.
        /// </summary>
        /// <param name="inches">The number of inches</param>
        /// <returns>The number of kilometers</returns>
        public static float Kilometers(float inches)
        {
            return inches * Units.Kilometers.PER_INCH;
        }

        /// <summary>
        /// Convert from inches to miles.
        /// </summary>
        /// <param name="inches">The number of inches</param>
        /// <returns>The number of miles</returns>
        public static float Miles(float inches)
        {
            return inches * Units.Miles.PER_INCH;
        }
    }
}