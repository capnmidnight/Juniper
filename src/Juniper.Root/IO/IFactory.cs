namespace Juniper.IO
{
    public interface IFactory<ResultT, out MediaTypeT> : ISerializer<ResultT, MediaTypeT>, IDeserializer<ResultT, MediaTypeT>
        where MediaTypeT : MediaType
    {
    }
}