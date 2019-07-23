namespace Hjg.Pngcs
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception for CRC check
    /// </summary>
    [Serializable]
    public class PngjBadCrcException : PngjException
    {
        private const long serialVersionUID = 1L;

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
            : base()
        {
        }

        protected PngjBadCrcException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}