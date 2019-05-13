namespace Juniper.Units
{
    /// <summary>
    /// Conversions from ratios. These conversions are only available for direct conversion,
    /// as they require two parameters.
    /// </summary>
    public static class Ratio
    {
        /// <summary>
        /// Convert a ratio of two numbers to a percentage.
        /// </summary>
        /// <param name="a">The amount to compare to <paramref name="b"/></param>
        /// <param name="b">The amount that <paramref name="a"/> is being compared to</param>
        /// <returns>The percentage of the whole that <paramref name="a"/> represents</returns>
        public static float Percent(float a, float b)
        {
            return Units.Proportion.Percent(Proportion(a, b));
        }

        /// <summary>
        /// Convert a ratio of two numbers to a proportion.
        /// </summary>
        /// <param name="a">The amount to compare to <paramref name="b"/></param>
        /// <param name="b">The amount that <paramref name="a"/> is being compared to</param>
        /// <returns>The proportion of the whole that <paramref name="a"/> represents</returns>
        public static float Proportion(float a, float b)
        {
            return a / (a + b);
        }
    }
}
