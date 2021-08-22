namespace Juniper.Units
{
    /// <summary>
    /// Conversions from liquid quarts
    /// </summary>
    public static class LiquidQuarts
    {
        public const float PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_LIQUID_QUART;
        public const float PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_LIQUID_QUART;
        public const float PER_MINIM = 1 / Units.Minims.PER_LIQUID_QUART;
        public const float PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_LIQUID_QUART;
        public const float PER_MILLILITER = 1 / Units.Milliliters.PER_LIQUID_QUART;
        public const float PER_FLUID_DRAM = 1 / Units.FluidDrams.PER_LIQUID_QUART;
        public const float PER_TEASPOON = 1 / Units.Teaspoons.PER_LIQUID_QUART;
        public const float PER_TABLESPOON = 1 / Units.Tablespoons.PER_LIQUID_QUART;
        public const float PER_CUBIC_INCH = 1 / Units.CubicInches.PER_LIQUID_QUART;
        public const float PER_FLUID_OUNCE = 1 / Units.FluidOunces.PER_LIQUID_QUART;
        public const float PER_GILL = 1 / Units.Gills.PER_LIQUID_QUART;
        public const float PER_CUP = 1 / Units.Cups.PER_LIQUID_QUART;
        public const float PER_LIQUID_PINT = 1 / Units.LiquidPints.PER_LIQUID_QUART;
        public const float PER_LIQUID_QUART = 1;
        public const float PER_LITER = 1.05668821f;
        public const float PER_GALLON = 4;
        public const float PER_CUBIC_FOOT = PER_GALLON * Units.Gallons.PER_CUBIC_FOOT;
        public const float PER_CUBIC_METER = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_METER;
        public const float PER_KILOLITER = PER_CUBIC_METER;
        public const float PER_CUBIC_KILOMETER = PER_CUBIC_METER * Units.CubicMeters.PER_CUBIC_KILOMETER;
        public const float PER_CUBIC_MILE = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_MILE;

        public static float CubicMicrometers(float liquidQuarts)
        {
            return liquidQuarts * Units.CubicMicrometers.PER_LIQUID_QUART;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic millimeters.
        /// </summary>
        /// <param name="liquidQuarts">The number of cubic micrometers</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float liquidQuarts)
        {
            return liquidQuarts * Units.CubicMillimeters.PER_LIQUID_QUART;
        }

        public static float Minims(float liquidQuarts)
        {
            return liquidQuarts * Units.Minims.PER_LIQUID_QUART;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic centimeters.
        /// </summary>
        /// <param name="liquidQuarts">The number of cubic micrometers</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float liquidQuarts)
        {
            return liquidQuarts * Units.CubicCentimeters.PER_LIQUID_QUART;
        }

        /// <summary>
        /// Convert from cubic micrometers to milliliters.
        /// </summary>
        /// <param name="liquidQuarts">The number of cubic micrometers</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float liquidQuarts)
        {
            return CubicCentimeters(liquidQuarts);
        }

        public static float FluidDrams(float liquidQuarts)
        {
            return liquidQuarts * Units.FluidDrams.PER_LIQUID_QUART;
        }

        public static float Teaspoons(float liquidQuarts)
        {
            return liquidQuarts * Units.Teaspoons.PER_LIQUID_QUART;
        }

        public static float Tablespoons(float liquidQuarts)
        {
            return liquidQuarts * Units.Tablespoons.PER_LIQUID_QUART;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic inches.
        /// </summary>
        /// <param name="liquidQuarts">The number of cubic micrometers</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float liquidQuarts)
        {
            return liquidQuarts * Units.CubicInches.PER_LIQUID_QUART;
        }

        public static float FluidOunces(float liquidQuarts)
        {
            return liquidQuarts * Units.FluidOunces.PER_LIQUID_QUART;
        }

        public static float Gills(float liquidQuarts)
        {
            return liquidQuarts * Units.Gills.PER_LIQUID_QUART;
        }

        public static float Cups(float liquidQuarts)
        {
            return liquidQuarts * Units.Cups.PER_LIQUID_QUART;
        }

        public static float LiquidPints(float liquidQuarts)
        {
            return liquidQuarts * Units.LiquidPints.PER_LIQUID_QUART;
        }

        /// <summary>
        /// Convert from cubic micrometers to liters.
        /// </summary>
        /// <param name="liquidQuarts">The number of cubic micrometers</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float liquidQuarts)
        {
            return liquidQuarts * Units.Liters.PER_LIQUID_QUART;
        }

        public static float Gallons(float liquidQuarts)
        {
            return liquidQuarts * Units.Gallons.PER_LIQUID_QUART;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic feet.
        /// </summary>
        /// <param name="liquidQuarts">The number of cubic micrometers</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float liquidQuarts)
        {
            return liquidQuarts * Units.CubicFeet.PER_LIQUID_QUART;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic meters.
        /// </summary>
        /// <param name="liquidQuarts">The number of cubic micrometers</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float liquidQuarts)
        {
            return liquidQuarts * Units.CubicMeters.PER_LIQUID_QUART;
        }

        /// <summary>
        /// Convert from cubic micrometers to kiloliters.
        /// </summary>
        /// <param name="liquidQuarts">The number of cubic micrometers</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float liquidQuarts)
        {
            return CubicMeters(liquidQuarts);
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic kilometers.
        /// </summary>
        /// <param name="liquidQuarts">The number of cubic micrometers</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float liquidQuarts)
        {
            return liquidQuarts * Units.CubicKilometers.PER_LIQUID_QUART;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic miles.
        /// </summary>
        /// <param name="liquidQuarts">The number of cubic micrometers</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float liquidQuarts)
        {
            return liquidQuarts * Units.CubicMiles.PER_LIQUID_QUART;
        }
    }
}
