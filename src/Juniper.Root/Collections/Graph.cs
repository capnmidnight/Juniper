using System;
using System.IO;

using Juniper.IO;

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

            if (MediaType.Application.Json.Matches(file))
            {
                var json = new JsonFactory<Graph<NodeT>>();
                using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
                return json.Deserialize(stream);
            }
            else if (MediaType.Application.Octet_Stream.Matches(file))
            {
                var bin = new BinaryFactory<Graph<NodeT>>();
                using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
                return bin.Deserialize(stream);
            }
            else
            {
                return null;
            }
        }

        public static Graph<NodeT> Load<NodeT>(string fileName)
            where NodeT : IComparable<NodeT>
        {
            return Load<NodeT>(new FileInfo(fileName.ValidateFileName()));
        }
    }
}
