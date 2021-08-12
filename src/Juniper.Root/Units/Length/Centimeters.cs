namespace Juniper.Units
{
    /// <summary>
    /// Conversions from centimeters
    /// </summary>
    public static class Centimeters
    {
        /// <summary>
        /// Conversion factor from micrometers to millimeters.
        /// </summary>
        public const float PER_MICROMETER = 1 / Units.Micrometers.PER_CENTIMETER;

        /// <summary>
        /// Conversion factor from millimeters to centimeters.
        /// </summary>
        public const float PER_MILLIMETER = 1 / Units.Millimeters.PER_CENTIMETER;

        /// <summary>
        /// Conversion factor from inches to centimeters.
        /// </summary>
        public const float PER_INCH = 2.54f;

        /// <summary>
        /// Conversion factor from feet to centimeters.
        /// </summary>
        public const float PER_FOOT = PER_INCH * Units.Inches.PER_FOOT;

        /// <summary>
        /// Conversion factor from yards to centimeters.
        /// </summary>
        public const float PER_YARD = PER_FOOT * Units.Feet.PER_YARD;

        /// <summary>
        /// Conversion factor from meters to centimeters.
        /// </summary>
        public const float PER_METER = 100f;

        /// <summary>
        /// Conversion factor from rods to centimeters.
        /// </summary>
        public const float PER_ROD = PER_FOOT * Units.Feet.PER_ROD;

        /// <summary>
        /// Conversion factor from furlongs to centimeters.
        /// </summary>
        public const float PER_FURLONG = PER_ROD * Units.Rods.PER_FURLONG;

        /// <summary>
        /// Conversion factor from kilometers to centimeters.
        /// </summary>
        public const float PER_KILOMETER = PER_METER * Units.Meters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from miles to centimeters.
        /// </summary>
        public const float PER_MILE = PER_FURLONG * Units.Furlongs.PER_MILE;

        /// <summary>
        /// Convert from centimeters to micrometers.
        /// </summary>
        /// <param name="centimeters">The number of centimeters</param>
        /// <returns>The number of micrometers</returns>
        public static float Micrometers(float centimeters)
        {
            return centimeters * Units.Micrometers.PER_CENTIMETER;
        }

        /// <summary>
        /// Convert from centimeters to millimeters.
        /// </summary>
        /// <param name="centimeters">The number of centimeters</param>
        /// <returns>The number of millimeters</returns>
        public static float Millimeters(float centimeters)
        {
            return centimeters * Units.Millimeters.PER_CENTIMETER;
        }

        /// <summary>
        /// Convert from centimeters to inches.
        /// </summary>
        /// <param name="centimeters">The number of centimeters</param>
        /// <returns>The number of inches</returns>
        public static float Inches(float centimeters)
        {
            return centimeters * Units.Inches.PER_CENTIMETER;
        }

        /// <summary>
        /// Convert from centimeters to feet.
        /// </summary>
        /// <param name="centimeters">The number of centimeters</param>
        /// <returns>The number of feet</returns>
        public static float Feet(float centimeters)
        {
            return centimeters * Units.Feet.PER_CENTIMETER;
        }

        /// <summary>
        /// Convert from centimeters to yards.
        /// </summary>
        /// <param name="centimeters">The number of centimeters</param>
        /// <returns>The number of yards</returns>
        public static float Yards(float centimeters)
        {
            return centimeters * Units.Yards.PER_CENTIMETER;
        }

        /// <summary>
        /// Convert from centimeters to meters.
        /// </summary>
        /// <param name="centimeters">The number of centimeters</param>
        /// <returns>The number of meters</returns>
        public static float Meters(float centimeters)
        {
            return centimeters * Units.Meters.PER_CENTIMETER;
        }

        /// <summary>
        /// Convert from centimeters to rods.
        /// </summary>
        /// <param name="centimeters">The number of centimeters</param>
        /// <returns>The number of rods</returns>
        public static float Rods(float centimeters)
        {
            return centimeters * Units.Rods.PER_CENTIMETER;
        }

        /// <summary>
        /// Convert from centimeters to furlongs.
        /// </summary>
        /// <param name="centimeters">The number of centimeters</param>
        /// <returns>The number of furlongs</returns>
        public static float Furlongs(float centimeters)
        {
            return centimeters * Units.Furlongs.PER_CENTIMETER;
        }

        /// <summary>
        /// Convert from centimeters to kilometers.
        /// </summary>
        /// <param name="centimeters">The number of centimeters</param>
        /// <returns>The number of kilometers</returns>
        public static float Kilometers(float centimeters)
        {
            return centimeters * Units.Kilometers.PER_CENTIMETER;
        }

        /// <summary>
        /// Convert from centimeters to miles.
        /// </summary>
        /// <param name="centimeters">The number of centimeters</param>
        /// <returns>The number of miles</returns>
        public static float Miles(float centimeters)
        {
            return centimeters * Units.Miles.PER_CENTIMETER;
        }
    }
}