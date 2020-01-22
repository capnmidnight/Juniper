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
        public const float FREEZING_POINT = 0f;

        /// <summary>
        /// The temperature in Celsius at which pure water boils.
        /// </summary>
        public const float BOILING_POINT = 100f;

        /// <summary>
        /// The range across the freezing and boiling point of pure water.
        /// </summary>
        public const float LIQUID_BAND = BOILING_POINT - FREEZING_POINT;

        /// <summary>
        /// Conversion factor from farenheit to celsius.
        /// </summary>
        public const float PER_FARENHEIT = LIQUID_BAND / Units.Farenheit.LIQUID_BAND;

        /// <summary>
        /// Conversion factor from kelvin to celsius.
        /// </summary>
        public const float PER_KELVIN = LIQUID_BAND / Units.Kelvin.LIQUID_BAND;

        /// <summary>
        /// Convert from celsius to farenheit.
        /// </summary>
        /// <param name="celcius">The number of celcius</param>
        /// <returns>The number of degrees farenheit</returns>
        public static float Farenheit(float celcius)
        {
            return ((celcius - FREEZING_POINT) * Units.Farenheit.PER_CELSIUS) + Units.Farenheit.FREEZING_POINT;
        }

        /// <summary>
        /// Convert from celsius to kelvin.
        /// </summary>
        /// <param name="celcius">The number of celcius</param>
        /// <returns>The number of degrees kelvin</returns>
        public static float Kelvin(float celcius)
        {
            return ((celcius - FREEZING_POINT) * Units.Kelvin.PER_CELSIUS) + Units.Kelvin.FREEZING_POINT;
        }
    }
}