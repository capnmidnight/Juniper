using System.IO;

namespace Juniper.IO
{
    public interface ICacheDestinationLayer : ICacheSourceLayer
    {
        bool CanCache(ContentReference fileRef);

        Stream Create(ContentReference fileRef);

        Stream Cache(ContentReference fileRef, Stream stream);

        bool Delete(ContentReference fileRef);
    }
}
