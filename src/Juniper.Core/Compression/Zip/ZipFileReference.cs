using System.IO;
using System.Threading.Tasks;

using Juniper.IO;
using Juniper.Progress;

namespace Juniper.Compression.Zip
{
    public class ZipFileReference<MediaTypeT> : IStreamSource<MediaTypeT>
        where MediaTypeT : MediaType
    {
        private readonly ZipFileCacheLayer layer;
        private readonly IContentReference<MediaTypeT> fileRef;
        private readonly string cacheFileName;

        public ZipFileReference(ZipFileCacheLayer layer, string cacheFileName, IContentReference<MediaTypeT> fileRef)
        {
            this.layer = layer;
            this.cacheFileName = cacheFileName;
            this.fileRef = fileRef;
        }

        public string CacheID
        {
            get
            {
                return fileRef.CacheID;
            }
        }

        public MediaTypeT ContentType
        {
            get
            {
                return fileRef.ContentType;
            }
        }

        public Task<Stream> GetStream(IProgress prog)
        {
            Stream stream = null;
            var zip = Decompressor.OpenZip(layer.zipFile);
            var entry = zip.GetEntry(cacheFileName);
            if (entry != null)
            {
                stream = new ZipFileEntryStream(zip, entry, prog);
                if (prog != null)
                {
                    var length = entry.Size;
                    stream = new ProgressStream(stream, length, prog);
                }
            }
            return Task.FromResult(stream);
        }
    }
}
