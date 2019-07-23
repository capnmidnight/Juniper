namespace Hjg.Pngcs
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Gral exception class for PNGCS library
    /// </summary>
    [Serializable]
    public class PngjException : Exception
    {
        private const long serialVersionUID = 1L;

        public PngjException(string message, Exception cause)
            : base(message, cause)
        {
        }

        public PngjException(string message)
            : base(message)
        {
        }

        public PngjException(Exception cause)
            : base(cause.Message, cause)
        {
        }

        public PngjException()
            : base()
        {
        }

        protected PngjException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}