namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic miles
    /// </summary>
    public static class CubicMiles
    {
        public const float PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_CUBIC_MILE;
        public const float PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_CUBIC_MILE;
        public const float PER_MINIM = 1 / Units.Minims.PER_CUBIC_MILE;
        public const float PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_CUBIC_MILE;
        public const float PER_MILLILITER = 1 / Units.Milliliters.PER_CUBIC_MILE;
        public const float PER_FLUID_DRAM = 1 / Units.FluidDrams.PER_CUBIC_MILE;
        public const float PER_TEASPOON = 1 / Units.Teaspoons.PER_CUBIC_MILE;
        public const float PER_TABLESPOON = 1 / Units.Tablespoons.PER_CUBIC_MILE;
        public const float PER_CUBIC_INCH = 1 / Units.CubicInches.PER_CUBIC_MILE;
        public const float PER_FLUID_OUNCE = 1 / Units.FluidOunces.PER_CUBIC_MILE;
        public const float PER_GILL = 1 / Units.Gills.PER_CUBIC_MILE;
        public const float PER_CUP = 1 / Units.Cups.PER_CUBIC_MILE;
        public const float PER_LIQUID_PINT = 1 / Units.LiquidPints.PER_CUBIC_MILE;
        public const float PER_LIQUID_QUART = 1 / Units.LiquidQuarts.PER_CUBIC_MILE;
        public const float PER_LITER = 1 / Units.Liters.PER_CUBIC_MILE;
        public const float PER_GALLON = 1 / Units.Gallons.PER_CUBIC_MILE;
        public const float PER_CUBIC_FOOT = 1 / Units.CubicFeet.PER_CUBIC_MILE;
        public const float PER_CUBIC_METER = 1 / Units.CubicMeters.PER_CUBIC_MILE;
        public const float PER_KILOLITER = 1 / Units.Kiloliters.PER_CUBIC_MILE;
        public const float PER_CUBIC_KILOMETER = 1 / Units.CubicKilometers.PER_CUBIC_MILE;
        public const float PER_CUBIC_MILE = 1;

        /// <summary>
        /// Convert from cubic miles to cubic micrometers.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float cubicMiles)
        {
            return cubicMiles * Units.CubicMicrometers.PER_CUBIC_MILE;
        }

        /// <summary>
        /// Convert from cubic miles to cubic millimeters.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float cubicMiles)
        {
            return cubicMiles * Units.CubicMillimeters.PER_CUBIC_MILE;
        }

        public static float Minims(float cubicMiles)
        {
            return cubicMiles * Units.Minims.PER_CUBIC_MILE;
        }

        /// <summary>
        /// Convert from cubic miles to cubic centimeters.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float cubicMiles)
        {
            return cubicMiles * Units.CubicCentimeters.PER_CUBIC_MILE;
        }

        /// <summary>
        /// Convert from cubic miles to milliliters.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float cubicMiles)
        {
            return CubicCentimeters(cubicMiles);
        }

        public static float FluidDrams(float cubicMiles)
        {
            return cubicMiles * Units.FluidDrams.PER_CUBIC_MILE;
        }

        public static float Teaspoons(float cubicMiles)
        {
            return cubicMiles * Units.Teaspoons.PER_CUBIC_MILE;
        }

        public static float Tablespoons(float cubicMiles)
        {
            return cubicMiles * Units.Tablespoons.PER_CUBIC_MILE;
        }

        /// <summary>
        /// Convert from cubic miles to cubic inches.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float cubicMiles)
        {
            return cubicMiles * Units.CubicInches.PER_CUBIC_MILE;
        }

        public static float FluidOunces(float cubicMiles)
        {
            return cubicMiles * Units.FluidOunces.PER_CUBIC_MILE;
        }

        public static float Gills(float cubicMiles)
        {
            return cubicMiles * Units.Gills.PER_CUBIC_MILE;
        }

        public static float Cups(float cubicMiles)
        {
            return cubicMiles * Units.Cups.PER_CUBIC_MILE;
        }

        public static float LiquidPints(float cubicMiles)
        {
            return cubicMiles * Units.LiquidPints.PER_CUBIC_MILE;
        }

        public static float LiquidQuarts(float cubicMiles)
        {
            return cubicMiles * Units.LiquidQuarts.PER_CUBIC_MILE;
        }

        /// <summary>
        /// Convert from cubic miles to liters.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float cubicMiles)
        {
            return cubicMiles * Units.Liters.PER_CUBIC_MILE;
        }

        public static float Gallons(float cubicMiles)
        {
            return cubicMiles * Units.Gallons.PER_CUBIC_MILE;
        }

        /// <summary>
        /// Convert from cubic miles to cubic feet.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float cubicMiles)
        {
            return cubicMiles * Units.CubicFeet.PER_CUBIC_MILE;
        }

        /// <summary>
        /// Convert from cubic miles to cubic meters.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float cubicMiles)
        {
            return cubicMiles * Units.CubicMeters.PER_CUBIC_MILE;
        }

        /// <summary>
        /// Convert from cubic miles to kiloliters.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float cubicMiles)
        {
            return CubicMeters(cubicMiles);
        }

        /// <summary>
        /// Convert from cubic miles to cubic kilometers.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float cubicMiles)
        {
            return cubicMiles * Units.CubicKilometers.PER_CUBIC_MILE;
        }
    }
}