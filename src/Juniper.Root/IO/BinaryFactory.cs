using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Juniper.Progress;

namespace Juniper.IO
{
    public class BinaryFactory<ResultT> :
        SerializationBinder,
        IFactory<ResultT, MediaType.Application>
    {
        public BinaryFactory()
        { }

        public MediaType.Application ContentType
        {
            get { return MediaType.Application.Octet_Stream; }
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            var assembly = Assembly.Load(assemblyName);
            var type = assembly.GetType(typeName);
            return type;
        }

        public ResultT Deserialize(Stream stream, IProgress prog)
        {
            prog.Report(0);
            ResultT value = default;
            if (stream is object)
            {
                using (stream)
                {
                    var serializer = new BinaryFormatter
                    {
                        Binder = this
                    };

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