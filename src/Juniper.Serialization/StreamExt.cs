using Juniper.Serialization;
using Juniper.Streams;

namespace System.IO
{
    public static class StreamExt
    {
        public static DataSource DetermineSource(this Stream imageStream)
        {
            var source = DataSource.None;
            if (imageStream is IStreamWrapper wrapper)
            {
                source = wrapper.UnderlyingStream.DetermineSource();
            }
            else if (imageStream is FileStream)
            {
                source = DataSource.File;
            }

            return source;
        }
    }
}