using Juniper.Serialization;
using Juniper.Streams;

namespace System.IO
{
    public static class StreamExt
    {
        public static DataSource DetermineSource(this Stream stream)
        {
            if (stream is CachingStream)
            {
                return DataSource.Network;
            }
            else if (stream is IStreamWrapper wrapper)
            {
                return wrapper.UnderlyingStream.DetermineSource();
            }
            else if (stream is FileStream)
            {
                return DataSource.File;
            }

            return DataSource.None;
        }
    }
}