
namespace System
{
    /// <summary>
    /// An encapsulated Exception for passing to an <see cref="ErrorHandler"/>.
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="System.ErrorEventArgs"/> class.
        /// </summary>
        /// <param name="exp">Exp.</param>
        public ErrorEventArgs(Exception exp)
        {
            Exception = exp;
        }

        public Exception Exception { get; }
    }
}