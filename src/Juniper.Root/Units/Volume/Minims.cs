namespace Juniper.Units
{
    /// <summary>
    /// Conversions from US minims
    /// </summary>
    public static class Minims
    {
        public const float PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_MINIM;
        public const float PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_MINIM;
        public const float PER_MINIM = 1;
        public const float PER_CUBIC_CENTIMETER = PER_MILLILITER;
        public const float PER_MILLILITER = 16.230730828281f;
        public const float PER_FLUID_DRAM = 60;
        public const float PER_TEASPOON = 80;
        public const float PER_TABLESPOON = PER_TEASPOON * Units.Teaspoons.PER_TABLESPOON;
        public const float PER_FLUID_OUNCE = PER_FLUID_DRAM * Units.FluidDrams.PER_FLUID_OUNCE;
        public const float PER_CUBIC_INCH = PER_TABLESPOON * Units.Tablespoons.PER_CUBIC_INCH;
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

        public static float CubicMicrometers(float minims)
        {
            return minims * Units.CubicMicrometers.PER_MINIM;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic millimeters.
        /// </summary>
        /// <param name="minims">The number of cubic micrometers</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float minims)
        {
            return minims * Units.CubicMillimeters.PER_MINIM;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic centimeters.
        /// </summary>
        /// <param name="minims">The number of cubic micrometers</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float minims)
        {
            return minims * Units.CubicCentimeters.PER_MINIM;
        }

        /// <summary>
        /// Convert from cubic micrometers to milliliters.
        /// </summary>
        /// <param name="minims">The number of cubic micrometers</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float minims)
        {
            return CubicCentimeters(minims);
        }

        public static float FluidDrams(float minims)
        {
            return minims * Units.FluidDrams.PER_MINIM;
        }

        public static float Teaspoons(float minims)
        {
            return minims * Units.Teaspoons.PER_MINIM;
        }

        public static float Tablespoons(float minims)
        {
            return minims * Units.Tablespoons.PER_MINIM;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic inches.
        /// </summary>
        /// <param name="minims">The number of cubic micrometers</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float minims)
        {
            return minims * Units.CubicInches.PER_MINIM;
        }

        public static float FluidOunces(float minims)
        {
            return minims * Units.FluidOunces.PER_MINIM;
        }

        public static float Gills(float minims)
        {
            return minims * Units.Gills.PER_MINIM;
        }

        public static float Cups(float minims)
        {
            return minims * Units.Cups.PER_MINIM;
        }

        public static float LiquidPints(float minims)
        {
            return minims * Units.LiquidPints.PER_MINIM;
        }

        public static float LiquidQuarts(float minims)
        {
            return minims * Units.LiquidQuarts.PER_MINIM;
        }

        /// <summary>
        /// Convert from cubic micrometers to liters.
        /// </summary>
        /// <param name="minims">The number of cubic micrometers</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float minims)
        {
            return minims * Units.Liters.PER_MINIM;
        }

        public static float Gallons(float minims)
        {
            return minims * Units.Gallons.PER_MINIM;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic feet.
        /// </summary>
        /// <param name="minims">The number of cubic micrometers</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float minims)
        {
            return minims * Units.CubicFeet.PER_MINIM;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic meters.
        /// </summary>
        /// <param name="minims">The number of cubic micrometers</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float minims)
        {
            return minims * Units.CubicMeters.PER_MINIM;
        }

        /// <summary>
        /// Convert from cubic micrometers to kiloliters.
        /// </summary>
        /// <param name="minims">The number of cubic micrometers</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float minims)
        {
            return CubicMeters(minims);
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic kilometers.
        /// </summary>
        /// <param name="minims">The number of cubic micrometers</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float minims)
        {
            return minims * Units.CubicKilometers.PER_MINIM;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic miles.
        /// </summary>
        /// <param name="minims">The number of cubic micrometers</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float minims)
        {
            return minims * Units.CubicMiles.PER_MINIM;
        }
    }
}
