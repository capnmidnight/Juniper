using Juniper.Collections;

namespace Juniper.Compression;


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
                let isFile = entry.IsFile
                let parts = isFile ? PathExt.PathParts(entry.FullName) : null
                let name = isFile ? parts.Take(parts.Length - 1).ToString("/") : entry.FullName
                where !isFile || name.Length > 0
                select isFile ? new CompressedFileInfo(name) : entry)
            .Distinct();
    }

    public static Tree<CompressedFileInfo> Tree(this IEnumerable<CompressedFileInfo> rawEntries)
    {
        rawEntries = rawEntries.ToArray();

        var tree = new Tree<CompressedFileInfo>();
        var directories = rawEntries.Directories();
        var files = rawEntries.Files();
        var entries = directories.Concat(files)
            .Select(e => new Tree<CompressedFileInfo>(e))
            .Where(e => e.Value is not null)
            .ToDictionary(e => e.Value!.FullName);

        foreach (var entry in entries.Values)
        {
            var parent = entry.Value!.ParentPath is null
                ? tree
                : entries[entry.Value.ParentPath];
            parent.Connect(entry);
        }

        return tree;
    }
}
