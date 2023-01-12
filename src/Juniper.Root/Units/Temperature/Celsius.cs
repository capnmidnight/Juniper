namespace Juniper.Units
{
    /// <summary>
    /// Conversions from celsius
    /// </summary>
    public static class Celsius
    {
        /// <summary>
        /// The temperature in Celsius at which pure water freezes.
        /// </summary>
        public const double FREEZING_POINT = 0;

        /// <summary>
        /// The temperature in Celsius at which pure water boils.
        /// </summary>
        public const double BOILING_POINT = 100;

        /// <summary>
        /// The range across the freezing and boiling point of pure water.
        /// </summary>
        public const double LIQUID_BAND = BOILING_POINT - FREEZING_POINT;

        /// <summary>
        /// Conversion factor from farenheit to celsius.
        /// </summary>
        public const double PER_FARENHEIT = LIQUID_BAND / Units.Farenheit.LIQUID_BAND;

        /// <summary>
        /// Conversion factor from kelvin to celsius.
        /// </summary>
        public const double PER_KELVIN = LIQUID_BAND / Units.Kelvin.LIQUID_BAND;

        /// <summary>
        /// Convert from celsius to farenheit.
        /// </summary>
        /// <param name="celcius">The number of celcius</param>
        /// <returns>The number of degrees farenheit</returns>
        public static double Farenheit(double celcius)
        {
            return ((celcius - FREEZING_POINT) * Units.Farenheit.PER_CELSIUS) + Units.Farenheit.FREEZING_POINT;
        }

        /// <summary>
        /// Convert from celsius to kelvin.
        /// </summary>
        /// <param name="celcius">The number of celcius</param>
        /// <returns>The number of degrees kelvin</returns>
        public static double Kelvin(double celcius)
        {
            return ((celcius - FREEZING_POINT) * Units.Kelvin.PER_CELSIUS) + Units.Kelvin.FREEZING_POINT;
        }
    }
}