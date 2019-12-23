using System.IO;

using System.Runtime.Serialization.Formatters.Binary;

using Juniper.Progress;

namespace Juniper.IO
{
    public class BinaryFactory<ResultT> : IFactory<ResultT, MediaType.Application>
    {
        public BinaryFactory()
        { }

        public MediaType.Application ContentType
        {
            get { return MediaType.Application.Octet_Stream; }
        }

        public ResultT Deserialize(Stream stream, IProgress prog)
        {
            prog.Report(0);
            ResultT value = default;
            if (stream != null)
            {
                using (stream)
                {
                    var serializer = new BinaryFormatter();
                    value = (ResultT)serializer.Deserialize(stream);
                }
            }

            prog.Report(1);
            return value;
        }

        public void Serialize(Stream stream, ResultT value, IProgress prog = null)
        {
            prog.Report(0);
            var serializer = new BinaryFormatter();
            serializer.Serialize(stream, value);
            stream.Flush();
            prog.Report(1);
        }
    }
}