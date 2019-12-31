using System.Collections.Generic;
using System.IO;
using System.Linq;

using Juniper.Collections;

namespace Juniper.Compression
{

    public static class CompressedFileInfoExt
    {
        public static IEnumerable<string> Names(this IEnumerable<CompressedFileInfo> entries)
        {
            return from entry in entries
                   select entry.FullName;
        }

        public static IEnumerable<CompressedFileInfo> Files(this IEnumerable<CompressedFileInfo> entries)
        {
            return from entry in entries
                   where entry.IsFile
                   select entry;
        }

        public static IEnumerable<CompressedFileInfo> Directories(this IEnumerable<CompressedFileInfo> entries)
        {
            return (from entry in entries
                    where !entry.IsFile
                    select entry.FullName)
                .Union(from entry in entries
                       where entry.IsFile
                       let parts = PathExt.PathParts(entry.FullName)
                       let name = string.Join("/", parts.Take(parts.Length - 1))
                       where name.Length > 0
                       select name)
                .Distinct()
                .Select(d => new CompressedFileInfo(d));
        }

        public static NAryTree<CompressedFileInfo> Tree(this IEnumerable<CompressedFileInfo> entries)
        {
            var tree = new NAryTree<CompressedFileInfo>(new CompressedFileInfo());

            foreach (var entry in entries.Directories().Concat(entries.Files()))
            {
                tree.Add(entry, (parent, child) => parent.Contains(child));
            }

            return tree;
        }
    }
}
