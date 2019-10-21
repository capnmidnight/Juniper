namespace Juniper.IO
{
    public interface IFactory<ResultT, out MediaTypeT> : ISerializer<ResultT>, IDeserializer<ResultT>, IContentHandler<MediaTypeT>
        where MediaTypeT : MediaType
    {
    }
}