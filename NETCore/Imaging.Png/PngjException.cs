using System.Runtime.Serialization;

namespace Hjg.Pngcs;

/// <summary>
/// Gral exception class for PNGCS library
/// </summary>
[Serializable]
public class PngjException : Exception
{
    public PngjException(string message, Exception cause)
        : base(message, cause)
    {
    }

    public PngjException(string message)
        : base(message)
    {
    }

    public PngjException(Exception cause)
        : base(cause?.Message, cause)
    {
    }

    public PngjException()
    {
    }

    protected PngjException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}