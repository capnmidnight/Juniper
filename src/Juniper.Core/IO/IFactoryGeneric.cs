namespace Juniper.IO
{
    public interface IFactory<ResultT, out MediaTypeT> : ISerializer<ResultT>, IDeserializer<ResultT, MediaTypeT>
        where MediaTypeT : MediaType
    {
    }
}