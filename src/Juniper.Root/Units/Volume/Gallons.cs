namespace Juniper.Units
{
    /// <summary>
    /// Conversions from US liquid gallons
    /// </summary>
    public static class Gallons
    {
        public const float PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_GALLON;
        public const float PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_GALLON;
        public const float PER_MINIM = 1 / Units.Minims.PER_GALLON;
        public const float PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_GALLON;
        public const float PER_MILLILITER = 1 / Units.Milliliters.PER_GALLON;
        public const float PER_FLUID_DRAM = 1 / Units.FluidDrams.PER_GALLON;
        public const float PER_TEASPOON = 1 / Units.Teaspoons.PER_GALLON;
        public const float PER_TABLESPOON = 1 / Units.Tablespoons.PER_GALLON;
        public const float PER_CUBIC_INCH = 1 / Units.CubicInches.PER_GALLON;
        public const float PER_FLUID_OUNCE = 1 / Units.FluidOunces.PER_GALLON;
        public const float PER_GILL = 1 / Units.Gills.PER_GALLON;
        public const float PER_CUP = 1 / Units.Cups.PER_GALLON;
        public const float PER_LIQUID_PINT = 1 / Units.LiquidPints.PER_GALLON;
        public const float PER_LIQUID_QUART = 1 / Units.LiquidQuarts.PER_GALLON;
        public const float PER_LITER = 1 / Units.Liters.PER_GALLON;
        public const float PER_GALLON = 1;
        public const float PER_CUBIC_FOOT = PER_CUBIC_INCH * Units.CubicInches.PER_CUBIC_FOOT;
        public const float PER_CUBIC_METER = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_METER;
        public const float PER_KILOLITER = PER_CUBIC_METER;
        public const float PER_CUBIC_KILOMETER = PER_CUBIC_METER * Units.CubicMeters.PER_CUBIC_KILOMETER;
        public const float PER_CUBIC_MILE = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_MILE;

        /// <summary>
        /// Convert from cubic millimeters to cubic micrometers.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float gallons)
        {
            return gallons * Units.CubicMicrometers.PER_GALLON;
        }

        public static float CubicMillimeters(float gallons)
        {
            return gallons * Units.CubicMillimeters.PER_GALLON;
        }

        public static float Minims(float gallons)
        {
            return gallons * Units.Minims.PER_GALLON;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic centimeters.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float gallons)
        {
            return gallons * Units.CubicCentimeters.PER_GALLON;
        }

        /// <summary>
        /// Convert from cubic millimeters to milliliters.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float gallons)
        {
            return CubicCentimeters(gallons);
        }

        public static float FluidDrams(float gallons)
        {
            return gallons * Units.FluidDrams.PER_GALLON;
        }

        public static float Teaspoons(float gallons)
        {
            return gallons * Units.Teaspoons.PER_GALLON;
        }

        public static float Tablespoons(float gallons)
        {
            return gallons * Units.Tablespoons.PER_GALLON;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic inches.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float gallons)
        {
            return gallons * Units.CubicInches.PER_GALLON;
        }

        public static float FluidOunces(float gallons)
        {
            return gallons * Units.FluidOunces.PER_GALLON;
        }

        public static float Gills(float gallons)
        {
            return gallons * Units.Gills.PER_GALLON;
        }

        public static float Cups(float gallons)
        {
            return gallons * Units.Cups.PER_GALLON;
        }

        public static float LiquidPints(float gallons)
        {
            return gallons * Units.LiquidPints.PER_GALLON;
        }

        public static float LiquidQuarts(float gallons)
        {
            return gallons * Units.LiquidQuarts.PER_GALLON;
        }

        /// <summary>
        /// Convert from cubic millimeters to liters.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float gallons)
        {
            return gallons * Units.Liters.PER_GALLON;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic feet.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float gallons)
        {
            return gallons * Units.CubicFeet.PER_GALLON;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic meters.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float gallons)
        {
            return gallons * Units.CubicMeters.PER_GALLON;
        }

        /// <summary>
        /// Convert from cubic millimeters to kiloliters.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float gallons)
        {
            return CubicMeters(gallons);
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic kilometers.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float gallons)
        {
            return gallons * Units.CubicKilometers.PER_GALLON;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic miles.
        /// </summary>
        /// <param name="gallons">The number of cubic millimeters</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float gallons)
        {
            return gallons * Units.CubicMiles.PER_GALLON;
        }
    }
}
