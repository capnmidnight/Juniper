namespace Juniper.Units
{
    /// <summary>
    /// Whole categories of units, such as the US Customary units.
    /// </summary>
    public enum SystemOfMeasure
    {
        /// <summary>
        /// The system of measure is not known.
        /// </summary>
        Unknown,

        /// <summary>
        /// What people often erroneously call "Imperial".
        /// </summary>
        USCustomary,

        /// <summary>
        /// What people are surprised to hear is actually the official system of measurement in the
        /// United States.
        /// </summary>
        Metric
    }
}
