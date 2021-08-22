namespace Juniper.Units
{
    /// <summary>
    /// Conversions from US cups
    /// </summary>
    public static class Cups
    {
        public const float PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_CUP;
        public const float PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_CUP;
        public const float PER_MINIM = 1 / Units.Minims.PER_CUP;
        public const float PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_CUP;
        public const float PER_MILLILITER = 1 / Units.Milliliters.PER_CUP;
        public const float PER_FLUID_DRAM = 1 / Units.FluidDrams.PER_CUP;
        public const float PER_TEASPOON = 1 / Units.Teaspoons.PER_CUP;
        public const float PER_TABLESPOON = 1 / Units.Tablespoons.PER_CUP;
        public const float PER_CUBIC_INCH = 1 / Units.CubicInches.PER_CUP;
        public const float PER_FLUID_OUNCE = 1 / Units.FluidOunces.PER_CUP;
        public const float PER_GILL = 1 / Units.Gills.PER_CUP;
        public const float PER_CUP = 1;
        public const float PER_LIQUID_PINT = 2;
        public const float PER_LIQUID_QUART = PER_LIQUID_PINT * Units.LiquidPints.PER_LIQUID_QUART;
        public const float PER_LITER = PER_LIQUID_QUART * Units.LiquidQuarts.PER_LITER;
        public const float PER_GALLON = PER_LIQUID_QUART * Units.LiquidQuarts.PER_GALLON;
        public const float PER_CUBIC_FOOT = PER_GALLON * Units.Gallons.PER_CUBIC_FOOT;
        public const float PER_CUBIC_METER = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_METER;
        public const float PER_KILOLITER = PER_CUBIC_METER;
        public const float PER_CUBIC_KILOMETER = PER_CUBIC_METER * Units.CubicMeters.PER_CUBIC_KILOMETER;
        public const float PER_CUBIC_MILE = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_MILE;

        /// <summary>
        /// Convert from cubic millimeters to cubic micrometers.
        /// </summary>
        /// <param name="cups">The number of cubic millimeters</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float cups)
        {
            return cups * Units.CubicMicrometers.PER_CUP;
        }

        public static float CubicMillimeters(float cups)
        {
            return cups * Units.CubicMillimeters.PER_CUP;
        }

        public static float Minims(float cups)
        {
            return cups * Units.Minims.PER_CUP;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic centimeters.
        /// </summary>
        /// <param name="cups">The number of cubic millimeters</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float cups)
        {
            return cups * Units.CubicCentimeters.PER_CUP;
        }

        /// <summary>
        /// Convert from cubic millimeters to milliliters.
        /// </summary>
        /// <param name="cups">The number of cubic millimeters</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float cups)
        {
            return CubicCentimeters(cups);
        }

        public static float FluidDrams(float cups)
        {
            return cups * Units.FluidDrams.PER_CUP;
        }

        public static float Teaspoons(float cups)
        {
            return cups * Units.Teaspoons.PER_CUP;
        }

        public static float Tablespoons(float cups)
        {
            return cups * Units.Tablespoons.PER_CUP;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic inches.
        /// </summary>
        /// <param name="cups">The number of cubic millimeters</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float cups)
        {
            return cups * Units.CubicInches.PER_CUP;
        }

        public static float FluidOunces(float cups)
        {
            return cups * Units.FluidOunces.PER_CUP;
        }

        public static float Gills(float cups)
        {
            return cups * Units.Gills.PER_CUP;
        }

        public static float LiquidPints(float cups)
        {
            return cups * Units.LiquidPints.PER_CUP;
        }

        public static float LiquidQuarts(float cups)
        {
            return cups * Units.LiquidQuarts.PER_CUP;
        }

        /// <summary>
        /// Convert from cubic millimeters to liters.
        /// </summary>
        /// <param name="cups">The number of cubic millimeters</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float cups)
        {
            return cups * Units.Liters.PER_CUP;
        }

        public static float Gallons(float cups)
        {
            return cups * Units.Gallons.PER_CUP;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic feet.
        /// </summary>
        /// <param name="cups">The number of cubic millimeters</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float cups)
        {
            return cups * Units.CubicFeet.PER_CUP;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic meters.
        /// </summary>
        /// <param name="cups">The number of cubic millimeters</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float cups)
        {
            return cups * Units.CubicMeters.PER_CUP;
        }

        /// <summary>
        /// Convert from cubic millimeters to kiloliters.
        /// </summary>
        /// <param name="cups">The number of cubic millimeters</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float cups)
        {
            return CubicMeters(cups);
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic kilometers.
        /// </summary>
        /// <param name="cups">The number of cubic millimeters</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float cups)
        {
            return cups * Units.CubicKilometers.PER_CUP;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic miles.
        /// </summary>
        /// <param name="cups">The number of cubic millimeters</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float cups)
        {
            return cups * Units.CubicMiles.PER_CUP;
        }
    }
}
