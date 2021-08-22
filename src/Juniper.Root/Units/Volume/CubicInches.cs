namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic inches
    /// </summary>
    public static class CubicInches
    {
        public const float PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_CUBIC_INCH;
        public const float PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_CUBIC_INCH;
        public const float PER_MINIM = 1 / Units.Minims.PER_CUBIC_INCH;
        public const float PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_CUBIC_INCH;
        public const float PER_MILLILITER = 1 / Units.Milliliters.PER_CUBIC_INCH;
        public const float PER_FLUID_DRAM = 1 / Units.FluidDrams.PER_CUBIC_INCH;
        public const float PER_TEASPOON = 1 / Units.Teaspoons.PER_CUBIC_INCH;
        public const float PER_TABLESPOON = 1 / Units.Tablespoons.PER_CUBIC_INCH;
        public const float PER_CUBIC_INCH = 1;
        public const float PER_FLUID_OUNCE = PER_GILL * Units.Gills.PER_FLUID_OUNCE;
        public const float PER_GILL = PER_CUP * Units.Cups.PER_GILL;
        public const float PER_CUP = PER_LIQUID_PINT * Units.LiquidPints.PER_CUP;
        public const float PER_LIQUID_PINT = PER_LIQUID_QUART * Units.LiquidQuarts.PER_LIQUID_PINT;
        public const float PER_LIQUID_QUART = PER_GALLON * Units.Gallons.PER_LIQUID_QUART;
        public const float PER_LITER = PER_MILLILITER * Units.Milliliters.PER_LITER;
        public const float PER_GALLON = 231;
        public const float PER_CUBIC_FOOT = SquareInches.PER_SQUARE_FOOT * Inches.PER_FOOT;
        public const float PER_CUBIC_METER = SquareInches.PER_SQUARE_METER * Inches.PER_METER;
        public const float PER_KILOLITER = PER_CUBIC_METER;
        public const float PER_CUBIC_KILOMETER = SquareInches.PER_SQUARE_KILOMETER * Inches.PER_KILOMETER;
        public const float PER_CUBIC_MILE = SquareInches.PER_SQUARE_MILE * Inches.PER_MILE;


        /// <summary>
        /// Convert from cubic inches to cubic micrometers.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float cubicInches)
        {
            return cubicInches * Units.CubicMicrometers.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to cubic millimeters.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float cubicInches)
        {
            return cubicInches * Units.CubicMillimeters.PER_CUBIC_INCH;
        }

        public static float Minims(float cubicInches)
        {
            return cubicInches * Units.Minims.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to cubic centimeters.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float cubicInches)
        {
            return cubicInches * Units.CubicCentimeters.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to milliliters.
        /// </summary>
        /// <param name="cubicIncehs">The number of cubic inches</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float cubicIncehs)
        {
            return CubicCentimeters(cubicIncehs);
        }

        public static float FluidDrams(float cubicInches)
        {
            return cubicInches * Units.FluidDrams.PER_CUBIC_INCH;
        }

        public static float Teaspoons(float cubicInches)
        {
            return cubicInches * Units.Teaspoons.PER_CUBIC_INCH;
        }

        public static float Tablespoons(float cubicInches)
        {
            return cubicInches * Units.Tablespoons.PER_CUBIC_INCH;
        }

        public static float FluidOunces(float cubicInches)
        {
            return cubicInches * Units.FluidOunces.PER_CUBIC_INCH;
        }

        public static float Gills(float cubicInches)
        {
            return cubicInches * Units.Gills.PER_CUBIC_INCH;
        }

        public static float Cups(float cubicInches)
        {
            return cubicInches * Units.Cups.PER_CUBIC_INCH;
        }

        public static float LiquidPints(float cubicInches)
        {
            return cubicInches * Units.LiquidPints.PER_CUBIC_INCH;
        }

        public static float LiquidQuarts(float cubicInches)
        {
            return cubicInches * Units.LiquidQuarts.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to liters.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float cubicInches)
        {
            return cubicInches * Units.Liters.PER_CUBIC_INCH;
        }

        public static float Gallons(float cubicInches)
        {
            return cubicInches * Units.Gallons.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to cubic feet.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float cubicInches)
        {
            return cubicInches * Units.CubicFeet.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to cubic meters.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float cubicInches)
        {
            return cubicInches * Units.CubicMeters.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to kiloliters.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float cubicInches)
        {
            return CubicMeters(cubicInches);
        }

        /// <summary>
        /// Convert from cubic inches to cubic kilometers.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float cubicInches)
        {
            return cubicInches * Units.CubicKilometers.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to cubic miles.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float cubicInches)
        {
            return cubicInches * Units.CubicMiles.PER_CUBIC_INCH;
        }
    }
}