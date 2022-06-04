namespace Juniper.Units
{

    /// <summary>
    /// All permutations of Cartesian Axises.
    /// </summary>
    [Flags]
    public enum CartesianAxisFlags
    {
        /// <summary>
        /// No axis combination
        /// </summary>
        None,

        /// <summary>
        /// The horizontal axis
        /// </summary>
        X = 1,

        /// <summary>
        /// The vertical axis
        /// </summary>
        Y = 1 << 1,

        /// <summary>
        /// The horizontal and vertical axes
        /// </summary>
        XY = X | Y,

        /// <summary>
        /// The depth axis
        /// </summary>
        Z = 1 << 2,

        /// <summary>
        /// The horizontal and depth axes
        /// </summary>
        XZ = X | Z,

        /// <summary>
        /// The vertical and depth axes
        /// </summary>
        YZ = Y | Z,

        /// <summary>
        /// All axes
        /// </summary>
        XYZ = X | Y | Z
    }
}