namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic centimeters
    /// </summary>
    public static class CubicCentimeters
    {
        /// <summary>
        /// Conversion factor from cubic micrometers to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_MICROMETER = SquareCentimeters.PER_SQUARE_MICROMETER * Centimeters.PER_MICROMETER;

        /// <summary>
        /// Conversion factor from cubic millimeters to cubic centimeters.
        /// </summary>
        public const float PER_CUBIC_MILLIMETER = SquareCentimeters.PER_SQUARE_MILLIMETER * Centimeters.PER_MILLIMETER;

        /// <summary>
        /// Conversion factor from milliliters to cubic centimeters.
        /// </summary>
        public const float PER_MILLILITER = 1;

        /// <summary>
        /// Conversion factor from cubic inches to cubic centimeters.
        /// </summary>
        public const float PER_CUBIC_INCH = SquareCentimeters.PER_SQUARE_INCH * Centimeters.PER_INCH;

        /// <summary>
        /// Conversion factor from liters to cubic centimeters.
        /// </summary>
        public const float PER_LITER = PER_MILLILITER * Units.Milliliters.PER_LITER;

        /// <summary>
        /// Conversion factor from cubic feet to cubic centimeters.
        /// </summary>
        public const float PER_CUBIC_FOOT = SquareCentimeters.PER_SQUARE_FOOT * Centimeters.PER_FOOT;

        /// <summary>
        /// Conversion factor from cubic meters to cubic centimeters.
        /// </summary>
        public const float PER_CUBIC_METER = SquareCentimeters.PER_SQUARE_METER * Centimeters.PER_METER;

        /// <summary>
        /// Conversion factor from kiloliters to cubic centimeters.
        /// </summary>
        public const float PER_KILOLITER = PER_LITER * Units.Liters.PER_KILOLITER;

        /// <summary>
        /// Conversion factor from cubic kilometers to cubic centimeters.
        /// </summary>
        public const float PER_CUBIC_KILOMETER = SquareCentimeters.PER_SQUARE_KILOMETER * Centimeters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from cubic miles to cubic centimeters.
        /// </summary>
        public const float PER_CUBIC_MILE = SquareCentimeters.PER_SQUARE_MILE * Centimeters.PER_MILE;

        /// <summary>
        /// Convert from cubic centimeters to cubic micrometers.
        /// </summary>
        /// <param name="cubicCentimeters">The number of cubic centimeters</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float cubicCentimeters)
        {
            return cubicCentimeters * Units.CubicMicrometers.PER_CUBIC_CENTIMETER;
        }

        /// <summary>
        /// Convert from cubic centimeters to cubic millimeters.
        /// </summary>
        /// <param name="cubicCentimeters">The number of cubic centimeters</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float cubicCentimeters)
        {
            return cubicCentimeters * Units.CubicMillimeters.PER_CUBIC_CENTIMETER;
        }

        /// <summary>
        /// Convert from cubic centimeters to milliliters.
        /// </summary>
        /// <param name="cubicCentimeters">The number of cubic centimeters</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float cubicCentimeters)
        {
            return cubicCentimeters;
        }

        /// <summary>
        /// Convert from cubic centimeters to cubic inches.
        /// </summary>
        /// <param name="cubicCentimeters">The number of cubic centimeters</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float cubicCentimeters)
        {
            return cubicCentimeters * Units.CubicInches.PER_CUBIC_CENTIMETER;
        }

        /// <summary>
        /// Convert from cubic centimeters to liters.
        /// </summary>
        /// <param name="cubicCentimeters">The number of cubic centimeters</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float cubicCentimeters)
        {
            return cubicCentimeters * Units.Liters.PER_CUBIC_CENTIMETER;
        }

        /// <summary>
        /// Convert from cubic centimeters to cubic feet.
        /// </summary>
        /// <param name="cubicCentimeters">The number of cubic centimeters</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float cubicCentimeters)
        {
            return cubicCentimeters * Units.CubicFeet.PER_CUBIC_CENTIMETER;
        }

        /// <summary>
        /// Convert from cubic centimeters to cubic meters.
        /// </summary>
        /// <param name="cubicCentimeters">The number of cubic centimeters</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicMeters(float cubicCentimeters)
        {
            return cubicCentimeters * Units.CubicMeters.PER_CUBIC_CENTIMETER;
        }

        /// <summary>
        /// Convert from cubic centimeters to kiloliters.
        /// </summary>
        /// <param name="cubicCentimeters">The number of cubic centimeters</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float cubicCentimeters)
        {
            return CubicMeters(cubicCentimeters);
        }

        /// <summary>
        /// Convert from cubic centimeters to cubic kilometers.
        /// </summary>
        /// <param name="cubicCentimeters">The number of cubic centimeters</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float cubicCentimeters)
        {
            return cubicCentimeters * Units.CubicKilometers.PER_CUBIC_CENTIMETER;
        }

        /// <summary>
        /// Convert from cubic centimeters to cubic miles.
        /// </summary>
        /// <param name="cubicCentimeters">The number of cubic centimeters</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float cubicCentimeters)
        {
            return cubicCentimeters * Units.CubicMiles.PER_CUBIC_CENTIMETER;
        }
    }
}