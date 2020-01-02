using System;
using System.Runtime.Serialization;

namespace Hjg.Pngcs
{
    /// <summary>
    /// Exception for CRC check
    /// </summary>
    [Serializable]
    public class PngjBadCrcException : PngjException
    {
        public PngjBadCrcException(string message, Exception cause)
            : base(message, cause)
        {
        }

        public PngjBadCrcException(string message)
            : base(message)
        {
        }

        public PngjBadCrcException(Exception cause)
            : base(cause)
        {
        }

        public PngjBadCrcException()
        {
        }

        protected PngjBadCrcException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}