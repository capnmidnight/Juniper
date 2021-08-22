namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic centimeters
    /// </summary>
    public static class CubicCentimeters
    {
        public const float PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_CUBIC_CENTIMETER;
        public const float PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_CUBIC_CENTIMETER;
        public const float PER_MINIM = Units.Milliliters.PER_MINIM;
        public const float PER_CUBIC_CENTIMETER = 1;
        public const float PER_MILLILITER = 1;
        public const float PER_FLUID_DRAM = Units.Milliliters.PER_FLUID_DRAM;
        public const float PER_TEASPOON = Units.Milliliters.PER_TEASPOON;
        public const float PER_TABLESPOON = Units.Milliliters.PER_TABLESPOON;
        public const float PER_CUBIC_INCH = SquareCentimeters.PER_SQUARE_INCH * Centimeters.PER_INCH;
        public const float PER_FLUID_OUNCE = Units.Milliliters.PER_FLUID_OUNCE;
        public const float PER_GILL = Units.Milliliters.PER_GILL;
        public const float PER_CUP = Units.Milliliters.PER_CUP;
        public const float PER_LIQUID_PINT = Units.Milliliters.PER_LIQUID_PINT;
        public const float PER_LIQUID_QUART = Units.Milliliters.PER_LIQUID_QUART;
        public const float PER_LITER = Units.Milliliters.PER_LITER;
        public const float PER_GALLON = Units.Milliliters.PER_GALLON;
        public const float PER_CUBIC_FOOT = SquareCentimeters.PER_SQUARE_FOOT * Centimeters.PER_FOOT;
        public const float PER_CUBIC_METER = SquareCentimeters.PER_SQUARE_METER * Centimeters.PER_METER;
        public const float PER_KILOLITER = Units.Milliliters.PER_KILOLITER;
        public const float PER_CUBIC_KILOMETER = SquareCentimeters.PER_SQUARE_KILOMETER * Centimeters.PER_KILOMETER;
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

        public static float Minims(float cubicCentimeters)
        {
            return cubicCentimeters * Units.Minims.PER_CUBIC_CENTIMETER;
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

        public static float FluidDrams(float cubicCentimeters)
        {
            return cubicCentimeters * Units.FluidDrams.PER_CUBIC_CENTIMETER;
        }

        public static float Teaspoons(float cubicCentimeters)
        {
            return cubicCentimeters * Units.Teaspoons.PER_CUBIC_CENTIMETER;
        }

        public static float Tablespoons(float cubicCentimeters)
        {
            return cubicCentimeters * Units.Tablespoons.PER_CUBIC_CENTIMETER;
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

        public static float FluidOunces(float cubicCentimeters)
        {
            return cubicCentimeters * Units.FluidOunces.PER_CUBIC_CENTIMETER;
        }

        public static float Gills(float cubicCentimeters)
        {
            return cubicCentimeters * Units.Gills.PER_CUBIC_CENTIMETER;
        }

        public static float Cups(float cubicCentimeters)
        {
            return cubicCentimeters * Units.Cups.PER_CUBIC_CENTIMETER;
        }

        public static float LiquidPints(float cubicCentimeters)
        {
            return cubicCentimeters * Units.LiquidPints.PER_CUBIC_CENTIMETER;
        }

        public static float LiquidQuarts(float cubicCentimeters)
        {
            return cubicCentimeters * Units.LiquidQuarts.PER_CUBIC_CENTIMETER;
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

        public static float Gallons(float cubicCentimeters)
        {
            return cubicCentimeters * Units.Gallons.PER_CUBIC_CENTIMETER;
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