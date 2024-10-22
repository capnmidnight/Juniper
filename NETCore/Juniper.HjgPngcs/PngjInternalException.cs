using System;
using System.Runtime.Serialization;

namespace Hjg.Pngcs
{
    /// <summary>
    /// Exception for internal problems
    /// </summary>
    [Serializable]
    public class PngjInternalException : Exception
    {
        public PngjInternalException()
        {
        }

        public PngjInternalException(string message, Exception cause)
            : base(message, cause)
        {
        }

        public PngjInternalException(string message)
            : base(message)
        {
        }

        public PngjInternalException(Exception cause)
            : base(cause?.Message, cause)
        {
        }

        protected PngjInternalException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}