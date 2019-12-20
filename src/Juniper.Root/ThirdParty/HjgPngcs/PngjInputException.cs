using System;
using System.Runtime.Serialization;

namespace Hjg.Pngcs
{
    /// <summary>
    /// Exception associated with input (reading) operations
    /// </summary>
    [Serializable]
    public class PngjInputException : PngjException
    {
        public PngjInputException(string message, Exception cause)
            : base(message, cause)
        {
        }

        public PngjInputException(string message)
            : base(message)
        {
        }

        public PngjInputException(Exception cause)
            : base(cause)
        {
        }

        public PngjInputException()
        {
        }

        protected PngjInputException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}