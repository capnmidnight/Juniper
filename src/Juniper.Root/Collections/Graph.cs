using Juniper.IO;

using System;
using System.IO;

namespace Juniper.Collections
{
    public abstract class Graph
    {
        public static Graph<NodeT> Load<NodeT>(FileInfo file)
            where NodeT : IComparable<NodeT>
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!file.Exists)
            {
                throw new FileNotFoundException(file.FullName);
            }

            using var stream = file.OpenRead();
            if (MediaType.Application.Json.GuessMatches(file))
            {
                return LoadJSON<NodeT>(stream);
            }
            else if (MediaType.Application.Octet_Stream.GuessMatches(file))
            {
                return LoadBinary<NodeT>(stream);
            }
            else
            {
                throw new InvalidOperationException("Don't know how to read the file type.");
            }
        }

        public static Graph<NodeT> LoadBinary<NodeT>(Stream stream) where NodeT : IComparable<NodeT>
        {
            return Load(new BinaryFactory<Graph<NodeT>>(), stream);
        }

        public static Graph<NodeT> LoadJSON<NodeT>(Stream stream) where NodeT : IComparable<NodeT>
        {
            return Load(new JsonFactory<Graph<NodeT>>(), stream);
        }

        private static Graph<NodeT> Load<NodeT>(IDeserializer<Graph<NodeT>> deserializer, Stream stream)
            where NodeT : IComparable<NodeT>
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            return deserializer.Deserialize(stream);
        }

        public static Graph<NodeT> Load<NodeT>(string fileName)
            where NodeT : IComparable<NodeT>
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileName.Length == 0)
            {
                throw new ArgumentException("path must not be empty string", nameof(fileName));
            }

            return Load<NodeT>(new FileInfo(fileName));
        }
    }
}
