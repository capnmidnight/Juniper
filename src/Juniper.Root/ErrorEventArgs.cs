using System;
using System.Reflection.Emit;

namespace Juniper
{
    /// <summary>
    /// An encapsulated Exception for passing to an <see cref="ErrorHandler"/>.
    /// </summary>
    public class ErrorEventArgs : EventArgs<Exception>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorEventArgs"/> class.
        /// </summary>
        /// <param name="exp">Exp.</param>
        public ErrorEventArgs(Exception exp)
            : base(exp)
        { }
    }
}