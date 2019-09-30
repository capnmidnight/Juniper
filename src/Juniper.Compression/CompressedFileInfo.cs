using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;

using Juniper.Collections;

namespace Juniper.Compression
{
    [Serializable]
    public sealed class CompressedFileInfo : IEquatable<CompressedFileInfo>, IComparable<CompressedFileInfo>, ISerializable
    {
        public readonly string Name;
        public readonly bool IsDirectory;
        public readonly bool IsFile;
        public readonly long Size;

        internal readonly string[] pathParts;

        public CompressedFileInfo(string name, bool isDirectory, bool isFile, long size, string[] pathParts)
        {
            Name = name;
            IsDirectory = isDirectory;
            IsFile = isFile;
            Size = size;
            this.pathParts = pathParts;
        }

        internal CompressedFileInfo()
            : this(null, true, false, 0, Array.Empty<string>())
        { }

        public CompressedFileInfo(string name)
            : this(name, true, false, 0) { }

        public CompressedFileInfo(string name, bool isDirectory, bool isFile, long size)
            : this(name, isDirectory, isFile, size, PathExt.PathParts(name))
        { }

        public CompressedFileInfo(ZipEntry entry)
            : this(entry.Name, entry.IsDirectory, entry.IsFile, entry.Size)
        { }

        public CompressedFileInfo(TarEntry entry)
            : this(entry.Name, entry.IsDirectory, !entry.IsDirectory, entry.Size)
        { }

        private CompressedFileInfo(SerializationInfo info, StreamingContext context)
            : this(info.GetString(nameof(Name)),
                  info.GetBoolean(nameof(IsDirectory)),
                  info.GetBoolean(nameof(IsFile)),
                  info.GetInt64(nameof(Size)))
        { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(IsDirectory), IsDirectory);
            info.AddValue(nameof(IsFile), IsFile);
            info.AddValue(nameof(Size), Size);
        }

        public bool Contains(CompressedFileInfo other)
        {
            return IsDirectory
                && other.pathParts.Length >= pathParts.Length
                && other.pathParts.Take(pathParts.Length).Matches(pathParts);
        }

        public bool Equals(CompressedFileInfo other)
        {
            return other is object
                && Name == other.Name
                && IsDirectory == other.IsDirectory
                && IsFile == other.IsFile
                && Size == other.Size;
        }

        public override bool Equals(object obj)
        {
            return obj is CompressedFileInfo cfi
                && Equals(cfi);
        }

        public static bool operator==(CompressedFileInfo left, CompressedFileInfo right)
        {
            return left is null && right is null
                || left is object && left.Equals(right);
        }

        public static bool operator!=(CompressedFileInfo left, CompressedFileInfo right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode()
                ^ IsDirectory.GetHashCode()
                ^ IsFile.GetHashCode()
                ^ Size.GetHashCode();
        }

        public override string ToString()
        {
            if (IsDirectory)
            {
                return "+" + Name;
            }
            else if(IsFile)
            {
                return "-" + Name;
            }
            else
            {
                return "?" + Name;
            }
        }

        public int CompareTo(CompressedFileInfo other)
        {
            var name = Name.CompareTo(other?.Name);
            if (name != 0)
            {
                return name;
            }

            var size = Size.CompareTo(other?.Size ?? 0);
            if(size != 0)
            {
                return size;
            }

            var dir = IsDirectory.CompareTo(other?.IsDirectory == true);
            if(dir != 0)
            {
                return dir;
            }

            var file = IsFile.CompareTo(other?.IsFile == true);
            return file;
        }
    }

    public static class CompressedFileInfoExt
    {
        public static IEnumerable<string> Names(this IEnumerable<CompressedFileInfo> entries)
        {
            return from entry in entries
                   select entry.Name;
        }

        public static IEnumerable<CompressedFileInfo> Files(this IEnumerable<CompressedFileInfo> entries)
        {
            return from entry in entries
                   where entry.IsFile
                   select entry;
        }

        public static IEnumerable<CompressedFileInfo> Directories(this IEnumerable<CompressedFileInfo> entries)
        {
            return from entry in entries
                   where entry.IsDirectory
                   select entry;
        }

        public static NAryTree<CompressedFileInfo> Tree(this IEnumerable<CompressedFileInfo> entries)
        {
            var tree = new NAryTree<CompressedFileInfo>(new CompressedFileInfo());
            foreach(var entry in entries)
            {
                tree.Add(entry, (parent, child) => parent.Contains(child));
            }
            return tree;
        }
    }
}
