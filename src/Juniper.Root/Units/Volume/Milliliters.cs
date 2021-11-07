namespace Juniper.Units
{
    /// <summary>
    /// Conversions from milliliters
    /// </summary>
    public static class Milliliters
    {
        public const float PER_CUBIC_MICROMETER = Units.CubicCentimeters.PER_CUBIC_MICROMETER;
        public const float PER_CUBIC_MILLIMETER = Units.CubicCentimeters.PER_CUBIC_MILLIMETER;
        public const float PER_MINIM = 1 / Units.Minims.PER_MILLILITER;
        public const float PER_CUBIC_CENTIMETER = 1;
        public const float PER_MILLILITER = 1;
        public const float PER_FLUID_DRAM = PER_TEASPOON * Units.Teaspoons.PER_FLUID_DRAM;
        public const float PER_TEASPOON = PER_TABLESPOON * Units.Tablespoons.PER_TEASPOON;
        public const float PER_TABLESPOON = PER_FLUID_OUNCE * Units.FluidOunces.PER_TABLESPOON;
        public const float PER_CUBIC_INCH = Units.CubicCentimeters.PER_CUBIC_INCH;
        public const float PER_FLUID_OUNCE = PER_GILL * Units.Gills.PER_FLUID_OUNCE;
        public const float PER_GILL = PER_CUP * Units.Cups.PER_GILL;
        public const float PER_CUP = PER_LIQUID_PINT * Units.LiquidPints.PER_CUP;
        public const float PER_LIQUID_PINT = PER_LIQUID_QUART * Units.LiquidQuarts.PER_LIQUID_PINT;
        public const float PER_LIQUID_QUART = PER_GALLON * Units.Gallons.PER_LIQUID_QUART;
        public const float PER_LITER = 1000;
        public const float PER_GALLON = PER_LITER * Units.Liters.PER_GALLON;
        public const float PER_CUBIC_FOOT = Units.CubicCentimeters.PER_CUBIC_FOOT;
        public const float PER_CUBIC_METER = Units.CubicCentimeters.PER_CUBIC_METER;
        public const float PER_KILOLITER = PER_LITER * Units.Liters.PER_KILOLITER;
        public const float PER_CUBIC_KILOMETER = Units.CubicCentimeters.PER_CUBIC_KILOMETER;
        public const float PER_CUBIC_MILE = Units.CubicCentimeters.PER_CUBIC_MILE;

        /// <summary>
        /// Convert from cubic micrometers to milliliters.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of milliliters</returns>
        public static float CubicMicrometers(float milliliters)
        {
            return milliliters * Units.CubicMicrometers.PER_MILLILITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic millimeters.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float milliliters)
        {
            return milliliters * Units.CubicMillimeters.PER_MILLILITER;
        }

        public static float Minims(float milliliters)
        {
            return milliliters * Units.Minims.PER_MILLILITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic centimeters.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float milliliters)
        {
            return milliliters * Units.CubicCentimeters.PER_MILLILITER;
        }

        public static float FluidDrams(float milliliters)
        {
            return milliliters * Units.FluidDrams.PER_MILLILITER;
        }

        public static float Teaspoons(float milliliters)
        {
            return milliliters * Units.Teaspoons.PER_MILLILITER;
        }

        public static float Tablespoons(float milliliters)
        {
            return milliliters * Units.Tablespoons.PER_MILLILITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic inches.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float milliliters)
        {
            return milliliters * Units.CubicInches.PER_MILLILITER;
        }

        public static float FluidOunces(float milliliters)
        {
            return milliliters * Units.FluidOunces.PER_MILLILITER;
        }

        public static float Gills(float milliliters)
        {
            return milliliters * Units.Gills.PER_MILLILITER;
        }

        public static float Cups(float milliliters)
        {
            return milliliters * Units.Cups.PER_MILLILITER;
        }

        public static float LiquidPints(float milliliters)
        {
            return milliliters * Units.LiquidPints.PER_MILLILITER;
        }

        public static float LiquidQuarts(float milliliters)
        {
            return milliliters * Units.LiquidQuarts.PER_MILLILITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to liters.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float milliliters)
        {
            return milliliters * Units.Liters.PER_MILLILITER;
        }

        public static float Gallons(float milliliters)
        {
            return milliliters * Units.Gallons.PER_MILLILITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic feet.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float milliliters)
        {
            return milliliters * Units.CubicFeet.PER_MILLILITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic meters.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float milliliters)
        {
            return milliliters * Units.CubicMeters.PER_MILLILITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to kiloliters.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float milliliters)
        {
            return CubicMeters(milliliters);
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic kilometers.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float milliliters)
        {
            return milliliters * Units.CubicKilometers.PER_MILLILITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic miles.
        /// </summary>
        /// <param name="milliliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float milliliters)
        {
            return milliliters * Units.CubicMiles.PER_MILLILITER;
        }
    }
}