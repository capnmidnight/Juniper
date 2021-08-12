namespace Juniper.Units
{
    /// <summary>
    /// Conversions from yards
    /// </summary>
    public static class Yards
    {
        /// <summary>
        /// Conversion factor from micrometers to yards.
        /// </summary>
        public const float PER_MICROMETER = 1 / Units.Micrometers.PER_YARD;

        /// <summary>
        /// Conversion factor from millimeters to yards.
        /// </summary>
        public const float PER_MILLIMETER = 1 / Units.Millimeters.PER_YARD;

        /// <summary>
        /// Conversion factor from centimeters to yards.
        /// </summary>
        public const float PER_CENTIMETER = 1 / Units.Centimeters.PER_YARD;

        /// <summary>
        /// Conversion factor from inches to yards.
        /// </summary>
        public const float PER_INCH = 1 / Units.Inches.PER_YARD;

        /// <summary>
        /// Conversion factor from feet to yards.
        /// </summary>
        public const float PER_FOOT = 1 / Units.Feet.PER_YARD;

        /// <summary>
        /// Conversion factor from meters to yards.
        /// </summary>
        public const float PER_METER = PER_INCH * Units.Inches.PER_METER;

        /// <summary>
        /// Conversion factor from kilometers to yards.
        /// </summary>
        public const float PER_KILOMETER = PER_METER * Units.Meters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from miles to yards.
        /// </summary>
        public const float PER_MILE = 5280;

        /// <summary>
        /// Convert from yards to micrometers.
        /// </summary>
        /// <param name="yards">The number of yards</param>
        /// <returns>The number of micrometers</returns>
        public static float Micrometers(float yards)
        {
            return yards * Units.Micrometers.PER_YARD;
        }

        /// <summary>
        /// Convert from yards to millimeters.
        /// </summary>
        /// <param name="yards">The number of yards</param>
        /// <returns>The number of millimeters</returns>
        public static float Millimeters(float yards)
        {
            return yards * Units.Millimeters.PER_YARD;
        }

        /// <summary>
        /// Convert from yards to centimeters.
        /// </summary>
        /// <param name="yards">The number of yards</param>
        /// <returns>The number of centimeters</returns>
        public static float Centimeters(float yards)
        {
            return yards * Units.Centimeters.PER_YARD;
        }

        /// <summary>
        /// Convert from yards to inches.
        /// </summary>
        /// <param name="yards">The number of yards</param>
        /// <returns>The number of inches</returns>
        public static float Inches(float yards)
        {
            return yards * Units.Inches.PER_YARD;
        }

        /// <summary>
        /// Convert from yards to feet.
        /// </summary>
        /// <param name="yards">The number of yards</param>
        /// <returns>The number of feet</returns>
        public static float Feet(float yards)
        {
            return yards * Units.Feet.PER_YARD;
        }

        /// <summary>
        /// Convert from yards to meters.
        /// </summary>
        /// <param name="yards">The number of yards</param>
        /// <returns>The number of meters</returns>
        public static float Meters(float yards)
        {
            return yards * Units.Meters.PER_YARD;
        }

        /// <summary>
        /// Convert from yards to kilometers.
        /// </summary>
        /// <param name="yards">The number of yards</param>
        /// <returns>The number of kilometers</returns>
        public static float Kilometers(float yards)
        {
            return yards * Units.Kilometers.PER_YARD;
        }

        /// <summary>
        /// Convert from yards to miles.
        /// </summary>
        /// <param name="yards">The number of yards</param>
        /// <returns>The number of miles</returns>
        public static float Miles(float yards)
        {
            return yards * Units.Miles.PER_YARD;
        }
    }
}