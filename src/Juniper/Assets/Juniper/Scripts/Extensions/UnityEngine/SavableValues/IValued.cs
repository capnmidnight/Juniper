namespace UnityEngine
{
    /// <summary>
    /// An abstraction for Reference{T}s and Varabile{T}s to make them usuable in shared contexts.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValued<T>
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        T Value
        {
            get;
        }
    }
}
