namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic feet
    /// </summary>
    public static class CubicFeet
    {
        public const float PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_CUBIC_FOOT;
        public const float PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_CUBIC_FOOT;
        public const float PER_MINIM = 1 / Units.Minims.PER_CUBIC_FOOT;
        public const float PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_CUBIC_FOOT;
        public const float PER_MILLILITER = 1 / Units.Milliliters.PER_CUBIC_FOOT;
        public const float PER_FLUID_DRAM = 1 / Units.FluidDrams.PER_CUBIC_FOOT;
        public const float PER_TEASPOON = 1 / Units.Teaspoons.PER_CUBIC_FOOT;
        public const float PER_TABLESPOON = 1 / Units.Tablespoons.PER_CUBIC_FOOT;
        public const float PER_CUBIC_INCH = 1 / Units.CubicInches.PER_CUBIC_FOOT;
        public const float PER_FLUID_OUNCE = 1 / Units.FluidOunces.PER_CUBIC_FOOT;
        public const float PER_GILL = 1 / Units.Gills.PER_CUBIC_FOOT;
        public const float PER_CUP = 1 / Units.Cups.PER_CUBIC_FOOT;
        public const float PER_LIQUID_PINT = 1 / Units.LiquidPints.PER_CUBIC_FOOT;
        public const float PER_LIQUID_QUART = 1 / Units.LiquidQuarts.PER_CUBIC_FOOT;
        public const float PER_LITER = 1 / Units.Liters.PER_CUBIC_FOOT;
        public const float PER_GALLON = 1 / Units.Gallons.PER_CUBIC_FOOT;
        public const float PER_CUBIC_FOOT = 1;
        public const float PER_CUBIC_METER = SquareFeet.PER_SQUARE_METER * Feet.PER_METER;
        public const float PER_KILOLITER = PER_CUBIC_METER;
        public const float PER_CUBIC_KILOMETER = SquareFeet.PER_SQUARE_KILOMETER * Feet.PER_KILOMETER;
        public const float PER_CUBIC_MILE = SquareFeet.PER_SQUARE_MILE * Feet.PER_MILE;


        /// <summary>
        /// Convert from cubic feet to cubic micrometers.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float cubicFeet)
        {
            return cubicFeet * Units.CubicMicrometers.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to cubic millimeters.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float cubicFeet)
        {
            return cubicFeet * Units.CubicMillimeters.PER_CUBIC_FOOT;
        }

        public static float Minims(float cubicFeet)
        {
            return cubicFeet * Units.Minims.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to cubic centimeters.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float cubicFeet)
        {
            return cubicFeet * Units.CubicCentimeters.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to milliliters.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float cubicFeet)
        {
            return CubicCentimeters(cubicFeet);
        }

        public static float FluidDrams(float cubicFeet)
        {
            return cubicFeet * Units.FluidDrams.PER_CUBIC_FOOT;
        }

        public static float Teaspoons(float cubicFeet)
        {
            return cubicFeet * Units.Teaspoons.PER_CUBIC_FOOT;
        }

        public static float Tablespoons(float cubicFeet)
        {
            return cubicFeet * Units.Tablespoons.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to cubic inches.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float cubicFeet)
        {
            return cubicFeet * Units.CubicInches.PER_CUBIC_FOOT;
        }

        public static float FluidOunces(float cubicFeet)
        {
            return cubicFeet * Units.FluidOunces.PER_CUBIC_FOOT;
        }

        public static float Gills(float cubicFeet)
        {
            return cubicFeet * Units.Gills.PER_CUBIC_FOOT;
        }

        public static float Cups(float cubicFeet)
        {
            return cubicFeet * Units.Cups.PER_CUBIC_FOOT;
        }

        public static float LiquidPints(float cubicFeet)
        {
            return cubicFeet * Units.LiquidPints.PER_CUBIC_FOOT;
        }

        public static float LiquidQuarts(float cubicFeet)
        {
            return cubicFeet * Units.LiquidQuarts.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to liters.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float cubicFeet)
        {
            return cubicFeet * Units.Liters.PER_CUBIC_FOOT;
        }

        public static float Gallons(float cubicFeet)
        {
            return cubicFeet * Units.Gallons.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to cubic meters.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float cubicFeet)
        {
            return cubicFeet * Units.CubicMeters.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to kiloliters.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float cubicFeet)
        {
            return CubicMeters(cubicFeet);
        }

        /// <summary>
        /// Convert from cubic feet to cubic kilometers.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float cubicFeet)
        {
            return cubicFeet * Units.CubicKilometers.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to cubic miles.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float cubicFeet)
        {
            return cubicFeet * Units.CubicMiles.PER_CUBIC_FOOT;
        }
    }
}