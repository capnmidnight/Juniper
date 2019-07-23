namespace Juniper.Units
{
    /// <summary>
    /// Conversions from feet
    /// </summary>
    public static class Feet
    {
        /// <summary>
        /// Conversion factor from millimeters to feet.
        /// </summary>
        public const float PER_MILLIMETER = 1 / Units.Millimeters.PER_FOOT;

        /// <summary>
        /// Conversion factor from centimeters to feet.
        /// </summary>
        public const float PER_CENTIMETER = 1 / Units.Centimeters.PER_FOOT;

        /// <summary>
        /// Conversion factor from inches to feet.
        /// </summary>
        public const float PER_INCH = 1 / Units.Inches.PER_FOOT;

        /// <summary>
        /// Conversion factor from meters to feet.
        /// </summary>
        public const float PER_METER = PER_INCH * Units.Inches.PER_METER;

        /// <summary>
        /// Conversion factor from kilometers to feet.
        /// </summary>
        public const float PER_KILOMETER = PER_METER * Units.Meters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from miles to feet.
        /// </summary>
        public const float PER_MILE = 5280;

        /// <summary>
        /// Convert from feet to millimeters.
        /// </summary>
        /// <param name="feet">The number of feet</param>
        /// <returns>The number of millimeters</returns>
        public static float Millimeters(float feet)
        {
            return feet * Units.Millimeters.PER_FOOT;
        }

        /// <summary>
        /// Convert from feet to centimeters.
        /// </summary>
        /// <param name="feet">The number of feet</param>
        /// <returns>The number of centimeters</returns>
        public static float Centimeters(float feet)
        {
            return feet * Units.Centimeters.PER_FOOT;
        }

        /// <summary>
        /// Convert from feet to inches.
        /// </summary>
        /// <param name="feet">The number of feet</param>
        /// <returns>The number of inches</returns>
        public static float Inches(float feet)
        {
            return feet * Units.Inches.PER_FOOT;
        }

        /// <summary>
        /// Convert from feet to meters.
        /// </summary>
        /// <param name="feet">The number of feet</param>
        /// <returns>The number of meters</returns>
        public static float Meters(float feet)
        {
            return feet * Units.Meters.PER_FOOT;
        }

        /// <summary>
        /// Convert from feet to kilometers.
        /// </summary>
        /// <param name="feet">The number of feet</param>
        /// <returns>The number of kilometers</returns>
        public static float Kilometers(float feet)
        {
            return feet * Units.Kilometers.PER_FOOT;
        }

        /// <summary>
        /// Convert from feet to miles.
        /// </summary>
        /// <param name="feet">The number of feet</param>
        /// <returns>The number of miles</returns>
        public static float Miles(float feet)
        {
            return feet * Units.Miles.PER_FOOT;
        }
    }
}