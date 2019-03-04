using Juniper.Progress;

using Newtonsoft.Json;

namespace System.IO
{
    public static class StreamExt
    {
        private const int BLOCK_SIZE = 4096;

        public static void Pipe(this Stream inStream, Stream outStream, IProgressReceiver prog = null)
        {
            prog?.SetProgress(0);
            inStream = new StreamProgress(inStream, prog);
            var read = int.MaxValue;
            var buf = new byte[BLOCK_SIZE];
            while (read > 0)
            {
                read = inStream.Read(buf, 0, BLOCK_SIZE);
                if (read > 0)
                {
                    outStream.Write(buf, 0, read);
                }
            }
            outStream.Flush();
            prog?.SetProgress(1);
        }

        public static byte[] ReadBytes(this Stream stream, IProgressReceiver prog = null)
        {
            prog?.SetProgress(0);
            var streamProg = new StreamProgress(stream, prog);
            var buf = new byte[stream.Length];
            for (var i = 0; i < buf.Length; i += BLOCK_SIZE)
            {
                streamProg.Read(buf, i, BLOCK_SIZE);
            }
            prog?.SetProgress(1);
            return buf;
        }

        public static string ReadString(this Stream stream, IProgressReceiver prog = null)
        {
            var streamProg = new StreamProgress(stream, prog);
            using (var reader = new StreamReader(streamProg))
            {
                return reader.ReadToEnd();
            }
        }

        public static T ReadObject<T>(this Stream stream, IProgressReceiver prog = null)
        {
            using (var streamProg = new StreamProgress(stream, prog))
            using (var reader = new StreamReader(streamProg))
            {
                return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
            }
        }
    }
}
