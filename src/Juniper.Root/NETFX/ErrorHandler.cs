namespace System
{
    /// <summary>
    /// An encapsulated Exception for passing to an <see cref="ErrorHandler"/>.
    /// </summary>
    public class ErrorArgs : EventArgs
    {
        /// <summary>
        /// The exception that is being reported.
        /// </summary>
        public readonly Exception exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="System.ErrorArgs"/> class.
        /// </summary>
        /// <param name="exp">Exp.</param>
        public ErrorArgs(Exception exp)
        {
            exception = exp;
        }
    }
}