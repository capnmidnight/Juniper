using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;

using Juniper.Compression.Tar;

namespace Juniper.Compression
{
    [Serializable]
    public sealed class CompressedFileInfo : IEquatable<CompressedFileInfo>, IComparable<CompressedFileInfo>, ISerializable
    {
        public readonly string FullName;
        public readonly bool IsFile;
        public readonly long Length;

        internal readonly string[] pathParts;

        internal CompressedFileInfo()
            : this(null, false, 0, Array.Empty<string>())
        { }

        private CompressedFileInfo(string name, bool isFile, long size, string[] pathParts)
        {
            FullName = name;
            IsFile = isFile;
            Length = size;
            this.pathParts = pathParts;
        }

        internal CompressedFileInfo(string name, bool isFile, long size)
            : this(name, isFile, size, PathExt.PathParts(name))
        { }

        public CompressedFileInfo(string name)
            : this(name, false, 0, PathExt.PathParts(name)) { }

        public CompressedFileInfo(ZipArchiveEntry entry)
            : this(entry.FullName, true, entry.Length)
        { }

        public CompressedFileInfo(TarArchiveEntry entry)
            : this(entry.FullName, true, entry.Length)
        { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private CompressedFileInfo(SerializationInfo info, StreamingContext context)
            : this(info.GetString(nameof(FullName)),
                info.GetBoolean(nameof(IsFile)),
                info.GetInt64(nameof(Length)))
        { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(FullName), FullName);
            info.AddValue(nameof(IsFile), IsFile);
            info.AddValue(nameof(Length), Length);
        }

        public bool Contains(CompressedFileInfo other)
        {
            return !IsFile
                && other.pathParts.Length >= pathParts.Length
                && other.pathParts.Take(pathParts.Length).Matches(pathParts);
        }

        public bool Equals(CompressedFileInfo other)
        {
            return other is object
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
                || (left is object && left.Equals(right));
        }

        public static bool operator !=(CompressedFileInfo left, CompressedFileInfo right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode()
                ^ IsFile.GetHashCode()
                ^ Length.GetHashCode();
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

        public static bool operator <(CompressedFileInfo left, CompressedFileInfo right)
        {
            return left is null ? right is object : left.CompareTo(right) < 0;
        }

        public static bool operator <=(CompressedFileInfo left, CompressedFileInfo right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(CompressedFileInfo left, CompressedFileInfo right)
        {
            return left is object && left.CompareTo(right) > 0;
        }

        public static bool operator >=(CompressedFileInfo left, CompressedFileInfo right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }
    }
}
