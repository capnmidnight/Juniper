namespace Juniper.Units
{
    /// <summary>
    /// Conversions from millimeters
    /// </summary>
    public static class Millimeters
    {
        /// <summary>
        /// Conversion factor from centimeters to millimeters.
        /// </summary>
        public const float PER_CENTIMETER = 10;

        /// <summary>
        /// Conversion factor from inches to millimeters.
        /// </summary>
        public const float PER_INCH = PER_CENTIMETER * Units.Centimeters.PER_INCH;

        /// <summary>
        /// Conversion factor from feet to millimeters.
        /// </summary>
        public const float PER_FOOT = PER_INCH * Units.Inches.PER_FOOT;

        /// <summary>
        /// Conversion factor from meters to millimeters.
        /// </summary>
        public const float PER_METER = 1000f;

        /// <summary>
        /// Conversion factor from kilometers to millimeters.
        /// </summary>
        public const float PER_KILOMETER = PER_METER * Units.Meters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from miles to millimeters.
        /// </summary>
        public const float PER_MILE = PER_FOOT * Units.Feet.PER_MILE;

        /// <summary>
        /// Convert from millimeters to centimeters.
        /// </summary>
        /// <param name="millimeters">The number of millimeters</param>
        /// <returns>The number of centimeters</returns>
        public static float Centimeters(float millimeters)
        {
            return millimeters * Units.Centimeters.PER_MILLIMETER;
        }

        /// <summary>
        /// Convert from millimeters to inches.
        /// </summary>
        /// <param name="millimeters">The number of millimeters</param>
        /// <returns>The number of inches</returns>
        public static float Inches(float millimeters)
        {
            return millimeters * Units.Inches.PER_MILLIMETER;
        }

        /// <summary>
        /// Convert from millimeters to feet.
        /// </summary>
        /// <param name="millimeters">The number of millimeters</param>
        /// <returns>The number of feet</returns>
        public static float Feet(float millimeters)
        {
            return millimeters * Units.Feet.PER_MILLIMETER;
        }

        /// <summary>
        /// Convert from millimeters to meters.
        /// </summary>
        /// <param name="millimeters">The number of millimeters</param>
        /// <returns>The number of meters</returns>
        public static float Meters(float millimeters)
        {
            return millimeters * Units.Meters.PER_MILLIMETER;
        }

        /// <summary>
        /// Convert from millimeters to kilometers.
        /// </summary>
        /// <param name="millimeters">The number of millimeters</param>
        /// <returns>The number of kilometers</returns>
        public static float Kilometers(float millimeters)
        {
            return millimeters * Units.Kilometers.PER_MILLIMETER;
        }

        /// <summary>
        /// Convert from millimeters to miles.
        /// </summary>
        /// <param name="millimeters">The number of millimeters</param>
        /// <returns>The number of miles</returns>
        public static float Miles(float millimeters)
        {
            return millimeters * Units.Miles.PER_MILLIMETER;
        }
    }
}
