namespace Juniper.Units
{
    /// <summary>
    /// Conversions from milliliters
    /// </summary>
    public static class Milliliters
    {
        public const double PER_CUBIC_MICROMETER = Units.CubicCentimeters.PER_CUBIC_MICROMETER;
        public const double PER_CUBIC_MILLIMETER = Units.CubicCentimeters.PER_CUBIC_MILLIMETER;
        public const double PER_MINIM = 1 / Units.Minims.PER_MILLILITER;
        public const double PER_CUBIC_CENTIMETER = 1;
        public const double PER_MILLILITER = 1;
        public const double PER_FLUID_DRAM = PER_TEASPOON * Units.Teaspoons.PER_FLUID_DRAM;
        public const double PER_TEASPOON = PER_TABLESPOON * Units.Tablespoons.PER_TEASPOON;
        public const double PER_TABLESPOON = PER_FLUID_OUNCE * Units.FluidOunces.PER_TABLESPOON;
        public const double PER_CUBIC_INCH = Units.CubicCentimeters.PER_CUBIC_INCH;
        public const double PER_FLUID_OUNCE = PER_GILL * Units.Gills.PER_FLUID_OUNCE;
        public const double PER_GILL = PER_CUP * Units.Cups.PER_GILL;
        public const double PER_CUP = PER_LIQUID_PINT * Units.LiquidPints.PER_CUP;
        public const double PER_LIQUID_PINT = PER_LIQUID_QUART * Units.LiquidQuarts.PER_LIQUID_PINT;
        public const double PER_LIQUID_QUART = PER_GALLON * Units.Gallons.PER_LIQUID_QUART;
        public const double PER_LITER = 1000;
        public const double PER_GALLON = PER_LITER * Units.Liters.PER_GALLON;
        public const double PER_CUBIC_FOOT = Units.CubicCentimeters.PER_CUBIC_FOOT;
        public const double PER_CUBIC_METER = Units.CubicCentimeters.PER_CUBIC_METER;
        public const double PER_KILOLITER = PER_LITER * Units.Liters.PER_KILOLITER;
        public const double PER_CUBIC_KILOMETER = Units.CubicCentimeters.PER_CUBIC_KILOMETER;
        public const double PER_CUBIC_MILE = Units.CubicCentimeters.PER_CUBIC_MILE;

        /// <summary>
        /// Convert from cubic micrometers to milliliters.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of milliliters</returns>
        public static double CubicMicrometers(double milliliters)
        {
            return milliliters * Units.CubicMicrometers.PER_MILLILITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic millimeters.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic millimeters</returns>
        public static double CubicMillimeters(double milliliters)
        {
            return milliliters * Units.CubicMillimeters.PER_MILLILITER;
        }

        public static double Minims(double milliliters)
        {
            return milliliters * Units.Minims.PER_MILLILITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic centimeters.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic centimeters</returns>
        public static double CubicCentimeters(double milliliters)
        {
            return milliliters * Units.CubicCentimeters.PER_MILLILITER;
        }

        public static double FluidDrams(double milliliters)
        {
            return milliliters * Units.FluidDrams.PER_MILLILITER;
        }

        public static double Teaspoons(double milliliters)
        {
            return milliliters * Units.Teaspoons.PER_MILLILITER;
        }

        public static double Tablespoons(double milliliters)
        {
            return milliliters * Units.Tablespoons.PER_MILLILITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic inches.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic inches</returns>
        public static double CubicInches(double milliliters)
        {
            return milliliters * Units.CubicInches.PER_MILLILITER;
        }

        public static double FluidOunces(double milliliters)
        {
            return milliliters * Units.FluidOunces.PER_MILLILITER;
        }

        public static double Gills(double milliliters)
        {
            return milliliters * Units.Gills.PER_MILLILITER;
        }

        public static double Cups(double milliliters)
        {
            return milliliters * Units.Cups.PER_MILLILITER;
        }

        public static double LiquidPints(double milliliters)
        {
            return milliliters * Units.LiquidPints.PER_MILLILITER;
        }

        public static double LiquidQuarts(double milliliters)
        {
            return milliliters * Units.LiquidQuarts.PER_MILLILITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to liters.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of liters</returns>
        public static double Liters(double milliliters)
        {
            return milliliters * Units.Liters.PER_MILLILITER;
        }

        public static double Gallons(double milliliters)
        {
            return milliliters * Units.Gallons.PER_MILLILITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic feet.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic feet</returns>
        public static double CubicFeet(double milliliters)
        {
            return milliliters * Units.CubicFeet.PER_MILLILITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic meters.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic meters</returns>
        public static double CubicMeters(double milliliters)
        {
            return milliliters * Units.CubicMeters.PER_MILLILITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to kiloliters.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of kiloliters</returns>
        public static double Kiloliters(double milliliters)
        {
            return CubicMeters(milliliters);
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic kilometers.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic kilometers</returns>
        public static double CubicKilometers(double milliliters)
        {
            return milliliters * Units.CubicKilometers.PER_MILLILITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic miles.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic miles</returns>
        public static double CubicMiles(double milliliters)
        {
            return milliliters * Units.CubicMiles.PER_MILLILITER;
        }
    }
}