namespace Juniper.Units
{
    /// <summary>
    /// Conversions from US teaspoons
    /// </summary>
    public static class Teaspoons
    {
        public const double PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_TEASPOON;
        public const double PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_TEASPOON;
        public const double PER_MINIM = 1 / Units.Minims.PER_TEASPOON;
        public const double PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_TEASPOON;
        public const double PER_MILLILITER = 1 / Units.Milliliters.PER_TEASPOON;
        public const double PER_FLUID_DRAM = 1 / Units.FluidDrams.PER_TEASPOON;
        public const double PER_TEASPOON = 1;
        public const double PER_TABLESPOON = 3;
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

        public static double CubicMicrometers(double teaspoons)
        {
            return teaspoons * Units.CubicMicrometers.PER_TEASPOON;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic millimeters.
        /// </summary>
        /// <param name="teaspoons">The number of cubic micrometers</param>
        /// <returns>The number of cubic millimeters</returns>
        public static double CubicMillimeters(double teaspoons)
        {
            return teaspoons * Units.CubicMillimeters.PER_TEASPOON;
        }

        public static double Minims(double teaspoons)
        {
            return teaspoons * Units.Minims.PER_TEASPOON;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic centimeters.
        /// </summary>
        /// <param name="teaspoons">The number of cubic micrometers</param>
        /// <returns>The number of cubic centimeters</returns>
        public static double CubicCentimeters(double teaspoons)
        {
            return teaspoons * Units.CubicCentimeters.PER_TEASPOON;
        }

        /// <summary>
        /// Convert from cubic micrometers to milliliters.
        /// </summary>
        /// <param name="teaspoons">The number of cubic micrometers</param>
        /// <returns>The number of milliliters</returns>
        public static double Milliliters(double teaspoons)
        {
            return CubicCentimeters(teaspoons);
        }

        public static double FluidDrams(double teaspoons)
        {
            return teaspoons * Units.FluidDrams.PER_TEASPOON;
        }

        public static double Tablespoons(double teaspoons)
        {
            return teaspoons * Units.Tablespoons.PER_TEASPOON;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic inches.
        /// </summary>
        /// <param name="teaspoons">The number of cubic micrometers</param>
        /// <returns>The number of cubic inches</returns>
        public static double CubicInches(double teaspoons)
        {
            return teaspoons * Units.CubicInches.PER_TEASPOON;
        }

        public static double FluidOunces(double teaspoons)
        {
            return teaspoons * Units.FluidOunces.PER_TEASPOON;
        }

        public static double Gills(double teaspoons)
        {
            return teaspoons * Units.Gills.PER_TEASPOON;
        }

        public static double Cups(double teaspoons)
        {
            return teaspoons * Units.Cups.PER_TEASPOON;
        }

        public static double LiquidPints(double teaspoons)
        {
            return teaspoons * Units.LiquidPints.PER_TEASPOON;
        }

        public static double LiquidQuarts(double teaspoons)
        {
            return teaspoons * Units.LiquidQuarts.PER_TEASPOON;
        }

        /// <summary>
        /// Convert from cubic micrometers to liters.
        /// </summary>
        /// <param name="teaspoons">The number of cubic micrometers</param>
        /// <returns>The number of liters</returns>
        public static double Liters(double teaspoons)
        {
            return teaspoons * Units.Liters.PER_TEASPOON;
        }

        public static double Gallons(double teaspoons)
        {
            return teaspoons * Units.Gallons.PER_TEASPOON;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic feet.
        /// </summary>
        /// <param name="teaspoons">The number of cubic micrometers</param>
        /// <returns>The number of cubic feet</returns>
        public static double CubicFeet(double teaspoons)
        {
            return teaspoons * Units.CubicFeet.PER_TEASPOON;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic meters.
        /// </summary>
        /// <param name="teaspoons">The number of cubic micrometers</param>
        /// <returns>The number of cubic meters</returns>
        public static double CubicMeters(double teaspoons)
        {
            return teaspoons * Units.CubicMeters.PER_TEASPOON;
        }

        /// <summary>
        /// Convert from cubic micrometers to kiloliters.
        /// </summary>
        /// <param name="teaspoons">The number of cubic micrometers</param>
        /// <returns>The number of kiloliters</returns>
        public static double Kiloliters(double teaspoons)
        {
            return CubicMeters(teaspoons);
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic kilometers.
        /// </summary>
        /// <param name="teaspoons">The number of cubic micrometers</param>
        /// <returns>The number of cubic kilometers</returns>
        public static double CubicKilometers(double teaspoons)
        {
            return teaspoons * Units.CubicKilometers.PER_TEASPOON;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic miles.
        /// </summary>
        /// <param name="teaspoons">The number of cubic micrometers</param>
        /// <returns>The number of cubic miles</returns>
        public static double CubicMiles(double teaspoons)
        {
            return teaspoons * Units.CubicMiles.PER_TEASPOON;
        }
    }
}
