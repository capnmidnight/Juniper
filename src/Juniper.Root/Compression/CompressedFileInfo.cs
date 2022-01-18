using Juniper.Compression.Tar;

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;

namespace Juniper.Compression
{
    [Serializable]
    public sealed class CompressedFileInfo :
        IEquatable<CompressedFileInfo>,
        IComparable<CompressedFileInfo>,
        ISerializable
    {
        public string Name { get; }
        public string FullName { get; }
        public bool IsFile { get; }
        public long Length { get; }
        public string ParentPath { get; }

        internal readonly string[] pathParts;

        internal CompressedFileInfo()
            : this(null, false, 0, Array.Empty<string>())
        { }

        private CompressedFileInfo(string name, bool isFile, long size, string[] pathParts)
        {
            Name = name;
            FullName = pathParts.Join(Path.DirectorySeparatorChar);
            IsFile = isFile;
            Length = size;
            this.pathParts = pathParts;
            if (pathParts.Length > 0)
            {
                var parentPathParts = pathParts.Take(pathParts.Length - 1).ToArray();
                ParentPath = parentPathParts.Join(Path.DirectorySeparatorChar);
            }
        }

        internal CompressedFileInfo(string name, bool isFile, long size)
            : this(name ?? throw new ArgumentNullException(nameof(name)), isFile, size, PathExt.PathParts(name))
        { }

        public CompressedFileInfo(string name)
            : this(name ?? throw new ArgumentNullException(nameof(name)), false, 0, PathExt.PathParts(name)) { }

        public CompressedFileInfo(ZipArchiveEntry entry)
            : this(entry?.FullName ?? throw new ArgumentNullException(nameof(entry)), true, entry.Length)
        { }

        public CompressedFileInfo(TarArchiveEntry entry)
            : this(entry?.FullName ?? throw new ArgumentNullException(nameof(entry)), true, entry.Length)
        { }

        private CompressedFileInfo(SerializationInfo info, StreamingContext context)
            : this(info?.GetString(nameof(FullName)) ?? throw new ArgumentNullException(nameof(info)),
                info.GetBoolean(nameof(IsFile)),
                info.GetInt64(nameof(Length)))
        { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(FullName), FullName);
            info.AddValue(nameof(IsFile), IsFile);
            info.AddValue(nameof(Length), Length);
        }

        public bool Contains(CompressedFileInfo other)
        {
            return !IsFile
                && other is not null
                && !other.IsFile
                && other.FullName == ParentPath;
        }

        public bool Equals(CompressedFileInfo other)
        {
            return other is not null
                && FullName == other.FullName
                && IsFile == other.IsFile
                && Length == other.Length;
        }

        public override bool Equals(object obj)
        {
            return obj is CompressedFileInfo cfi
                && Equals(cfi);
        }

        public static bool operator ==(CompressedFileInfo left, CompressedFileInfo right)
        {
            return (left is null && right is null)
                || (left is not null && left.Equals(right));
        }

        public static bool operator !=(CompressedFileInfo left, CompressedFileInfo right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            if (IsFile)
            {
                return "-" + FullName;
            }
            else
            {
                return "+" + FullName;
            }
        }

        public int CompareTo(CompressedFileInfo other)
        {
            var name = string.Compare(FullName, other?.FullName, StringComparison.Ordinal);
            if (name != 0)
            {
                return name;
            }

            var size = Length.CompareTo(other?.Length ?? 0);
            if (size != 0)
            {
                return size;
            }

            var file = IsFile.CompareTo(other?.IsFile == true);
            return file;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FullName, IsFile, Length);
        }

        public static bool operator <(CompressedFileInfo left, CompressedFileInfo right)
        {
            return left is null ? right is not null : left.CompareTo(right) < 0;
        }

        public static bool operator <=(CompressedFileInfo left, CompressedFileInfo right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(CompressedFileInfo left, CompressedFileInfo right)
        {
            return left is not null && left.CompareTo(right) > 0;
        }

        public static bool operator >=(CompressedFileInfo left, CompressedFileInfo right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }
    }
}
