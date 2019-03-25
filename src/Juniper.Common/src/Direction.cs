namespace Juniper
{
    /// <summary>
    /// The directions we can move into and out of the state.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Movement in the backwards direction.
        /// </summary>
        Reverse = -1,

        /// <summary>
        /// No movement.
        /// </summary>
        Stopped = 0,

        /// <summary>
        /// The normal direction.
        /// </summary>
        Forward = 1
    }
}
