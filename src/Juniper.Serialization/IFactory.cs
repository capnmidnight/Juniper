namespace Juniper.Serialization
{
    public interface IFactory : ISerializer, IDeserializer
    {
    }

    public interface IFactory<T> : ISerializer<T>, IDeserializer<T>
    {
    }

    public static class IFactoryExt
    {
        public static IFactory<T> Specialize<T>(this IFactory factory)
        {
            return new SpecializedFactory<T>(factory);
        }
    }
}