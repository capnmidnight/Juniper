namespace Juniper.Serialization
{
    public interface ISerializer
    {
        string Serialize<T>(string name, T value);
    }
}
