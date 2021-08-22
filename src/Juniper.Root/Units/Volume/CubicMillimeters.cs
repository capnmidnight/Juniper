namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic millimeters
    /// </summary>
    public static class CubicMillimeters
    {
        public const float PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_CUBIC_MILLIMETER;
        public const float PER_CUBIC_MILLIMETER = 1;
        public const float PER_MINIM = PER_MILLILITER / Units.Minims.PER_MILLILITER;
        public const float PER_CUBIC_CENTIMETER = SquareMillimeters.PER_SQUARE_CENTIMETER * Millimeters.PER_CENTIMETER;
        public const float PER_MILLILITER = PER_CUBIC_CENTIMETER;
        public const float PER_FLUID_DRAM = PER_MILLILITER * Units.Milliliters.PER_FLUID_DRAM;
        public const float PER_TEASPOON = PER_FLUID_DRAM * Units.FluidDrams.PER_TEASPOON;
        public const float PER_TABLESPOON = PER_FLUID_DRAM * Units.FluidDrams.PER_TABLESPOON;
        public const float PER_CUBIC_INCH = SquareMillimeters.PER_SQUARE_INCH * Millimeters.PER_INCH;
        public const float PER_FLUID_OUNCE = PER_TABLESPOON * Units.Tablespoons.PER_FLUID_OUNCE;
        public const float PER_GILL = PER_FLUID_OUNCE * Units.FluidOunces.PER_GILL;
        public const float PER_CUP = PER_GILL * Units.Gills.PER_CUP;
        public const float PER_LIQUID_PINT = PER_CUP * Units.Cups.PER_LIQUID_PINT;
        public const float PER_LIQUID_QUART = PER_LIQUID_PINT * Units.Cups.PER_LIQUID_QUART;
        public const float PER_LITER = PER_MILLILITER * Units.Milliliters.PER_LITER;
        public const float PER_GALLON = PER_LIQUID_QUART * Units.LiquidQuarts.PER_GALLON;
        public const float PER_CUBIC_FOOT = SquareMillimeters.PER_SQUARE_FOOT * Millimeters.PER_FOOT;
        public const float PER_CUBIC_METER = SquareMillimeters.PER_SQUARE_METER * Millimeters.PER_METER;
        public const float PER_KILOLITER = PER_LITER * Units.Liters.PER_KILOLITER;
        public const float PER_CUBIC_KILOMETER = SquareMillimeters.PER_SQUARE_KILOMETER * Millimeters.PER_KILOMETER;
        public const float PER_CUBIC_MILE = SquareMillimeters.PER_SQUARE_MILE * Millimeters.PER_MILE;

        /// <summary>
        /// Convert from cubic millimeters to cubic micrometers.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicMicrometers.PER_CUBIC_MILLIMETER;
        }

        public static float Minims(float cubicMillimeters)
        {
            return cubicMillimeters * Units.Minims.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic centimeters.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicCentimeters.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to milliliters.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float cubicMillimeters)
        {
            return CubicCentimeters(cubicMillimeters);
        }

        public static float FluidDrams(float cubicMillimeters)
        {
            return cubicMillimeters * Units.FluidDrams.PER_CUBIC_MILLIMETER;
        }

        public static float Teaspoons(float cubicMillimeters)
        {
            return cubicMillimeters * Units.Teaspoons.PER_CUBIC_MILLIMETER;
        }

        public static float Tablespoons(float cubicMillimeters)
        {
            return cubicMillimeters * Units.Tablespoons.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic inches.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicInches.PER_CUBIC_MILLIMETER;
        }

        public static float FluidOunces(float cubicMillimeters)
        {
            return cubicMillimeters * Units.FluidOunces.PER_CUBIC_MILLIMETER;
        }

        public static float Gills(float cubicMillimeters)
        {
            return cubicMillimeters * Units.Gills.PER_CUBIC_MILLIMETER;
        }

        public static float Cups(float cubicMillimeters)
        {
            return cubicMillimeters * Units.Cups.PER_CUBIC_MILLIMETER;
        }

        public static float LiquidPints(float cubicMillimeters)
        {
            return cubicMillimeters * Units.LiquidPints.PER_CUBIC_MILLIMETER;
        }

        public static float LiquidQuarts(float cubicMillimeters)
        {
            return cubicMillimeters * Units.LiquidQuarts.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to liters.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float cubicMillimeters)
        {
            return cubicMillimeters * Units.Liters.PER_CUBIC_MILLIMETER;
        }

        public static float Gallons(float cubicMillimeters)
        {
            return cubicMillimeters * Units.Gallons.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic feet.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicFeet.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic meters.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicMeters.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to kiloliters.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float cubicMillimeters)
        {
            return CubicMeters(cubicMillimeters);
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic kilometers.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicKilometers.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic miles.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicMiles.PER_CUBIC_MILLIMETER;
        }
    }
}