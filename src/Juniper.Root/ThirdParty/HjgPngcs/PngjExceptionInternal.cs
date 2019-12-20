using System;
using System.Runtime.Serialization;

namespace Hjg.Pngcs
{
    /// <summary>
    /// Exception for internal problems
    /// </summary>
    [Serializable]
    public class PngjExceptionInternal : Exception
    {
        public PngjExceptionInternal()
        {
        }

        public PngjExceptionInternal(string message, Exception cause)
            : base(message, cause)
        {
        }

        public PngjExceptionInternal(string message)
            : base(message)
        {
        }

        public PngjExceptionInternal(Exception cause)
            : base(cause.Message, cause)
        {
        }

        protected PngjExceptionInternal(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}