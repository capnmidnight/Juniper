using System.IO;
using System.Threading.Tasks;

using Juniper.Progress;

namespace Juniper.IO
{
    public interface ICacheDestinationLayer : ICacheSourceLayer
    {
        bool CanCache(ContentReference fileRef);

        Stream Create(ContentReference fileRef, bool overwrite);

        Stream Cache(ContentReference fileRef, Stream stream);

        bool Delete(ContentReference fileRef);
    }
}
