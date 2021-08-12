namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic feet
    /// </summary>
    public static class CubicFeet
    {
        /// <summary>
        /// Conversion factor from cubic micrometers to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_MICROMETER = SquareFeet.PER_SQUARE_MICROMETER * Feet.PER_MICROMETER;

        /// <summary>
        /// Conversion factor from cubic millimeters to cubic feet.
        /// </summary>
        public const float PER_CUBIC_MILLIMETER = SquareFeet.PER_SQUARE_MILLIMETER * Feet.PER_MILLIMETER;

        /// <summary>
        /// Conversion factor from cubic centimeters to cubic feet.
        /// </summary>
        public const float PER_CUBIC_CENTIMETER = SquareFeet.PER_SQUARE_CENTIMETER * Feet.PER_CENTIMETER;

        /// <summary>
        /// Conversion factor from milliliters to cubic feet.
        /// </summary>
        public const float PER_MILLILITER = PER_CUBIC_CENTIMETER;

        /// <summary>
        /// Conversion factor from cubic inches to cubic feet.
        /// </summary>
        public const float PER_CUBIC_INCH = SquareFeet.PER_SQUARE_INCH * Feet.PER_INCH;

        /// <summary>
        /// Conversion factor from liters to cubic feet.
        /// </summary>
        public const float PER_LITER = PER_MILLILITER * Units.Milliliters.PER_LITER;

        /// <summary>
        /// Conversion factor from cubic meters to cubic feet.
        /// </summary>
        public const float PER_CUBIC_METER = SquareFeet.PER_SQUARE_METER * Feet.PER_METER;

        /// <summary>
        /// Conversion factor from kiloliters to cubic feet.
        /// </summary>
        public const float PER_KILOLITER = PER_CUBIC_METER;

        /// <summary>
        /// Conversion factor from cubic kilometers to cubic feet.
        /// </summary>
        public const float PER_CUBIC_KILOMETER = SquareFeet.PER_SQUARE_KILOMETER * Feet.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from cubic miles to cubic feet.
        /// </summary>
        public const float PER_CUBIC_MILE = SquareFeet.PER_SQUARE_MILE * Feet.PER_MILE;

        /// <summary>
        /// Convert from cubic feet to cubic micrometers.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float cubicFeet)
        {
            return cubicFeet * Units.CubicMicrometers.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to cubic millimeters.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float cubicFeet)
        {
            return cubicFeet * Units.CubicMillimeters.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to cubic centimeters.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float cubicFeet)
        {
            return cubicFeet * Units.CubicCentimeters.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to milliliters.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float cubicFeet)
        {
            return CubicCentimeters(cubicFeet);
        }

        /// <summary>
        /// Convert from cubic feet to cubic inches.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float cubicFeet)
        {
            return cubicFeet * Units.CubicInches.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to liters.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float cubicFeet)
        {
            return cubicFeet * Units.Liters.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to cubic meters.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float cubicFeet)
        {
            return cubicFeet * Units.CubicMeters.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to kiloliters.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float cubicFeet)
        {
            return CubicMeters(cubicFeet);
        }

        /// <summary>
        /// Convert from cubic feet to cubic kilometers.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float cubicFeet)
        {
            return cubicFeet * Units.CubicKilometers.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to cubic miles.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float cubicFeet)
        {
            return cubicFeet * Units.CubicMiles.PER_CUBIC_FOOT;
        }
    }
}