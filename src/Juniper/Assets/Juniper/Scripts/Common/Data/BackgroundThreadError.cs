using System;

namespace Juniper.Data
{
    /// <summary>
    /// Exception details for an error that occured in a <see cref="AbstractBackgroundThread"/>.
    /// </summary>
    public class BackgroundThreadErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Create a new thread exception message.
        /// </summary>
        /// <param name="exp"></param>
        public BackgroundThreadErrorEventArgs(Exception exp)
        {
            Error = exp;
        }

        /// <summary>
        /// The exception that occured.
        /// </summary>
        public Exception Error
        {
            get; private set;
        }
    }
}
