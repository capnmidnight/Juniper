namespace Juniper.Units
{
    /// <summary>
    /// Conversions from kelvin
    /// </summary>
    public static class Kelvin
    {
        /// <summary>
        /// The temperature in Kelvin at which pure water freezes.
        /// </summary>
        public const double FREEZING_POINT = 273.15;

        /// <summary>
        /// The temperature in Kelvin at which pure water boils.
        /// </summary>
        public const double BOILING_POINT = 373.15;

        /// <summary>
        /// The range across the freezing and boiling point of pure water.
        /// </summary>
        public const double LIQUID_BAND = BOILING_POINT - FREEZING_POINT;

        /// <summary>
        /// Conversion factor from farenheit to kelvin.
        /// </summary>
        public const double PER_FARENHEIT = LIQUID_BAND / Units.Farenheit.LIQUID_BAND;

        /// <summary>
        /// Conversion factor from celsius to kelvin.
        /// </summary>
        public const double PER_CELSIUS = LIQUID_BAND / Units.Celsius.LIQUID_BAND;

        /// <summary>
        /// Convert from kelvin to farenheit.
        /// </summary>
        /// <param name="kelvin">The number of kelvin</param>
        /// <returns>The number of degrees farenheit</returns>
        public static double Farenheit(double kelvin)
        {
            return ((kelvin - FREEZING_POINT) * Units.Farenheit.PER_KELVIN) + Units.Farenheit.FREEZING_POINT;
        }

        /// <summary>
        /// Convert from kelvin to celsius.
        /// </summary>
        /// <param name="kelvin">The number of kelvin</param>
        /// <returns>The number of degrees celsius</returns>
        public static double Celsius(double kelvin)
        {
            return ((kelvin - FREEZING_POINT) * Units.Celsius.PER_KELVIN) + Units.Celsius.FREEZING_POINT;
        }
    }
}