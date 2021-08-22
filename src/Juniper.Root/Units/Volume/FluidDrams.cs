namespace Juniper.Units
{
    /// <summary>
    /// Conversions from US fluid drams
    /// </summary>
    public static class FluidDrams
    {
        public const float PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_FLUID_DRAM;
        public const float PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_FLUID_DRAM;
        public const float PER_MINIM = 1 / Units.Minims.PER_FLUID_DRAM;
        public const float PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_FLUID_DRAM;
        public const float PER_MILLILITER = 1 / Units.Milliliters.PER_FLUID_DRAM;
        public const float PER_FLUID_DRAM = 1;
        public const float PER_TEASPOON = PER_MINIM * Units.Minims.PER_TEASPOON;
        public const float PER_TABLESPOON = 4;
        public const float PER_CUBIC_INCH = PER_TABLESPOON * Units.Tablespoons.PER_CUBIC_INCH;
        public const float PER_FLUID_OUNCE = PER_TABLESPOON * Units.Tablespoons.PER_FLUID_OUNCE;
        public const float PER_GILL = PER_FLUID_OUNCE * Units.FluidOunces.PER_GILL;
        public const float PER_CUP = PER_GILL * Units.Gills.PER_CUP;
        public const float PER_LIQUID_PINT = PER_CUP * Units.Cups.PER_LIQUID_PINT;
        public const float PER_LIQUID_QUART = PER_LIQUID_PINT * Units.LiquidPints.PER_LIQUID_QUART;
        public const float PER_LITER = PER_LIQUID_QUART * Units.LiquidQuarts.PER_LITER;
        public const float PER_GALLON = PER_LIQUID_QUART * Units.LiquidQuarts.PER_GALLON;
        public const float PER_CUBIC_FOOT = PER_GALLON * Units.Gallons.PER_CUBIC_FOOT;
        public const float PER_CUBIC_METER = PER_LITER * Units.Liters.PER_CUBIC_METER;
        public const float PER_KILOLITER = PER_CUBIC_METER;
        public const float PER_CUBIC_KILOMETER = PER_CUBIC_METER * Units.CubicMeters.PER_CUBIC_KILOMETER;
        public const float PER_CUBIC_MILE = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_MILE;

        /// <summary>
        /// Convert from cubic millimeters to cubic micrometers.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float fluidDrams)
        {
            return fluidDrams * Units.CubicMicrometers.PER_FLUID_DRAM;
        }

        public static float CubicMillimeters(float fluidDrams)
        {
            return fluidDrams * Units.CubicMillimeters.PER_FLUID_DRAM;
        }

        public static float Minims(float fluidDrams)
        {
            return fluidDrams * Units.Minims.PER_FLUID_DRAM;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic centimeters.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float fluidDrams)
        {
            return fluidDrams * Units.CubicCentimeters.PER_FLUID_DRAM;
        }

        /// <summary>
        /// Convert from cubic millimeters to milliliters.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float fluidDrams)
        {
            return CubicCentimeters(fluidDrams);
        }

        public static float Teaspoons(float fluidDrams)
        {
            return fluidDrams * Units.Teaspoons.PER_FLUID_DRAM;
        }

        public static float Tablespoons(float fluidDrams)
        {
            return fluidDrams * Units.Tablespoons.PER_FLUID_DRAM;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic inches.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float fluidDrams)
        {
            return fluidDrams * Units.CubicInches.PER_FLUID_DRAM;
        }

        public static float FluidOunces(float fluidDrams)
        {
            return fluidDrams * Units.FluidOunces.PER_FLUID_DRAM;
        }

        public static float Gills(float fluidDrams)
        {
            return fluidDrams * Units.Gills.PER_FLUID_DRAM;
        }

        public static float Cups(float fluidDrams)
        {
            return fluidDrams * Units.Cups.PER_FLUID_DRAM;
        }

        public static float LiquidPints(float fluidDrams)
        {
            return fluidDrams * Units.LiquidPints.PER_FLUID_DRAM;
        }

        public static float LiquidQuarts(float fluidDrams)
        {
            return fluidDrams * Units.LiquidQuarts.PER_FLUID_DRAM;
        }

        /// <summary>
        /// Convert from cubic millimeters to liters.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float fluidDrams)
        {
            return fluidDrams * Units.Liters.PER_FLUID_DRAM;
        }

        public static float Gallons(float fluidDrams)
        {
            return fluidDrams * Units.Gallons.PER_FLUID_DRAM;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic feet.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float fluidDrams)
        {
            return fluidDrams * Units.CubicFeet.PER_FLUID_DRAM;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic meters.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float fluidDrams)
        {
            return fluidDrams * Units.CubicMeters.PER_FLUID_DRAM;
        }

        /// <summary>
        /// Convert from cubic millimeters to kiloliters.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float fluidDrams)
        {
            return CubicMeters(fluidDrams);
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic kilometers.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float fluidDrams)
        {
            return fluidDrams * Units.CubicKilometers.PER_FLUID_DRAM;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic miles.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float fluidDrams)
        {
            return fluidDrams * Units.CubicMiles.PER_FLUID_DRAM;
        }
    }
}
