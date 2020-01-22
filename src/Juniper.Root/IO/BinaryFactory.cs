using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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

        public ResultT Deserialize(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var serializer = new BinaryFormatter
            {
                Binder = this
            };

            return (ResultT)serializer.Deserialize(stream);
        }

        public long Serialize(Stream stream, ResultT value)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var serializer = new BinaryFormatter();
            serializer.Serialize(stream, value);
            stream.Flush();
            return stream.Length;
        }
    }
}