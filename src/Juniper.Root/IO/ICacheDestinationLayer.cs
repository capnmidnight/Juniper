using System.IO;

namespace Juniper.IO
{
    public interface ICacheDestinationLayer : ICacheSourceLayer
    {
        bool CanCache(ContentReference fileRef);

        Stream Create(ContentReference fileRef);

        bool Delete(ContentReference fileRef);
    }
}
