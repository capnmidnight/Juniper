namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic millimeters
    /// </summary>
    public static class CubicMillimeters
    {
        public const double PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_CUBIC_MILLIMETER;
        public const double PER_CUBIC_MILLIMETER = 1;
        public const double PER_MINIM = PER_MILLILITER / Units.Minims.PER_MILLILITER;
        public const double PER_CUBIC_CENTIMETER = SquareMillimeters.PER_SQUARE_CENTIMETER * Millimeters.PER_CENTIMETER;
        public const double PER_MILLILITER = PER_CUBIC_CENTIMETER;
        public const double PER_FLUID_DRAM = PER_MILLILITER * Units.Milliliters.PER_FLUID_DRAM;
        public const double PER_TEASPOON = PER_FLUID_DRAM * Units.FluidDrams.PER_TEASPOON;
        public const double PER_TABLESPOON = PER_FLUID_DRAM * Units.FluidDrams.PER_TABLESPOON;
        public const double PER_CUBIC_INCH = SquareMillimeters.PER_SQUARE_INCH * Millimeters.PER_INCH;
        public const double PER_FLUID_OUNCE = PER_TABLESPOON * Units.Tablespoons.PER_FLUID_OUNCE;
        public const double PER_GILL = PER_FLUID_OUNCE * Units.FluidOunces.PER_GILL;
        public const double PER_CUP = PER_GILL * Units.Gills.PER_CUP;
        public const double PER_LIQUID_PINT = PER_CUP * Units.Cups.PER_LIQUID_PINT;
        public const double PER_LIQUID_QUART = PER_LIQUID_PINT * Units.Cups.PER_LIQUID_QUART;
        public const double PER_LITER = PER_MILLILITER * Units.Milliliters.PER_LITER;
        public const double PER_GALLON = PER_LIQUID_QUART * Units.LiquidQuarts.PER_GALLON;
        public const double PER_CUBIC_FOOT = SquareMillimeters.PER_SQUARE_FOOT * Millimeters.PER_FOOT;
        public const double PER_CUBIC_METER = SquareMillimeters.PER_SQUARE_METER * Millimeters.PER_METER;
        public const double PER_KILOLITER = PER_LITER * Units.Liters.PER_KILOLITER;
        public const double PER_CUBIC_KILOMETER = SquareMillimeters.PER_SQUARE_KILOMETER * Millimeters.PER_KILOMETER;
        public const double PER_CUBIC_MILE = SquareMillimeters.PER_SQUARE_MILE * Millimeters.PER_MILE;

        /// <summary>
        /// Convert from cubic millimeters to cubic micrometers.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic micrometers</returns>
        public static double CubicMicrometers(double cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicMicrometers.PER_CUBIC_MILLIMETER;
        }

        public static double Minims(double cubicMillimeters)
        {
            return cubicMillimeters * Units.Minims.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic centimeters.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic centimeters</returns>
        public static double CubicCentimeters(double cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicCentimeters.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to milliliters.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of milliliters</returns>
        public static double Milliliters(double cubicMillimeters)
        {
            return CubicCentimeters(cubicMillimeters);
        }

        public static double FluidDrams(double cubicMillimeters)
        {
            return cubicMillimeters * Units.FluidDrams.PER_CUBIC_MILLIMETER;
        }

        public static double Teaspoons(double cubicMillimeters)
        {
            return cubicMillimeters * Units.Teaspoons.PER_CUBIC_MILLIMETER;
        }

        public static double Tablespoons(double cubicMillimeters)
        {
            return cubicMillimeters * Units.Tablespoons.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic inches.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic inches</returns>
        public static double CubicInches(double cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicInches.PER_CUBIC_MILLIMETER;
        }

        public static double FluidOunces(double cubicMillimeters)
        {
            return cubicMillimeters * Units.FluidOunces.PER_CUBIC_MILLIMETER;
        }

        public static double Gills(double cubicMillimeters)
        {
            return cubicMillimeters * Units.Gills.PER_CUBIC_MILLIMETER;
        }

        public static double Cups(double cubicMillimeters)
        {
            return cubicMillimeters * Units.Cups.PER_CUBIC_MILLIMETER;
        }

        public static double LiquidPints(double cubicMillimeters)
        {
            return cubicMillimeters * Units.LiquidPints.PER_CUBIC_MILLIMETER;
        }

        public static double LiquidQuarts(double cubicMillimeters)
        {
            return cubicMillimeters * Units.LiquidQuarts.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to liters.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of liters</returns>
        public static double Liters(double cubicMillimeters)
        {
            return cubicMillimeters * Units.Liters.PER_CUBIC_MILLIMETER;
        }

        public static double Gallons(double cubicMillimeters)
        {
            return cubicMillimeters * Units.Gallons.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic feet.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic feet</returns>
        public static double CubicFeet(double cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicFeet.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic meters.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic meters</returns>
        public static double CubicMeters(double cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicMeters.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to kiloliters.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of kiloliters</returns>
        public static double Kiloliters(double cubicMillimeters)
        {
            return CubicMeters(cubicMillimeters);
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic kilometers.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic kilometers</returns>
        public static double CubicKilometers(double cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicKilometers.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic miles.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic miles</returns>
        public static double CubicMiles(double cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicMiles.PER_CUBIC_MILLIMETER;
        }
    }
}