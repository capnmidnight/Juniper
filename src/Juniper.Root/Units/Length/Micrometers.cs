namespace Juniper.Units
{
    /// <summary>
    /// Conversions from micrometers
    /// </summary>
    public static class Micrometers
    {
        /// <summary>
        /// Conversion factor from millimeters to micrometers.
        /// </summary>
        public const float PER_MILLIMETER = 1000;

        /// <summary>
        /// Conversion factor from centimeters to micrometers.
        /// </summary>
        public const float PER_CENTIMETER = PER_MILLIMETER * Units.Millimeters.PER_CENTIMETER;

        /// <summary>
        /// Conversion factor from inches to micrometers.
        /// </summary>
        public const float PER_INCH = PER_CENTIMETER * Units.Centimeters.PER_INCH;

        /// <summary>
        /// Conversion factor from feet to micrometers.
        /// </summary>
        public const float PER_FOOT = PER_INCH * Units.Inches.PER_FOOT;

        /// <summary>
        /// Conversion factor from yards to micrometers.
        /// </summary>
        public const float PER_YARD = PER_FOOT * Units.Feet.PER_YARD;

        /// <summary>
        /// Conversion factor from meters to micrometers.
        /// </summary>
        public const float PER_METER = PER_CENTIMETER * Units.Centimeters.PER_METER;

        /// <summary>
        /// Conversion factor from kilometers to micrometers.
        /// </summary>
        public const float PER_KILOMETER = PER_METER * Units.Meters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from miles to micrometers.
        /// </summary>
        public const float PER_MILE = PER_FOOT * Units.Feet.PER_MILE;

        /// <summary>
        /// Convert from micrometers to millimeters.
        /// </summary>
        /// <param name="micrometers">The number of micrometers</param>
        /// <returns>The number of millimeters</returns>
        public static float Millimeters(float micrometers)
        {
            return micrometers * Units.Millimeters.PER_MICROMETER;
        }

        /// <summary>
        /// Convert from micrometers to centimeters.
        /// </summary>
        /// <param name="micrometers">The number of micrometers</param>
        /// <returns>The number of centimeters</returns>
        public static float Centimeters(float micrometers)
        {
            return micrometers * Units.Centimeters.PER_MICROMETER;
        }

        /// <summary>
        /// Convert from micrometers to inches.
        /// </summary>
        /// <param name="micrometers">The number of micrometers</param>
        /// <returns>The number of inches</returns>
        public static float Inches(float micrometers)
        {
            return micrometers * Units.Inches.PER_MICROMETER;
        }

        /// <summary>
        /// Convert from micrometers to feet.
        /// </summary>
        /// <param name="micrometers">The number of micrometers</param>
        /// <returns>The number of feet</returns>
        public static float Feet(float micrometers)
        {
            return micrometers * Units.Feet.PER_MICROMETER;
        }

        /// <summary>
        /// Convert from micrometers to yards.
        /// </summary>
        /// <param name="micrometers">The number of micrometers</param>
        /// <returns>The number of yards</returns>
        public static float Yards(float micrometers)
        {
            return micrometers * Units.Yards.PER_MICROMETER;
        }

        /// <summary>
        /// Convert from micrometers to meters.
        /// </summary>
        /// <param name="micrometers">The number of micrometers</param>
        /// <returns>The number of meters</returns>
        public static float Meters(float micrometers)
        {
            return micrometers * Units.Meters.PER_MICROMETER;
        }

        /// <summary>
        /// Convert from micrometers to kilometers.
        /// </summary>
        /// <param name="micrometers">The number of micrometers</param>
        /// <returns>The number of kilometers</returns>
        public static float Kilometers(float micrometers)
        {
            return micrometers * Units.Kilometers.PER_MICROMETER;
        }

        /// <summary>
        /// Convert from micrometers to miles.
        /// </summary>
        /// <param name="micrometers">The number of micrometers</param>
        /// <returns>The number of miles</returns>
        public static float Miles(float micrometers)
        {
            return micrometers * Units.Miles.PER_MICROMETER;
        }
    }
}