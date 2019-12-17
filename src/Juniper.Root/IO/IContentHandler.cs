namespace Juniper.IO
{
    public interface IContentHandler<out MediaTypeT>
        where MediaTypeT : MediaType
    {
        MediaTypeT ContentType { get; }
    }

    public interface IContentHandler : IContentHandler<MediaType>
    {
    }
}