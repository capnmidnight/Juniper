using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Juniper.IO
{
    public abstract class AbstractTypeFilteredCacheLayer :
        ICacheDestinationLayer
    {
        private readonly ICacheDestinationLayer destination;

        public AbstractTypeFilteredCacheLayer(ICacheDestinationLayer destination)
        {
            this.destination = destination;
        }

        public virtual bool CanCache(ContentReference fileRef)
        {
            return destination.CanCache(fileRef);
        }

        public Stream Create(ContentReference fileRef)
        {
            return destination.Create(fileRef);
        }

        public bool Delete(ContentReference fileRef)
        {
            return destination.Delete(fileRef);
        }

        public IEnumerable<ContentReference> GetContentReferences(MediaType ofType)
        {
            return destination.GetContentReferences(ofType);
        }

        public Task<Stream> GetStreamAsync(ContentReference fileRef, IProgress prog)
        {
            return destination.GetStreamAsync(fileRef, prog);
        }

        public bool IsCached(ContentReference fileRef)
        {
            return destination.IsCached(fileRef);
        }
    }
}
