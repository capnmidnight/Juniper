namespace Juniper.Units
{
    /// <summary>
    /// Conversions from miles
    /// </summary>
    public static class Miles
    {
        /// <summary>
        /// Conversion factor from millimeters to miles.
        /// </summary>
        public const float PER_MILLIMETER = 1 / Units.Millimeters.PER_MILE;

        /// <summary>
        /// Conversion factor from centimeters to miles.
        /// </summary>
        public const float PER_CENTIMETER = 1 / Units.Centimeters.PER_MILE;

        /// <summary>
        /// Conversion factor from inches to miles.
        /// </summary>
        public const float PER_INCH = 1 / Units.Inches.PER_MILE;

        /// <summary>
        /// Conversion factor from feet to miles.
        /// </summary>
        public const float PER_FOOT = 1 / Units.Feet.PER_MILE;

        /// <summary>
        /// Conversion factor from meters to miles.
        /// </summary>
        public const float PER_METER = 1 / Units.Meters.PER_MILE;

        /// <summary>
        /// Conversion factor from kilometers to miles.
        /// </summary>
        public const float PER_KILOMETER = 1 / Units.Kilometers.PER_MILE;

        /// <summary>
        /// Convert from miles to millimeters.
        /// </summary>
        /// <param name="miles">The number of miles</param>
        /// <returns>The number of millimeters</returns>
        public static float Millimeters(float miles)
        {
            return miles * Units.Millimeters.PER_MILE;
        }

        /// <summary>
        /// Convert from miles to centimeters.
        /// </summary>
        /// <param name="miles">The number of miles</param>
        /// <returns>The number of centimeters</returns>
        public static float Centimeters(float miles)
        {
            return miles * Units.Centimeters.PER_MILE;
        }

        /// <summary>
        /// Convert from miles to inches.
        /// </summary>
        /// <param name="miles">The number of miles</param>
        /// <returns>The number of inches</returns>
        public static float Inches(float miles)
        {
            return miles * Units.Inches.PER_MILE;
        }

        /// <summary>
        /// Convert from miles to feet.
        /// </summary>
        /// <param name="miles">The number of miles</param>
        /// <returns>The number of feet</returns>
        public static float Feet(float miles)
        {
            return miles * Units.Feet.PER_MILE;
        }

        /// <summary>
        /// Convert from miles to meters.
        /// </summary>
        /// <param name="miles">The number of miles</param>
        /// <returns>The number of meters</returns>
        public static float Meters(float miles)
        {
            return miles * Units.Meters.PER_MILE;
        }

        /// <summary>
        /// Convert from miles to kilometers.
        /// </summary>
        /// <param name="miles">The number of miles</param>
        /// <returns>The number of kilometers</returns>
        public static float Kilometers(float miles)
        {
            return miles * Units.Kilometers.PER_MILE;
        }
    }
}