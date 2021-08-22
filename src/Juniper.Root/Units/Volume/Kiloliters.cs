namespace Juniper.Units
{
    /// <summary>
    /// Conversions from kiloliters
    /// </summary>
    public static class Kiloliters
    {
        public const float PER_CUBIC_MICROMETER = Units.CubicMeters.PER_CUBIC_MICROMETER;
        public const float PER_CUBIC_MILLIMETER = Units.CubicMeters.PER_CUBIC_MILLIMETER;
        public const float PER_MINIM = 1 / Units.Minims.PER_KILOLITER;
        public const float PER_CUBIC_CENTIMETER = Units.CubicMeters.PER_CUBIC_CENTIMETER;
        public const float PER_MILLILITER = 1 / Units.Milliliters.PER_KILOLITER;
        public const float PER_FLUID_DRAM = 1 / Units.FluidDrams.PER_KILOLITER;
        public const float PER_TEASPOON = 1 / Units.Teaspoons.PER_KILOLITER;
        public const float PER_TABLESPOON = 1 / Units.Tablespoons.PER_KILOLITER;
        public const float PER_CUBIC_INCH = Units.CubicMeters.PER_CUBIC_INCH;
        public const float PER_FLUID_OUNCE = 1 / Units.FluidOunces.PER_KILOLITER;
        public const float PER_GILL = 1 / Units.Gills.PER_KILOLITER;
        public const float PER_CUP = 1 / Units.Cups.PER_KILOLITER;
        public const float PER_LIQUID_PINT = 1 / Units.LiquidPints.PER_KILOLITER;
        public const float PER_LIQUID_QUART = 1 / Units.LiquidQuarts.PER_KILOLITER;
        public const float PER_LITER = 1 / Units.Liters.PER_KILOLITER;
        public const float PER_GALLON = 1 / Units.Gallons.PER_KILOLITER;
        public const float PER_CUBIC_FOOT = Units.CubicMeters.PER_CUBIC_FOOT;
        public const float PER_CUBIC_METER = 1;
        public const float PER_KILOLITER = 1;
        public const float PER_CUBIC_KILOMETER = Units.CubicMeters.PER_CUBIC_KILOMETER;
        public const float PER_CUBIC_MILE = Units.CubicMeters.PER_CUBIC_MILE;

        /// <summary>
        /// Convert from cubic micrometers to kiloliters.
        /// </summary>
        /// <param name="kiloliters">The number of cubic micrometers</param>
        /// <returns>The number of kiloliters</returns>
        public static float CubicMicrometers(float kiloliters)
        {
            return kiloliters * Units.CubicMicrometers.PER_KILOLITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic millimeters.
        /// </summary>
        /// <param name="kiloliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float kiloliters)
        {
            return kiloliters * Units.CubicMillimeters.PER_KILOLITER;
        }

        public static float Minims(float kiloliters)
        {
            return kiloliters * Units.Minims.PER_KILOLITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic centimeters.
        /// </summary>
        /// <param name="kiloliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float kiloliters)
        {
            return kiloliters * Units.CubicCentimeters.PER_KILOLITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to milliliters.
        /// </summary>
        /// <param name="kiloliters">The number of cubic micrometers</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float kiloliters)
        {
            return CubicCentimeters(kiloliters);
        }

        public static float FluidDrams(float kiloliters)
        {
            return kiloliters * Units.FluidDrams.PER_KILOLITER;
        }

        public static float Teaspoons(float kiloliters)
        {
            return kiloliters * Units.Teaspoons.PER_KILOLITER;
        }

        public static float Tablespoons(float kiloliters)
        {
            return kiloliters * Units.Tablespoons.PER_KILOLITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic inches.
        /// </summary>
        /// <param name="kiloliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float kiloliters)
        {
            return kiloliters * Units.CubicInches.PER_KILOLITER;
        }

        public static float FluidOunces(float kiloliters)
        {
            return kiloliters * Units.FluidOunces.PER_KILOLITER;
        }

        public static float Gills(float kiloliters)
        {
            return kiloliters * Units.Gills.PER_KILOLITER;
        }

        public static float Cups(float kiloliters)
        {
            return kiloliters * Units.Cups.PER_KILOLITER;
        }

        public static float LiquidPints(float kiloliters)
        {
            return kiloliters * Units.LiquidPints.PER_KILOLITER;
        }

        public static float LiquidQuarts(float kiloliters)
        {
            return kiloliters * Units.LiquidQuarts.PER_KILOLITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to liters.
        /// </summary>
        /// <param name="kiloliters">The number of cubic micrometers</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float kiloliters)
        {
            return kiloliters * Units.Liters.PER_KILOLITER;
        }

        public static float Gallons(float kiloliters)
        {
            return kiloliters * Units.Gallons.PER_KILOLITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic feet.
        /// </summary>
        /// <param name="kiloliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float kiloliters)
        {
            return kiloliters * Units.CubicFeet.PER_KILOLITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic meters.
        /// </summary>
        /// <param name="kiloliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float kiloliters)
        {
            return kiloliters * Units.CubicMeters.PER_KILOLITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic kilometers.
        /// </summary>
        /// <param name="kiloliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float kiloliters)
        {
            return kiloliters * Units.CubicKilometers.PER_KILOLITER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic miles.
        /// </summary>
        /// <param name="kiloliters">The number of cubic micrometers</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float kiloliters)
        {
            return kiloliters * Units.CubicMiles.PER_KILOLITER;
        }
    }
}