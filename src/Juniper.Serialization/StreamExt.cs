using Juniper.Progress;
using Juniper.Serialization;

namespace System.IO
{
    public static class StreamExt
    {
        public static DataSource DetermineSource(this Stream imageStream)
        {
            var source = DataSource.None;
            if (imageStream is FileStream)
            {
                source = DataSource.File;
            }
            else if (imageStream is CachingStream)
            {
                source = DataSource.Network;
            }
            else if (imageStream is ProgressStream prog)
            {
                source = prog.stream.DetermineSource();
            }

            return source;
        }
    }
}