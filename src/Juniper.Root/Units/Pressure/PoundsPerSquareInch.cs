namespace Juniper.Units
{
    /// <summary>
    /// Conversions from pounds per square inch
    /// </summary>
    public static class PoundsPerSquareInch
    {
        /// <summary>
        /// Conversion factor from pascals to pounds per square inch.
        /// </summary>
        public const float PER_PASCAL = 1 / Units.Pascals.PER_POUND_PER_SQUARE_INCH;

        /// <summary>
        /// Conversion factor from hectopascals to pounds per square inch.
        /// </summary>
        public const float PER_HECTOPASCAL = 1 / Units.Hectopascals.PER_POUND_PER_SQUARE_INCH;

        /// <summary>
        /// Conversion factor from millibars to pounds per square inch.
        /// </summary>
        public const float PER_MILLIBAR = 1 / Units.Millibars.PER_POUND_PER_SQUARE_INCH;

        /// <summary>
        /// Conversion factor from kilopascals to pounds per square inch.
        /// </summary>
        public const float PER_KILOPASCAL = 1 / Units.Kilopascals.PER_POUND_PER_SQUARE_INCH;

        /// <summary>
        /// Convert from pounds per square inch to pascals.
        /// </summary>
        /// <param name="psi">The number of pounds per square inch</param>
        /// <returns>The number of pascals</returns>
        public static float Pascals(float psi)
        {
            return psi * Units.Pascals.PER_POUND_PER_SQUARE_INCH;
        }

        /// <summary>
        /// Convert from pounds per square inch to hectopascals.
        /// </summary>
        /// <param name="psi">The number of pounds per square inch</param>
        /// <returns>The number of hectopascals</returns>
        public static float Hectopascals(float psi)
        {
            return psi * Units.Hectopascals.PER_POUND_PER_SQUARE_INCH;
        }

        /// <summary>
        /// Convert from pounds per square inch to millibars.
        /// </summary>
        /// <param name="psi">The number of pounds per square inch</param>
        /// <returns>The number of millibars</returns>
        public static float Millibars(float psi)
        {
            return psi * Units.Millibars.PER_POUND_PER_SQUARE_INCH;
        }

        /// <summary>
        /// Convert from pounds per square inch to kilopascals.
        /// </summary>
        /// <param name="psi">The number of pounds per square inch</param>
        /// <returns>The number of kilopascals</returns>
        public static float Kilopascals(float psi)
        {
            return psi * Units.Kilopascals.PER_POUND_PER_SQUARE_INCH;
        }
    }
}