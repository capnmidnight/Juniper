namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic meters
    /// </summary>
    public static class CubicMeters
    {
        public const float PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_CUBIC_METER;
        public const float PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_CUBIC_METER;
        public const float PER_MINIM = Units.Kiloliters.PER_MINIM;
        public const float PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_CUBIC_METER;
        public const float PER_MILLILITER = Units.Kiloliters.PER_MILLILITER;
        public const float PER_FLUID_DRAM = Units.Kiloliters.PER_FLUID_DRAM;
        public const float PER_TEASPOON = Units.Kiloliters.PER_TEASPOON;
        public const float PER_TABLESPOON = Units.Kiloliters.PER_TABLESPOON;
        public const float PER_CUBIC_INCH = 1 / Units.CubicInches.PER_CUBIC_METER;
        public const float PER_FLUID_OUNCE = Units.Kiloliters.PER_FLUID_OUNCE;
        public const float PER_GILL = Units.Kiloliters.PER_GILL;
        public const float PER_CUP = Units.Kiloliters.PER_CUP;
        public const float PER_LIQUID_PINT = Units.Kiloliters.PER_LIQUID_PINT;
        public const float PER_LIQUID_QUART = Units.Kiloliters.PER_LIQUID_QUART;
        public const float PER_LITER = Units.Kiloliters.PER_LITER;
        public const float PER_GALLON = Units.Kiloliters.PER_GALLON;
        public const float PER_CUBIC_FOOT = 1 / Units.CubicFeet.PER_CUBIC_METER;
        public const float PER_CUBIC_METER = 1;
        public const float PER_KILOLITER = 1;
        public const float PER_CUBIC_KILOMETER = SquareMeters.PER_SQUARE_KILOMETER * Meters.PER_KILOMETER;
        public const float PER_CUBIC_MILE = SquareMeters.PER_SQUARE_MILE * Meters.PER_MILE;

        /// <summary>
        /// Convert from cubic meters to cubic micrometers.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float cubicMeters)
        {
            return cubicMeters * Units.CubicMicrometers.PER_CUBIC_METER;
        }

        /// <summary>
        /// Convert from cubic meters to cubic millimeters.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float cubicMeters)
        {
            return cubicMeters * Units.CubicMillimeters.PER_CUBIC_METER;
        }

        public static float Minims(float cubicMeters)
        {
            return cubicMeters * Units.Minims.PER_CUBIC_METER;
        }

        /// <summary>
        /// Convert from cubic meters to cubic centimeters.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float cubicMeters)
        {
            return cubicMeters * Units.CubicCentimeters.PER_CUBIC_METER;
        }

        /// <summary>
        /// Convert from cubic meters to milliliters.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float cubicMeters)
        {
            return CubicCentimeters(cubicMeters);
        }

        public static float FluidDrams(float cubicMeters)
        {
            return cubicMeters * Units.FluidDrams.PER_CUBIC_METER;
        }

        public static float Teaspoons(float cubicMeters)
        {
            return cubicMeters * Units.Teaspoons.PER_CUBIC_METER;
        }

        public static float Tablespoons(float cubicMeters)
        {
            return cubicMeters * Units.Tablespoons.PER_CUBIC_METER;
        }

        /// <summary>
        /// Convert from cubic meters to cubic inches.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float cubicMeters)
        {
            return cubicMeters * Units.CubicInches.PER_CUBIC_METER;
        }

        public static float FluidOunces(float cubicMeters)
        {
            return cubicMeters * Units.FluidOunces.PER_CUBIC_METER;
        }

        public static float Gills(float cubicMeters)
        {
            return cubicMeters * Units.Gills.PER_CUBIC_METER;
        }

        public static float Cups(float cubicMeters)
        {
            return cubicMeters * Units.Cups.PER_CUBIC_METER;
        }

        public static float LiquidPints(float cubicMeters)
        {
            return cubicMeters * Units.LiquidPints.PER_CUBIC_METER;
        }

        public static float LiquidQuarts(float cubicMeters)
        {
            return cubicMeters * Units.LiquidQuarts.PER_CUBIC_METER;
        }

        /// <summary>
        /// Convert from cubic meters to liters.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float cubicMeters)
        {
            return cubicMeters * Units.Liters.PER_CUBIC_METER;
        }

        public static float Gallons(float cubicMeters)
        {
            return cubicMeters * Units.Gallons.PER_CUBIC_METER;
        }

        /// <summary>
        /// Convert from cubic meters to cubic feet.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float cubicMeters)
        {
            return cubicMeters * Units.CubicFeet.PER_CUBIC_METER;
        }

        /// <summary>
        /// Convert from cubic meters to kiloliters.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float cubicMeters)
        {
            return cubicMeters;
        }

        /// <summary>
        /// Convert from cubic meters to cubic kilometers.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float cubicMeters)
        {
            return cubicMeters * Units.CubicKilometers.PER_CUBIC_METER;
        }

        /// <summary>
        /// Convert from cubic meters to cubic miles.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float cubicMeters)
        {
            return cubicMeters * Units.CubicMiles.PER_CUBIC_METER;
        }
    }
}