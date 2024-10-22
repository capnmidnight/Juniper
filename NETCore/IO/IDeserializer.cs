namespace Juniper.IO;

public interface IDeserializer<out ResultT, out M> where M : MediaType
{
    M? InputContentType { get; }
    ResultT? Deserialize(Stream stream);
}