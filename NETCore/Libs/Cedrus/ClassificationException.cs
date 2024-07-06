using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus;

public class ClassificationException : Exception
{
    public ClassificationLevel Level { get; }

    public ClassificationException(ClassificationLevel level)
    {
        Level = level;
    }

    public ClassificationException(ClassificationLevel level, string? message) : base(message)
    {
        Level = level;
    }

    public ClassificationException(ClassificationLevel level, string? message, Exception? innerException) : base(message, innerException)
    {
        Level = level;
    }
}
