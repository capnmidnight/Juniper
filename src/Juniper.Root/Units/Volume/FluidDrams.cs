namespace Juniper.Units
{
    /// <summary>
    /// Conversions from US fluid drams
    /// </summary>
    public static class FluidDrams
    {
        public const double PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_FLUID_DRAM;
        public const double PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_FLUID_DRAM;
        public const double PER_MINIM = 1 / Units.Minims.PER_FLUID_DRAM;
        public const double PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_FLUID_DRAM;
        public const double PER_MILLILITER = 1 / Units.Milliliters.PER_FLUID_DRAM;
        public const double PER_FLUID_DRAM = 1;
        public const double PER_TEASPOON = PER_MINIM * Units.Minims.PER_TEASPOON;
        public const double PER_TABLESPOON = 4;
        public const double PER_CUBIC_INCH = PER_TABLESPOON * Units.Tablespoons.PER_CUBIC_INCH;
        public const double PER_FLUID_OUNCE = PER_TABLESPOON * Units.Tablespoons.PER_FLUID_OUNCE;
        public const double PER_GILL = PER_FLUID_OUNCE * Units.FluidOunces.PER_GILL;
        public const double PER_CUP = PER_GILL * Units.Gills.PER_CUP;
        public const double PER_LIQUID_PINT = PER_CUP * Units.Cups.PER_LIQUID_PINT;
        public const double PER_LIQUID_QUART = PER_LIQUID_PINT * Units.LiquidPints.PER_LIQUID_QUART;
        public const double PER_LITER = PER_LIQUID_QUART * Units.LiquidQuarts.PER_LITER;
        public const double PER_GALLON = PER_LIQUID_QUART * Units.LiquidQuarts.PER_GALLON;
        public const double PER_CUBIC_FOOT = PER_GALLON * Units.Gallons.PER_CUBIC_FOOT;
        public const double PER_CUBIC_METER = PER_LITER * Units.Liters.PER_CUBIC_METER;
        public const double PER_KILOLITER = PER_CUBIC_METER;
        public const double PER_CUBIC_KILOMETER = PER_CUBIC_METER * Units.CubicMeters.PER_CUBIC_KILOMETER;
        public const double PER_CUBIC_MILE = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_MILE;

        /// <summary>
        /// Convert from cubic millimeters to cubic micrometers.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of cubic micrometers</returns>
        public static double CubicMicrometers(double fluidDrams)
        {
            return fluidDrams * Units.CubicMicrometers.PER_FLUID_DRAM;
        }

        public static double CubicMillimeters(double fluidDrams)
        {
            return fluidDrams * Units.CubicMillimeters.PER_FLUID_DRAM;
        }

        public static double Minims(double fluidDrams)
        {
            return fluidDrams * Units.Minims.PER_FLUID_DRAM;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic centimeters.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of cubic centimeters</returns>
        public static double CubicCentimeters(double fluidDrams)
        {
            return fluidDrams * Units.CubicCentimeters.PER_FLUID_DRAM;
        }

        /// <summary>
        /// Convert from cubic millimeters to milliliters.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of milliliters</returns>
        public static double Milliliters(double fluidDrams)
        {
            return CubicCentimeters(fluidDrams);
        }

        public static double Teaspoons(double fluidDrams)
        {
            return fluidDrams * Units.Teaspoons.PER_FLUID_DRAM;
        }

        public static double Tablespoons(double fluidDrams)
        {
            return fluidDrams * Units.Tablespoons.PER_FLUID_DRAM;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic inches.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of cubic inches</returns>
        public static double CubicInches(double fluidDrams)
        {
            return fluidDrams * Units.CubicInches.PER_FLUID_DRAM;
        }

        public static double FluidOunces(double fluidDrams)
        {
            return fluidDrams * Units.FluidOunces.PER_FLUID_DRAM;
        }

        public static double Gills(double fluidDrams)
        {
            return fluidDrams * Units.Gills.PER_FLUID_DRAM;
        }

        public static double Cups(double fluidDrams)
        {
            return fluidDrams * Units.Cups.PER_FLUID_DRAM;
        }

        public static double LiquidPints(double fluidDrams)
        {
            return fluidDrams * Units.LiquidPints.PER_FLUID_DRAM;
        }

        public static double LiquidQuarts(double fluidDrams)
        {
            return fluidDrams * Units.LiquidQuarts.PER_FLUID_DRAM;
        }

        /// <summary>
        /// Convert from cubic millimeters to liters.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of liters</returns>
        public static double Liters(double fluidDrams)
        {
            return fluidDrams * Units.Liters.PER_FLUID_DRAM;
        }

        public static double Gallons(double fluidDrams)
        {
            return fluidDrams * Units.Gallons.PER_FLUID_DRAM;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic feet.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of cubic feet</returns>
        public static double CubicFeet(double fluidDrams)
        {
            return fluidDrams * Units.CubicFeet.PER_FLUID_DRAM;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic meters.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of cubic meters</returns>
        public static double CubicMeters(double fluidDrams)
        {
            return fluidDrams * Units.CubicMeters.PER_FLUID_DRAM;
        }

        /// <summary>
        /// Convert from cubic millimeters to kiloliters.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of kiloliters</returns>
        public static double Kiloliters(double fluidDrams)
        {
            return CubicMeters(fluidDrams);
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic kilometers.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of cubic kilometers</returns>
        public static double CubicKilometers(double fluidDrams)
        {
            return fluidDrams * Units.CubicKilometers.PER_FLUID_DRAM;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic miles.
        /// </summary>
        /// <param name="fluidDrams">The number of cubic millimeters</param>
        /// <returns>The number of cubic miles</returns>
        public static double CubicMiles(double fluidDrams)
        {
            return fluidDrams * Units.CubicMiles.PER_FLUID_DRAM;
        }
    }
}
