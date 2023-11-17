namespace Juniper.IO;

public interface ISerializer<in T, out M> where M : MediaType
{
    M OutputContentType { get; }
    long Serialize(Stream stream, T value);
}
