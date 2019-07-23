namespace Juniper.Units
{
    /// <summary>
    /// Conversions from meters
    /// </summary>
    public static class Meters
    {
        /// <summary>
        /// Conversion factor from millimeters to meters.
        /// </summary>
        public const float PER_MILLIMETER = 1 / Units.Millimeters.PER_METER;

        /// <summary>
        /// Conversion factor from millimeters to meters.
        /// </summary>
        public const float PER_CENTIMETER = 1 / Units.Centimeters.PER_METER;

        /// <summary>
        /// Conversion factor from inches to meters.
        /// </summary>
        public const float PER_INCH = 1 / Units.Inches.PER_METER;

        /// <summary>
        /// Conversion factor from feet to meters.
        /// </summary>
        public const float PER_FOOT = 1 / Units.Feet.PER_METER;

        /// <summary>
        /// Conversion factor from kilometers to meters.
        /// </summary>
        public const float PER_KILOMETER = 1000f;

        /// <summary>
        /// Conversion factor from miles to meters.
        /// </summary>
        public const float PER_MILE = PER_FOOT * Units.Feet.PER_MILE;

        /// <summary>
        /// Convert from meters to millimeters.
        /// </summary>
        /// <param name="meters">The number of meters</param>
        /// <returns>The number of millimeters</returns>
        public static float Millimeters(float meters)
        {
            return meters * Units.Millimeters.PER_METER;
        }

        /// <summary>
        /// Convert from meters to centimeters.
        /// </summary>
        /// <param name="meters">The number of meters</param>
        /// <returns>The number of centimeters</returns>
        public static float Centimeters(float meters)
        {
            return meters * Units.Centimeters.PER_METER;
        }

        /// <summary>
        /// Convert from meters to inches.
        /// </summary>
        /// <param name="meters">The number of meters</param>
        /// <returns>The number of inches</returns>
        public static float Inches(float meters)
        {
            return meters * Units.Inches.PER_METER;
        }

        /// <summary>
        /// Convert from meters to feet.
        /// </summary>
        /// <param name="meters">The number of meters</param>
        /// <returns>The number of feet</returns>
        public static float Feet(float meters)
        {
            return meters * Units.Feet.PER_METER;
        }

        /// <summary>
        /// Convert from meters to kilometers.
        /// </summary>
        /// <param name="meters">The number of meters</param>
        /// <returns>The number of kilometers</returns>
        public static float Kilometers(float meters)
        {
            return meters * Units.Kilometers.PER_METER;
        }

        /// <summary>
        /// Convert from meters to miles.
        /// </summary>
        /// <param name="meters">The number of meters</param>
        /// <returns>The number of miles</returns>
        public static float Miles(float meters)
        {
            return meters * Units.Miles.PER_METER;
        }
    }
}