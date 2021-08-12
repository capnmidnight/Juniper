namespace Juniper.Units
{
    /// <summary>
    /// Conversions from kiloliters
    /// </summary>
    public static class Kiloliters
    {
        /// <summary>
        /// Conversion factor from cubic micrometers to kiloliters.
        /// </summary>
        public const float PER_CUBIC_MICROMETER = Units.CubicMeters.PER_CUBIC_MICROMETER;

        /// <summary>
        /// Conversion factor from cubic millimeters to kiloliters.
        /// </summary>
        public const float PER_CUBIC_MILLIMETER = Units.CubicMeters.PER_CUBIC_MILLIMETER;

        /// <summary>
        /// Conversion factor from cubic millimeters to kiloliters.
        /// </summary>
        public const float PER_CUBIC_CENTIMETER = Units.CubicMeters.PER_CUBIC_CENTIMETER;

        /// <summary>
        /// Conversion factor from milliliters to kiloliters.
        /// </summary>
        public const float PER_MILLILITER = 1 / Units.Milliliters.PER_LITER;

        /// <summary>
        /// Conversion factor from cubic inches to kiloliters.
        /// </summary>
        public const float PER_CUBIC_INCH = Units.CubicMeters.PER_CUBIC_INCH;

        /// <summary>
        /// Conversion factor from cubic inches to kiloliters.
        /// </summary>
        public const float PER_LITER = 1 / Units.Liters.PER_KILOLITER;

        /// <summary>
        /// Conversion factor from cubic feet to kiloliters.
        /// </summary>
        public const float PER_CUBIC_FOOT = Units.CubicMeters.PER_CUBIC_FOOT;

        /// <summary>
        /// Conversion factor from kiloliters to kiloliters.
        /// </summary>
        public const float PER_CUBIC_METER = 1;

        /// <summary>
        /// Conversion factor from cubic kilometers to kiloliters.
        /// </summary>
        public const float PER_CUBIC_KILOMETER = Units.CubicMeters.PER_CUBIC_KILOMETER;

        /// <summary>
        /// Conversion factor from cubic miles to kiloliters.
        /// </summary>
        public const float PER_CUBIC_MILE = Units.CubicMeters.PER_CUBIC_MILE;

        /// <summary>
        /// Convert from kiloliters to cubic micrometers.
        /// </summary>
        /// <param name="kiloliters">The number of kiloliters</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float kiloliters)
        {
            return Units.CubicMeters.CubicMicrometers(kiloliters);
        }

        /// <summary>
        /// Convert from kiloliters to cubic millimeters.
        /// </summary>
        /// <param name="kiloliters">The number of kiloliters</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float kiloliters)
        {
            return Units.CubicMeters.CubicMillimeters(kiloliters);
        }

        /// <summary>
        /// Convert from kiloliters to cubic centimeters.
        /// </summary>
        /// <param name="kiloliters">The number of kiloliters</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float kiloliters)
        {
            return Units.CubicMeters.CubicCentimeters(kiloliters);
        }

        /// <summary>
        /// Convert from kiloliters to milliliters.
        /// </summary>
        /// <param name="kiloliters">The number of kiloliters</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float kiloliters)
        {
            return Units.CubicMeters.Milliliters(kiloliters);
        }

        /// <summary>
        /// Convert from kiloliters to cubic inches.
        /// </summary>
        /// <param name="kiloliters">The number of kiloliters</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float kiloliters)
        {
            return Units.CubicMeters.CubicInches(kiloliters);
        }

        /// <summary>
        /// Convert from kiloliters to liters.
        /// </summary>
        /// <param name="kiloliters">The number of kiloliters</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float kiloliters)
        {
            return Units.CubicMeters.Liters(kiloliters);
        }

        /// <summary>
        /// Convert from kiloliters to cubic feet.
        /// </summary>
        /// <param name="kiloliters">The number of kiloliters</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float kiloliters)
        {
            return Units.CubicMeters.CubicFeet(kiloliters);
        }

        /// <summary>
        /// Convert from kiloliters to cubic meters.
        /// </summary>
        /// <param name="kiloliters">The number of kiloliters</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float kiloliters)
        {
            return kiloliters;
        }

        /// <summary>
        /// Convert from kiloliters to cubic kilometers.
        /// </summary>
        /// <param name="kiloliters">The number of kiloliters</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float kiloliters)
        {
            return Units.CubicMeters.CubicKilometers(kiloliters);
        }

        /// <summary>
        /// Convert from kiloliters to cubic miles.
        /// </summary>
        /// <param name="kiloliters">The number of kiloliters</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float kiloliters)
        {
            return Units.CubicMeters.CubicMiles(kiloliters);
        }
    }
}