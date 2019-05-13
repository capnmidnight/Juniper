namespace Juniper.Units
{
    /// <summary>
    /// Conversions from percent
    /// </summary>
    public static class Percent
    {
        /// <summary>
        /// Convert from percent to proportion.
        /// </summary>
        /// <param name="percent">The percentage of a whole</param>
        /// <returns>The proportion the percentage represents</returns>
        public static float Proportion(float percent)
        {
            return percent * 0.01f;
        }
    }
}
