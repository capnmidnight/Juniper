using System.IO;
using System.Threading.Tasks;

using Juniper.HTTP;
using Juniper.Progress;

namespace Juniper.IO
{
    public class FileReference<MediaTypeT> : ContentReference<MediaTypeT>, IStreamSource<MediaTypeT>
        where MediaTypeT : MediaType
    {
        private readonly FileInfo file;

        public FileReference(FileInfo file, string fileName, MediaTypeT contentType)
            : base(fileName, contentType)
        {
            this.file = file;
        }

        public FileReference(DirectoryInfo directory, string fileName, MediaTypeT contentType)
            : base(fileName, contentType)
        {
            file = new FileInfo(Path.Combine(directory.FullName, fileName));
        }

        public async Task<Stream> GetStream(IProgress prog)
        {
            Stream stream = null;

            if (file.Exists)
            {
                stream = file.OpenRead();
                if (prog != null)
                {
                    stream = new ProgressStream(stream, file.Length, prog);
                }
            }

            return stream;
        }
    }
}
