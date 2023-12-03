namespace Juniper.Units
{
    /// <summary>
    /// Conversions from US liquid pints
    /// </summary>
    public static class LiquidPints
    {
        public const double PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_LIQUID_PINT;
        public const double PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_LIQUID_PINT;
        public const double PER_MINIM = 1 / Units.Minims.PER_LIQUID_PINT;
        public const double PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_LIQUID_PINT;
        public const double PER_MILLILITER = 1 / Units.Milliliters.PER_LIQUID_PINT;
        public const double PER_FLUID_DRAM = 1 / Units.FluidDrams.PER_LIQUID_PINT;
        public const double PER_TEASPOON = 1 / Units.Teaspoons.PER_LIQUID_PINT;
        public const double PER_TABLESPOON = 1 / Units.Tablespoons.PER_LIQUID_PINT;
        public const double PER_CUBIC_INCH = 1 / Units.CubicInches.PER_LIQUID_PINT;
        public const double PER_FLUID_OUNCE = 1 / Units.FluidOunces.PER_LIQUID_PINT;
        public const double PER_GILL = 1 / Units.Gills.PER_LIQUID_PINT;
        public const double PER_CUP = 1 / Units.Cups.PER_LIQUID_PINT;
        public const double PER_LIQUID_PINT = 1;
        public const double PER_LIQUID_QUART = 2;
        public const double PER_LITER = PER_LIQUID_QUART * Units.LiquidQuarts.PER_LITER;
        public const double PER_GALLON = PER_LIQUID_QUART * Units.LiquidQuarts.PER_GALLON;
        public const double PER_CUBIC_FOOT = PER_GALLON * Units.Gallons.PER_CUBIC_FOOT;
        public const double PER_CUBIC_METER = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_METER;
        public const double PER_KILOLITER = PER_CUBIC_METER;
        public const double PER_CUBIC_KILOMETER = PER_CUBIC_METER * Units.CubicMeters.PER_CUBIC_KILOMETER;
        public const double PER_CUBIC_MILE = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_MILE;

        public static double CubicMicrometers(double liquidPints)
        {
            return liquidPints * Units.CubicMicrometers.PER_LIQUID_PINT;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic millimeters.
        /// </summary>
        /// <param name="liquidPints">The number of cubic micrometers</param>
        /// <returns>The number of cubic millimeters</returns>
        public static double CubicMillimeters(double liquidPints)
        {
            return liquidPints * Units.CubicMillimeters.PER_LIQUID_PINT;
        }

        public static double Minims(double liquidPints)
        {
            return liquidPints * Units.Minims.PER_LIQUID_PINT;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic centimeters.
        /// </summary>
        /// <param name="liquidPints">The number of cubic micrometers</param>
        /// <returns>The number of cubic centimeters</returns>
        public static double CubicCentimeters(double liquidPints)
        {
            return liquidPints * Units.CubicCentimeters.PER_LIQUID_PINT;
        }

        /// <summary>
        /// Convert from cubic micrometers to milliliters.
        /// </summary>
        /// <param name="liquidPints">The number of cubic micrometers</param>
        /// <returns>The number of milliliters</returns>
        public static double Milliliters(double liquidPints)
        {
            return CubicCentimeters(liquidPints);
        }

        public static double FluidDrams(double liquidPints)
        {
            return liquidPints * Units.FluidDrams.PER_LIQUID_PINT;
        }

        public static double Teaspoons(double liquidPints)
        {
            return liquidPints * Units.Teaspoons.PER_LIQUID_PINT;
        }

        public static double Tablespoons(double liquidPints)
        {
            return liquidPints * Units.Tablespoons.PER_LIQUID_PINT;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic inches.
        /// </summary>
        /// <param name="liquidPints">The number of cubic micrometers</param>
        /// <returns>The number of cubic inches</returns>
        public static double CubicInches(double liquidPints)
        {
            return liquidPints * Units.CubicInches.PER_LIQUID_PINT;
        }

        public static double FluidOunces(double liquidPints)
        {
            return liquidPints * Units.FluidOunces.PER_LIQUID_PINT;
        }

        public static double Gills(double liquidPints)
        {
            return liquidPints * Units.Gills.PER_LIQUID_PINT;
        }

        public static double Cups(double liquidPints)
        {
            return liquidPints * Units.Cups.PER_LIQUID_PINT;
        }

        public static double LiquidQuarts(double liquidPints)
        {
            return liquidPints * Units.LiquidQuarts.PER_LIQUID_PINT;
        }

        /// <summary>
        /// Convert from cubic micrometers to liters.
        /// </summary>
        /// <param name="liquidPints">The number of cubic micrometers</param>
        /// <returns>The number of liters</returns>
        public static double Liters(double liquidPints)
        {
            return liquidPints * Units.Liters.PER_LIQUID_PINT;
        }

        public static double Gallons(double liquidPints)
        {
            return liquidPints * Units.Gallons.PER_LIQUID_PINT;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic feet.
        /// </summary>
        /// <param name="liquidPints">The number of cubic micrometers</param>
        /// <returns>The number of cubic feet</returns>
        public static double CubicFeet(double liquidPints)
        {
            return liquidPints * Units.CubicFeet.PER_LIQUID_PINT;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic meters.
        /// </summary>
        /// <param name="liquidPints">The number of cubic micrometers</param>
        /// <returns>The number of cubic meters</returns>
        public static double CubicMeters(double liquidPints)
        {
            return liquidPints * Units.CubicMeters.PER_LIQUID_PINT;
        }

        /// <summary>
        /// Convert from cubic micrometers to kiloliters.
        /// </summary>
        /// <param name="liquidPints">The number of cubic micrometers</param>
        /// <returns>The number of kiloliters</returns>
        public static double Kiloliters(double liquidPints)
        {
            return CubicMeters(liquidPints);
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic kilometers.
        /// </summary>
        /// <param name="liquidPints">The number of cubic micrometers</param>
        /// <returns>The number of cubic kilometers</returns>
        public static double CubicKilometers(double liquidPints)
        {
            return liquidPints * Units.CubicKilometers.PER_LIQUID_PINT;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic miles.
        /// </summary>
        /// <param name="liquidPints">The number of cubic micrometers</param>
        /// <returns>The number of cubic miles</returns>
        public static double CubicMiles(double liquidPints)
        {
            return liquidPints * Units.CubicMiles.PER_LIQUID_PINT;
        }
    }
}
