namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic inches
    /// </summary>
    public static class CubicInches
    {
        public const double PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_CUBIC_INCH;
        public const double PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_CUBIC_INCH;
        public const double PER_MINIM = 1 / Units.Minims.PER_CUBIC_INCH;
        public const double PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_CUBIC_INCH;
        public const double PER_MILLILITER = 1 / Units.Milliliters.PER_CUBIC_INCH;
        public const double PER_FLUID_DRAM = 1 / Units.FluidDrams.PER_CUBIC_INCH;
        public const double PER_TEASPOON = 1 / Units.Teaspoons.PER_CUBIC_INCH;
        public const double PER_TABLESPOON = 1 / Units.Tablespoons.PER_CUBIC_INCH;
        public const double PER_CUBIC_INCH = 1;
        public const double PER_FLUID_OUNCE = PER_GILL * Units.Gills.PER_FLUID_OUNCE;
        public const double PER_GILL = PER_CUP * Units.Cups.PER_GILL;
        public const double PER_CUP = PER_LIQUID_PINT * Units.LiquidPints.PER_CUP;
        public const double PER_LIQUID_PINT = PER_LIQUID_QUART * Units.LiquidQuarts.PER_LIQUID_PINT;
        public const double PER_LIQUID_QUART = PER_GALLON * Units.Gallons.PER_LIQUID_QUART;
        public const double PER_LITER = PER_MILLILITER * Units.Milliliters.PER_LITER;
        public const double PER_GALLON = 231;
        public const double PER_CUBIC_FOOT = SquareInches.PER_SQUARE_FOOT * Inches.PER_FOOT;
        public const double PER_CUBIC_METER = SquareInches.PER_SQUARE_METER * Inches.PER_METER;
        public const double PER_KILOLITER = PER_CUBIC_METER;
        public const double PER_CUBIC_KILOMETER = SquareInches.PER_SQUARE_KILOMETER * Inches.PER_KILOMETER;
        public const double PER_CUBIC_MILE = SquareInches.PER_SQUARE_MILE * Inches.PER_MILE;


        /// <summary>
        /// Convert from cubic inches to cubic micrometers.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic micrometers</returns>
        public static double CubicMicrometers(double cubicInches)
        {
            return cubicInches * Units.CubicMicrometers.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to cubic millimeters.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic millimeters</returns>
        public static double CubicMillimeters(double cubicInches)
        {
            return cubicInches * Units.CubicMillimeters.PER_CUBIC_INCH;
        }

        public static double Minims(double cubicInches)
        {
            return cubicInches * Units.Minims.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to cubic centimeters.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic centimeters</returns>
        public static double CubicCentimeters(double cubicInches)
        {
            return cubicInches * Units.CubicCentimeters.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to milliliters.
        /// </summary>
        /// <param name="cubicIncehs">The number of cubic inches</param>
        /// <returns>The number of milliliters</returns>
        public static double Milliliters(double cubicIncehs)
        {
            return CubicCentimeters(cubicIncehs);
        }

        public static double FluidDrams(double cubicInches)
        {
            return cubicInches * Units.FluidDrams.PER_CUBIC_INCH;
        }

        public static double Teaspoons(double cubicInches)
        {
            return cubicInches * Units.Teaspoons.PER_CUBIC_INCH;
        }

        public static double Tablespoons(double cubicInches)
        {
            return cubicInches * Units.Tablespoons.PER_CUBIC_INCH;
        }

        public static double FluidOunces(double cubicInches)
        {
            return cubicInches * Units.FluidOunces.PER_CUBIC_INCH;
        }

        public static double Gills(double cubicInches)
        {
            return cubicInches * Units.Gills.PER_CUBIC_INCH;
        }

        public static double Cups(double cubicInches)
        {
            return cubicInches * Units.Cups.PER_CUBIC_INCH;
        }

        public static double LiquidPints(double cubicInches)
        {
            return cubicInches * Units.LiquidPints.PER_CUBIC_INCH;
        }

        public static double LiquidQuarts(double cubicInches)
        {
            return cubicInches * Units.LiquidQuarts.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to liters.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of liters</returns>
        public static double Liters(double cubicInches)
        {
            return cubicInches * Units.Liters.PER_CUBIC_INCH;
        }

        public static double Gallons(double cubicInches)
        {
            return cubicInches * Units.Gallons.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to cubic feet.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic feet</returns>
        public static double CubicFeet(double cubicInches)
        {
            return cubicInches * Units.CubicFeet.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to cubic meters.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic meters</returns>
        public static double CubicMeters(double cubicInches)
        {
            return cubicInches * Units.CubicMeters.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to kiloliters.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of kiloliters</returns>
        public static double Kiloliters(double cubicInches)
        {
            return CubicMeters(cubicInches);
        }

        /// <summary>
        /// Convert from cubic inches to cubic kilometers.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic kilometers</returns>
        public static double CubicKilometers(double cubicInches)
        {
            return cubicInches * Units.CubicKilometers.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to cubic miles.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic miles</returns>
        public static double CubicMiles(double cubicInches)
        {
            return cubicInches * Units.CubicMiles.PER_CUBIC_INCH;
        }
    }
}