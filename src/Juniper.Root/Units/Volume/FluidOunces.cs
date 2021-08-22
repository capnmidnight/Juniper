namespace Juniper.Units
{
    /// <summary>
    /// Conversions from US fluid ounces
    /// </summary>
    public static class FluidOunces
    {
        public const float PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_FLUID_OUNCE;
        public const float PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_FLUID_OUNCE;
        public const float PER_MINIM = 1 / Units.Minims.PER_FLUID_OUNCE;
        public const float PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_FLUID_OUNCE;
        public const float PER_MILLILITER = 1 / Units.Milliliters.PER_FLUID_OUNCE;
        public const float PER_FLUID_DRAM = 1 / Units.FluidDrams.PER_FLUID_OUNCE;
        public const float PER_TEASPOON = 1 / Units.Teaspoons.PER_FLUID_OUNCE;
        public const float PER_TABLESPOON = 1 / Units.Tablespoons.PER_FLUID_OUNCE;
        public const float PER_CUBIC_INCH = 1 / Units.CubicInches.PER_FLUID_OUNCE;
        public const float PER_FLUID_OUNCE = 1;
        public const float PER_GILL = 4;
        public const float PER_CUP = PER_GILL * Units.Gills.PER_CUP;
        public const float PER_LIQUID_PINT = PER_CUP * Units.Cups.PER_LIQUID_PINT;
        public const float PER_LIQUID_QUART = PER_LIQUID_PINT * Units.LiquidPints.PER_LIQUID_QUART;
        public const float PER_LITER = PER_LIQUID_QUART * Units.LiquidQuarts.PER_LITER;
        public const float PER_GALLON = PER_LIQUID_QUART * Units.LiquidQuarts.PER_GALLON;
        public const float PER_CUBIC_FOOT = PER_GALLON * Units.Gallons.PER_CUBIC_FOOT;
        public const float PER_CUBIC_METER = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_METER;
        public const float PER_KILOLITER = PER_CUBIC_METER;
        public const float PER_CUBIC_KILOMETER = PER_CUBIC_METER * Units.CubicMeters.PER_CUBIC_KILOMETER;
        public const float PER_CUBIC_MILE = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_MILE;

        /// <summary>
        /// Convert from cubic millimeters to cubic micrometers.
        /// </summary>
        /// <param name="fluidOunces">The number of cubic millimeters</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float fluidOunces)
        {
            return fluidOunces * Units.CubicMicrometers.PER_FLUID_OUNCE;
        }

        public static float CubicMillimeters(float fluidOunces)
        {
            return fluidOunces * Units.CubicMillimeters.PER_FLUID_OUNCE;
        }

        public static float Minims(float fluidOunces)
        {
            return fluidOunces * Units.Minims.PER_FLUID_OUNCE;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic centimeters.
        /// </summary>
        /// <param name="fluidOunces">The number of cubic millimeters</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float fluidOunces)
        {
            return fluidOunces * Units.CubicCentimeters.PER_FLUID_OUNCE;
        }

        /// <summary>
        /// Convert from cubic millimeters to milliliters.
        /// </summary>
        /// <param name="fluidOunces">The number of cubic millimeters</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float fluidOunces)
        {
            return CubicCentimeters(fluidOunces);
        }

        public static float FluidDrams(float fluidOunces)
        {
            return fluidOunces * Units.FluidDrams.PER_FLUID_OUNCE;
        }

        public static float Teaspoons(float fluidOunces)
        {
            return fluidOunces * Units.Teaspoons.PER_FLUID_OUNCE;
        }

        public static float Tablespoons(float fluidOunces)
        {
            return fluidOunces * Units.Tablespoons.PER_FLUID_OUNCE;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic inches.
        /// </summary>
        /// <param name="fluidOunces">The number of cubic millimeters</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float fluidOunces)
        {
            return fluidOunces * Units.CubicInches.PER_FLUID_OUNCE;
        }

        public static float Gills(float fluidOunces)
        {
            return fluidOunces * Units.Gills.PER_FLUID_OUNCE;
        }

        public static float Cups(float fluidOunces)
        {
            return fluidOunces * Units.Cups.PER_FLUID_OUNCE;
        }

        public static float LiquidPints(float fluidOunces)
        {
            return fluidOunces * Units.LiquidPints.PER_FLUID_OUNCE;
        }

        public static float LiquidQuarts(float fluidOunces)
        {
            return fluidOunces * Units.LiquidQuarts.PER_FLUID_OUNCE;
        }

        /// <summary>
        /// Convert from cubic millimeters to liters.
        /// </summary>
        /// <param name="fluidOunces">The number of cubic millimeters</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float fluidOunces)
        {
            return fluidOunces * Units.Liters.PER_FLUID_OUNCE;
        }

        public static float Gallons(float fluidOunces)
        {
            return fluidOunces * Units.Gallons.PER_FLUID_OUNCE;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic feet.
        /// </summary>
        /// <param name="fluidOunces">The number of cubic millimeters</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float fluidOunces)
        {
            return fluidOunces * Units.CubicFeet.PER_FLUID_OUNCE;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic meters.
        /// </summary>
        /// <param name="fluidOunces">The number of cubic millimeters</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float fluidOunces)
        {
            return fluidOunces * Units.CubicMeters.PER_FLUID_OUNCE;
        }

        /// <summary>
        /// Convert from cubic millimeters to kiloliters.
        /// </summary>
        /// <param name="fluidOunces">The number of cubic millimeters</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float fluidOunces)
        {
            return CubicMeters(fluidOunces);
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic kilometers.
        /// </summary>
        /// <param name="fluidOunces">The number of cubic millimeters</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float fluidOunces)
        {
            return fluidOunces * Units.CubicKilometers.PER_FLUID_OUNCE;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic miles.
        /// </summary>
        /// <param name="fluidOunces">The number of cubic millimeters</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float fluidOunces)
        {
            return fluidOunces * Units.CubicMiles.PER_FLUID_OUNCE;
        }
    }
}
