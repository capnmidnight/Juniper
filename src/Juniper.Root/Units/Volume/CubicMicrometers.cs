namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic micrometers
    /// </summary>
    public static class CubicMicrometers
    {
        public const float PER_CUBIC_MICROMETER = 1;
        public const float PER_CUBIC_MILLIMETER = SquareMicrometers.PER_SQUARE_MILLIMETER * Micrometers.PER_MILLIMETER;
        public const float PER_MINIM = PER_CUBIC_MILLIMETER * Units.CubicMillimeters.PER_MINIM;
        public const float PER_CUBIC_CENTIMETER = SquareMicrometers.PER_SQUARE_CENTIMETER * Micrometers.PER_CENTIMETER;
        public const float PER_MILLILITER = PER_CUBIC_CENTIMETER;
        public const float PER_FLUID_DRAM = PER_MILLILITER * Units.Milliliters.PER_FLUID_DRAM;
        public const float PER_TEASPOON = PER_FLUID_DRAM * Units.FluidDrams.PER_TEASPOON;
        public const float PER_TABLESPOON = PER_FLUID_DRAM * Units.FluidDrams.PER_TABLESPOON;
        public const float PER_CUBIC_INCH = SquareMicrometers.PER_SQUARE_INCH * Micrometers.PER_INCH;
        public const float PER_FLUID_OUNCE = PER_TABLESPOON * Units.Tablespoons.PER_FLUID_OUNCE;
        public const float PER_GILL = PER_FLUID_OUNCE * Units.FluidOunces.PER_GILL;
        public const float PER_CUP = PER_GILL * Units.Gills.PER_CUP;
        public const float PER_LIQUID_PINT = PER_CUP * Units.Cups.PER_LIQUID_PINT;
        public const float PER_LIQUID_QUART = PER_LIQUID_PINT * Units.Cups.PER_LIQUID_QUART;
        public const float PER_LITER = PER_CUBIC_CENTIMETER * Units.CubicCentimeters.PER_LITER;
        public const float PER_GALLON = PER_LIQUID_QUART * Units.LiquidQuarts.PER_GALLON;
        public const float PER_CUBIC_FOOT = SquareMicrometers.PER_SQUARE_FOOT * Micrometers.PER_FOOT;
        public const float PER_CUBIC_METER = SquareMicrometers.PER_SQUARE_METER * Micrometers.PER_METER;
        public const float PER_KILOLITER = PER_CUBIC_METER;
        public const float PER_CUBIC_KILOMETER = SquareMicrometers.PER_SQUARE_KILOMETER * Micrometers.PER_KILOMETER;
        public const float PER_CUBIC_MILE = SquareMicrometers.PER_SQUARE_MILE * Micrometers.PER_MILE;

        /// <summary>
        /// Convert from cubic micrometers to cubic millimeters.
        /// </summary>
        /// <param name="cubicMicrometers">The number of cubic micrometers</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float cubicMicrometers)
        {
            return cubicMicrometers * Units.CubicMillimeters.PER_CUBIC_MICROMETER;
        }

        public static float Minims(float cubicMicrometers)
        {
            return cubicMicrometers * Units.Minims.PER_CUBIC_MICROMETER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic centimeters.
        /// </summary>
        /// <param name="cubicMicrometers">The number of cubic micrometers</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float cubicMicrometers)
        {
            return cubicMicrometers * Units.CubicCentimeters.PER_CUBIC_MICROMETER;
        }

        /// <summary>
        /// Convert from cubic micrometers to milliliters.
        /// </summary>
        /// <param name="cubicMicrometers">The number of cubic micrometers</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float cubicMicrometers)
        {
            return CubicCentimeters(cubicMicrometers);
        }

        public static float FluidDrams(float cubicMicrometers)
        {
            return cubicMicrometers * Units.FluidDrams.PER_CUBIC_MICROMETER;
        }

        public static float Teaspoons(float cubicMicrometers)
        {
            return cubicMicrometers * Units.Teaspoons.PER_CUBIC_MICROMETER;
        }

        public static float Tablespoons(float cubicMicrometers)
        {
            return cubicMicrometers * Units.Tablespoons.PER_CUBIC_MICROMETER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic inches.
        /// </summary>
        /// <param name="cubicMicrometers">The number of cubic micrometers</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float cubicMicrometers)
        {
            return cubicMicrometers * Units.CubicInches.PER_CUBIC_MICROMETER;
        }

        public static float FluidOunces(float cubicMicrometers)
        {
            return cubicMicrometers * Units.FluidOunces.PER_CUBIC_MICROMETER;
        }

        public static float Gills(float cubicMicrometers)
        {
            return cubicMicrometers * Units.Gills.PER_CUBIC_MICROMETER;
        }

        public static float Cups(float cubicMicrometers)
        {
            return cubicMicrometers * Units.Cups.PER_CUBIC_MICROMETER;
        }

        public static float LiquidPints(float cubicMicrometers)
        {
            return cubicMicrometers * Units.LiquidPints.PER_CUBIC_MICROMETER;
        }

        public static float LiquidQuarts(float cubicMicrometers)
        {
            return cubicMicrometers * Units.LiquidQuarts.PER_CUBIC_MICROMETER;
        }

        /// <summary>
        /// Convert from cubic micrometers to liters.
        /// </summary>
        /// <param name="cubicMicrometers">The number of cubic micrometers</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float cubicMicrometers)
        {
            return cubicMicrometers * Units.Liters.PER_CUBIC_MICROMETER;
        }

        public static float Gallons(float cubicMicrometers)
        {
            return cubicMicrometers * Units.Gallons.PER_CUBIC_MICROMETER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic feet.
        /// </summary>
        /// <param name="cubicMicrometers">The number of cubic micrometers</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float cubicMicrometers)
        {
            return cubicMicrometers * Units.CubicFeet.PER_CUBIC_MICROMETER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic meters.
        /// </summary>
        /// <param name="cubicMicrometers">The number of cubic micrometers</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float cubicMicrometers)
        {
            return cubicMicrometers * Units.CubicMeters.PER_CUBIC_MICROMETER;
        }

        /// <summary>
        /// Convert from cubic micrometers to kiloliters.
        /// </summary>
        /// <param name="cubicMicrometers">The number of cubic micrometers</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float cubicMicrometers)
        {
            return CubicMeters(cubicMicrometers);
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic kilometers.
        /// </summary>
        /// <param name="cubicMicrometers">The number of cubic micrometers</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float cubicMicrometers)
        {
            return cubicMicrometers * Units.CubicKilometers.PER_CUBIC_MICROMETER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic miles.
        /// </summary>
        /// <param name="cubicMicrometers">The number of cubic micrometers</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float cubicMicrometers)
        {
            return cubicMicrometers * Units.CubicMiles.PER_CUBIC_MICROMETER;
        }
    }
}