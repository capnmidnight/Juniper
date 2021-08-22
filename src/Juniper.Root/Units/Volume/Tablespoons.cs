namespace Juniper.Units
{
    /// <summary>
    /// Conversions from US tablespoons
    /// </summary>
    public static class Tablespoons
    {
        public const float PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_TABLESPOON;
        public const float PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_TABLESPOON;
        public const float PER_MINIM = 1 / Units.Minims.PER_TABLESPOON;
        public const float PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_TABLESPOON;
        public const float PER_MILLILITER = 1 / Units.Milliliters.PER_TABLESPOON;
        public const float PER_FLUID_DRAM = 1 / Units.FluidDrams.PER_TABLESPOON;
        public const float PER_TEASPOON = 1 / Units.Teaspoons.PER_TABLESPOON;
        public const float PER_TABLESPOON = 1;
        public const float PER_CUBIC_INCH = PER_GALLON / Units.CubicInches.PER_GALLON;
        public const float PER_FLUID_OUNCE = 2;
        public const float PER_GILL = PER_FLUID_OUNCE * Units.FluidOunces.PER_GILL;
        public const float PER_CUP = PER_GILL * Units.Gills.PER_CUP;
        public const float PER_LIQUID_PINT = PER_CUP * Units.Cups.PER_LIQUID_PINT;
        public const float PER_LIQUID_QUART = PER_LIQUID_PINT * Units.LiquidPints.PER_LIQUID_QUART;
        public const float PER_LITER = PER_LIQUID_QUART * Units.LiquidQuarts.PER_LITER;
        public const float PER_GALLON = PER_LIQUID_QUART * Units.LiquidQuarts.PER_GALLON;
        public const float PER_CUBIC_FOOT = PER_GALLON * Units.Gallons.PER_CUBIC_FOOT;
        public const float PER_CUBIC_METER = PER_LITER * Units.Liters.PER_CUBIC_METER;
        public const float PER_KILOLITER = PER_CUBIC_METER;
        public const float PER_CUBIC_KILOMETER = PER_CUBIC_METER * Units.CubicMeters.PER_CUBIC_KILOMETER;
        public const float PER_CUBIC_MILE = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_MILE;

        public static float CubicMicrometers(float tablespoons)
        {
            return tablespoons * Units.CubicMicrometers.PER_TABLESPOON;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic millimeters.
        /// </summary>
        /// <param name="tablespoons">The number of cubic micrometers</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float tablespoons)
        {
            return tablespoons * Units.CubicMillimeters.PER_TABLESPOON;
        }

        public static float Minims(float tablespoons)
        {
            return tablespoons * Units.Minims.PER_TABLESPOON;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic centimeters.
        /// </summary>
        /// <param name="tablespoons">The number of cubic micrometers</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float tablespoons)
        {
            return tablespoons * Units.CubicCentimeters.PER_TABLESPOON;
        }

        /// <summary>
        /// Convert from cubic micrometers to milliliters.
        /// </summary>
        /// <param name="tablespoons">The number of cubic micrometers</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float tablespoons)
        {
            return CubicCentimeters(tablespoons);
        }

        public static float FluidDrams(float tablespoons)
        {
            return tablespoons * Units.FluidDrams.PER_TABLESPOON;
        }

        public static float Teaspoons(float tablespoons)
        {
            return tablespoons * Units.Teaspoons.PER_TABLESPOON;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic inches.
        /// </summary>
        /// <param name="tablespoons">The number of cubic micrometers</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float tablespoons)
        {
            return tablespoons * Units.CubicInches.PER_TABLESPOON;
        }

        public static float FluidOunces(float tablespoons)
        {
            return tablespoons * Units.FluidOunces.PER_TABLESPOON;
        }

        public static float Gills(float tablespoons)
        {
            return tablespoons * Units.Gills.PER_TABLESPOON;
        }

        public static float Cups(float tablespoons)
        {
            return tablespoons * Units.Cups.PER_TABLESPOON;
        }

        public static float LiquidPints(float tablespoons)
        {
            return tablespoons * Units.LiquidPints.PER_TABLESPOON;
        }

        public static float LiquidQuarts(float tablespoons)
        {
            return tablespoons * Units.LiquidQuarts.PER_TABLESPOON;
        }

        /// <summary>
        /// Convert from cubic micrometers to liters.
        /// </summary>
        /// <param name="tablespoons">The number of cubic micrometers</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float tablespoons)
        {
            return tablespoons * Units.Liters.PER_TABLESPOON;
        }

        public static float Gallons(float tablespoons)
        {
            return tablespoons * Units.Gallons.PER_TABLESPOON;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic feet.
        /// </summary>
        /// <param name="tablespoons">The number of cubic micrometers</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float tablespoons)
        {
            return tablespoons * Units.CubicFeet.PER_TABLESPOON;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic meters.
        /// </summary>
        /// <param name="tablespoons">The number of cubic micrometers</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float tablespoons)
        {
            return tablespoons * Units.CubicMeters.PER_TABLESPOON;
        }

        /// <summary>
        /// Convert from cubic micrometers to kiloliters.
        /// </summary>
        /// <param name="tablespoons">The number of cubic micrometers</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float tablespoons)
        {
            return CubicMeters(tablespoons);
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic kilometers.
        /// </summary>
        /// <param name="tablespoons">The number of cubic micrometers</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float tablespoons)
        {
            return tablespoons * Units.CubicKilometers.PER_TABLESPOON;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic miles.
        /// </summary>
        /// <param name="tablespoons">The number of cubic micrometers</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float tablespoons)
        {
            return tablespoons * Units.CubicMiles.PER_TABLESPOON;
        }
    }
}
