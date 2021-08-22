namespace Juniper.Units
{
    /// <summary>
    /// Conversions from liters
    /// </summary>
    public static class Liters
    {
        public const float PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_LITER;
        public const float PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_LITER;
        public const float PER_MINIM = 1 / Units.Minims.PER_LITER;
        public const float PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_LITER;
        public const float PER_MILLILITER = 1 / Units.Milliliters.PER_LITER;
        public const float PER_FLUID_DRAM = 1 / Units.FluidDrams.PER_LITER;
        public const float PER_TEASPOON = 1 / Units.Teaspoons.PER_LITER;
        public const float PER_TABLESPOON = 1 / Units.Tablespoons.PER_LITER;
        public const float PER_CUBIC_INCH = 1 / Units.CubicInches.PER_LITER;
        public const float PER_FLUID_OUNCE = 1 / Units.FluidOunces.PER_LITER;
        public const float PER_GILL = 1 / Units.Gills.PER_LITER;
        public const float PER_CUP = 1 / Units.Cups.PER_LITER;
        public const float PER_LIQUID_PINT = 1 / Units.LiquidPints.PER_LITER;
        public const float PER_LIQUID_QUART = 1 / Units.LiquidQuarts.PER_LITER;
        public const float PER_LITER = 1;
        public const float PER_GALLON = PER_LIQUID_QUART * Units.LiquidQuarts.PER_GALLON;
        public const float PER_CUBIC_FOOT = PER_GALLON * Units.Gallons.PER_CUBIC_FOOT;
        public const float PER_CUBIC_METER = PER_KILOLITER;
        public const float PER_KILOLITER = 1000;
        public const float PER_CUBIC_KILOMETER = PER_CUBIC_METER * Units.CubicMeters.PER_CUBIC_KILOMETER;
        public const float PER_CUBIC_MILE = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_MILE;

        /// <summary>
        /// Convert from cubic micrometers to liters.
        /// </summary>
        /// <param name="liters">The number of cubic micrometers</param>
        /// <returns>The number of liters</returns>
        public static float CubicMicrometers(float liters)
        {
            return liters * Units.CubicMicrometers.PER_LITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic millimeters.
        /// </summary>
        /// <param name="liters">The number of cubic micrometers</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float liters)
        {
            return liters * Units.CubicMillimeters.PER_LITER;
        }

        public static float Minims(float liters)
        {
            return liters * Units.Minims.PER_LITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic centimeters.
        /// </summary>
        /// <param name="liters">The number of cubic micrometers</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float liters)
        {
            return liters * Units.CubicCentimeters.PER_LITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to milliliters.
        /// </summary>
        /// <param name="liters">The number of cubic micrometers</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float liters)
        {
            return CubicCentimeters(liters);
        }

        public static float FluidDrams(float liters)
        {
            return liters * Units.FluidDrams.PER_LITER;
        }

        public static float Teaspoons(float liters)
        {
            return liters * Units.Teaspoons.PER_LITER;
        }

        public static float Tablespoons(float liters)
        {
            return liters * Units.Tablespoons.PER_LITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic inches.
        /// </summary>
        /// <param name="liters">The number of cubic micrometers</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float liters)
        {
            return liters * Units.CubicInches.PER_LITER;
        }

        public static float FluidOunces(float liters)
        {
            return liters * Units.FluidOunces.PER_LITER;
        }

        public static float Gills(float liters)
        {
            return liters * Units.Gills.PER_LITER;
        }

        public static float Cups(float liters)
        {
            return liters * Units.Cups.PER_LITER;
        }

        public static float LiquidPints(float liters)
        {
            return liters * Units.LiquidPints.PER_LITER;
        }

        public static float LiquidQuarts(float liters)
        {
            return liters * Units.LiquidQuarts.PER_LITER;
        }

        public static float Gallons(float liters)
        {
            return liters * Units.Gallons.PER_LITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic feet.
        /// </summary>
        /// <param name="liters">The number of cubic micrometers</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float liters)
        {
            return liters * Units.CubicFeet.PER_LITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic meters.
        /// </summary>
        /// <param name="liters">The number of cubic micrometers</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float liters)
        {
            return liters * Units.CubicMeters.PER_LITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to kiloliters.
        /// </summary>
        /// <param name="liters">The number of cubic micrometers</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float liters)
        {
            return CubicMeters(liters);
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic kilometers.
        /// </summary>
        /// <param name="liters">The number of cubic micrometers</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float liters)
        {
            return liters * Units.CubicKilometers.PER_LITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic miles.
        /// </summary>
        /// <param name="liters">The number of cubic micrometers</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float liters)
        {
            return liters * Units.CubicMiles.PER_LITER;
        }
    }
}