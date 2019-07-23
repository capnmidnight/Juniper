using System;

namespace Juniper.Threading
{
    /// <summary>
    /// Event arguments for when the request poller returns good results.
    /// </summary>
    public class ManagedRequestMessageArgs<T> : EventArgs
    {
        /// <summary>
        /// Create a new result message to pass out of the event handling thread.
        /// </summary>
        /// <param name="value"></param>
        public ManagedRequestMessageArgs(T value)
        {
            Value = value;
        }

        /// <summary>
        /// The deserialzed return result from the request.
        /// </summary>
        public T Value
        {
            get; private set;
        }
    }
}