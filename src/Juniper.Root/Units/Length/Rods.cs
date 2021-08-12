namespace Juniper.Units
{
    /// <summary>
    /// Conversions from rods
    /// </summary>
    public static class Rods
    {
        /// <summary>
        /// Conversion factor from micrometers to rods.
        /// </summary>
        public const float PER_MICROMETER = 1 / Units.Micrometers.PER_ROD;

        /// <summary>
        /// Conversion factor from millimeters to rods.
        /// </summary>
        public const float PER_MILLIMETER = 1 / Units.Millimeters.PER_ROD;

        /// <summary>
        /// Conversion factor from centimeters to rods.
        /// </summary>
        public const float PER_CENTIMETER = 1 / Units.Centimeters.PER_ROD;

        /// <summary>
        /// Conversion factor from inches to rods.
        /// </summary>
        public const float PER_INCH = 1 / Units.Inches.PER_ROD;

        /// <summary>
        /// Conversion factor from feet to rods.
        /// </summary>
        public const float PER_FOOT = 1 / Units.Feet.PER_ROD;

        /// <summary>
        /// Conversion factor from yards to rods.
        /// </summary>
        public const float PER_YARD = 1 / Units.Yards.PER_ROD;

        /// <summary>
        /// Conversion factor from meters to rods.
        /// </summary>
        public const float PER_METER = 1 / Units.Meters.PER_ROD;

        /// <summary>
        /// Conversion factor from furlongs to rods.
        /// </summary>
        public const float PER_FURLONG = 40;

        /// <summary>
        /// Conversion factor from kilometers to rods.
        /// </summary>
        public const float PER_KILOMETER = PER_METER * Units.Meters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from miles to rods.
        /// </summary>
        public const float PER_MILE = PER_FURLONG * Units.Furlongs.PER_MILE;

        /// <summary>
        /// Convert from rods to micrometers.
        /// </summary>
        /// <param name="rods">The number of rods</param>
        /// <returns>The number of micrometers</returns>
        public static float Micrometers(float rods)
        {
            return rods * Units.Micrometers.PER_ROD;
        }

        /// <summary>
        /// Convert from rods to millimeters.
        /// </summary>
        /// <param name="rods">The number of rods</param>
        /// <returns>The number of millimeters</returns>
        public static float Millimeters(float rods)
        {
            return rods * Units.Millimeters.PER_ROD;
        }

        /// <summary>
        /// Convert from rods to centimeters.
        /// </summary>
        /// <param name="rods">The number of rods</param>
        /// <returns>The number of centimeters</returns>
        public static float Centimeters(float rods)
        {
            return rods * Units.Centimeters.PER_ROD;
        }

        /// <summary>
        /// Convert from rods to inches.
        /// </summary>
        /// <param name="rods">The number of rods</param>
        /// <returns>The number of inches</returns>
        public static float Inches(float rods)
        {
            return rods * Units.Inches.PER_ROD;
        }

        /// <summary>
        /// Convert from rods to feet.
        /// </summary>
        /// <param name="rods">The number of rods</param>
        /// <returns>The number of feet</returns>
        public static float Feet(float rods)
        {
            return rods * Units.Feet.PER_ROD;
        }

        /// <summary>
        /// Convert from rods to yards.
        /// </summary>
        /// <param name="rods">The number of rods</param>
        /// <returns>The number of yards</returns>
        public static float Yards(float rods)
        {
            return rods * Units.Yards.PER_ROD;
        }

        /// <summary>
        /// Convert from rods to meters.
        /// </summary>
        /// <param name="rods">The number of rods</param>
        /// <returns>The number of meters</returns>
        public static float Meters(float rods)
        {
            return rods * Units.Meters.PER_ROD;
        }

        /// <summary>
        /// Convert from rods to furlongs.
        /// </summary>
        /// <param name="rods">The number of rods</param>
        /// <returns>The number of furlongs</returns>
        public static float Furlongs(float rods)
        {
            return rods * Units.Furlongs.PER_ROD;
        }

        /// <summary>
        /// Convert from rods to kilometers.
        /// </summary>
        /// <param name="rods">The number of rods</param>
        /// <returns>The number of kilometers</returns>
        public static float Kilometers(float rods)
        {
            return rods * Units.Kilometers.PER_ROD;
        }

        /// <summary>
        /// Convert from rods to miles.
        /// </summary>
        /// <param name="rods">The number of rods</param>
        /// <returns>The number of miles</returns>
        public static float Miles(float rods)
        {
            return rods * Units.Miles.PER_ROD;
        }
    }
}