namespace Juniper.Units
{
    /// <summary>
    /// Conversions from liters
    /// </summary>
    public static class Liters
    {
        /// <summary>
        /// Conversion factor from cubic micrometers to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_LITER;

        /// <summary>
        /// Conversion factor from cubic millimeters to liters.
        /// </summary>
        public const float PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_LITER;

        /// <summary>
        /// Conversion factor from cubic centimeters to liters.
        /// </summary>
        public const float PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_LITER;

        /// <summary>
        /// Conversion factor from milliliters to liters.
        /// </summary>
        public const float PER_MILLILITER = 1 / Units.Milliliters.PER_LITER;

        /// <summary>
        /// Conversion factor from cubic inches to liters.
        /// </summary>
        public const float PER_CUBIC_INCH = 1 / Units.CubicInches.PER_LITER;

        /// <summary>
        /// Conversion factor from cubic feet to liters.
        /// </summary>
        public const float PER_CUBIC_FOOT = PER_CUBIC_INCH * Units.CubicInches.PER_CUBIC_FOOT;

        /// <summary>
        /// Conversion factor from cubic meters to liters.
        /// </summary>
        public const float PER_CUBIC_METER = PER_KILOLITER;

        /// <summary>
        /// Conversion factor from kiloliters to liters.
        /// </summary>
        public const float PER_KILOLITER = 1000;

        /// <summary>
        /// Conversion factor from cubic kilometers to liters.
        /// </summary>
        public const float PER_CUBIC_KILOMETER = PER_CUBIC_METER * Units.CubicMeters.PER_CUBIC_KILOMETER;

        /// <summary>
        /// Conversion factor from cubic miles to liters.
        /// </summary>
        public const float PER_CUBIC_MILE = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_MILE;

        /// <summary>
        /// Convert from liters to cubic micrometers.
        /// </summary>
        /// <param name="liters">The number of liters</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float liters)
        {
            return liters * Units.CubicMicrometers.PER_LITER;
        }

        /// <summary>
        /// Convert from liters to cubic millimeters.
        /// </summary>
        /// <param name="liters">The number of liters</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float liters)
        {
            return liters * Units.CubicMillimeters.PER_LITER;
        }

        /// <summary>
        /// Convert from liters to cubic centimeters.
        /// </summary>
        /// <param name="liters">The number of liters</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float liters)
        {
            return liters * Units.CubicCentimeters.PER_LITER;
        }

        /// <summary>
        /// Convert from liters to milliliters.
        /// </summary>
        /// <param name="liters">The number of liters</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float liters)
        {
            return CubicCentimeters(liters);
        }

        /// <summary>
        /// Convert from liters to cubic inches.
        /// </summary>
        /// <param name="liters">The number of liters</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float liters)
        {
            return liters * Units.CubicInches.PER_LITER;
        }

        /// <summary>
        /// Convert from liters to cubic feet.
        /// </summary>
        /// <param name="liters">The number of liters</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float liters)
        {
            return liters * Units.CubicFeet.PER_LITER;
        }

        /// <summary>
        /// Convert from liters to cubic meters.
        /// </summary>
        /// <param name="liters">The number of liters</param>
        /// <returns>The number of liters</returns>
        public static float CubicMeters(float liters)
        {
            return liters * Units.CubicMeters.PER_LITER;
        }

        /// <summary>
        /// Convert from liters to kiloliters.
        /// </summary>
        /// <param name="liters">The number of liters</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float liters)
        {
            return CubicMeters(liters);
        }

        /// <summary>
        /// Convert from liters to cubic kilometers.
        /// </summary>
        /// <param name="liters">The number of liters</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float liters)
        {
            return liters * Units.CubicKilometers.PER_LITER;
        }

        /// <summary>
        /// Convert from liters to cubic miles.
        /// </summary>
        /// <param name="liters">The number of liters</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float liters)
        {
            return liters * Units.CubicMiles.PER_LITER;
        }
    }
}