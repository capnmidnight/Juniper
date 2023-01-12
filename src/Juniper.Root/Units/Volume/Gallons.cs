namespace Juniper.Units
{
    /// <summary>
    /// Conversions from US liquid gallons
    /// </summary>
    public static class Gallons
    {
        public const double PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_GALLON;
        public const double PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_GALLON;
        public const double PER_MINIM = 1 / Units.Minims.PER_GALLON;
        public const double PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_GALLON;
        public const double PER_MILLILITER = 1 / Units.Milliliters.PER_GALLON;
        public const double PER_FLUID_DRAM = 1 / Units.FluidDrams.PER_GALLON;
        public const double PER_TEASPOON = 1 / Units.Teaspoons.PER_GALLON;
        public const double PER_TABLESPOON = 1 / Units.Tablespoons.PER_GALLON;
        public const double PER_CUBIC_INCH = 1 / Units.CubicInches.PER_GALLON;
        public const double PER_FLUID_OUNCE = 1 / Units.FluidOunces.PER_GALLON;
        public const double PER_GILL = 1 / Units.Gills.PER_GALLON;
        public const double PER_CUP = 1 / Units.Cups.PER_GALLON;
        public const double PER_LIQUID_PINT = 1 / Units.LiquidPints.PER_GALLON;
        public const double PER_LIQUID_QUART = 1 / Units.LiquidQuarts.PER_GALLON;
        public const double PER_LITER = 1 / Units.Liters.PER_GALLON;
        public const double PER_GALLON = 1;
        public const double PER_CUBIC_FOOT = PER_CUBIC_INCH * Units.CubicInches.PER_CUBIC_FOOT;
        public const double PER_CUBIC_METER = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_METER;
        public const double PER_KILOLITER = PER_CUBIC_METER;
        public const double PER_CUBIC_KILOMETER = PER_CUBIC_METER * Units.CubicMeters.PER_CUBIC_KILOMETER;
        public const double PER_CUBIC_MILE = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_MILE;

        /// <summary>
        /// Convert from cubic millimeters to cubic micrometers.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of cubic micrometers</returns>
        public static double CubicMicrometers(double gallons)
        {
            return gallons * Units.CubicMicrometers.PER_GALLON;
        }

        public static double CubicMillimeters(double gallons)
        {
            return gallons * Units.CubicMillimeters.PER_GALLON;
        }

        public static double Minims(double gallons)
        {
            return gallons * Units.Minims.PER_GALLON;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic centimeters.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of cubic centimeters</returns>
        public static double CubicCentimeters(double gallons)
        {
            return gallons * Units.CubicCentimeters.PER_GALLON;
        }

        /// <summary>
        /// Convert from cubic millimeters to milliliters.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of milliliters</returns>
        public static double Milliliters(double gallons)
        {
            return CubicCentimeters(gallons);
        }

        public static double FluidDrams(double gallons)
        {
            return gallons * Units.FluidDrams.PER_GALLON;
        }

        public static double Teaspoons(double gallons)
        {
            return gallons * Units.Teaspoons.PER_GALLON;
        }

        public static double Tablespoons(double gallons)
        {
            return gallons * Units.Tablespoons.PER_GALLON;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic inches.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of cubic inches</returns>
        public static double CubicInches(double gallons)
        {
            return gallons * Units.CubicInches.PER_GALLON;
        }

        public static double FluidOunces(double gallons)
        {
            return gallons * Units.FluidOunces.PER_GALLON;
        }

        public static double Gills(double gallons)
        {
            return gallons * Units.Gills.PER_GALLON;
        }

        public static double Cups(double gallons)
        {
            return gallons * Units.Cups.PER_GALLON;
        }

        public static double LiquidPints(double gallons)
        {
            return gallons * Units.LiquidPints.PER_GALLON;
        }

        public static double LiquidQuarts(double gallons)
        {
            return gallons * Units.LiquidQuarts.PER_GALLON;
        }

        /// <summary>
        /// Convert from cubic millimeters to liters.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of liters</returns>
        public static double Liters(double gallons)
        {
            return gallons * Units.Liters.PER_GALLON;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic feet.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of cubic feet</returns>
        public static double CubicFeet(double gallons)
        {
            return gallons * Units.CubicFeet.PER_GALLON;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic meters.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of cubic meters</returns>
        public static double CubicMeters(double gallons)
        {
            return gallons * Units.CubicMeters.PER_GALLON;
        }

        /// <summary>
        /// Convert from cubic millimeters to kiloliters.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of kiloliters</returns>
        public static double Kiloliters(double gallons)
        {
            return CubicMeters(gallons);
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic kilometers.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of cubic kilometers</returns>
        public static double CubicKilometers(double gallons)
        {
            return gallons * Units.CubicKilometers.PER_GALLON;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic miles.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of cubic miles</returns>
        public static double CubicMiles(double gallons)
        {
            return gallons * Units.CubicMiles.PER_GALLON;
        }
    }
}
