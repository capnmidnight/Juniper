namespace Juniper.Units
{
    /// <summary>
    /// Conversions from kilometers
    /// </summary>
    public static class Kilometers
    {
        /// <summary>
        /// Conversion factor from millimeters to kilometers.
        /// </summary>
        public const float PER_MILLIMETER = 1 / Units.Millimeters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from centimeters to kilometers.
        /// </summary>
        public const float PER_CENTIMETER = 1 / Units.Centimeters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from inches to kilometers.
        /// </summary>
        public const float PER_INCH = 1 / Units.Inches.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from feet to kilometers.
        /// </summary>
        public const float PER_FOOT = 1 / Units.Feet.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from meters to kilometers.
        /// </summary>
        public const float PER_METER = 1 / Units.Meters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from miles to kilometers.
        /// </summary>
        public const float PER_MILE = PER_FOOT * Units.Feet.PER_MILE;

        /// <summary>
        /// Convert from kilometers to millimeters.
        /// </summary>
        /// <param name="kilometers">The number of kilometers</param>
        /// <returns>The number of millimeters</returns>
        public static float Millimeters(float kilometers)
        {
            return kilometers * Units.Millimeters.PER_KILOMETER;
        }

        /// <summary>
        /// Convert from kilometers to centimeters.
        /// </summary>
        /// <param name="kilometers">The number of kilometers</param>
        /// <returns>The number of centimeters</returns>
        public static float Centimeters(float kilometers)
        {
            return kilometers * Units.Centimeters.PER_KILOMETER;
        }

        /// <summary>
        /// Convert from kilometers to inches.
        /// </summary>
        /// <param name="kilometers">The number of kilometers</param>
        /// <returns>The number of inches</returns>
        public static float Inches(float kilometers)
        {
            return kilometers * Units.Inches.PER_KILOMETER;
        }

        /// <summary>
        /// Convert from kilometers to feet.
        /// </summary>
        /// <param name="kilometers">The number of kilometers</param>
        /// <returns>The number of kilometers</returns>
        public static float Feet(float kilometers)
        {
            return kilometers * Units.Feet.PER_KILOMETER;
        }

        /// <summary>
        /// Convert from kilometers to meters.
        /// </summary>
        /// <param name="kilometers">The number of kilometers</param>
        /// <returns>The number of meters</returns>
        public static float Meters(float kilometers)
        {
            return kilometers * Units.Meters.PER_KILOMETER;
        }

        /// <summary>
        /// Convert from kilometers to miles.
        /// </summary>
        /// <param name="kilometers">The number of kilometers</param>
        /// <returns>The number of miles</returns>
        public static float Miles(float kilometers)
        {
            return kilometers * Units.Miles.PER_KILOMETER;
        }
    }
}