using System.Runtime.Serialization;

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

    protected ClassificationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Level = info.GetValue<ClassificationLevel>(nameof(Level))!;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(Level), Level);
    }
}
